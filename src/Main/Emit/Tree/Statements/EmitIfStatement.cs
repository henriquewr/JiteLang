using JiteLang.Main.Emit.Tree.Expressions;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitIfStatement : EmitStatement
    {
        public override EmitKind Kind => EmitKind.IfStatement;

        public EmitIfStatement(EmitNode parent, EmitExpression condition, EmitLabelStatement labelExit) : base(parent)
        {
            LabelExit = labelExit;
            ConditionStatement = new(this, condition, "ifEnd");
            Body = new(this);
        }

        public EmitIfStatement(EmitNode parent, EmitExpression condition) : base(parent)
        {
            LabelExit = EmitLabelStatement.Create(this, "ifExit");
            ConditionStatement = new(this, condition, "ifEnd");
            Body = new(this);
        }

        public EmitConditionStatement ConditionStatement { get; set; }
        public EmitBlockStatement<EmitNode> Body { get; set; }
        public EmitLabelStatement LabelExit { get; set; }
        public EmitElseStatement? Else { get; set; }
    }
}
