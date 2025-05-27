using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitElseStatement : EmitStatement
    {
        public override EmitKind Kind => EmitKind.ElseStatement;

        public EmitElseStatement(EmitNode? parent, EmitStatement @else, EmitLabelStatement labelExit) : base(parent)
        {
            Else = @else;
            LabelExit = labelExit;
        }

        public EmitStatement Else { get; set; }
        public EmitLabelStatement LabelExit { get; set; }

        public override void SetParent()
        {
            Else.Parent = this;
            LabelExit.Parent = this;
        }

        public override void SetParentRecursive()
        {
            Else.Parent = this;
            Else.SetParentRecursive();

            LabelExit.Parent = this;
            LabelExit.SetParentRecursive();
        }
    }
}