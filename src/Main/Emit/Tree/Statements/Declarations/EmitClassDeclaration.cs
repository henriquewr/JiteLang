using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitClassDeclaration : EmitDeclaration
    {
        public override EmitKind Kind => EmitKind.ClassDeclaration;

        public EmitClassDeclaration(EmitNode parent, string name) : base(parent)
        {
            Name = name;
            Body = new(this);
        }

        public string Name { get; set; }
        public EmitBlockStatement<EmitNode> Body { get; set; }
    }
}
