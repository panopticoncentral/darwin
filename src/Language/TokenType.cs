namespace Darwin.Language
{
    internal enum TokenType
    {
        Error,
        Whitespace,
        LineTerminator,
        Comment,
        EndOfText,
        Comma,
        Colon,
        Semicolon,
        OpenParen,
        CloseParen,
        OpenBrace,
        CloseBrace,
        OpenBracket,
        CloseBracket,
        Operator,
        Identifier,
        DecimalLiteral,
        HexadecimalLiteral,
        FloatingPointLiteral
    }
}
