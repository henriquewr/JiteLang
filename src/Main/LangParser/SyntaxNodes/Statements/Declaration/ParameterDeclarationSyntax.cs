using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class ParameterDeclarationSyntax : VariableDeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ParameterDeclaration;

        public ParameterDeclarationSyntax(IdentifierExpressionSyntax identifier, TypeSyntax type) : base(identifier, type)
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