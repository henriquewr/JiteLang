using System;

namespace JiteLang.Main.Shared.Type.Members.Method
{
    internal class ParameterSymbol
    {
        public ParameterSymbol(TypeSymbol type)
        {
            Type = type;
        }

        public TypeSymbol Type { get; }
    }
}