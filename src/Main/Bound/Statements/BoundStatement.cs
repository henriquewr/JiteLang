﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Bound.Statements
{
    internal abstract class BoundStatement : BoundNode
    {
        public BoundStatement(BoundNode parent) : base(parent)
        {
        }
    }
}
