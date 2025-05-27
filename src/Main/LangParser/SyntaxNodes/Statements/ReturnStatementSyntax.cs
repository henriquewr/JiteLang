using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class ReturnStatementSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.ReturnStatement;

        public ReturnStatementSyntax() : base()
        {
        }

        public ReturnStatementSyntax(ExpressionSyntax? returnValue = null) : base()
        {
            ReturnValue = returnValue;
        }

        public ExpressionSyntax? ReturnValue { get; set; }

        public override void SetParent()
        {
            if (ReturnValue is not null) 
            {
                ReturnValue.Parent = this;
            }
        }

        public override void SetParentRecursive()
        {
            if (ReturnValue is not null)
            {
                ReturnValue.Parent = this;
                ReturnValue.SetParentRecursive();
            }
        }
    }
}