using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.Emit.Tree.Statements;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitNewExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.NewExpression;
        public override TypeSymbol Type { get; }

        public EmitNewExpression(EmitNode parent, TypeSymbol type) : base(parent)
        {
            Type = type;
            Initializer = new(this);
        }

        public EmitBlockStatement<EmitNode, CodeLocal> Initializer { get; set; }
    }
}