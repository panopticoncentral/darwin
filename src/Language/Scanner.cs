using System;
using System.Globalization;

namespace Darwin.Language
{
    internal static class Scanner
    {
        // Optimize the ASCII range values
        public static bool IsWhitespace(char ch) =>
            ch == ' '
            || ch == '\t'
            || ch == '\v'
            || ch == '\f'
            || (ch > 127 && char.IsWhiteSpace(ch));

        public static bool IsNewLine(char ch) => ch == '\r' || ch == '\n';

        public static bool IsOperator(char ch) =>
            ch == '~'
            || ch == '!'
            || ch == '@'
            || ch == '$'
            || ch == '%'
            || ch == '^'
            || ch == '&'
            || ch == '*'
            || ch == '-'
            || ch == '+'
            || ch == '='
            || ch == '|'
            || ch == '\\'
            || ch == '<'
            || ch == '>'
            || ch == '.'
            || ch == '/'
            || ch == '?';

        public static bool IsIdentifierStartCharacter(char ch) =>
            ch == '_'
            || char.GetUnicodeCategory(ch) switch
            {
                UnicodeCategory.UppercaseLetter => true,
                UnicodeCategory.LowercaseLetter => true,
                UnicodeCategory.TitlecaseLetter => true,
                UnicodeCategory.ModifierLetter => true,
                UnicodeCategory.OtherLetter => true,
                UnicodeCategory.LetterNumber => true,
                _ => false
            };

        public static bool IsIdentifierCharacter(char ch) =>
            char.GetUnicodeCategory(ch) switch
            {
                UnicodeCategory.UppercaseLetter => true,
                UnicodeCategory.LowercaseLetter => true,
                UnicodeCategory.TitlecaseLetter => true,
                UnicodeCategory.ModifierLetter => true,
                UnicodeCategory.OtherLetter => true,
                UnicodeCategory.LetterNumber => true,
                UnicodeCategory.DecimalDigitNumber => true,
                UnicodeCategory.NonSpacingMark => true,
                UnicodeCategory.SpacingCombiningMark => true,
                UnicodeCategory.ConnectorPunctuation => true,
                _ => false
            };

        public static bool IsNumeric(char ch) => ch >= '0' && ch <= '9';

        public static bool IsHexNumeric(char ch) => IsNumeric(ch) || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');

        private static Token ScanWhitespace(TextCursor cursor)
        {
            while (cursor.Advance() && IsWhitespace(cursor.Current))
            {
            }

            return cursor.CreateToken(TokenType.Whitespace);
        }

        private static Token ScanLineTerminator(TextCursor cursor)
        {
            var first = cursor.Current;

            if (cursor.Advance() && first == '\r' && cursor.Current == '\n')
            {
                _ = cursor.Advance();
            }

            return cursor.CreateToken(TokenType.LineTerminator);
        }

        private static Token ScanComment(TextCursor cursor)
        {
            while (cursor.Advance() && !IsNewLine(cursor.Current))
            {
            }

            return cursor.CreateToken(TokenType.Comment);
        }

        private static Token ScanOperator(TextCursor cursor)
        {
            while (cursor.Advance() && IsOperator(cursor.Current))
            {
            }

            return cursor.CreateToken(TokenType.Operator);
        }

        private static Token ScanIdentifier(TextCursor cursor)
        {
            while (cursor.Advance() && IsIdentifierCharacter(cursor.Current))
            {
            }

            return cursor.CreateToken(TokenType.Identifier);
        }

        private static Token ScanNumericLiteral(TextCursor cursor)
        {
            var leadingZero = cursor.Current == '0';

            if (!cursor.Advance())
            {
                return cursor.CreateToken(TokenType.DecimalLiteral);
            }

            var location = cursor.Location;
            if (leadingZero 
                && (cursor.Current == 'x' || cursor.Current == 'X')
                && cursor.Advance()
                && IsHexNumeric(cursor.Current))
            {
                while (cursor.Advance() && IsHexNumeric(cursor.Current))
                {
                }

                return cursor.CreateToken(TokenType.HexadecimalLiteral);
            }
            else
            {
                cursor.Move(location);
            }

            while (IsNumeric(cursor.Current))
            {
                if (!cursor.Advance())
                {
                    return cursor.CreateToken(TokenType.DecimalLiteral);
                }
            }

            var isFloatingPoint = false;
            location = cursor.Location;

            if (cursor.Current == '.'
                && cursor.Advance()
                && IsNumeric(cursor.Current))
            {
                isFloatingPoint = true;

                while (IsNumeric(cursor.Current))
                {
                    if (!cursor.Advance())
                    {
                        return cursor.CreateToken(TokenType.FloatingPointLiteral);
                    }
                }
            }
            else
            {
                cursor.Move(location);
            }

            location = cursor.Location;
            if ((cursor.Current == 'e' || cursor.Current == 'E')
                && cursor.Advance()
                && (IsNumeric(cursor.Current)
                    || ((cursor.Current == '+' || cursor.Current == '-')
                        && cursor.Advance()
                        && IsNumeric(cursor.Current))))
            {
                isFloatingPoint = true;
                while (IsNumeric(cursor.Current))
                {
                    if (!cursor.Advance())
                    {
                        return cursor.CreateToken(TokenType.FloatingPointLiteral);
                    }
                }
            }
            else
            {
                cursor.Move(location);
            }

            return cursor.CreateToken(isFloatingPoint ? TokenType.FloatingPointLiteral :TokenType.DecimalLiteral);
        }

