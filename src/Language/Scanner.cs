using System;
using System.Collections;
using System.Collections.Generic;

namespace Darwin.Language
{
    internal static class Scanner
    {
        // Optimize the ASCII range values
        public static bool IsWhitespace(char ch)
        {
            return ch == ' '
                || ch == '\t'
                || ch == '\v'
                || ch == '\f'
                || (ch > 127 && char.IsWhiteSpace(ch));
        }

        public static bool IsNewLine(char ch) => ch == '\r' || ch == '\n';

        private static Token ScanWhitespace(ReadOnlySpan<char> text, int offset)
        {
            var start = offset;

            while (offset < text.Length && IsWhitespace(text[offset]))
            {
                offset++;
            }

            return new Token(TokenType.Whitespace, start..offset);
        }

        private static Token ScanLineTerminator(ReadOnlySpan<char> text, int offset)
        {
            int ch = text[offset];
            var start = offset++;

            if (ch == '\r' && offset < text.Length && text[offset] == '\n')
            {
                offset++;
            }

            return new Token(TokenType.LineTerminator, start..offset);
        }

        private static Token ScanComment(ReadOnlySpan<char> text, int offset)
        {
            var start = offset++;

            while (offset < text.Length && !IsNewLine(text[offset]))
            {
                offset++;
            }

            return new Token(TokenType.Comment, start..offset);
        }

        private static Token ScanPunctuator(int offset, TokenType tokenType) => new Token(tokenType, offset..(offset + 1));

        public static Token Scan(ReadOnlySpan<char> text, int offset)
        {
            if (offset >= text.Length)
            {
                return new Token(TokenType.EndOfText, text.Length..text.Length);
            }

            var ch = text[offset];

            switch (ch)
            {
                case ' ':
                case '\t': // U+0009 CHARACTER TABULATION (Tab)
                case '\v': // U+000B LINE TABULATION (Vertical tab)
                case '\f': // U+000C FORM FEED
                    return ScanWhitespace(text, offset);

                case '\n': // U+000A LINE FEED
                case '\r': // U+000D CARRIAGE RETURN
                    return ScanLineTerminator(text, offset);

                case '#':
                    return ScanComment(text, offset);

                //case 'a':
                //case 'b':
                //case 'c':
                //case 'd':
                //case 'e':
                //case 'f':
                //case 'g':
                //case 'h':
                //case 'i':
                //case 'j':
                //case 'k':
                //case 'l':
                //case 'm':
                //case 'n':
                //case 'o':
                //case 'p':
                //case 'q':
                //case 'r':
                //case 's':
                //case 't':
                //case 'u':
                //case 'v':
                //case 'w':
                //case 'x':
                //case 'y':
                //case 'z':
                //case 'A':
                //case 'B':
                //case 'C':
                //case 'D':
                //case 'E':
                //case 'F':
                //case 'G':
                //case 'H':
                //case 'I':
                //case 'J':
                //case 'K':
                //case 'L':
                //case 'M':
                //case 'N':
                //case 'O':
                //case 'P':
                //case 'Q':
                //case 'R':
                //case 'S':
                //case 'T':
                //case 'U':
                //case 'V':
                //case 'W':
                //case 'X':
                //case 'Y':
                //case 'Z':
                //case '_':
                //    return ScanIdentifier(text, offset);

                //case '0':
                //case '1':
                //case '2':
                //case '3':
                //case '4':
                //case '5':
                //case '6':
                //case '7':
                //case '8':
                //case '9':
                //    return ScanNumericLiteral(text, offset);

                //case '\"':
                //    return ScanStringLiteral(text, offset);

                //case '`':
                //    return ScanDomainSpecificLiteral(text, offset);

                case ',':
                    return new Token(TokenType.Comma, offset..(offset + 1));

                case ':':
                    return new Token(TokenType.Colon, offset..(offset + 1));

                case ';':
                    return new Token(TokenType.Semicolon, offset..(offset + 1));

                case '(':
                    return new Token(TokenType.OpenParen, offset..(offset + 1));

                case ')':
                    return new Token(TokenType.CloseParen, offset..(offset + 1));

                case '{':
                    return new Token(TokenType.OpenBrace, offset..(offset + 1));

                case '}':
                    return new Token(TokenType.CloseBrace, offset..(offset + 1));

                case '[':
                    return new Token(TokenType.OpenBracket, offset..(offset + 1));

                case ']':
                    return new Token(TokenType.CloseBracket, offset..(offset + 1));

                //case '~':
                //case '!':
                //case '@':
                //case '$':
                //case '%':
                //case '^':
                //case '&':
                //case '*':
                //case '-':
                //case '+':
                //case '=':
                //case '|':
                //case '\\':
                //case '<':
                //case '>':
                //case '.':
                //case '/':
                //case '?':
                //    return ScanOperator(text, offset);

                //default:
                //    if (CharacterInfo.IsIdentifierStartCharacter(character))
                //    {
                //        goto case 'a';
                //    }

                //    TextWindow.AdvanceChar();

                //    if (_badTokenCount++ > 200)
                //    {
                //        // If we get too many characters that we cannot make sense of, absorb the rest of the input.
                //        var position = TextWindow.Position - 1;
                //        var end = TextWindow.Text.Length;
                //        var width = end - position;
                //        info.Text = TextWindow.Text.ToString(new TextSpan(position, width));
                //        TextWindow.Reset(end);
                //    }
                //    else
                //    {
                //        info.Text = TextWindow.GetText(true);
                //    }

                //    AddError(ErrorCode.UnexpectedCharacterError, info.Text);
                //    break;

                default:
                    if (ch > 127 && IsWhitespace(ch))
                    {
                        goto case '\t';
                    }

                    return new Token(TokenType.Error, -1..-1);
            }
        }
    }
}
