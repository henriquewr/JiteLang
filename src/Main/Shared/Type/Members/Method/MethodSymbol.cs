using System;

namespace JiteLang.Main.Shared.Type.Members.Method
{
    internal class MethodSymbol : TypedMemberSymbol
    {
        public MethodSymbol(string name, DelegateTypeSymbol type) : base(name, type)
        {
        }
    }
}