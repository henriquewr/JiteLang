using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitMemberExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.MemberExpression;
        public override TypeSymbol Type => Right.Type;

        public EmitMemberExpression(EmitNode parent, EmitExpression left, EmitIdentifierExpression right) : base(parent)
        {
            Left = left;
            Right = right;
        }

        public EmitExpression Left { get; set; }
        public EmitIdentifierExpression Right { get; set; }
    }
}