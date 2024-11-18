using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree
{
    internal abstract class EmitNode
    {
        public EmitNode(EmitNode parent)
        {
            Parent = parent;
        }

        public EmitNode Parent { get; set; }

        public abstract EmitKind Kind { get; }
    }
}