        public static Token Scan(ReadOnlySpan<char> text, int offset)
        {
            var cursor = new TextCursor(text, offset);

            if (cursor.AtEndOfSpan)
            {
                return cursor.CreateToken(TokenType.EndOfText);
            }

            switch (cursor.Current)
            {
                case ' ':
                case '\t': // U+0009 CHARACTER TABULATION (Tab)
                case '\v': // U+000B LINE TABULATION (Vertical tab)
                case '\f': // U+000C FORM FEED
                    return ScanWhitespace(cursor);

                case '\n': // U+000A LINE FEED
                case '\r': // U+000D CARRIAGE RETURN
                    return ScanLineTerminator(cursor);

                case '#':
                    return ScanComment(cursor);

                case >= 'a' and <= 'z':
                case >= 'A' and <= 'Z':
                case '_':
                    return ScanIdentifier(cursor);

                case >= '0' and <= '9':
                    return ScanNumericLiteral(cursor);

                //case '\"':
                //    return ScanStringLiteral(cursor);

                //case '`':
                //    return ScanDomainSpecificLiteral(cursor);

                case ',':
                    _ = cursor.Advance();
                    return cursor.CreateToken(TokenType.Comma);

                case ':':
                    _ = cursor.Advance();
                    return cursor.CreateToken(TokenType.Colon);

                case ';':
                    _ = cursor.Advance();
                    return cursor.CreateToken(TokenType.Semicolon);

                case '(':
                    _ = cursor.Advance();
                    return cursor.CreateToken(TokenType.OpenParen);

                case ')':
                    _ = cursor.Advance();
                    return cursor.CreateToken(TokenType.CloseParen);

                case '{':
                    _ = cursor.Advance();
                    return cursor.CreateToken(TokenType.OpenBrace);

                case '}':
                    _ = cursor.Advance();
                    return cursor.CreateToken(TokenType.CloseBrace);

                case '[':
                    _ = cursor.Advance();
                    return cursor.CreateToken(TokenType.OpenBracket);

                case ']':
                    _ = cursor.Advance();
                    return cursor.CreateToken(TokenType.CloseBracket);

                case '~':
                case '!':
                case '@':
                case '$':
                case '%':
                case '^':
                case '&':
                case '*':
                case '-':
                case '+':
                case '=':
                case '|':
                case '\\':
                case '<':
                case '>':
                case '.':
                case '/':
                case '?':
                    return ScanOperator(cursor);

                case > (char)127:
                    if (IsWhitespace(cursor.Current))
                    {
                        goto case '\t';
                    }

                    if (IsIdentifierStartCharacter(cursor.Current))
                    {
                        return ScanIdentifier(cursor);
                    }
                    goto default;

                default:
                    return cursor.CreateToken(TokenType.Error);
            }
        }

        private ref struct TextCursor
        {
            private readonly int _start;
            private readonly ReadOnlySpan<char> _span;

            public Range Range => _start..Location;

            public int Location { get; private set; }

            public int Length => Location - _start;

            public bool AtEndOfSpan => Location == _span.Length;

            public char Current => AtEndOfSpan ? throw new InvalidOperationException() : _span[Location];

            public char? Peek() => (AtEndOfSpan || Location == _span.Length - 1) ? null : _span[Location + 1];

            public bool Advance() => ++Location < _span.Length;

            public void Move(int location) => Location = location;

            public TextCursor(ReadOnlySpan<char> span, int start)
            {
                _span = span;
                _start = Location = start;
            }

            public Token CreateToken(TokenType type) => new(type, Range);
        }
    }
}
