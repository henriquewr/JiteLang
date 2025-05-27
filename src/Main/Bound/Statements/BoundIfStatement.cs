using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Visitor.Type.Scope;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundIfStatement : BoundStatement
    {
        public override BoundKind Kind => BoundKind.IfStatement;

        public BoundIfStatement(
            BoundNode? parent,
            BoundExpression condition,
            BoundBlockStatement<BoundNode, TypeLocal> body,
            BoundElseStatement? @else = null) : base(parent)
        {
            Condition = condition;
            Body = body;
            Else = @else;
        }

        public BoundExpression Condition { get; set; }
        public BoundBlockStatement<BoundNode, TypeLocal> Body { get; set; }
        public BoundElseStatement? Else { get; set; }


        public override void SetParent()
        {
            Condition.Parent = this;
            Body.Parent = this;

            if (Else is not null)
            {
                Else.Parent = this;
            }
        }

        public override void SetParentRecursive()
        {
            Condition.Parent = this;
            Condition.SetParentRecursive();

            Body.Parent = this;
            Body.SetParentRecursive();

            if (Else is not null)
            {
                Else.Parent = this;
                Else.SetParentRecursive();
            }
        }
    }
}