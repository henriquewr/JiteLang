
namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundUnaryExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.UnaryExpression;
        public BoundUnaryExpression(BoundNode parent) : base(parent)
        {
        }
    }
}
