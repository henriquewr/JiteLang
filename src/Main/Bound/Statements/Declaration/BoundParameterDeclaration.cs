using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundParameterDeclaration : BoundVariableDeclaration
    {
        public override BoundKind Kind => BoundKind.ParameterDeclaration;

        public BoundParameterDeclaration(BoundNode? parent, BoundIdentifierExpression identifierExpression, TypeSymbol type) : base(parent, identifierExpression, type)
        {
        }

        public override void SetParent()
        {
            Identifier.Parent = this;
        }

        public override void SetParentRecursive()
        {
            SetParent();
            Identifier.SetParentRecursive();
        }
    }
}