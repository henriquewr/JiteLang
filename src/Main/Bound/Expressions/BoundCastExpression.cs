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

        public BoundExpression Value
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
    }
}