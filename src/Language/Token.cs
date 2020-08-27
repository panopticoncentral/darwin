using System;
using System.Diagnostics;

namespace Darwin.Language
{
    [DebuggerDisplay("{Type} {Range}")]
    internal readonly struct Token : IEquatable<Token>
    {
        public TokenType Type { get; }
        public Range Range { get; }

        public Token(TokenType type, Range range)
        {
            Type = type;
            Range = range;
        }

        public bool Equals(Token other) => Type == other.Type && Range.Equals(other.Range);

        public override bool Equals(object obj) => obj is Token other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Type, Range);
    }
}
