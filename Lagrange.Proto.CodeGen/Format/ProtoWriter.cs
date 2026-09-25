using Lagrange.Proto.CodeGen.Utility;

namespace Lagrange.Proto.CodeGen.Format;

internal sealed class ProtoWriter
{
    private readonly SourceWriter _writer = new();

    public string WriteProto(ProtoFile proto)
    {
        _writer.Reset();

        WriteSyntax(proto.Syntax);
        WritePackage(proto.Package);

        foreach (var enumDef in proto.Enums)
        {
            WriteEnum(enumDef);
            _writer.WriteLine();
        }

        foreach (var message in proto.Messages)
        {
            WriteMessage(message);
            _writer.WriteLine();
        }

        return _writer.ToSourceText().Trim();
    }

    private void WriteSyntax(string syntax)
    {
        _writer.WriteLine($"""syntax = "{syntax}";""");
        _writer.WriteLine();
    }

    private void WritePackage(string package)
    {
        if (!string.IsNullOrEmpty(package))
        {
            _writer.WriteLine($"package {package};");
            _writer.WriteLine();
        }
    }

    private void WriteEnum(ProtoEnum enumDef)
    {
        _writer.WriteLine($"enum {enumDef.Name} {{");
        _writer.Indentation++;

        foreach (var (name, value) in enumDef.Values) _writer.WriteLine($"{name} = {value};");

        _writer.Indentation--;
        _writer.WriteLine('}');
    }

    private void WriteMessage(ProtoMessage message)
    {
        _writer.WriteLine($"message {message.Name} {{");
        _writer.Indentation++;

        foreach (var field in message.Fields) _writer.WriteLine($"{field.Type} {field.Name} = {field.Number};");

        _writer.Indentation--;
        _writer.WriteLine('}');
    }
}