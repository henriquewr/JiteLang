using JiteLang.Main.Shared.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Visitor.Type.Scope
{
    internal class TypeField : TypeVariable
    {
        public TypeField(TypeSymbol varType, string name) : base(varType, name)
        {
        }
    }
}
