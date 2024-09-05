using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Syntax;

namespace JiteLang.Main.Built
{
    internal abstract class BuiltNode
    {
        public abstract BuiltKind Kind { get; }
    }
}
