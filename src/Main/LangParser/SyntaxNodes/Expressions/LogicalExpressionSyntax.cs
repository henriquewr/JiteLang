using JiteLang.Main.Shared;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class LogicalExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.LogicalExpression;

        public LogicalExpressionSyntax(ExpressionSyntax left, LogicalOperatorKind operation, ExpressionSyntax right) : base()
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public ExpressionSyntax Left
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
        public LogicalOperatorKind Operation { get; set; }
        public ExpressionSyntax Right
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