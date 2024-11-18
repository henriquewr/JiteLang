using JiteLang.Main.Bound.TypeResolvers;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;
using JiteLang.Main.Visitor.Type.Scope;
using JiteLang.Syntax;
using System;
using System.Diagnostics;
using System.Linq;

namespace JiteLang.Main.Visitor.Type.Visitor
{
    internal class ExpressionVisitor : IExpressionVisitor<TypeSymbol>
    {
        private readonly Action<string, SyntaxPosition> _addErrorFunc;

        public ExpressionVisitor(Action<string, SyntaxPosition> addErrorFunc)
        {
            _addErrorFunc = addErrorFunc;
        }

        protected void AddError(string errorMessage, in SyntaxPosition position)
        {
            _addErrorFunc(errorMessage, position);
        }

        public TypeSymbol VisitExpression(ExpressionSyntax expressionSyntax, TypeScope scope)
        {
            switch (expressionSyntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return VisitLiteralExpression((LiteralExpressionSyntax)expressionSyntax, scope);

                case SyntaxKind.MemberExpression:
                    return VisitMemberExpression((MemberExpressionSyntax)expressionSyntax, scope);

                case SyntaxKind.UnaryExpression:
                    return VisitUnaryExpression((UnaryExpressionSyntax)expressionSyntax, scope);

                case SyntaxKind.CastExpression:
                    return VisitCastExpression((CastExpressionSyntax)expressionSyntax, scope);

                case SyntaxKind.BinaryExpression:
                    return VisitBinaryExpression((BinaryExpressionSyntax)expressionSyntax, scope);

                case SyntaxKind.LogicalExpression:
                    return VisitLogicalExpression((LogicalExpressionSyntax)expressionSyntax, scope);

                case SyntaxKind.IdentifierExpression:
                    return VisitIdentifierExpression((IdentifierExpressionSyntax)expressionSyntax, scope);

                case SyntaxKind.CallExpression:
                    return VisitCallExpression((CallExpressionSyntax)expressionSyntax, scope);

                case SyntaxKind.AssignmentExpression:
                    return VisitAssignmentExpression((AssignmentExpressionSyntax)expressionSyntax, scope);

                default:
                    throw new UnreachableException();
            }
        }

        public TypeSymbol VisitCallExpression(CallExpressionSyntax callExpressionSyntax, TypeScope scope)
        {
            var method = scope.GetMethod(((IdentifierExpressionSyntax)callExpressionSyntax.Caller).Text);

            if (method.Params.Count != callExpressionSyntax.Args.Count)
            {
                AddError($"Expected {method.Params.Count} parameters, but got {callExpressionSyntax.Args.Count}", callExpressionSyntax.Position);
            }
            else
            {
                for (int i = 0; i < callExpressionSyntax.Args.Count; i++)
                {
                    var argItem = callExpressionSyntax.Args[i];

                    var argType = VisitExpression(argItem, scope);

                    var paramItem = method.Params.ElementAt(i);
                    var paramItemType = paramItem.Value.Type;

                    if (!argType.IsEqualsNotNone(paramItemType))
                    {
                        AddError($"Expected type {paramItemType.Text}", argItem.Position);
                    }
                }
            }

            return method.ReturnType;
        }

        public TypeSymbol VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, TypeScope scope)
        {
            var rightType = VisitExpression(assignmentExpressionSyntax.Right, scope);
            var leftType = VisitExpression(assignmentExpressionSyntax.Left, scope);

            if (!rightType.IsEqualsNotNone(leftType))
            {
                AddError($"Cannot implicit convert '{leftType.Text}' to type '{rightType.Text}'", assignmentExpressionSyntax.Position);
                return TypeSymbol.None;
            }

            return rightType;
        }

        public TypeSymbol VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, TypeScope scope)
        {
            var leftType = VisitExpression(binaryExpressionSyntax.Left, scope);
            var rightType = VisitExpression(binaryExpressionSyntax.Right, scope);

            var resultType = BinaryExprTypeResolver.Resolve(leftType, binaryExpressionSyntax.Operation, rightType);

            if (resultType.Equals(TypeSymbol.None))
            {
                AddError($"Cannot implicit convert '{leftType.Text}' to type '{rightType.Text}'", binaryExpressionSyntax.Left.Position);
            }

            return resultType;
        }

        public TypeSymbol VisitCastExpression(CastExpressionSyntax castExpressionSyntax, TypeScope scope)
        {
            throw new NotImplementedException();
        }

        public TypeSymbol VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, TypeScope scope)
        {
            var variable = scope.GetVariable(identifierExpressionSyntax.Text);
            return variable.Type;
        }

        public TypeSymbol VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, TypeScope scope)
        {
            switch (literalExpressionSyntax.Value.Kind)
            {
                case SyntaxKind.StringLiteralToken:
                    return ConstantValue.GetTypeFromConstantValue(ConstantValueKind.String);
                case SyntaxKind.CharLiteralToken:
                    return ConstantValue.GetTypeFromConstantValue(ConstantValueKind.Char);
                case SyntaxKind.IntLiteralToken:
                    return ConstantValue.GetTypeFromConstantValue(ConstantValueKind.Int);
                case SyntaxKind.LongLiteralToken:
                    return ConstantValue.GetTypeFromConstantValue(ConstantValueKind.Long);
                case SyntaxKind.FalseLiteralToken:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.TrueLiteralToken:
                case SyntaxKind.TrueKeyword:
                    return ConstantValue.GetTypeFromConstantValue(ConstantValueKind.Bool);                
                case SyntaxKind.NullKeyword:
                case SyntaxKind.NullLiteralToken:
                    return ConstantValue.GetTypeFromConstantValue(ConstantValueKind.Null);
                default:
                    throw new UnreachableException();
            }
        }

        public TypeSymbol VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, TypeScope scope)
        {
            var leftType = VisitExpression(logicalExpressionSyntax.Left, scope);
            var rightType = VisitExpression(logicalExpressionSyntax.Right, scope);

            var resultType = LogicalExprTypeResolver.Resolve(leftType, logicalExpressionSyntax.Operation, rightType);

            if (resultType.Equals(TypeSymbol.None))
            {
                AddError($"Operator not defined for '{leftType.Text}' and '{rightType.Text}'", logicalExpressionSyntax.Left.Position);
            }

            return resultType;
        }

        public TypeSymbol VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, TypeScope scope)
        {
            throw new NotImplementedException();
        }

        public TypeSymbol VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, TypeScope scope)
        {
            throw new NotImplementedException();
        }
    }
}
