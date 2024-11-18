using JiteLang.Main.Emit.Tree.Expressions;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitWhileStatement : EmitStatement
    {
        public override EmitKind Kind => EmitKind.WhileStatement;
        public EmitWhileStatement(EmitNode parent, EmitConditionStatement conditionStatement, EmitJumpStatement jumpStart) : base(parent)
        {
            JumpStart = jumpStart;
            ConditionStatement = conditionStatement;
            Body = new(this);
        }  
        
        public EmitWhileStatement(EmitNode parent, EmitExpression condition) : base(parent)
        {
            EmitJumpStatement jumpStart = new(this, EmitLabelStatement.Create(null!, "whileStart"));
            jumpStart.Label.Parent = jumpStart;
            JumpStart = jumpStart;

            ConditionStatement = new(this, condition, "whileEnd");
            Body = new(this);
        }

        public EmitConditionStatement ConditionStatement { get; set; }

        public EmitBlockStatement<EmitNode> Body { get; set; }

        public EmitJumpStatement JumpStart { get; set; }
    }
}
