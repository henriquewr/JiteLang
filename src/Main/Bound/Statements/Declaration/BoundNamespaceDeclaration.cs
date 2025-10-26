using JiteLang.Main.Visitor.Type.Scope;

namespace JiteLang.Main.Bound.Statements.Declaration
{
    internal class BoundNamespaceDeclaration : BoundStatement
    {
        public override BoundKind Kind => BoundKind.NamespaceDeclaration;

        public BoundNamespaceDeclaration(BoundNode? parent, string name, BoundBlockStatement<BoundClassDeclaration, TypeVariable> body) : base(parent)
        {
            Body = body;
            Name = name;
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

        public string Name { get; set; }
    }
}