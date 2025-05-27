
namespace JiteLang.Main.Emit.Tree.Statements
{
    internal abstract class EmitStatement : EmitNode
    {
        public EmitStatement(EmitNode? parent) : base(parent)
        {
        }
    }
}