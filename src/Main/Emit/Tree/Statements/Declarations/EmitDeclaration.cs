﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal abstract class EmitDeclaration : EmitStatement
    {
        public EmitDeclaration(EmitNode parent, string name) : base(parent)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
