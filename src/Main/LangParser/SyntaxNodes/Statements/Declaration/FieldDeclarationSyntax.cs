using JiteLang.Syntax;
using System.Collections.Generic;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class FieldDeclarationSyntax : DeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.FieldDeclaration;
        public FieldDeclarationSyntax(SyntaxNode parent, VariableDeclarationSyntax variableDeclarationSyntax, List<SyntaxToken> modifiers) : base(parent, variableDeclarationSyntax.Identifier)
        {
            Variable = variableDeclarationSyntax;
            Modifiers = new();
        }
        
        public FieldDeclarationSyntax(SyntaxNode parent, VariableDeclarationSyntax variableDeclarationSyntax) : this(parent, variableDeclarationSyntax, new())
        {   
        }

        public VariableDeclarationSyntax Variable { get; set; }
        public List<SyntaxToken> Modifiers { get; set; }
    }
}