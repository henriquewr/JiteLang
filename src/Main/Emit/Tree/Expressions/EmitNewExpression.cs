using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.Emit.Tree.Statements;
using JiteLang.Main.Shared.Type;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitNewExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.NewExpression;
        public override TypeSymbol Type { get; set; }

        public EmitNewExpression(EmitNode? parent, TypeSymbol type, EmitBlockStatement<EmitNode, CodeLocal> initializer) : base(parent)
        {
            Type = type;
            Initializer = initializer;
        }

        public EmitBlockStatement<EmitNode, CodeLocal> Initializer
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