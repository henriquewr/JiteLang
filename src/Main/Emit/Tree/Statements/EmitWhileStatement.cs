using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.Emit.Tree.Utils;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitWhileStatement : EmitStatement
    {
        public override EmitKind Kind => EmitKind.WhileStatement;
        public EmitWhileStatement(EmitNode parent, EmitCondition conditionStatement, EmitJumpStatement jumpStart) : base(parent)
        {
            JumpStart = jumpStart;
            Condition = conditionStatement;
            Body = new(this);
        }  
        
        public EmitWhileStatement(EmitNode parent, EmitExpression condition) : base(parent)
        {
            EmitJumpStatement jumpStart = new(this, EmitLabelStatement.Create(null!, "whileStart"));
            jumpStart.Label.Parent = jumpStart;
            JumpStart = jumpStart;

            Condition = new(this, condition, "whileEnd");
            Body = new(this);
        }

        public EmitCondition Condition { get; set; }

        public EmitBlockStatement<EmitNode, CodeLocal> Body { get; set; }

        public EmitJumpStatement JumpStart { get; set; }
    }
}
