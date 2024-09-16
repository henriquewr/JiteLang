using JiteLang.Main.CodeAnalysis.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Visitor.Type.Scope
{
    internal class TypeMethodParameter : TypeVariable
    {
        public TypeMethodParameter(LangType varType) : base(varType)
        {
        }
    }
}
