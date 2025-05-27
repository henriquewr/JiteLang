using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace JiteLang.Main.Shared.Type
{
    internal abstract class TypeSymbol : IEquatable<TypeSymbol>
    {
        public TypeSymbol(string fullText, string text, bool isReferenceType)
        {
            FullText = fullText;
            Text = text;
            IsReferenceType = isReferenceType;
        }

        public abstract int Size { get; }

        public string FullText { get; set; }
        public string Text { get; set; }

        public bool IsReferenceType { get; }

        public bool IsEqualsNotError(TypeSymbol? type)
        {
            return IsEqualsNotError(this, type);
        }

        public static bool IsEqualsNotError(TypeSymbol? x, TypeSymbol? y)
        {
            var isSame = x?.FullText == y?.FullText;
            var isSameNotNone = isSame && x?.FullText != ErrorTypeSymbol.Instance.FullText;

            return isSameNotNone;
        }

        public bool Equals(TypeSymbol? other)
        {
            return FullText == other?.FullText;
        }

        public bool IsError()
        {
            return this.Equals(ErrorTypeSymbol.Instance);
        }
    }

    internal class TypeSymbolEqualityComparer : IEqualityComparer<TypeSymbol>
    {
        public bool Equals(TypeSymbol? x, TypeSymbol? y)
        {
            return TypeSymbol.IsEqualsNotError(x, y);
        }

        public int GetHashCode(TypeSymbol obj)
        {
            return obj?.FullText?.GetHashCode() ?? 0;
        }
    }

    internal class TypeSymbolTupleEqualityComparer : IEqualityComparer<(TypeSymbol from, TypeSymbol to)>
    {
        public bool Equals((TypeSymbol from, TypeSymbol to) x, (TypeSymbol from, TypeSymbol to) y)
        {
            var xIsEquals = TypeSymbol.IsEqualsNotError(x.from, x.to);
            var yIsEquals = TypeSymbol.IsEqualsNotError(y.from, y.to);

            return xIsEquals == yIsEquals;
        }

        public int GetHashCode([DisallowNull] (TypeSymbol from, TypeSymbol to) obj)
        {
            return obj.GetHashCode();
        }
    }
}
