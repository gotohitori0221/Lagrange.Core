using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Lagrange.Proto.CodeGen.Format;

public ref struct ProtoLexer(ReadOnlySpan<char> input)
{
    private int _position = 0;
    private readonly int _length = input.Length;
    private readonly ref char _first = ref MemoryMarshal.GetReference(input);

    private static readonly HashSet<string> Keywords = ["syntax", "package", "message", "enum", "import", "true", "false", "map", "repeated", "optional", "required", "oneof", "extend", "option", "group", "reserved", "to", "max", "min"];

    public List<ProtoToken> Tokenize()
    {
        var tokens = new List<ProtoToken>();

        while (_position < _length)
        {
            char current = Peek();

            if (char.IsWhiteSpace(current))
            {
                SkipWhitespace();
            }
            else if (current == '/' && Peek(1) == '/')
            {
                SkipLineComment();
            }
            else if (current == '/' && Peek(1) == '*')
            {
                SkipBlockComment();
            }
            else if (current is '"' or '\'')
            {
                tokens.Add(ReadStringLiteral());
            }
            else if (char.IsDigit(current))
            {
                tokens.Add(ReadIntegerLiteral());
            }
            else if (char.IsLetter(current) || current == '_')
            {
                tokens.Add(ReadIdentifierOrKeyword());
            }
            else if (current is '<' or '>' or ',')
            {
                tokens.Add(new ProtoToken(TokenType.Symbol, current.ToString()));
                _position++;
            }
            else if ("{}[]=;".Contains(current))
            {
                tokens.Add(new ProtoToken(TokenType.Symbol, current.ToString()));
                _position++;
            }
            else
            {
                int line = MemoryMarshal.CreateReadOnlySpan(ref _first, _length)[.._position].Count('\n') + 1;
                throw new Exception($"Unexpected character '{current}' at position {_position} (line {line})");
            }
        }

        tokens.Add(new ProtoToken(TokenType.EOF, ""));
        return tokens;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private char Peek(int offset = 0) => _position + offset < _length ? Unsafe.Add(ref _first, _position + offset) : '\0';

    private void SkipWhitespace()
    {
        while (_position < _length && char.IsWhiteSpace(Peek())) _position++;
    }

    private void SkipLineComment()
    {
        _position += 2;
        while (_position < _length && Peek() != '\n') _position++;
    }

    private void SkipBlockComment()
    {
        _position += 2;
        while (_position < _length && !(Peek() == '*' && Peek(1) == '/')) _position++;
        if (_position < _length) _position += 2;
    }

    private ProtoToken ReadStringLiteral()
    {
        char quote = Peek();
        _position++;
        var sb = new StringBuilder();
        while (_position < _length && Peek() != quote)
        {
            if (Peek() == '\\')
            {
                _position++;
                if (_position < _length) sb.Append(Peek());
            }
            else
            {
                sb.Append(Peek());
            }
            _position++;
        }
        _position++;
        return new ProtoToken(TokenType.StringLiteral, sb.ToString());
    }

    private ProtoToken ReadIntegerLiteral()
    {
        var sb = new StringBuilder();
        while (_position < _length && char.IsDigit(Peek()))
        {
            sb.Append(Peek());
            _position++;
        }
        return new ProtoToken(TokenType.IntegerLiteral, sb.ToString());
    }

    private ProtoToken ReadIdentifierOrKeyword()
    {
        var sb = new StringBuilder();
        while (_position < _length && (char.IsLetterOrDigit(Peek()) || Peek() == '_' || Peek() == '.'))
        {
            sb.Append(Peek());
            _position++;
        }

        string value = sb.ToString();
        return Keywords.Contains(value)
            ? new ProtoToken(TokenType.Keyword, value)
            : new ProtoToken(TokenType.Identifier, value);
    }
}
