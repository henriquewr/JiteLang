using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class LocalDeclarationSyntax : VariableDeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.LocalDeclaration;

        public LocalDeclarationSyntax(IdentifierExpressionSyntax identifier, TypeSyntax type, ExpressionSyntax? initialValue = null) : base(identifier, type, initialValue)
        {
        }

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