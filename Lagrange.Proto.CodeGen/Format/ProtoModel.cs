namespace Lagrange.Proto.CodeGen.Format;

public class ProtoFile
{
    public string Syntax { get; set; } = "proto3";
    public string Package { get; set; } = "";
    public List<ProtoMessage> Messages { get; } = [];
    public List<ProtoEnum> Enums { get; } = [];
}

public class ProtoMessage
{
    public string Name { get; set; } = "";
    
    public List<ProtoField> Fields { get; } = [];
}

public class ProtoEnum
{
    public string Name { get; set; } = "";
    
    public Dictionary<string, int> Values { get; } = new();
}

public class ProtoField
{
    public string Label { get; set; } = "";
    
    public string Type { get; set; } = "";
    
    public string Name { get; set; } = "";
    
    public int Number { get; set; }
}