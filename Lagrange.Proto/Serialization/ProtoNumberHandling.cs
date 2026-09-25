namespace Lagrange.Proto.Serialization;

[Flags]
public enum ProtoNumberHandling : byte
{
    
    
    
    Default = 0b0000000,
    
    
    
    
    Fixed32 = 0b0000001,
    
    
    
    
    Fixed64 = 0b0000010,
    
    
    
    
    Signed = 0b0000100
}