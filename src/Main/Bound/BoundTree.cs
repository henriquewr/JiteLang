﻿using JiteLang.Main.Bound.Context;
using JiteLang.Main.Bound.Statements.Declaration;
using System.Collections.Generic;

namespace JiteLang.Main.Bound
{
    internal class BoundTree
    {
        public BoundTree(BoundNamespaceDeclaration root, List<string> errors)
        {
            Errors = errors;
            Root = root;
            BindingContext = new(new());
        }

        public BoundTree(BoundNamespaceDeclaration root) : this(root, new())
        {
            BindingContext = new(new());
        }

        public BindingContext BindingContext { get; set; }
        public BoundNamespaceDeclaration Root { get; set; }

        public bool HasErrors => Errors.Count > 0;
        public List<string> Errors { get; set; }
    }
}