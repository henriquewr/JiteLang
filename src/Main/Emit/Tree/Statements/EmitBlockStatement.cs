using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Shared;
using System.Collections.Generic;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitBlockStatement<TMembers, TVar> : EmitStatement,
        IVarDeclarable<TVar> where TMembers : EmitNode
        where TVar : CodeVariable
    {
        public override EmitKind Kind => EmitKind.BlockStatement;
        public EmitBlockStatement(EmitNode? parent, List<TMembers> members, Dictionary<string, TVar> variables) : base(parent)
        {
            Members = members;
            Variables = variables;
        }

        public EmitBlockStatement(EmitNode? parent, List<TMembers> members) : this(parent, members, new())
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