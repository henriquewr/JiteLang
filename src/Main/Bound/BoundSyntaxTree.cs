using JiteLang.Main.Bound.Statements.Declaration;
using System.Collections.Generic;

namespace JiteLang.Main.Bound
{
    internal class BoundSyntaxTree
    {
        public BoundSyntaxTree(BoundNamespaceDeclaration root, List<string> errors)
        {
            Errors = errors;
            Root = root;
        }

        public BoundSyntaxTree(BoundNamespaceDeclaration root) : this(root, new())
        {
        }


        public BoundNamespaceDeclaration Root { get; set; }

        public bool HasErrors => Errors.Count > 0;
        public List<string> Errors { get; set; }
    }
}
