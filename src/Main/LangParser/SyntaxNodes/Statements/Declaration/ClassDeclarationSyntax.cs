using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class ClassDeclarationSyntax : DeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ClassDeclaration;

        public ClassDeclarationSyntax(IdentifierExpressionSyntax identifier, BlockStatement<SyntaxNode> body) : base(identifier)
        {
            Identifier = identifier;
            Body = body;
        }

        public BlockStatement<SyntaxNode> Body
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public override IdentifierExpressionSyntax Identifier
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }


        public string GetFullName()
        {
            var parentNamespace = (NamespaceDeclarationSyntax)Parent.Parent!;
            return $"{parentNamespace.Identifier.Text}.{Identifier.Text}";
        }
    }
}