using JiteLang.Main.Shared.Type.Members.Method;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace JiteLang.Main.Shared.Type
{
    internal class TypeSymbol : IEquatable<TypeSymbol>
    {
        public TypeSymbol(string text, bool isReferenceType, TypeSymbol subClass, IImmutableDictionary<string, MethodSymbol> methods)
        {
            Text = text;
            Methods = methods;
            IsReferenceType = isReferenceType;
            SubClass = subClass;
        }


        public TypeSymbol(string text, bool isReferenceType, IImmutableDictionary<string, MethodSymbol> methods)
        {
            Text = text;
            Methods = methods;
            IsReferenceType = isReferenceType;

            if (IsReferenceType)
            {
                SubClass = PredefinedTypeSymbol.Object;
            }
        }

        public TypeSymbol? SubClass { get; init; }

        public bool IsSubClassOf(TypeSymbol typeSymbol)
        {
            var currentSubClass = SubClass;

            while (currentSubClass != null)
            {
                if (currentSubClass.Equals(typeSymbol))
                {
                    return true;
                }

                currentSubClass = currentSubClass?.SubClass;
            }

            return false;
        }

        public string Text { get; set; }
        public bool IsReferenceType { get; set; }

        public static TypeSymbol None => new("?", false, ImmutableDictionary<string, MethodSymbol>.Empty);

        public bool IsEqualsNotNone(TypeSymbol? type)
        {
            return IsEqualsNotNone(this, type);
        }

        public static bool IsEqualsNotNone(TypeSymbol? x, TypeSymbol? y)
        {
            var isSame = x?.Text == y?.Text;
            var isSameNotNone = isSame && x?.Text != TypeSymbol.None.Text;

            return isSameNotNone;
        }

        public bool Equals(TypeSymbol? other)
        {
            return Text == other?.Text;
        }

        public IImmutableDictionary<string, MethodSymbol> Methods { get; set; }
    }

    internal class TypeSymbolEqualityComparer : IEqualityComparer<TypeSymbol>
    {
        public bool Equals(TypeSymbol? x, TypeSymbol? y)
        {
            return TypeSymbol.IsEqualsNotNone(x, y);
        }

        public int GetHashCode(TypeSymbol obj)
        {
            return obj?.Text?.GetHashCode() ?? 0;
        }
    }

    internal class TypeSymbolTupleEqualityComparer : IEqualityComparer<(TypeSymbol from, TypeSymbol to)>
    {
        public bool Equals((TypeSymbol from, TypeSymbol to) x, (TypeSymbol from, TypeSymbol to) y)
        {
            var xIsEquals = TypeSymbol.IsEqualsNotNone(x.from, x.to);
            var yIsEquals = TypeSymbol.IsEqualsNotNone(y.from, y.to);

            return xIsEquals == yIsEquals;
        }

        public int GetHashCode([DisallowNull] (TypeSymbol from, TypeSymbol to) obj)
        {
            return obj.GetHashCode();
        }
    }
}
