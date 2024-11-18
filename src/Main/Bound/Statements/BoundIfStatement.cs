using JiteLang.Main.Bound.Expressions;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundIfStatement : BoundStatement
    {
        public override BoundKind Kind => BoundKind.IfStatement;

        public BoundIfStatement(BoundNode parent,
            BoundExpression condition,
            BoundBlockStatement<BoundNode> body,
            BoundElseStatement? @else = null) : base(parent)
        {
            Condition = condition;
            Body = body;
            Else = @else;
        }

        public BoundIfStatement(BoundNode parent,
           BoundExpression condition,
           BoundElseStatement? @else = null) : base(parent)
        {
            Condition = condition;
            Body = new(this);
            Else = @else;
        }

        public BoundExpression Condition { get; set; }
        public BoundBlockStatement<BoundNode> Body { get; set; }
        public BoundElseStatement? Else { get; set; }
    }
}
