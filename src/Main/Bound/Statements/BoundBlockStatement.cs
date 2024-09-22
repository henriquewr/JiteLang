using System.Collections.Generic;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundBlockStatement<TMembers> : BoundStatement where TMembers : BoundNode
    {
        public override BoundKind Kind => BoundKind.BlockStatement;

        public BoundBlockStatement(List<TMembers> members)
        {
            Members = members;
        }

        public BoundBlockStatement() : this(new())
        {
        }

        public List<TMembers> Members { get; set; }
    }
}
