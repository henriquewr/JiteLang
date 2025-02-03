using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Shared.Type.Members
{
    internal class FieldSymbol : TypedMemberSymbol
    {
        public FieldSymbol(string name, TypeSymbol type) : base(name, type)
        {
        }
    }
}
