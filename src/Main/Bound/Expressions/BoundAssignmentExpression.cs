
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundAssignmentExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.AssignmentExpression;

        public override TypeSymbol Type
        {
            get
            {
                return Left.Type;
            }
            set
            {
                Left.Type = value;
            }
        }

        public BoundAssignmentExpression(BoundNode? parent, BoundExpression left, BoundKind @operator, BoundExpression right) : base(parent)
        {
            Right = right;
            Operator = @operator;
            Left = left;
        }

        public override void SetParent()
        {
            Right.Parent = this;
            Left.Parent = this;
        }

        public override void SetParentRecursive()
        {
            SetParent();

            Right.SetParentRecursive();
            Left.SetParentRecursive();
        }

        public BoundExpression Left { get; set; }
        public BoundKind Operator { get; set; }
        public BoundExpression Right { get; set; }
    }
}