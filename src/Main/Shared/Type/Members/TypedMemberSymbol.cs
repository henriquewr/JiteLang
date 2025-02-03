using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Shared.Type.Members
{
    internal abstract class TypedMemberSymbol : MemberSymbol
    {
        public TypedMemberSymbol(string name, TypeSymbol type) : base(name)
        {
            Type = type;
        }

        public TypeSymbol Type { get; set; }
    }
}