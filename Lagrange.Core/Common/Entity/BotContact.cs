namespace Lagrange.Core.Common.Entity;

public abstract class BotContact
{
    public abstract long Uin { get; }
    
    public abstract string Nickname { get; }
    
    public abstract string Uid { get; }
}