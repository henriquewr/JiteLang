
namespace JiteLang.Main.Bound.Statements
{
    internal class BoundElseStatement : BoundStatement
    {
        public override BoundKind Kind => BoundKind.ElseStatement;

        public BoundElseStatement(BoundNode parent, BoundStatement @else) : base(parent)
        {
            Else = @else;
        }

        public BoundStatement Else { get; set; }
    }
}
