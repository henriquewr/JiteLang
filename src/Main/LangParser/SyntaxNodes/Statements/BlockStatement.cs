using System.Collections.Generic;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes.Statements
{
    internal class BlockStatement<TMembers> : StatementSyntax where TMembers : SyntaxNode
    {
        public override SyntaxKind Kind => SyntaxKind.BlockStatement;

        public IList<TMembers> Members { get; set; } = new List<TMembers>();
    }
}
