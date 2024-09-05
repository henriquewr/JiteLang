using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class ClassDeclarationSyntax : DeclarationSyntax
    {
        public ClassDeclarationSyntax(IdentifierExpressionSyntax identifier) : base(identifier)
        {
        }
        public override SyntaxKind Kind => SyntaxKind.ClassDeclaration;

        public BlockStatement<SyntaxNode> Body { get; set; } = new();
    }
}
