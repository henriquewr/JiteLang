using JiteLang.Main.Emit.Tree.Expressions;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitConditionStatement : EmitStatement
    {
        public override EmitKind Kind => EmitKind.ConditionStatement;
        public EmitConditionStatement(EmitNode parent, EmitExpression condition, EmitJumpStatement jumpIfFalse) : base(parent)
        {
            Condition = condition;
            JumpIfFalse = jumpIfFalse;
        }

        public EmitConditionStatement(EmitNode parent, EmitExpression condition, string jumpIfFalseLabelName) : base(parent)
        {
            Condition = condition;

            EmitJumpStatement jumpIfFalse = new(this, EmitLabelStatement.Create(null!, jumpIfFalseLabelName));
            jumpIfFalse.Label.Parent = jumpIfFalse;
            JumpIfFalse = jumpIfFalse;
        }

        public EmitExpression Condition { get; set; }

        public EmitJumpStatement JumpIfFalse { get; set; }
    }
}
