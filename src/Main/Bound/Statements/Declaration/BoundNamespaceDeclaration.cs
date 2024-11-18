using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundNamespaceDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.NamespaceDeclaration;

        public BoundNamespaceDeclaration(BoundNode parent, BoundIdentifierExpression identifier, BoundBlockStatement<BoundClassDeclaration> body) : base(parent, identifier)
        {
            Body = body;
        }

        public BoundNamespaceDeclaration(BoundNode parent, BoundIdentifierExpression identifier) : base(parent, identifier)
        {
            Body = new(this);
        }

        public BoundBlockStatement<BoundClassDeclaration> Body { get; set; }
    }
}
