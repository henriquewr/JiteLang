using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundParameterDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.ParameterDeclaration;

        public BoundParameterDeclaration(BoundNode parent, BoundIdentifierExpression identifierExpression, TypeSymbol type) : base(parent, identifierExpression)
        {
            Type = type;
        }

        public TypeSymbol Type { get; set; }
    }
}
