using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration
{
    internal class LocalDeclarationSyntax : VariableDeclarationSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.LocalDeclaration;

        public LocalDeclarationSyntax(IdentifierExpressionSyntax identifier, TypeSyntax type, ExpressionSyntax? initialValue = null) : base(identifier, type)
        {
            Identifier = identifier;
            InitialValue = initialValue;
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