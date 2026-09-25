using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Lagrange.Proto.CodeGen.Format;

public ref struct ProtoParser(List<ProtoToken> tokens)
{
    private readonly ref ProtoToken _first = ref MemoryMarshal.GetReference(CollectionsMarshal.AsSpan(tokens));
    private int _current;

    private ProtoToken Current => Unsafe.Add(ref _first, _current);
    private ProtoToken Consume() => Unsafe.Add(ref _first, _current++);

    private bool Match(params string[] values)
    {
        if (values.Contains(Current.Value))
        {
            Consume();
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ProtoToken Expect(TokenType type, string? value = null)
    {
        var token = Current;
        if (token.Type != type || (value != null && token.Value != value))
            throw new Exception($"Expected {type} '{value}', got {token.Type} '{token.Value}'");
        return Consume();
    }

    public ProtoFile ParseProto()
    {
        var proto = new ProtoFile();
        while (Current.Type != TokenType.EOF)
        {
            if (Match("syntax"))
            {
                Expect(TokenType.Symbol, "=");
                proto.Syntax = Expect(TokenType.StringLiteral).Value;
                Expect(TokenType.Symbol, ";");
            }
            else if (Match("package"))
            {
                proto.Package = Expect(TokenType.Identifier).Value;
                Expect(TokenType.Symbol, ";");
            }
            else if (Match("message"))
            {
                proto.Messages.Add(ParseMessage());
            }
            else if (Match("enum"))
            {
                proto.Enums.Add(ParseEnum());
            }
            else
            {
                Consume();
            }
        }
        return proto;
    }

    private ProtoMessage ParseMessage()
    {
        var msg = new ProtoMessage { Name = Expect(TokenType.Identifier).Value };
        Expect(TokenType.Symbol, "{");

        while (!Match("}"))
        {
            string label = string.Empty;
            if (Match("optional", "required", "repeated"))
            {
                label = Unsafe.Add(ref _first, _current - 1).Value;
            }

            string type;
            if (Match("map"))
            {
                Expect(TokenType.Symbol, "<");
                string keyType = Expect(TokenType.Identifier).Value;
                Expect(TokenType.Symbol, ",");
                string valueType = Expect(TokenType.Identifier).Value;
                Expect(TokenType.Symbol, ">");
                type = $"map<{keyType},{valueType}>";
            }
            else
            {
                type = Expect(TokenType.Identifier).Value;
            }

            string name = Expect(TokenType.Identifier).Value;
            Expect(TokenType.Symbol, "=");
            int number = int.Parse(Expect(TokenType.IntegerLiteral).Value);
            Expect(TokenType.Symbol, ";");

            msg.Fields.Add(new ProtoField
            {
                Label = label,
                Type = type,
                Name = name,
                Number = number
            });
        }

        return msg;
    }

    private ProtoEnum ParseEnum()
    {
        var @enum = new ProtoEnum { Name = Expect(TokenType.Identifier).Value };
        Expect(TokenType.Symbol, "{");

        while (!Match("}"))
        {
            string name = Expect(TokenType.Identifier).Value;
            Expect(TokenType.Symbol, "=");
            int number = int.Parse(Expect(TokenType.IntegerLiteral).Value);
            Expect(TokenType.Symbol, ";");
            @enum.Values[name] = number;
        }

        return @enum;
    }
}
