using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundMemberExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.MemberExpression;
        public BoundMemberExpression(BoundNode parent, BoundExpression left, BoundIdentifierExpression right) : base(parent)
        {
            Left = left;
            Right = right;
        }

        public BoundExpression Left { get; set; }
        public BoundIdentifierExpression Right { get; set; }
    }
}