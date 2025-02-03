using JiteLang.Main.Shared.Type.Members.Method;
using System.Collections.Immutable;

namespace JiteLang.Main.Shared.Type.Members
{
    internal class CtorSymbol
    {
        public CtorSymbol(TypeSymbol type, IImmutableList<ParameterSymbol> parameters)
        {
            Type = type;
            Parameters = parameters;
        }

        public CtorSymbol(TypeSymbol type) : this(type, ImmutableList<ParameterSymbol>.Empty)
        {
            Type = type;
        }

        public TypeSymbol Type { get; set; }
        public IImmutableList<ParameterSymbol> Parameters { get; set; }
    }
}
