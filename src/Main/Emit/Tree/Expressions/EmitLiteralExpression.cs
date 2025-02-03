using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitLiteralExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.LiteralExpression;
        public override TypeSymbol Type => Value.Type;
        public EmitLiteralExpression(EmitNode parent, ConstantValue value) : base(parent)
        {
            Value = value;
        }

        public ConstantValue Value { get; set; }
    }
}