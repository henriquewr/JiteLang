using JiteLang.Main.Bound.Statements.Declaration;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class ClassDeclarationSyntax : DeclarationSyntax
    {
        public ClassDeclarationSyntax(SyntaxNode parent, IdentifierExpressionSyntax identifier) : base(parent, identifier)
        {
            Body = new(this);
        }
        public override SyntaxKind Kind => SyntaxKind.ClassDeclaration;

        public BlockStatement<SyntaxNode> Body { get; set; }

        public string GetFullName()
        {
            var parentNamespace = (NamespaceDeclarationSyntax)Parent.Parent!;
            return $"{parentNamespace.Identifier.Text}.{Identifier.Text}";
        }
    }
}
