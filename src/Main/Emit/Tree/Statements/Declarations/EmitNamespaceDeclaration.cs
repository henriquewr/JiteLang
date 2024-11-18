using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitNamespaceDeclaration : EmitDeclaration
    {
        public override EmitKind Kind => EmitKind.NamespaceDeclaration;

        public EmitNamespaceDeclaration(EmitNode parent) : base(parent)
        {
            Body = new(this);
        }

        public EmitBlockStatement<EmitClassDeclaration> Body { get; set; }
    }
}
