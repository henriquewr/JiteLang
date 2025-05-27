using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Visitor.Type.Scope;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundWhileStatement : BoundLoopStatement
    {
        public override BoundKind Kind => BoundKind.WhileStatement;

        public BoundWhileStatement(
            BoundNode? parent,
            BoundExpression condition,
            BoundBlockStatement<BoundNode, TypeLocal> body) : base(parent)
        {
            Condition = condition;
            Body = body;
        }

        public BoundExpression Condition { get; set; }
        public BoundBlockStatement<BoundNode, TypeLocal> Body { get; set; }

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