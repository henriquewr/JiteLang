using JiteLang.Main.LangParser.Types;
using System.Collections.Generic;

namespace JiteLang.Main.AsmBuilder.Scope
{
    internal class CodeMethod
    {
        public CodeMethod(TypeSyntax returnType, Dictionary<string, CodeMethodParameter> @params)
        {
            ReturnType = returnType;
            Params = @params;
        }

        public CodeMethod(TypeSyntax returnType) : this(returnType, new())
        {
        }

        public Dictionary<string, CodeMethodParameter> Params { get; set; }
        public TypeSyntax ReturnType { get; set; }
    }
}
