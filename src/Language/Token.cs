using System;

namespace Darwin.Language
{
    internal readonly record struct Token(TokenType Type, Range Range);
}
