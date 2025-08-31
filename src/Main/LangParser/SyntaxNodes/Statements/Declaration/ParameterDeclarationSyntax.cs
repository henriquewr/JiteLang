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
            Identifier = identifier;
        }

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