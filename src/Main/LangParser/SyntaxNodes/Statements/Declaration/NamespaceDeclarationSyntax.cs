using System.Collections.Generic;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class NamespaceDeclarationSyntax : DeclarationSyntax
    {
        public NamespaceDeclarationSyntax(IdentifierExpressionSyntax identifier) : base(identifier)
        {
        }

        public override SyntaxKind Kind => SyntaxKind.NamespaceDeclaration;
        public BlockStatement<ClassDeclarationSyntax> Body { get; set; } = new();
    }
}
