using System.Collections.Generic;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;

namespace JiteLang.Main.LangParser.SyntaxTree
{
    internal class ParsedSyntaxTree
    {
        public ParsedSyntaxTree(HashSet<string> errors, NamespaceDeclarationSyntax root) 
        {
            Errors = errors;
            Root = root;
        }

        public NamespaceDeclarationSyntax Root { get; set; }

        public bool HasErrors => Errors.Count > 0;
        public HashSet<string> Errors { get; set; }
    }
}