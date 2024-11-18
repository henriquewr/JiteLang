
using System.Linq.Expressions;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundAssignmentExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.AssignmentExpression;

        public BoundAssignmentExpression(BoundNode parent, BoundExpression left, BoundKind @operator, BoundExpression right) : base(parent)
        {
            Right = right;
            Operator = @operator;
            Left = left;
        }

        public BoundExpression Left { get; set; }
        public BoundKind Operator { get; set; }
        public BoundExpression Right { get; set; }

    }
}
