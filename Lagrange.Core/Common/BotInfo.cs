namespace Lagrange.Core.Common;

[Serializable]
public class BotInfo(byte age, byte gender, string name)
{
    public byte Age { get; set; } = age;

    public byte Gender { get; set; } = gender;

    public string Name { get; set; } = name;

    public override string ToString() => $"Bot name: {Name} | Gender: {Gender} | Age: {Age}";
}