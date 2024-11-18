using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Shared.Type.Members.Method
{
    internal class ParameterSymbol
    {
        public ParameterSymbol(Lazy<TypeSymbol> type)
        {
            Type = type;
        }

        public Lazy<TypeSymbol> Type { get; }
    }
}
