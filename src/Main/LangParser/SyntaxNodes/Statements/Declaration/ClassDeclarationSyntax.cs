using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class ClassDeclarationSyntax : DeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ClassDeclaration;

        public ClassDeclarationSyntax(IdentifierExpressionSyntax identifier, BlockStatement<SyntaxNode> body) : base(identifier)
        {
            Body = body;
        }

        public BlockStatement<SyntaxNode> Body { get; set; }

        public string GetFullName()
        {
            var parentNamespace = (NamespaceDeclarationSyntax)Parent.Parent!;
            return $"{parentNamespace.Identifier.Text}.{Identifier.Text}";
        }

        public override void SetParent()
        {
            Identifier.Parent = this;
            Body.Parent = this;
        }

        public override void SetParentRecursive()
        {
            Identifier.Parent = this;
            Body.Parent = this;

            Identifier.SetParentRecursive();
            Body.SetParentRecursive();
        }
    }
}