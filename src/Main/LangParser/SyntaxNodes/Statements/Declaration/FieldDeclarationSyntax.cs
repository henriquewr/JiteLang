using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;
using System.Collections.Generic;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class FieldDeclarationSyntax : VariableDeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.FieldDeclaration;
        
        public FieldDeclarationSyntax(List<SyntaxToken> modifiers, IdentifierExpressionSyntax identifier, TypeSyntax type, ExpressionSyntax? initialValue = null) : base(identifier, type, initialValue)
        {
            Modifiers = modifiers;
        }

        public List<SyntaxToken> Modifiers { get; set; }

        public override void SetParent()
        {
            Identifier.Parent = this;

            if (InitialValue is not null)
            {
                InitialValue.Parent = this;
            }
        }

        public override void SetParentRecursive()
        {
            Identifier.Parent = this;
            Identifier.SetParentRecursive();

            if (InitialValue is not null)
            {
                InitialValue.Parent = this;
                InitialValue.SetParentRecursive();
            }
        }
    }
}