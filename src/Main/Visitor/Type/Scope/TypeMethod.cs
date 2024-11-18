using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.Visitor.Type.Scope
{
    internal class TypeMethod
    {
        public TypeMethod(TypeSymbol returnType, Dictionary<string, TypeMethodParameter> @params)
        {
            ReturnType = returnType;
            Params = @params;
        }

        public TypeMethod(TypeSymbol returnType) : this(returnType, new())
        {
        }

        public Dictionary<string, TypeMethodParameter> Params { get; set; }

        public TypeSymbol ReturnType { get; set; }
    }
}
