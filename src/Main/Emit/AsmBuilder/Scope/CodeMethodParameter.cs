using JiteLang.Main.LangParser.Types;
using JiteLang.Main.Shared.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.AsmBuilder.Scope
{
    internal class CodeMethodParameter
    {
        public CodeMethodParameter(TypeSymbol type)
        {
            Type = type;
        }

        public TypeSymbol Type { get; set; }
    }
}
