using JiteLang.Main.Shared.Type.Members.Method;
using System;
using System.Collections.Immutable;

namespace JiteLang.Main.Shared.Type
{
    internal class DelegateTypeSymbol : TypeSymbol
    {
        public DelegateTypeSymbol(string fullText, string text, TypeSymbol returnType, IImmutableList<ParameterSymbol> parameters) : base(fullText, text, true)
        {
            ReturnType = returnType;
            Parameters = parameters;
        }

        public TypeSymbol ReturnType { get; set; }
        public IImmutableList<ParameterSymbol> Parameters { get; set; }
        public override int Size => 8;
    }
}