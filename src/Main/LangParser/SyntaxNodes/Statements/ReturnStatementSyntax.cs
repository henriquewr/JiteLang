using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class ReturnStatementSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ReturnStatement;

        public ReturnStatementSyntax(ExpressionSyntax? returnValue = null) : base()
        {
            ReturnValue = returnValue;
        }

        public ExpressionSyntax? ReturnValue
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