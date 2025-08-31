using JiteLang.Main.Emit.AsmBuilder.Scope;
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

        public EmitJumpStatement JumpStart
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
    }
}