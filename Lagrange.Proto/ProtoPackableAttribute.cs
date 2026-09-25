namespace Lagrange.Proto;

[AttributeUsage(AttributeTargets.Class)]
public class ProtoPackableAttribute : Attribute
{
    
    
    
    
    public bool IgnoreDefaultFields { get; init; }
}