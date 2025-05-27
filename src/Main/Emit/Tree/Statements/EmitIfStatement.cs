using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.Emit.Tree.Utils;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitIfStatement : EmitStatement
    {
        public override EmitKind Kind => EmitKind.IfStatement;

        public EmitIfStatement(EmitNode? parent, EmitCondition condition, EmitBlockStatement<EmitNode, CodeLocal> body, EmitLabelStatement labelExit) : base(parent)
        {
            LabelExit = labelExit;
            Condition = condition;
            Body = body;
        }

        public EmitCondition Condition { get; set; }
        public EmitBlockStatement<EmitNode, CodeLocal> Body { get; set; }
        public EmitLabelStatement LabelExit { get; set; }
        public EmitElseStatement? Else { get; set; }
        public bool IsSingleIf => Parent!.Kind != EmitKind.ElseStatement && Else is null;

        public override void SetParent()
        {
            Condition.Condition.Parent = this;
            Condition.JumpIfFalse.Parent = this;

            Body.Parent = this;
            LabelExit.Parent = this;

            if (Else is not null)
            {
                Else.Parent = this;
            }
        }

        public override void SetParentRecursive()
        {
            Condition.Condition.Parent = this;
            Condition.JumpIfFalse.SetParentRecursive();
            Condition.Condition.Parent = this;
            Condition.JumpIfFalse.SetParentRecursive();

            Body.Parent = this;
            Body.SetParentRecursive();

            LabelExit.Parent = this;
            LabelExit.SetParentRecursive();

            if (Else is not null)
            {
                Else.Parent = this;
                Else.SetParentRecursive();
            }
        }
    }
}