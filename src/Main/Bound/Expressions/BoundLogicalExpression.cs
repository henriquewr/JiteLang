
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.Shared;
using System;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundLogicalExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.LogicalExpression;

        public BoundLogicalExpression(BoundExpression left, LogicalOperatorKind operation, BoundExpression right)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }

        public BoundExpression Left { get; set; }
        public LogicalOperatorKind Operation { get; set; }
        public BoundExpression Right { get; set; }
    }
}
