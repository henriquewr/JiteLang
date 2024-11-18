using JiteLang.Main.Shared;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitLiteralExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.LiteralExpression;
        public EmitLiteralExpression(EmitNode parent, ConstantValue value) : base(parent)
        {
            Value = value;
        }

        public ConstantValue Value { get; set; }
    }
}
