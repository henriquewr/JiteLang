using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundBinaryExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.BinaryExpression;
        public override TypeSymbol Type { get; set; }
        public BoundBinaryExpression(BoundNode? parent, BoundExpression left, BinaryOperatorKind operation, BoundExpression right, TypeSymbol type) : base(parent)
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
        public BinaryOperatorKind Operation { get; set; }
        public BoundExpression Right { get; set; }
    }
}