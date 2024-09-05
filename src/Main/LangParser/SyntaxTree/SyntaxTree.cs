using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.LangParser.SyntaxTree
{
    internal class SyntaxTree
    {
        public SyntaxTree(SyntaxNode root)
        {
            Root = root;
        }

        public SyntaxNode Root { get; private init; }
    }
}
