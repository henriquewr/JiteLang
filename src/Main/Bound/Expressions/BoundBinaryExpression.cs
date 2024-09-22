using JiteLang.Main.Shared;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundBinaryExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.BinaryExpression;

        public BoundBinaryExpression(BoundExpression left, BinaryOperatorKind operation, BoundExpression right)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public BoundExpression Left { get; set; }
        public BinaryOperatorKind Operation { get; set; }
        public BoundExpression Right { get; set; }

    }
}
