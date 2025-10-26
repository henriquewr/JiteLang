using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundParameterDeclaration : BoundVariableDeclaration
    {
        public override BoundKind Kind => BoundKind.ParameterDeclaration;

        public BoundParameterDeclaration(BoundNode? parent, BoundIdentifierExpression identifier, TypeSymbol type) : base(parent, identifier, type)
        {
            Identifier = identifier;
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