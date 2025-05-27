using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundLogicalExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.LogicalExpression;
        public override TypeSymbol Type { get; set; }

        public BoundLogicalExpression(BoundNode? parent, BoundExpression left, LogicalOperatorKind operation, BoundExpression right, TypeSymbol type) : base(parent)
        {
            Left = left;
            Operation = operation;
            Right = right;
            Type = type;
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
        public LogicalOperatorKind Operation { get; set; }
        public BoundExpression Right { get; set; }
    }
}
