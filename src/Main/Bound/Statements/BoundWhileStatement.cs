using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundWhileStatement : BoundLoopStatement
    {
        public override BoundKind Kind => BoundKind.WhileStatement;

        public BoundWhileStatement(BoundNode parent,
            BoundExpression condition,
            BoundBlockStatement<BoundNode> body) : base(parent)
        {
            Condition = condition;
            Body = body;
        }

        public BoundWhileStatement(BoundNode parent,
            BoundExpression condition) : base(parent)
        {
            Condition = condition;
            Body = new(this);
        }

        public BoundExpression Condition { get; set; }
        public BoundBlockStatement<BoundNode> Body { get; set; }
    }
}
