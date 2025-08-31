using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class NamespaceDeclarationSyntax : DeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.NamespaceDeclaration;

        public NamespaceDeclarationSyntax(IdentifierExpressionSyntax identifier, BlockStatement<ClassDeclarationSyntax> body) : base(identifier)
        {
            Identifier = identifier;
            Body = body;
        }

        public BlockStatement<ClassDeclarationSyntax> Body
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
    }
}
