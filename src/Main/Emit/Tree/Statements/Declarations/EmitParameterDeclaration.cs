using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitParameterDeclaration : EmitDeclaration
    {
        public override EmitKind Kind => EmitKind.ParameterDeclaration;
        public EmitParameterDeclaration(EmitNode parent, string name) : base(parent, name)
        {
        }
    }
}
