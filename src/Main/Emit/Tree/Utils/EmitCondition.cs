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

        public EmitExpression Condition { get; set; }

        public EmitJumpStatement JumpIfFalse { get; set; }
    }
}