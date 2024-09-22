using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundWhileStatement : BoundStatement
    {
        public override BoundKind Kind => BoundKind.WhileStatement;

        public BoundWhileStatement(BoundExpression condition, BoundBlockStatement<BoundNode> body)
        {
            Condition = condition;
            Body = body;
        }

        public BoundExpression Condition { get; set; }

        public BoundBlockStatement<BoundNode> Body { get; set; }
    }
}
