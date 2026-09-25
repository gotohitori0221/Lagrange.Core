namespace Lagrange.Core.Common.Entity;

public class BotFriendCategory(int id, string name, int count, int sortId)
{
    public int Id { get; } = id;

    public string Name { get; } = name ?? string.Empty;
    
    public int Count { get; } = count;
    
    public int SortId { get; } = sortId;
}