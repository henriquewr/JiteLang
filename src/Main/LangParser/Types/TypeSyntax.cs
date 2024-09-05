using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.LangParser.Types
{
    internal abstract class TypeSyntax
    {
        public abstract bool IsPreDefined { get; }
    }
}
