using JiteLang.Main.Shared.Type;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace JiteLang.Main.Bound.Context.Conversions
{
    internal class ConversionTable
    {
        public ConversionTable() 
        {
        }

        private readonly Dictionary<(TypeSymbol From, TypeSymbol To), Conversion> _conversions = new(new TypeSymbolTupleEqualityComparer())
        {
            { (PredefinedTypeSymbol.Int, PredefinedTypeSymbol.Long), new(ConversionType.Implicit) },
            { (PredefinedTypeSymbol.Int, PredefinedTypeSymbol.Char), new(ConversionType.Explicit) },

            { (PredefinedTypeSymbol.Long, PredefinedTypeSymbol.Int), new(ConversionType.Explicit)  },
            { (PredefinedTypeSymbol.Long, PredefinedTypeSymbol.Char), new(ConversionType.Explicit)  },

            { (PredefinedTypeSymbol.Char, PredefinedTypeSymbol.Long), new(ConversionType.Implicit) },
            { (PredefinedTypeSymbol.Char, PredefinedTypeSymbol.Int), new(ConversionType.Implicit) },

            { (PredefinedTypeSymbol.String, PredefinedTypeSymbol.Object), new(ConversionType.Implicit) },
        };

        public bool TryGetImplicitConversion(TypeSymbol from, TypeSymbol to, [NotNullWhen(true)] out Conversion? conversion)
        {
            if (TryGetConversion(from, to, out conversion))
            {
                if (conversion.Type != ConversionType.Implicit)
                {
                    conversion = null;
                    return false;
                }

                return true;
            }

            return false;
        }

        public bool TryGetConversion(TypeSymbol from, TypeSymbol to, [NotNullWhen(true)] out Conversion? conversion)
        {
            if(_conversions.TryGetValue((from, to), out conversion))
            {
                return true;
            }

            if (from is ClassTypeSymbol fromClass && to is ClassTypeSymbol toClass)
            {
                if (fromClass.IsSubClassOf(toClass))
                {
                    conversion = new(ConversionType.Implicit);
                    return true;
                }
            }

            return false;
        }

        public bool HasConversion(TypeSymbol from, TypeSymbol to)
        {
            return TryGetConversion(from, to, out _);
        }
    }
}
