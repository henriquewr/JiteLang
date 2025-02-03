using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.Visitor.Type.Scope
{
    internal class TypeMethod : TypeIdentifier
    {
        public TypeMethod(DelegateTypeSymbol methodType, string name, Dictionary<string, TypeMethodParameter> @params) : base(methodType, name)
        {
            Type = methodType;
            Params = @params;
        }

        public TypeMethod(DelegateTypeSymbol methodType, string name) : this(methodType, name, new())
        {
        }

        public Dictionary<string, TypeMethodParameter> Params { get; set; }

        public new DelegateTypeSymbol Type { get; set; }
    }
}