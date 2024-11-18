using System;
using System.Collections.Immutable;

namespace JiteLang.Main.Shared.Type.Members.Method
{
    internal class MethodSymbol : MemberSymbol
    {
        public MethodSymbol(Lazy<TypeSymbol> returnType, IImmutableList<ParameterSymbol> parameters)
        {
            ReturnType = returnType;
            Parameters = parameters;
        }

        public Lazy<TypeSymbol> ReturnType { get; set; }
        public IImmutableList<ParameterSymbol> Parameters { get; set; }
    }
}
