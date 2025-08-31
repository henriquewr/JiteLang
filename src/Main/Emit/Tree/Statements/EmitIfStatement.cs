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

        public EmitCondition Condition
        {
            get;
            set
            {
                field = value;
                field?.ParentToSet = this;
            }
        }

        public EmitBlockStatement<EmitNode, CodeLocal> Body
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public EmitLabelStatement LabelExit
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public EmitElseStatement? Else
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public bool IsSingleIf => Parent!.Kind != EmitKind.ElseStatement && Else is null;
    }
}