using JiteLang.Main.Shared;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitLogicalExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.LogicalExpression;
        public EmitLogicalExpression(EmitNode parent, EmitExpression left, LogicalOperatorKind operation, EmitExpression right) : base(parent)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public EmitExpression Left { get; set; }
        public LogicalOperatorKind Operation { get; set; }
        public EmitExpression Right { get; set; }
    }
}
