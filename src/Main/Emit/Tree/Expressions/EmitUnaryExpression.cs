using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitUnaryExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.UnaryExpression;
        public EmitUnaryExpression(EmitNode parent) : base(parent)
        {
            
        }
    }
}
