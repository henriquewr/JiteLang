using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundCastExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.CastExpression;
        public override TypeSymbol Type { get; set; }
        public BoundCastExpression(BoundNode? parent, BoundExpression value, TypeSymbol toType) : base(parent)
        {
            Value = value;
            Type = toType;
        }

        public override void SetParent()
        {
            Value.Parent = this;
        }

        public override void SetParentRecursive()
        {
            Value.Parent = this;
            Value.SetParentRecursive();
        }

        public BoundExpression Value { get; set; }
    }
}