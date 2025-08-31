using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundParameterDeclaration : BoundVariableDeclaration
    {
        public override BoundKind Kind => BoundKind.ParameterDeclaration;

        public BoundParameterDeclaration(BoundNode? parent, BoundIdentifierExpression identifierExpression, TypeSymbol type) : base(parent, type)
        {
            Identifier = identifierExpression;
        }

        public override BoundIdentifierExpression Identifier
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