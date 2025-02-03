using JiteLang.Main.Shared;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class LogicalExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.LogicalExpression;

        public LogicalExpressionSyntax(SyntaxNode parent, ExpressionSyntax left, LogicalOperatorKind operation, ExpressionSyntax right) : base(parent)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public ExpressionSyntax Left { get; set; }
        public LogicalOperatorKind Operation { get; set; }
        public ExpressionSyntax Right { get; set; }
    }
}
