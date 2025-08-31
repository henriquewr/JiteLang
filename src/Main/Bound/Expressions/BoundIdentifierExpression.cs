using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundIdentifierExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.IdentifierExpression;
        public override TypeSymbol Type { get; set; }
        public BoundIdentifierExpression(BoundNode? parent, string text, TypeSymbol type) : base(parent)
        {
            Text = text;
            Type = type;
        }

        public string Text { get; set; }
    }
}