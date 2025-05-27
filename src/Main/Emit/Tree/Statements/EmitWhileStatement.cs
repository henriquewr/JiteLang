using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.Emit.Tree.Utils;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitWhileStatement : EmitStatement
    {
        public override EmitKind Kind => EmitKind.WhileStatement;
        public EmitWhileStatement(EmitNode? parent, EmitCondition condition, EmitBlockStatement<EmitNode, CodeLocal> body) : base(parent)
        {
            JumpStart = new(this, null!);
            JumpStart.Label = EmitLabelStatement.Create(JumpStart, "whileStart");
            JumpStart.SetParent();
            Condition = condition;
            Body = body;
        }  
       
        public EmitCondition Condition { get; set; }

        public EmitBlockStatement<EmitNode, CodeLocal> Body { get; set; }

        public EmitJumpStatement JumpStart { get; set; }

        public override void SetParent()
        {
            Condition.Condition.Parent = this;
            Condition.JumpIfFalse.Parent = this;

            Body.Parent = this;
            JumpStart.Parent = this;
        }

        public override void SetParentRecursive()
        {
            Condition.Condition.Parent = this;
            Condition.JumpIfFalse.Parent = this;
            Condition.Condition.SetParentRecursive();
            Condition.JumpIfFalse.SetParentRecursive();

            Body.Parent = this;
            Body.SetParentRecursive();

            JumpStart.Parent = this;
            JumpStart.SetParentRecursive();
        }
    }
}