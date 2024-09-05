using System;
using JiteLang.Syntax;

namespace JiteLang.Main.LangLexer.Token
{
    internal struct TokenInfo : IEquatable<TokenInfo>
    {
        internal SyntaxKind Kind { get; set; }
        internal string Text { get; set; }

        internal string? StringValue { get; set; }
        internal char CharValue { get; set; }

        internal int IntValue { get; set; }
        internal uint UintValue { get; set; }
        internal long LongValue { get; set; }
        internal ulong UlongValue { get; set; }
        internal float FloatValue { get; set; }
        internal double DoubleValue { get; set; }
        internal decimal DecimalValue { get; set; }

        internal SyntaxPosition Position { get; set; }

        public readonly bool Equals(TokenInfo other)
        {
            var isSameKind = Kind == other.Kind;
            return isSameKind;
        }
    }
}
