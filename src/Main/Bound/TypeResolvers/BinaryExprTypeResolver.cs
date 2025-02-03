using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace JiteLang.Main.Bound.TypeResolvers
{
    internal class BinaryExprTypeResolver : TypeResolver
    {
        private static readonly FrozenDictionary<(TypeSymbol, BinaryOperatorKind, TypeSymbol), TypeSymbol> OperatorTypeMap = new Dictionary<(TypeSymbol, BinaryOperatorKind, TypeSymbol), TypeSymbol>
        {
            { (PredefinedTypeSymbol.Int, BinaryOperatorKind.Plus, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Int },
            { (PredefinedTypeSymbol.Int, BinaryOperatorKind.Plus, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Long },
            { (PredefinedTypeSymbol.Long, BinaryOperatorKind.Plus, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Long },
            { (PredefinedTypeSymbol.Long, BinaryOperatorKind.Plus, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Long },

            { (PredefinedTypeSymbol.Int, BinaryOperatorKind.Minus, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Int },
            { (PredefinedTypeSymbol.Int, BinaryOperatorKind.Minus, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Long },
            { (PredefinedTypeSymbol.Long, BinaryOperatorKind.Minus, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Long },
            { (PredefinedTypeSymbol.Long, BinaryOperatorKind.Minus, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Long },

            { (PredefinedTypeSymbol.Int, BinaryOperatorKind.Multiply, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Int },
            { (PredefinedTypeSymbol.Int, BinaryOperatorKind.Multiply, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Long },
            { (PredefinedTypeSymbol.Long, BinaryOperatorKind.Multiply, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Long },
            { (PredefinedTypeSymbol.Long, BinaryOperatorKind.Multiply, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Long },

            { (PredefinedTypeSymbol.Int, BinaryOperatorKind.Divide, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Int },
            { (PredefinedTypeSymbol.Int, BinaryOperatorKind.Divide, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Long },
            { (PredefinedTypeSymbol.Long, BinaryOperatorKind.Divide, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Long },
            { (PredefinedTypeSymbol.Long, BinaryOperatorKind.Divide, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Long },

            { (PredefinedTypeSymbol.Int, BinaryOperatorKind.Modulus, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Int },
            { (PredefinedTypeSymbol.Int, BinaryOperatorKind.Modulus, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Long },
            { (PredefinedTypeSymbol.Long, BinaryOperatorKind.Modulus, PredefinedTypeSymbol.Int), PredefinedTypeSymbol.Long },
            { (PredefinedTypeSymbol.Long, BinaryOperatorKind.Modulus, PredefinedTypeSymbol.Long), PredefinedTypeSymbol.Long },
        }.ToFrozenDictionary();

        public static TypeSymbol Resolve(TypeSymbol left, BinaryOperatorKind operatorKind, TypeSymbol right)
        {
            var key = (left, operatorKind, right);

            if (OperatorTypeMap.TryGetValue(key, out var resultType))
            {
                return resultType;
            }

            return ErrorTypeSymbol.Instance;
        }
    }
}
