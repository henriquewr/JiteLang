using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitLiteralExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.LiteralExpression;
        public override TypeSymbol Type { get; set; }
        public EmitLiteralExpression(EmitNode? parent, ConstantValue value) : base(parent)
        {
            Value = value;
            Type = value.Type;
        }

        public ConstantValue Value { get; set; }
    }
}