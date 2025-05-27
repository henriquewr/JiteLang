using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Visitor.Type.Scope;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundNamespaceDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.NamespaceDeclaration;

        public BoundNamespaceDeclaration(BoundNode? parent, BoundIdentifierExpression identifier, BoundBlockStatement<BoundClassDeclaration, TypeVariable> body) : base(parent, identifier)
        {
            Body = body;
        }

        public override void SetParent()
        {
            Body.Parent = this;
            Identifier.Parent = this;
        }

        public override void SetParentRecursive()
        {
            SetParent();
            Body.SetParentRecursive();
            Identifier.SetParentRecursive();
        }

        public BoundBlockStatement<BoundClassDeclaration, TypeVariable> Body { get; set; }
    }
}