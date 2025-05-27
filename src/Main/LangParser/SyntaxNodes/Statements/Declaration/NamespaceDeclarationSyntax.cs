using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class NamespaceDeclarationSyntax : DeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.NamespaceDeclaration;

        public NamespaceDeclarationSyntax(IdentifierExpressionSyntax identifier, BlockStatement<ClassDeclarationSyntax> body) : base(identifier)
        {
            Body = body;
        }

        public BlockStatement<ClassDeclarationSyntax> Body { get; set; }

        public override void SetParent()
        {
            Body.Parent = this;
        }

        public override void SetParentRecursive()
        {
            Body.Parent = this;
            Body.SetParentRecursive();
        }
    }
}
