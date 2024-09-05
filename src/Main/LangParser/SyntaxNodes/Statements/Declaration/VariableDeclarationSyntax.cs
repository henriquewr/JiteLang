using System.Collections.Generic;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class VariableDeclarationSyntax : DeclarationSyntax
    {
        public VariableDeclarationSyntax(IdentifierExpressionSyntax identifier, TypeSyntax type) : base(identifier)
        {
            Type = type;
        }

        public VariableDeclarationSyntax(IdentifierExpressionSyntax identifier, TypeSyntax type, ExpressionSyntax initialValue) : this(identifier, type)
        {
            InitialValue = initialValue;
        }
        
        public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;
        public TypeSyntax Type { get; set; }
        public ExpressionSyntax? InitialValue { get; set; }
        public List<SyntaxToken> Modifiers { get; set; } = new();
    }
}
