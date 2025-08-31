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

        public EmitExpression Condition
        {
            get;
            set
            {
                field = value;
                field?.Parent = ParentToSet;
            }
        }

        public EmitJumpStatement JumpIfFalse
        {
            get;
            set
            {
                field = value;
                field?.Parent = ParentToSet;
            }
        }

        public EmitNode? ParentToSet
        {
            get;
            set
            {
                field = value;
                JumpIfFalse?.Parent = field;
                Condition?.Parent = field;
            }
        }
    }
}