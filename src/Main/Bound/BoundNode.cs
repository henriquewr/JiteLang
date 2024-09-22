using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Syntax;

namespace JiteLang.Main.Bound
{
    internal abstract class BoundNode
    {
        public abstract BoundKind Kind { get; }
        public SyntaxPosition Position { get; set; }
    }
}
