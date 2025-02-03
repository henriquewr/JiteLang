using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Shared.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.AsmBuilder.Scope
{
    internal class CodeField : CodeVariable
    {
        public CodeField(TypeSymbol type, int location) : base(type)
        {
            Location = location;
        }

        public int Location { get; set; }
    }
}