using JiteLang.Main.CodeAnalysis.Types;
using System.Collections.Generic;

namespace JiteLang.Main.Visitor.Type.Scope
{
    internal class TypeMethod
    {
        public TypeMethod(LangType returnType, Dictionary<string, TypeMethodParameter> @params)
        {
            ReturnType = returnType;
            Params = @params;
        }

        public TypeMethod(LangType returnType) : this(returnType, new())
        {
        }

        public Dictionary<string, TypeMethodParameter> Params { get; set; }

        public LangType ReturnType { get; set; }
    }
}
