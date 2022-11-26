using System.Collections.Generic;

using Xunit;

namespace Darwin.Language.UnitTests
{
    public class ScannerTests
    {
        private static IEnumerable<Token> GetAllTokens(string text)
        {
            var current = 0;
            var length = text.Length;

            while (current < length)
            {
                var token = Scanner.Scan(text, current);
                current = token.Range.End.Value;
                yield return token;
            }

            yield return Scanner.Scan(text, current);
        }

        [Fact]
        public void Whitespace() => Assert.Equal(GetAllTokens(" \t\r\n"), new Token[]
        {
            new(TokenType.Whitespace, 0..2),
            new(TokenType.LineTerminator, 2..4),
            new(TokenType.EndOfText, 4..4)
        });

        [Fact]
        public void Comments() => Assert.Equal(GetAllTokens("  # Now is the time"), new Token[]
        {
            new(TokenType.Whitespace, 0..2),
            new(TokenType.Comment, 2..19),
            new(TokenType.EndOfText, 19..19)
        });

        [Fact]
        public void Punctuators() => Assert.Equal(GetAllTokens(",:;(){}[]"), new Token[]
        {
            new(TokenType.Comma, 0..1),
            new(TokenType.Colon, 1..2),
            new(TokenType.Semicolon, 2..3),
            new(TokenType.OpenParen, 3..4),
            new(TokenType.CloseParen, 4..5),
            new(TokenType.OpenBrace, 5..6),
            new(TokenType.CloseBrace, 6..7),
            new(TokenType.OpenBracket, 7..8),
            new(TokenType.CloseBracket, 8..9),
            new(TokenType.EndOfText, 9..9)
        });

        [Fact]
        public void Operators() => Assert.Equal(GetAllTokens("+ - ++ -- ~!@$%^&*-+=\\|<>.?/"), new Token[]
        {
            new(TokenType.Operator, 0..1),
            new(TokenType.Whitespace, 1..2),
            new(TokenType.Operator, 2..3),
            new(TokenType.Whitespace, 3..4),
            new(TokenType.Operator, 4..6),
            new(TokenType.Whitespace, 6..7),
            new(TokenType.Operator, 7..9),
            new(TokenType.Whitespace, 9..10),
            new(TokenType.Operator, 10..28),
            new(TokenType.EndOfText, 28..28)
        });

        [Fact]
        public void Identifiers() => Assert.Equal(GetAllTokens("abc abc123 _abc _"), new Token[]
        {
            new(TokenType.Identifier, 0..3),
            new(TokenType.Whitespace, 3..4),
            new(TokenType.Identifier, 4..10),
            new(TokenType.Whitespace, 10..11),
            new(TokenType.Identifier, 11..15),
            new(TokenType.Whitespace, 15..16),
            new(TokenType.Identifier, 16..17),
            new(TokenType.EndOfText, 17..17)
        });

        [Fact]
        public void DecimalLiteral() => Assert.Equal(GetAllTokens("1 123 123fpm 491mph123abc"), new Token[]
        {
            new(TokenType.DecimalLiteral, 0..1),
            new(TokenType.Whitespace, 1..2),
            new(TokenType.DecimalLiteral, 2..5),
            new(TokenType.Whitespace, 5..6),
            new(TokenType.DecimalLiteral, 6..12),
            new(TokenType.Whitespace, 12..13),
            new(TokenType.DecimalLiteral, 13..19),
            new(TokenType.DecimalLiteral, 19..25),
            new(TokenType.EndOfText, 25..25)
        });

        [Fact]
        public void HexLiteral() => Assert.Equal(GetAllTokens("0x0 0x123 0xabc5def 0xABC5DEF"), new Token[]
        {
            new(TokenType.HexadecimalLiteral, 0..3),
            new(TokenType.Whitespace, 3..4),
            new(TokenType.HexadecimalLiteral, 4..9),
            new(TokenType.Whitespace, 9..10),
            new(TokenType.HexadecimalLiteral, 10..19),
            new(TokenType.Whitespace, 19..20),
            new(TokenType.HexadecimalLiteral, 20..29),
            new(TokenType.EndOfText, 29..29)
        });

        [Fact]
        public void BadHexLiteral() => Assert.Equal(GetAllTokens("0x 0xg"), new Token[]
        {
            new(TokenType.Error, 0..2),
            new(TokenType.Whitespace, 2..3),
            new(TokenType.Error, 3..5),
            new(TokenType.Identifier, 5..6),
            new(TokenType.EndOfText, 6..6)
        });

        [Fact]
        public void FloatingPointLiteral() => Assert.Equal(GetAllTokens("0.0 123.456 1E3 1E-3 1E+4 1e3 1.0e-3"), new Token[]
        {
            new(TokenType.FloatingPointLiteral, 0..3),
            new(TokenType.Whitespace, 3..4),
            new(TokenType.FloatingPointLiteral, 4..11),
            new(TokenType.Whitespace, 11..12),
            new(TokenType.FloatingPointLiteral, 12..15),
            new(TokenType.Whitespace, 15..16),
            new(TokenType.FloatingPointLiteral, 16..20),
            new(TokenType.Whitespace, 20..21),
            new(TokenType.FloatingPointLiteral, 21..25),
            new(TokenType.Whitespace, 25..26),
            new(TokenType.FloatingPointLiteral, 26..29),
            new(TokenType.Whitespace, 29..30),
            new(TokenType.FloatingPointLiteral, 30..36),
            new(TokenType.EndOfText, 36..36)
        });

        [Fact]
        public void BadFloatingPointLiteral() => Assert.Equal(GetAllTokens("0. 0.a 0e"), new Token[]
        {
            new(TokenType.Error, 0..2),
            new(TokenType.Whitespace, 2..3),
            new(TokenType.DecimalLiteral, 3..4),
            new(TokenType.Operator, 4..5),
            new(TokenType.Identifier, 5..6),
            new(TokenType.Whitespace, 6..7),
            new(TokenType.EndOfText, 6..6)
        });
    }
}
