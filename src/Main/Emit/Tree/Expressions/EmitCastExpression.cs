using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitCastExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.CastExpression;
        public override TypeSymbol Type { get; set; }
        public EmitCastExpression(EmitNode? parent, TypeSymbol type) : base(parent)
        {
            Type = type;
        }
    }
}