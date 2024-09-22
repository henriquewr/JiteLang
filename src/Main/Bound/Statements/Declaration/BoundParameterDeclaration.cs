using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundParameterDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.ParameterDeclaration;

        public BoundParameterDeclaration(BoundIdentifierExpression identifierExpression, TypeSymbol type) : base(identifierExpression)
        {
            Type = type;
        }

        public TypeSymbol Type { get; set; }
    }
}
