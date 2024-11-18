using JiteLang.Main.Shared;
using JiteLang.Main.Visitor.Type.Scope;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundBlockStatement<TMembers> : BoundStatement, 
        IVarDeclarable<TypeVariable> where TMembers : BoundNode
    {
        public override BoundKind Kind => BoundKind.BlockStatement;

        public BoundBlockStatement(BoundNode parent, List<TMembers> members, Dictionary<string, TypeVariable> variables) : base(parent)
        {
            Members = members;
            Variables = variables;
        }

        public BoundBlockStatement(BoundNode parent) : this(parent, new(), new())
        {
        }

        public List<TMembers> Members { get; set; }
        public Dictionary<string, TypeVariable> Variables { get; set; }
    }
}
