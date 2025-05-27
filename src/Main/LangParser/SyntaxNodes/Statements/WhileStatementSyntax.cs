using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class WhileStatementSyntax : StatementSyntax
    {
        public override SyntaxKind Kind => SyntaxKind.WhileStatement;

        public WhileStatementSyntax(ExpressionSyntax condition, BlockStatement<SyntaxNode> body) : base()
        {
            Condition = condition;
            Body = body;
        }

        public ExpressionSyntax Condition { get; set; }

        public BlockStatement<SyntaxNode> Body { get; set; }

        public override void SetParent()
        {
            Condition.Parent = this;
            Body.Parent = this;
        }

        public override void SetParentRecursive()
        {
            SetParent();

            Condition.SetParentRecursive();
            Body.SetParentRecursive();
        }
    }
}
