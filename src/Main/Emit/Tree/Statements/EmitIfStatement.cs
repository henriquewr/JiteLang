using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.Emit.Tree.Utils;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitIfStatement : EmitStatement
    {
        public override EmitKind Kind => EmitKind.IfStatement;

        public EmitIfStatement(EmitNode parent, EmitExpression condition, EmitLabelStatement labelExit) : base(parent)
        {
            LabelExit = labelExit;
            Condition = new(this, condition, "ifEnd");
            Body = new(this);
        }

        public EmitIfStatement(EmitNode parent, EmitExpression condition) : base(parent)
        {
            LabelExit = EmitLabelStatement.Create(this, "ifExit");
            Condition = new(this, condition, "ifEnd");
            Body = new(this);
        }

        public EmitCondition Condition { get; set; }
        public EmitBlockStatement<EmitNode, CodeLocal> Body { get; set; }
        public EmitLabelStatement LabelExit { get; set; }
        public EmitElseStatement? Else { get; set; }
        public bool IsSingleIf => Parent.Kind != EmitKind.ElseStatement && Else is null;
    }
}
