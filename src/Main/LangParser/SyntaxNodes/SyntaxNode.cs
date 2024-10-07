using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Syntax;

namespace JiteLang.Main.LangParser.SyntaxNodes
{
    internal abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }
        public SyntaxPosition Position { get; set; }
    }
}
