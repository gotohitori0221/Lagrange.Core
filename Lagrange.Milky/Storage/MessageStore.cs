using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entities;
using Lagrange.Milky.Configurations;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lagrange.Milky.Storage;






public sealed class MessageStore : IHostedService, IDisposable
{
    private readonly string _dbPath;
    private readonly ILogger<MessageStore> _logger;
    private SqliteConnection? _conn;

    public MessageStore(IHostEnvironment environment, LagrangeConfiguration lagrangeConfig, ILogger<MessageStore> logger)
    {
        long uin = lagrangeConfig.Login.Uin;
        _dbPath = Path.Combine(environment.ContentRootPath, $"msg-{uin}.db");
        _logger = logger;
    }

    

    public Task StartAsync(CancellationToken ct)
    {
        _conn = new SqliteConnection($"Data Source={_dbPath}");
        _conn.Open();
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS messages (
                type     INTEGER NOT NULL,
                peer_uin INTEGER NOT NULL,
                sequence INTEGER NOT NULL,
                data     TEXT    NOT NULL,
                PRIMARY KEY (type, peer_uin, sequence)
            );
            """;
        cmd.ExecuteNonQuery();
        _logger.LogInformation("MessageStore started, DB path: {Path}", _dbPath);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct)
    {
        _conn?.Close();
        return Task.CompletedTask;
    }

    

    
    public void Upsert(BotMessage message)
    {
        if (_conn == null) return;
        var row = MessageRow.FromBotMessage(message);
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = """
            INSERT OR REPLACE INTO messages (type, peer_uin, sequence, data)
            VALUES ($type, $peer, $seq, $data);
            """;
        cmd.Parameters.AddWithValue("$type", (int)row.Type);
        cmd.Parameters.AddWithValue("$peer", row.PeerUin);
        cmd.Parameters.AddWithValue("$seq", (long)row.Sequence);
        cmd.Parameters.AddWithValue("$data", row.Data);
        cmd.ExecuteNonQuery();
    }

    
    public BotMessage? Get(MessageType type, long peerUin, ulong sequence)
    {
        if (_conn == null) return null;
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = "SELECT data FROM messages WHERE type=$type AND peer_uin=$peer AND sequence=$seq LIMIT 1;";
        cmd.Parameters.AddWithValue("$type", (int)type);
        cmd.Parameters.AddWithValue("$peer", peerUin);
        cmd.Parameters.AddWithValue("$seq", (long)sequence);
        var json = cmd.ExecuteScalar() as string;
        return json == null ? null : MessageRow.Deserialize(json);
    }

    
    
    
    public IReadOnlyList<BotMessage> GetRange(MessageType type, long peerUin, ulong startSeq, ulong endSeq, int limit = 100)
    {
        if (_conn == null) return [];
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = """
            SELECT data FROM messages
            WHERE type=$type AND peer_uin=$peer AND sequence>$start AND sequence<=$end
            ORDER BY sequence DESC
            LIMIT $limit;
            """;
        cmd.Parameters.AddWithValue("$type", (int)type);
        cmd.Parameters.AddWithValue("$peer", peerUin);
        cmd.Parameters.AddWithValue("$start", (long)startSeq);
        cmd.Parameters.AddWithValue("$end", (long)endSeq);
        cmd.Parameters.AddWithValue("$limit", limit);

        var result = new List<BotMessage>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var msg = MessageRow.Deserialize(reader.GetString(0));
            if (msg != null) result.Add(msg);
        }
        return result;
    }

    public ulong? GetLatestSequence(MessageType type, long peerUin)
    {
        if (_conn == null) return null;
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = "SELECT sequence FROM messages WHERE type=$type AND peer_uin=$peer ORDER BY sequence DESC LIMIT 1;";
        cmd.Parameters.AddWithValue("$type", (int)type);
        cmd.Parameters.AddWithValue("$peer", peerUin);
        var obj = cmd.ExecuteScalar();
        return obj is long l ? (ulong)l : null;
    }

    public void Dispose() => _conn?.Dispose();

    

    private sealed class MessageRow
    {
        public MessageType Type { get; set; }
        public long PeerUin { get; set; }
        public ulong Sequence { get; set; }
        public string Data { get; set; } = string.Empty;

        public static MessageRow FromBotMessage(BotMessage message)
        {
            long peerUin = message.Contact switch
            {
                BotGroupMember m => m.Group.Uin,
                BotFriend f => f.Uin,
                BotStranger s => s.Uin,
                _ => message.Contact.Uin,
            };
            ulong seq = message.Type == MessageType.Private ? message.ClientSequence : message.Sequence;
            return new MessageRow
            {
                Type = message.Type,
                PeerUin = peerUin,
                Sequence = seq,
                Data = JsonSerializer.Serialize(new StoredMessage(message), StoredMessageContext.Default.StoredMessage),
            };
        }

        public static BotMessage? Deserialize(string json)
        {
            var stored = JsonSerializer.Deserialize(json, StoredMessageContext.Default.StoredMessage);
            return stored?.ToBotMessage();
        }
    }
}



[JsonSerializable(typeof(StoredMessage))]
[JsonSerializable(typeof(StoredEntity))]
[JsonSerializable(typeof(List<StoredEntity>))]
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
internal sealed partial class StoredMessageContext : JsonSerializerContext;

internal sealed class StoredMessage
{
    [JsonPropertyName("type")] public int Type { get; set; }
    [JsonPropertyName("peer_uin")] public long PeerUin { get; set; }
    [JsonPropertyName("sender_uin")] public long SenderUin { get; set; }
    [JsonPropertyName("sender_name")] public string? SenderName { get; set; }
    [JsonPropertyName("sequence")] public ulong Sequence { get; set; }
    [JsonPropertyName("client_sequence")] public ulong ClientSequence { get; set; }
    [JsonPropertyName("time")] public long Time { get; set; }
    [JsonPropertyName("entities")] public List<StoredEntity> Entities { get; set; } = [];

    public StoredMessage() { }

    public StoredMessage(BotMessage msg)
    {
        Type = (int)msg.Type;
        PeerUin = msg.Contact switch
        {
            BotGroupMember m => m.Group.Uin,
            BotFriend f => f.Uin,
            BotStranger s => s.Uin,
            _ => msg.Contact.Uin,
        };
        SenderUin = msg.Contact.Uin;
        SenderName = msg.Contact switch
        {
            BotGroupMember m => m.MemberCard ?? m.Nickname,
            BotFriend f => f.Nickname,
            BotStranger s => s.Nickname,
            _ => null,
        };
        Sequence = msg.Sequence;
        ClientSequence = msg.ClientSequence;
        Time = msg.Time;

        foreach (var entity in msg.Entities)
            Entities.Add(StoredEntity.From(entity));
    }

    public BotMessage? ToBotMessage()
    {
        var entities = new List<IMessageEntity>();
        foreach (var se in Entities)
        {
            var e = se.ToEntity();
            if (e != null) entities.Add(e);
        }

        return BotMessage.Restore(
            (MessageType)Type,
            PeerUin,
            SenderUin,
            SenderName ?? string.Empty,
            Sequence,
            ClientSequence,
            Time,
            entities);
    }
}

internal sealed class StoredEntity
{
    [JsonPropertyName("k")] public string Kind { get; set; } = string.Empty;
    [JsonPropertyName("t")] public string? Text { get; set; }
    [JsonPropertyName("u")] public long? Uin { get; set; }
    [JsonPropertyName("n")] public string? Display { get; set; }
    [JsonPropertyName("su")] public ulong? SourceSeq { get; set; }
    [JsonPropertyName("sp")] public string? SourcePreview { get; set; }
    [JsonPropertyName("fu")] public string? FileUrl { get; set; }
    [JsonPropertyName("fn")] public string? FileName { get; set; }
    [JsonPropertyName("fs")] public uint? FileSize { get; set; }
    [JsonPropertyName("fw")] public float? Width { get; set; }
    [JsonPropertyName("fh")] public float? Height { get; set; }
    [JsonPropertyName("ri")] public bool? IsRaw { get; set; }

    public static StoredEntity From(IMessageEntity entity) => entity switch
    {
        TextEntity e => new StoredEntity { Kind = "text", Text = e.Text },
        MentionEntity e => new StoredEntity { Kind = "at", Uin = e.Uin, Display = e.Display },
        ImageEntity e => new StoredEntity { Kind = "img", FileUrl = e.FileUrl, FileName = e.FileName, FileSize = e.FileSize, Width = e.ImageSize.X, Height = e.ImageSize.Y },
        VideoEntity e => new StoredEntity { Kind = "video", FileUrl = e.FileUrl, FileName = e.FileName, FileSize = e.FileSize, Width = e.VideoSize.X, Height = e.VideoSize.Y },
        RecordEntity e => new StoredEntity { Kind = "record", FileUrl = e.FileUrl, FileName = e.FileName, FileSize = e.FileSize },
        ReplyEntity e => new StoredEntity { Kind = "reply", SourceSeq = e.SrcSequence },
        LightAppEntity e => new StoredEntity { Kind = "app", Text = e.Payload },
        MultiMsgEntity e => new StoredEntity { Kind = "fwd", Text = e.ResId },
        _ => new StoredEntity { Kind = "unknown" },
    };

    public IMessageEntity? ToEntity() => Kind switch
    {
        "text" => new TextEntity(Text ?? string.Empty),
        "at" => new MentionEntity(Uin ?? 0, Display),
        "img" => new ImageEntity { FileUrl = FileUrl ?? string.Empty, FileName = FileName ?? string.Empty, FileSize = FileSize ?? 0, ImageSize = new Vector2(Width ?? 0, Height ?? 0) },
        "video" => new VideoEntity { FileUrl = FileUrl ?? string.Empty, FileName = FileName ?? string.Empty, FileSize = FileSize ?? 0, VideoSize = new Vector2(Width ?? 0, Height ?? 0) },
        "record" => new RecordEntity { FileUrl = FileUrl ?? string.Empty, FileName = FileName ?? string.Empty, FileSize = FileSize ?? 0 },
        "reply" => new ReplyEntity { SrcSequence = SourceSeq ?? 0 },
        "app" => new LightAppEntity(Text ?? string.Empty),
        "fwd" => new MultiMsgEntity(Text),
        _ => null,
    };
}
