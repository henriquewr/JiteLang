using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal abstract class EmitStatement : EmitNode
    {
        public EmitStatement(EmitNode parent) : base(parent)
        {
        }
    }
}