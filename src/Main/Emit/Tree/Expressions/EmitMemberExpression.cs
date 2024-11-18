using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.Tree.Expressions
{
    internal class EmitMemberExpression : EmitExpression
    {
        public override EmitKind Kind => EmitKind.MemberExpression;
        public EmitMemberExpression(EmitNode parent) : base(parent)
        {
            
        }
    }
}
