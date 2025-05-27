using JiteLang.Main.Shared;
using JiteLang.Main.Visitor.Type.Scope;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundBlockStatement<TMembers, TVar> : BoundStatement, 
        IVarDeclarable<TVar> where TMembers : BoundNode
         where TVar : TypeVariable
    {
        public override BoundKind Kind => BoundKind.BlockStatement;

        public BoundBlockStatement(BoundNode? parent, List<TMembers> members, Dictionary<string, TVar> variables) : base(parent)
        {
            Members = members;
            Variables = variables;
        }

        public BoundBlockStatement(BoundNode? parent, List<TMembers> members) : this(parent, members, new())
        {
        }

        public List<TMembers> Members { get; set; }
        public Dictionary<string, TVar> Variables { get; set; }

        public override void SetParent()
        {
            foreach (var member in Members)
            {
                member.Parent = this;
            }
        }

        public override void SetParentRecursive()
        {
            foreach (var member in Members)
            {
                member.Parent = this;
                member.SetParentRecursive();
            }
        }
    }
}