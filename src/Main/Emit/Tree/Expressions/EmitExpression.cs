using JiteLang.Main.AsmBuilder.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal abstract class EmitExpression : EmitNode
    {
        public EmitExpression(EmitNode parent) : base(parent)
        {

        }
    }
}
