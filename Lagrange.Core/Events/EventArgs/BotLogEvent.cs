namespace Lagrange.Core.Events.EventArgs;

public enum LogLevel
{
    Trace = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    Critical = 5
}

public class BotLogEvent(string tag, LogLevel level, string message, Exception? exception) : EventBase
{
    public string Tag { get; } = tag;
    
    public LogLevel Level { get; } = level;
    
    public string Message { get; } = message;

    public Exception? Exception { get; } = exception;
    
    public override string ToEventMessage() => $"[{Tag}] [{Level.ToString().ToUpper()}]: {Message}";
}