using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Expressions
{
    internal class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;

        public AssignmentExpressionSyntax(ExpressionSyntax left, SyntaxKind @operator, ExpressionSyntax right) : base()
        {
            Left = left;
            Operator = @operator;
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
        public SyntaxKind Operator { get; set; }
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