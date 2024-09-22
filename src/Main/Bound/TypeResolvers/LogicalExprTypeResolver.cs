using JiteLang.Main.Shared;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.TypeResolvers
{
    internal class LogicalExprTypeResolver : TypeResolver
    {
        private static readonly Dictionary<(TypeSymbol, LogicalOperatorKind, TypeSymbol), TypeSymbol> OperatorTypeMap = new()
        {
            { (PredefinedTypeSymbol.Bool, LogicalOperatorKind.AndAnd, PredefinedTypeSymbol.Bool), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Bool, LogicalOperatorKind.OrOr, PredefinedTypeSymbol.Bool), PredefinedTypeSymbol.Bool },


            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.GreaterThan, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.GreaterThan, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.GreaterThan, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.GreaterThan, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.GreaterThan, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.GreaterThan, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.GreaterThan, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.GreaterThan, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.GreaterThan, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },


            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.GreaterThanOrEquals, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.GreaterThanOrEquals, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.GreaterThanOrEquals, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.GreaterThanOrEquals, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.GreaterThanOrEquals, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.GreaterThanOrEquals, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.GreaterThanOrEquals, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.GreaterThanOrEquals, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.GreaterThanOrEquals, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },


            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.LessThan, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.LessThan, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.LessThan, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.LessThan, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.LessThan, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.LessThan, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.LessThan, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.LessThan, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.LessThan, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },


            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.LessThanOrEquals, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.LessThanOrEquals, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.LessThanOrEquals, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.LessThanOrEquals, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.LessThanOrEquals, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.LessThanOrEquals, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.LessThanOrEquals, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.LessThanOrEquals, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.LessThanOrEquals, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },

            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.EqualsEquals, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.EqualsEquals, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.EqualsEquals, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.EqualsEquals, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.EqualsEquals, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.EqualsEquals, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.EqualsEquals, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.EqualsEquals, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.EqualsEquals, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Bool, LogicalOperatorKind.EqualsEquals, PredefinedTypeSymbol.Bool), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.String, LogicalOperatorKind.EqualsEquals, PredefinedTypeSymbol.String), PredefinedTypeSymbol.Bool },

            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.NotEquals, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.NotEquals, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.NotEquals, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.NotEquals, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.NotEquals, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Int, LogicalOperatorKind.NotEquals, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Long, LogicalOperatorKind.NotEquals, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.NotEquals, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Char, LogicalOperatorKind.NotEquals, PredefinedTypeSymbol.Char), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.Bool, LogicalOperatorKind.NotEquals, PredefinedTypeSymbol.Bool), PredefinedTypeSymbol.Bool },
            { (PredefinedTypeSymbol.String, LogicalOperatorKind.NotEquals, PredefinedTypeSymbol.String), PredefinedTypeSymbol.Bool },
        };

        public static bool Exists(TypeSymbol left, LogicalOperatorKind operatorKind, TypeSymbol right)
        {
            var key = (left, operatorKind, right);
            var exists = OperatorTypeMap.ContainsKey(key);
            return exists;
        }

        public static TypeSymbol Resolve(TypeSymbol left, LogicalOperatorKind operatorKind, TypeSymbol right)
        {
            var key = (left, operatorKind, right);

            if (OperatorTypeMap.TryGetValue(key, out var resultType))
            {
                return resultType;
            }

            return TypeSymbol.None;
        }
    }
}
