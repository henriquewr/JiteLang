using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundNamespaceDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.NamespaceDeclaration;

        public BoundNamespaceDeclaration(BoundIdentifierExpression identifier, BoundBlockStatement<BoundClassDeclaration> body) : base(identifier)
        {
            Body = body;
        }

        public BoundBlockStatement<BoundClassDeclaration> Body { get; set; }
    }
}
