using System.Collections.Generic;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class BlockStatement<TMembers> : StatementSyntax where TMembers : SyntaxNode
    {
        public BlockStatement(List<TMembers> members) : base()
        {
            Members = members;
        }

        public override SyntaxKind Kind => SyntaxKind.BlockStatement;

        public List<TMembers> Members { get; set; }

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