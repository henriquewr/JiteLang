using JiteLang.Main.Shared;
using JiteLang.Main.Visitor.Type.Scope;
using JiteLang.Utilities;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.Statements
{
    internal class BoundBlockStatement<TMembers, TVar> : BoundStatement, 
        IVarDeclarable<TVar> where TMembers : BoundNode
        where TVar : TypeVariable
    {
        public override BoundKind Kind => BoundKind.BlockStatement;

        public BoundBlockStatement(BoundNode? parent, NotifyAddList<TMembers> members, Dictionary<string, TVar> variables) : base(parent)
        {
            Members = members;
            Variables = variables;
        }

        public BoundBlockStatement(BoundNode? parent, NotifyAddList<TMembers> members) : this(parent, members, new())
        {
        }
        public BoundBlockStatement(NotifyAddList<TMembers> members) : this(null, members, new())
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