using System.Collections.Generic;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class BlockStatement<TMembers> : StatementSyntax where TMembers : SyntaxNode
    {
        public BlockStatement(SyntaxNode parent) : base(parent)
        {
        }

        public override SyntaxKind Kind => SyntaxKind.BlockStatement;

        public List<TMembers> Members { get; set; } = new List<TMembers>();
    }
}
