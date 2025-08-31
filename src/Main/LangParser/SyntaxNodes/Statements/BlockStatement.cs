using JiteLang.Syntax;
using JiteLang.Utilities;
using System.Collections.Generic;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class BlockStatement<TMembers> : StatementSyntax where TMembers : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.BlockStatement;

        public BlockStatement(IEnumerable<TMembers> members) : base()
        {
            Members = new(members);
        }

        public BlockStatement() : base()
        {
            Members = new();
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
    }
}