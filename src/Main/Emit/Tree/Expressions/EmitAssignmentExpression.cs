using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitAssignmentExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.AssignmentExpression;
        public override TypeSymbol Type => Left.Type;

        public EmitAssignmentExpression(EmitNode parent, EmitExpression left, EmitExpression right) : base(parent)
        {
            Left = left;
            Right = right;
        }

        public EmitExpression Left { get; set; }
        public EmitExpression Right { get; set; }
    }
}