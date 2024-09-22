using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundClassDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.ClassDeclaration;

        public BoundClassDeclaration(BoundIdentifierExpression identifier, BoundBlockStatement<BoundNode> body) : base(identifier)
        {
            Body = body;
        }

        public BoundBlockStatement<BoundNode> Body { get; set; }
    }
}
