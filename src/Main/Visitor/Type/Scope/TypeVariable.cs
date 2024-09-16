using JiteLang.Main.CodeAnalysis.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Visitor.Type.Scope
{
    internal class TypeVariable
    {
        public TypeVariable(LangType varType)
        {
            Type = varType;
        }
        public LangType Type { get; set; }
    }
}
