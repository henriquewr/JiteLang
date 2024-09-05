using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;

namespace JiteLang.Main.LangParser.SyntaxTree
{
    internal class ParsedSyntaxTree
    {
        public ParsedSyntaxTree() 
        {
            Errors = new List<string>();
        }

        public NamespaceDeclarationSyntax Root { get; set; }

        public bool HasErrors => Errors.Count > 0;
        public IList<string> Errors { get; set; }
    }
}
