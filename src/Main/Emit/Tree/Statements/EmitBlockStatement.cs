using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Shared;
using JiteLang.Utilities;
using System.Collections.Generic;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitBlockStatement<TMembers, TVar> : EmitStatement,
        IVarDeclarable<TVar> where TMembers : EmitNode
        where TVar : CodeVariable
    {
        public override EmitKind Kind => EmitKind.BlockStatement;
        public EmitBlockStatement(EmitNode? parent, NotifyAddList<TMembers> members, Dictionary<string, TVar> variables) : base(parent)
        {
            Members = members;
            Variables = variables;
        }

        public EmitBlockStatement(EmitNode? parent, NotifyAddList<TMembers> members) : this(parent, members, new())
        {
        }

        protected void OnAdd(TMembers item)
        {
            item.Parent = this;
        }

        public NotifyAddList<TMembers> Members
        {
            get;
            set
            {
                field?.OnAdd -= OnAdd;
                field = value;

                if (field is not null)
                {
                    field.OnAdd += OnAdd;

                    foreach (var member in Members)
                    {
                        OnAdd(member);
                    }
                }
            }
        }

        public Dictionary<string, TVar> Variables { get; set; }
    }
}