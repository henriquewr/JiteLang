using System.Collections.Generic;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class NamespaceDeclarationSyntax : DeclarationSyntax
    {
        public NamespaceDeclarationSyntax(SyntaxNode parent, IdentifierExpressionSyntax identifier) : base(parent, identifier)
        {
            Body = new(this);
        }

        public override SyntaxKind Kind => SyntaxKind.NamespaceDeclaration;
        public BlockStatement<ClassDeclarationSyntax> Body { get; set; }
    }
}
