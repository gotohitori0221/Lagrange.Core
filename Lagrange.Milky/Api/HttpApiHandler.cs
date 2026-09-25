using System;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Exceptions;
using Lagrange.Milky.Api.Handlers;
using Lagrange.Milky.Configurations;
using Lagrange.Milky.Extensions;
using Lagrange.Milky.Http;
using Lagrange.Milky.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lagrange.Milky.Api;

public class HttpApiHandler(IServiceScopeFactory scopeFactory, ILogger<HttpApiHandler> logger, MilkyConfiguration configuration, MilkyHttpApiConfiguration httpConfiguration, BotContext lagrange) : IHttpHandler
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<HttpApiHandler> _logger = logger;

    private readonly BotContext _lagrange = lagrange;

    private readonly string? _token = configuration.AccessToken;
    private readonly string? _allowCorsOrigins = httpConfiguration.AllowCorsOrigins;

    public bool CanHandle(HttpListenerContext context)
        => (context.Request.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase)
            ? _allowCorsOrigins != null
            : context.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
        && context.Request.Url != null
        && context.Request.Url.Segments.Length == 3
        && context.Request.Url.Segments[1].Equals("api/", StringComparison.OrdinalIgnoreCase);

    public Task HandleAsync(HttpListenerContext context, CancellationToken ct) => context.Request.HttpMethod.ToUpperInvariant() switch
    {
        "OPTIONS" => SendOptionsResponse(context, ct),
        "POST" => SendPostResponse(context, ct),
        _ => throw new Exception(),
    };

    private Task SendOptionsResponse(HttpListenerContext context, CancellationToken ct)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NoContent;
        context.Response.Headers["Access-Control-Allow-Origin"] = _allowCorsOrigins;
        context.Response.Headers["Access-Control-Allow-Methods"] = "OPTIONS, POST";
        context.Response.Headers["Access-Control-Allow-Headers"] = "*";
        context.Response.Close();
        return Task.CompletedTask;
    }

    private async Task SendPostResponse(HttpListenerContext context, CancellationToken ct)
    {
        var id = context.Request.RequestTraceIdentifier;

        try
        {
            if (!ValidateAccessToken(context))
            {
                _logger.LogAccessTokenRejected(id);
                context.ReturnStatus(HttpStatusCode.Unauthorized, _allowCorsOrigins);
                return;
            }

            if (!ValidateContextType(context))
            {
                _logger.LogContentTypeRejected(id, context.Request.ContentType ?? "");
                context.ReturnStatus(HttpStatusCode.UnsupportedMediaType, _allowCorsOrigins);
                return;
            }

            string name = context.Request.Url!.Segments[2].TrimEnd('/');

            await using var scope = _scopeFactory.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetKeyedService<IApiHandler>(name);
            if (handler == null)
            {
                _logger.LogHandlerNotFound(id, name);
                context.ReturnStatus(HttpStatusCode.NotFound, _allowCorsOrigins);
                return;
            }

            object request;
            try
            {
                request = await Serializer.JsonDeserializeAsync(context.Request.InputStream, handler.RequestType, ct)
                    ?? throw new JsonException("Deserialized request body is null");
            }
            catch (JsonException e)
            {
                _logger.LogRequestInvalid(id, e);
                await SendResponseAsync(context, new MilkyApiResponse(Constants.RetcodeInvalidParameter, "参数解析失败"), ct);
                return;
            }

            if (!_lagrange.IsOnline && name != "get_impl_info")
            {
                _logger.LogNotLoggedIn(id, name);
                await SendResponseAsync(context, new MilkyApiResponse(Constants.RetcodeNotLoggedIn, "未处于登录状态"), ct);
                return;
            }

            MilkyApiResponse response;
            try
            {
                response = await handler.HandleAsync(request, ct);
            }
            catch (Exception e)
            {
                _logger.LogApiException(id, e);

                string message = e is OperationException { ErrMsg: { } errMsg } ? errMsg : e.Message;
                await SendResponseAsync(context, new MilkyApiResponse(Constants.RetcodeOperationFailed, message), ct);
                return;
            }

            await SendResponseAsync(context, response, ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogApiCancelled(id);
            try { context.ReturnStatus(HttpStatusCode.ServiceUnavailable, _allowCorsOrigins); } catch { }
        }
        catch (Exception e)
        {
            _logger.LogApiException(id, e);
            try { context.ReturnStatus(HttpStatusCode.InternalServerError, _allowCorsOrigins); } catch { }
        }
    }

    private async Task SendResponseAsync(HttpListenerContext context, MilkyApiResponse response, CancellationToken ct)
    {
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        if (_allowCorsOrigins != null) context.Response.Headers["Access-Control-Allow-Origin"] = _allowCorsOrigins;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        await Serializer.JsonSerializableAsync(context.Response.OutputStream, response, ct);
        context.Response.Close();
    }

    private bool ValidateAccessToken(HttpListenerContext context)
    {
        return string.IsNullOrEmpty(_token) || ValidateAuthorization() || ValidateQueryString();

        bool ValidateAuthorization()
        {
            string? authorization = context.Request.Headers["Authorization"];
            return !string.IsNullOrEmpty(authorization)
                && authorization.StartsWith("Bearer ", StringComparison.Ordinal)
                && authorization.AsSpan(7).Equals(_token, StringComparison.Ordinal);
        }

        bool ValidateQueryString()
        {
            string? queryToken = context.Request.QueryString["access_token"];
            return !string.IsNullOrEmpty(queryToken) && queryToken.Equals(_token, StringComparison.Ordinal);
        }
    }

    private bool ValidateContextType(HttpListenerContext context)
    {
        if (context.Request.ContentType == null) return false;
        string[] contentTypeParts = context.Request.ContentType.Split(';');
        if (contentTypeParts.Length < 1) return false;
        return contentTypeParts[0].Equals(MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase);
    }
}

public static partial class HttpApiHandlerLoggerExtension
{
    [LoggerMessage(LogLevel.Warning, "{Id} rejected: invalid access token")]
    public static partial void LogAccessTokenRejected(this ILogger<HttpApiHandler> logger, Guid id);

    [LoggerMessage(LogLevel.Warning, "{Id} rejected: invalid content type: {ContentType}")]
    public static partial void LogContentTypeRejected(this ILogger<HttpApiHandler> logger, Guid id, string contentType);

    [LoggerMessage(LogLevel.Warning, "{Id} handler not found: {Handler}")]
    public static partial void LogHandlerNotFound(this ILogger<HttpApiHandler> logger, Guid id, string handler);

    [LoggerMessage(LogLevel.Warning, "{Id} rejected: invalid request body")]
    public static partial void LogRequestInvalid(this ILogger<HttpApiHandler> logger, Guid id, Exception exception);

    [LoggerMessage(LogLevel.Warning, "{Id} rejected: not logged in, {handler}")]
    public static partial void LogNotLoggedIn(this ILogger<HttpApiHandler> logger, Guid id, string handler);

    [LoggerMessage(LogLevel.Information, "{Id} request cancelled")]
    public static partial void LogApiCancelled(this ILogger<HttpApiHandler> logger, Guid id);

    [LoggerMessage(LogLevel.Error, "{Id} api handled with exception")]
    public static partial void LogApiException(this ILogger<HttpApiHandler> logger, Guid id, Exception exception);
}