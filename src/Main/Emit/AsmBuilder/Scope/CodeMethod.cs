using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.AsmBuilder.Scope
{
    internal class CodeMethod
    {
        public CodeMethod(TypeSymbol returnType, Dictionary<string, CodeMethodParameter> @params)
        {
            ReturnType = returnType;
            Params = @params;
        }

        public CodeMethod(TypeSymbol returnType) : this(returnType, new())
        {
        }

        public Dictionary<string, CodeMethodParameter> Params { get; set; }
        public TypeSymbol ReturnType { get; set; }
    }
}
