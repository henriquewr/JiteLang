using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundClassDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.ClassDeclaration;

        public BoundClassDeclaration(BoundNode parent, BoundIdentifierExpression identifier, BoundBlockStatement<BoundNode> body) : base(parent, identifier)
        {
            Body = body;
        }

        public BoundClassDeclaration(BoundNode parent, BoundIdentifierExpression identifier) : base(parent, identifier)
        {
            Body = new(this);
        }

        public BoundBlockStatement<BoundNode> Body { get; set; }
    }
}
