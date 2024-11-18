using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitJumpStatement : EmitStatement
    {
        public override EmitKind Kind => EmitKind.JumpStatement;
        public EmitJumpStatement(EmitNode parent, EmitLabelStatement label) : base(parent)
        {
            Label = label;
        }

        public EmitLabelStatement Label { get; set; }
    }
}
