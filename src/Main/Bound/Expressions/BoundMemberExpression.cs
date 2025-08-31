using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundMemberExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.MemberExpression;
        public override TypeSymbol Type { get; set; }

        public BoundMemberExpression(BoundNode? parent, BoundExpression left, BoundIdentifierExpression right, TypeSymbol type) : base(parent)
        {
            Left = left;
            Right = right;
            Type = type;
        }

        public BoundExpression Left
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
        public BoundIdentifierExpression Right
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