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

        public T? GetFirstOrDefaultOfType<T>()
        {
            var currentParent = Parent;

            while (currentParent != null)
            { 
                if (currentParent is T firstOfType)
                {
                    return firstOfType;
                }

                currentParent = currentParent.Parent;
            }

            return default;
        }
    }
}