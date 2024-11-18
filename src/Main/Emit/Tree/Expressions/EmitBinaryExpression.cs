using JiteLang.Main.Shared;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitBinaryExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.BinaryExpression;
        public EmitBinaryExpression(EmitNode parent, EmitExpression left, BinaryOperatorKind operation, EmitExpression right) : base(parent)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public EmitExpression Left { get; set; }
        public BinaryOperatorKind Operation { get; set; }
        public EmitExpression Right { get; set; }
    }
}
