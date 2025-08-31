using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;
using System.Collections.Generic;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class FieldDeclarationSyntax : VariableDeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.FieldDeclaration;
        
        public FieldDeclarationSyntax(List<SyntaxToken> modifiers, IdentifierExpressionSyntax identifier, TypeSyntax type, ExpressionSyntax? initialValue = null) : base(identifier, type)
        {
            Modifiers = modifiers;
            Identifier = identifier;
        }

        public List<SyntaxToken> Modifiers { get; set; }
        public override ExpressionSyntax? InitialValue
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