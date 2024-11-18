using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitAssignmentExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.AssignmentExpression;

        public EmitAssignmentExpression(EmitNode parent, EmitExpression left, EmitExpression right) : base(parent)
        {
            Left = left;
            Right = right;
        }

        public EmitExpression Left { get; set; }
        public EmitExpression Right { get; set; }
    }
}
