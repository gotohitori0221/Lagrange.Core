using Lagrange.Core.Common.Entity;

namespace Lagrange.Core.Services;




[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ServiceAttribute(
    string command,
    RequestType requestType = RequestType.D2Auth,
    EncryptType encryptType = EncryptType.EncryptD2Key) : Attribute
{
    public string Command { get; } = command;

    public RequestType RequestType { get; } = requestType;

    public EncryptType EncryptType { get; } = encryptType;

    public bool DisableLog { get; init; }
}
