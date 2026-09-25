using System.Buffers;
using System.Buffers.Binary;
using System.Security.Cryptography;
using Lagrange.Core.Internal.Events.System;
using Lagrange.Core.Internal.Packets.Service;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Extension;

namespace Lagrange.Core.Internal.Context;

internal class HighwayContext
{
    private const string Tag = nameof(HighwayContext);
    
    private readonly BotContext _context;

    private readonly HttpClient _client;
    
    private readonly ulong _chunkSize;

    private readonly int _concurrent;
    
    private int _sequence;

    private (byte[], DateTime)? _ticket;

    private string? _url;
    
    public HighwayContext(BotContext context)
    {
        _context = context;

        _client = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true });
        _client.DefaultRequestHeaders.Add("Accept-Encoding", "identity");
        _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2)");
        
        _sequence = 0;
        _chunkSize = context.Config.HighwayChunkSize;
        _concurrent = (int)context.Config.HighwayConcurrent;
    }

    public async Task<bool> UploadFile(Stream stream, int commandId, ReadOnlyMemory<byte> extendInfo)
    {
        if (_ticket == null || _url == null || DateTime.Now - _ticket.Value.Item2 > TimeSpan.FromDays(0.5))
        {
            var resp = await _context.EventContext.SendEvent<HighwaySessionEventResp>(new HighwaySessionEventReq());
            _ticket = (resp.SigSession, DateTime.Now);
            _url = resp.HighwayUrls[1][0];
        }

        var tasks = new List<Task<bool>>();
        bool result = true;

        ulong fileSize = (ulong)stream.Length;
        ulong offset = 0;
        var fileMd5 = stream.Md5();
        while (offset < fileSize)
        {
            var buffer = ArrayPool<byte>.Shared.Rent((int)_chunkSize);
            ulong payload = (ulong)await stream.ReadAsync(buffer.AsMemory(0, (int)_chunkSize));

            ulong currentBlockOffset = offset;
            var task = Task.Run(async () => 
            {
                var bufferSpan = buffer.AsSpan(0, (int)payload);
                int sequence = GetNewSequence();

                var head = new DataHighwayHead
                {
                    Version = 1,
                    Uin = _context.Keystore.Uin.ToString(),
                    Command = "PicUp.DataUp",
                    Seq = (uint)sequence,
                    AppId = (uint)_context.AppInfo.AppId,
                    DataFlag = 16,
                    CommandId = (uint)commandId,
                };
                var segHead = new SegHead
                {
                    Filesize = fileSize,
                    DataOffset = currentBlockOffset,
                    DataLength = (uint)payload,
                    ServiceTicket = _ticket.Value.Item1,
                    Md5 = MD5.HashData(bufferSpan),
                    FileMd5 = fileMd5,
                };
                var loginHead = new LoginSigHead
                {
                    Uint32LoginSigType = 8,
                    BytesLoginSig = _context.Keystore.WLoginSigs.A2,
                    AppId = (uint)_context.AppInfo.AppId
                };
                var highwayHead = new ReqDataHighwayHead
                {
                    MsgBaseHead = head,
                    MsgSegHead = segHead,
                    BytesReqExtendInfo = extendInfo,
                    Timestamp = 0,
                    MsgLoginSigHead = loginHead
                };
                var headProto = ProtoHelper.Serialize(highwayHead);

                bool end = currentBlockOffset + payload >= fileSize;
                var upload = ArrayPool<byte>.Shared.Rent(1 + 1 + 4 + 4 + headProto.Length + (int)payload);
                var memory = upload.AsMemory(0, 1 + 1 + 4 + 4 + headProto.Length + (int)payload);

                memory.Span[0] = 0x28;
                BinaryPrimitives.WriteUInt32BigEndian(memory.Span[1..], (uint)headProto.Length);
                BinaryPrimitives.WriteUInt32BigEndian(memory.Span[5..], (uint)payload);
                headProto.Span.CopyTo(memory.Span[9..]);
                bufferSpan.CopyTo(memory.Span[(9 + headProto.Length)..]);
                memory.Span[^1] = 0x29;

                var request = new HttpRequestMessage(HttpMethod.Post, $"http://{_url}")
                {
                    Content = new ReadOnlyMemoryContent(memory), Headers = { { "Connection", end ? "close" : "keep-alive" } }
                };

                try
                {
                    var response = await _client.SendAsync(request);
                    var reader = new BinaryPacket((await response.Content.ReadAsByteArrayAsync()).AsSpan());

                    if (reader.Read<byte>() == 0x28)
                    {
                        int headLen = reader.Read<int>();
                        int bodyLen = reader.Read<int>();
                        var respHead = reader.CreateSpan(headLen);
                        var body = GC.AllocateUninitializedArray<byte>(bodyLen);
                        reader.ReadBytes(body.AsSpan());

                        if (reader.Read<byte>() == 0x29)
                        {
                            var obj = ProtoHelper.Deserialize<RespDataHighwayHead>(respHead);
                            _context.LogDebug(Tag, "Highway Block Result: {0} | {1} | {2}", obj.ErrorCode, obj.MsgSegHead?.RetCode, Convert.ToHexString(body));
                            return obj.ErrorCode == 0;
                        }
                    }
                }
                catch (Exception e)
                {
                    _context.LogError(Tag, "Highway HTTP error: {0}", e, e.Message);
                    if (e.StackTrace is { } stack) _context.LogDebug(Tag, stack);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                    ArrayPool<byte>.Shared.Return(upload);
                    request.Dispose();
                    new ReadOnlyMemoryContent(memory).Dispose();
                }

                return false;
            });
            offset += payload;

            tasks.Add(task);
            if (tasks.Count == (_concurrent))
            {
                var successBlocks = await Task.WhenAll(tasks);
                foreach (bool t in successBlocks) result &= t;
                tasks.Clear();
            }
            
            if (tasks.Count != 0)
            {
                var finalBlocks = await Task.WhenAll(tasks);
                foreach (bool t in finalBlocks) result &= t;
                tasks.Clear();
            }
        }

        return result;
    }

    private int GetNewSequence()
    {
        Interlocked.CompareExchange(ref _sequence, 0, 100000);
        return Interlocked.Increment(ref _sequence);
    }
}