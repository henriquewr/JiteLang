using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Shared.Type.Members
{
    internal class MemberSymbol
    {
        public MemberSymbol(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
