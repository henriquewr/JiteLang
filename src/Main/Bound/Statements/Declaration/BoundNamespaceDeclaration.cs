using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Visitor.Type.Scope;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundNamespaceDeclaration : BoundDeclaration
    {
        public override BoundKind Kind => BoundKind.NamespaceDeclaration;

        public BoundNamespaceDeclaration(BoundNode? parent, BoundIdentifierExpression identifier, BoundBlockStatement<BoundClassDeclaration, TypeVariable> body) : base(parent)
        {
            Body = body;
            Identifier = identifier;
        }

        public BoundBlockStatement<BoundClassDeclaration, TypeVariable> Body
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
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