namespace JiteLang.Main.Bound.Statements
{
    internal abstract class BoundStatement : BoundNode
    {
        public BoundStatement(BoundNode? parent) : base(parent)
        {
        }
    }
}