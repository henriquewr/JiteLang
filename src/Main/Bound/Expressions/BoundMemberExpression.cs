using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundMemberExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.MemberExpression;
        public BoundMemberExpression()
        {
        }
    }
}
