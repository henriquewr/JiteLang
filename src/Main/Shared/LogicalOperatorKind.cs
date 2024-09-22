using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Shared
{
    internal enum LogicalOperatorKind
    {
        None = 0,

        GreaterThan,
        GreaterThanOrEquals,

        LessThan,
        LessThanOrEquals,

        EqualsEquals,
        NotEquals,

        AndAnd,
        OrOr,
    }
}
