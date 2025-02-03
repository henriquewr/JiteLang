using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.Emit.Tree.Statements;

namespace JiteLang.Main.Emit.Tree.Utils
{
    internal class EmitCondition
    {
        public EmitCondition(EmitExpression condition, EmitJumpStatement jumpIfFalse)
        {
            Condition = condition;
            JumpIfFalse = jumpIfFalse;
        }

        public EmitCondition(EmitNode parent, EmitExpression condition, string jumpIfFalseLabelName)
        {
            Condition = condition;

            EmitJumpStatement jumpIfFalse = new(parent, EmitLabelStatement.Create(null!, jumpIfFalseLabelName));
            jumpIfFalse.Label.Parent = jumpIfFalse;
            JumpIfFalse = jumpIfFalse;
        }

        public EmitExpression Condition { get; set; }

        public EmitJumpStatement JumpIfFalse { get; set; }
    }
}