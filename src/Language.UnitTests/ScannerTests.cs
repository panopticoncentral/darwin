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
        public void Punctuators() => Assert.Equal(GetAllTokens(",:;(){}[]"), new[]
        {
            new Token(TokenType.Comma, 0..1),
            new Token(TokenType.Colon, 1..2),
            new Token(TokenType.Semicolon, 2..3),
            new Token(TokenType.OpenParen, 3..4),
            new Token(TokenType.CloseParen, 4..5),
            new Token(TokenType.OpenBrace, 5..6),
            new Token(TokenType.CloseBrace, 6..7),
            new Token(TokenType.OpenBracket, 7..8),
            new Token(TokenType.CloseBracket, 8..9),
            new Token(TokenType.EndOfText, 9..9)
        });

        [Fact]
        public void Operators() => Assert.Equal(GetAllTokens("+ - ++ -- ~!@$%^&*-+=\\|<>.?/"), new[]
        {
            new Token(TokenType.Operator, 0..1),
            new Token(TokenType.Whitespace, 1..2),
            new Token(TokenType.Operator, 2..3),
            new Token(TokenType.Whitespace, 3..4),
            new Token(TokenType.Operator, 4..6),
            new Token(TokenType.Whitespace, 6..7),
            new Token(TokenType.Operator, 7..9),
            new Token(TokenType.Whitespace, 9..10),
            new Token(TokenType.Operator, 10..28),
            new Token(TokenType.EndOfText, 28..28)
        });

        [Fact]
        public void Identifiers() => Assert.Equal(GetAllTokens("abc abc123 _abc _"), new[]
        {
            new Token(TokenType.Identifier, 0..3),
            new Token(TokenType.Whitespace, 3..4),
            new Token(TokenType.Identifier, 4..10),
            new Token(TokenType.Whitespace, 10..11),
            new Token(TokenType.Identifier, 11..15),
            new Token(TokenType.Whitespace, 15..16),
            new Token(TokenType.Identifier, 16..17),
            new Token(TokenType.EndOfText, 17..17)
        });
    }
}
