using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundLocalDeclaration : BoundVariableDeclaration
    {
        public override BoundKind Kind => BoundKind.LocalDeclaration;

        public BoundLocalDeclaration(BoundNode parent, BoundIdentifierExpression identifier, TypeSymbol type, BoundExpression? initialValue = null) : base(parent, identifier, type)
        {
            InitialValue = initialValue;
        }

        public BoundExpression? InitialValue { get; set; }
    }
}
