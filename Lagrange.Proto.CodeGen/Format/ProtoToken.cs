namespace Lagrange.Proto.CodeGen.Format;

public enum TokenType
{
    Keyword,
    Identifier,
    Symbol,
    StringLiteral,
    IntegerLiteral,
    EOF,
}

public readonly record struct ProtoToken(TokenType Type, string Value);