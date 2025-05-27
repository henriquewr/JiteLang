//using JiteLang.Main.Bound.Context;
//using JiteLang.Main.Bound.TypeResolvers;
//using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
//using JiteLang.Main.LangParser.Types;
//using JiteLang.Main.Shared;
//using JiteLang.Main.Shared.Type;
//using JiteLang.Main.Visitor.Type.Scope;
//using JiteLang.Syntax;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;

//namespace JiteLang.Main.Visitor.Type.Visitor
//{
//    internal class ExpressionVisitor : IExpressionVisitor<TypeSymbol>
//    {
//        private readonly Func<string, SyntaxPosition, string> _getErrorText;
//        private readonly Func<TypeSyntax, TypeSymbol> _resolveType;
//        private readonly BindingContext _bindingContext;
//        public ExpressionVisitor(BindingContext bindingContext, Func<string, SyntaxPosition, string> getErrorTextFunc, Func<TypeSyntax, TypeSymbol> resolveTypeFunc)
//        {
//            _getErrorText = getErrorTextFunc;
//            _resolveType = resolveTypeFunc;
//            _bindingContext = bindingContext;
//        }

//        protected virtual string GetErrorText(string errorMessage, SyntaxPosition position)
//        {
//            return _getErrorText(errorMessage, position);
//        }
       
//        protected virtual TypeSymbol ResolveType(TypeSyntax typeSyntax)
//        {
//            return _resolveType(typeSyntax);
//        }

//        public TypeSymbol VisitExpression(ExpressionSyntax expressionSyntax, TypeScope scope)
//        {
//            switch (expressionSyntax.Kind)
//            {
//                case SyntaxKind.LiteralExpression:
//                    return VisitLiteralExpression((LiteralExpressionSyntax)expressionSyntax, scope);

//                case SyntaxKind.MemberExpression:
//                    return VisitMemberExpression((MemberExpressionSyntax)expressionSyntax, scope);

//                case SyntaxKind.UnaryExpression:
//                    return VisitUnaryExpression((UnaryExpressionSyntax)expressionSyntax, scope);

//                case SyntaxKind.CastExpression:
//                    return VisitCastExpression((CastExpressionSyntax)expressionSyntax, scope);

//                case SyntaxKind.BinaryExpression:
//                    return VisitBinaryExpression((BinaryExpressionSyntax)expressionSyntax, scope);

//                case SyntaxKind.LogicalExpression:
//                    return VisitLogicalExpression((LogicalExpressionSyntax)expressionSyntax, scope);

//                case SyntaxKind.IdentifierExpression:
//                    return VisitIdentifierExpression((IdentifierExpressionSyntax)expressionSyntax, scope);

//                case SyntaxKind.CallExpression:
//                    return VisitCallExpression((CallExpressionSyntax)expressionSyntax, scope);

//                case SyntaxKind.AssignmentExpression:
//                    return VisitAssignmentExpression((AssignmentExpressionSyntax)expressionSyntax, scope);

//                case SyntaxKind.NewExpression:
//                    return VisitNewExpression((NewExpressionSyntax)expressionSyntax, scope);

//                default:
//                    throw new UnreachableException();
//            }
//        }

//        public IEnumerable<string> VisitCallExpression(CallExpressionSyntax callExpressionSyntax, TypeScope scope)
//        {
//            var callerType = VisitExpression(callExpressionSyntax.Caller, scope);

//            if (callerType is not DelegateTypeSymbol callerTypeDelegate)
//            {
//                yield return GetErrorText($"The type {callerType.Text} is not a method type", callExpressionSyntax.Position);
//            }

//            if (callerTypeDelegate.Parameters.Count != callExpressionSyntax.Args.Count)
//            {
//                yield return GetErrorText($"Expected {callerTypeDelegate.Parameters.Count} parameters, but got {callExpressionSyntax.Args.Count}", callExpressionSyntax.Position);
//            }
//            else
//            {
//                for (int i = 0; i < callExpressionSyntax.Args.Count; i++)
//                {
//                    var item = callExpressionSyntax.Args[i];

//                    var itemType = VisitExpression(item, scope);
//                    var methodParamType = callerTypeDelegate.Parameters[i].Type;

//                    if (!itemType.Equals(methodParamType) && !_bindingContext.ConversionTable.TryGetImplicitConversion(itemType, methodParamType, out var conversion))
//                    {
//                        AddError($"Argument {i + 1} cannot be implicitly converted from type '{itemType.Text}' to type '{methodParamType.Text}'", item.Position);
//                    }
//                }
//            }

//            return callerTypeDelegate.ReturnType;
//        }

//        public TypeSymbol VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, TypeScope scope)
//        {
//            var rightType = VisitExpression(assignmentExpressionSyntax.Right, scope);
//            var leftType = VisitExpression(assignmentExpressionSyntax.Left, scope);

//            if (!rightType.IsEqualsNotError(leftType))
//            {
//                AddError($"Cannot implicit convert '{leftType.Text}' to type '{rightType.Text}'", assignmentExpressionSyntax.Position);
//                return ErrorTypeSymbol.Instance;
//            }

//            return rightType;
//        }

//        public TypeSymbol VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, TypeScope scope)
//        {
//            var leftType = VisitExpression(binaryExpressionSyntax.Left, scope);
//            var rightType = VisitExpression(binaryExpressionSyntax.Right, scope);

//            var resultType = BinaryExprTypeResolver.Resolve(leftType, binaryExpressionSyntax.Operation, rightType);

//            if (resultType.IsError())
//            {
//                AddError($"Cannot implicit convert '{leftType.Text}' to type '{rightType.Text}'", binaryExpressionSyntax.Left.Position);
//            }

//            return resultType;
//        }

//        public TypeSymbol VisitCastExpression(CastExpressionSyntax castExpressionSyntax, TypeScope scope)
//        {
//            throw new NotImplementedException();
//        }

//        public TypeSymbol VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, TypeScope scope)
//        {
//            var identifier = scope.GetIdentifier(identifierExpressionSyntax.Text)!;
//            return identifier.Type;
//        }

//        public TypeSymbol VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, TypeScope scope)
//        {
//            switch (literalExpressionSyntax.Value.Kind)
//            {
//                case SyntaxKind.StringLiteralToken:
//                    return ConstantValue.GetTypeFromConstantValue(ConstantValueKind.String);
//                case SyntaxKind.CharLiteralToken:
//                    return ConstantValue.GetTypeFromConstantValue(ConstantValueKind.Char);
//                case SyntaxKind.IntLiteralToken:
//                    return ConstantValue.GetTypeFromConstantValue(ConstantValueKind.Int);
//                case SyntaxKind.LongLiteralToken:
//                    return ConstantValue.GetTypeFromConstantValue(ConstantValueKind.Long);
//                case SyntaxKind.FalseLiteralToken:
//                case SyntaxKind.FalseKeyword:
//                case SyntaxKind.TrueLiteralToken:
//                case SyntaxKind.TrueKeyword:
//                    return ConstantValue.GetTypeFromConstantValue(ConstantValueKind.Bool);                
//                case SyntaxKind.NullKeyword:
//                case SyntaxKind.NullLiteralToken:
//                    return ConstantValue.GetTypeFromConstantValue(ConstantValueKind.Null);
//                default:
//                    throw new UnreachableException();
//            }
//        }

//        public TypeSymbol VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, TypeScope scope)
//        {
//            var leftType = VisitExpression(logicalExpressionSyntax.Left, scope);
//            var rightType = VisitExpression(logicalExpressionSyntax.Right, scope);

//            var resultType = LogicalExprTypeResolver.Resolve(leftType, logicalExpressionSyntax.Operation, rightType);

//            if (resultType.IsError())
//            {
//                AddError($"Operator not defined for '{leftType.Text}' and '{rightType.Text}'", logicalExpressionSyntax.Left.Position);
//            }

//            return resultType;
//        }

//        public TypeSymbol VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, TypeScope scope)
//        {
//            var leftType = VisitExpression(memberExpressionSyntax.Left, scope);

//            if (leftType is not MemberedTypeSymbol)
//            {
//                throw new Exception("isnt membered");
//            }

//            var leftMemberedType = (MemberedTypeSymbol)leftType;

//            var rightMember = leftMemberedType.GetTypedMembers().FirstOrDefault(x => x.Name == memberExpressionSyntax.Right.Text);

//            if (rightMember is null)
//            {
//                if (!leftType.IsError())
//                {
//                    AddError($"The type '{leftType.Text}' does not have any member called '{memberExpressionSyntax.Right.Text}'", memberExpressionSyntax.Right.Position);
//                }

//                return ErrorTypeSymbol.Instance;
//            }

//            return rightMember.Type;
//        }

//        public TypeSymbol VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, TypeScope scope)
//        {
//            throw new NotImplementedException();
//        }

//        public TypeSymbol VisitNewExpression(NewExpressionSyntax newExpressionSyntax, TypeScope scope)
//        {
//            var targetType = ResolveType(newExpressionSyntax.Type);

//            if (targetType is not MemberedTypeSymbol)
//            {
//                throw new Exception("isnt membered");
//            }

//            var targetMemberedType = (MemberedTypeSymbol)targetType;

//            var targetCtor = targetMemberedType.Constructors.Find(x => x.Parameters.Count == newExpressionSyntax.Args.Count);

//            if (targetCtor is null)
//            {
//                AddError($"The type '{targetMemberedType.Text}' does not have any constructor with {newExpressionSyntax.Args.Count} arguments", newExpressionSyntax.Position);
//            }
//            else
//            {
//                for (int i = 0; i < newExpressionSyntax.Args.Count; i++)
//                {
//                    var item = newExpressionSyntax.Args[i];

//                    var itemType = VisitExpression(item, scope);
//                    var methodParamType = targetCtor.Parameters[i].Type;

//                    if (!itemType.Equals(methodParamType) && !_bindingContext.ConversionTable.TryGetImplicitConversion(itemType, methodParamType, out var conversion))
//                    {
//                        AddError($"Argument {i + 1} cannot be implicitly converted from type '{itemType.Text}' to type '{methodParamType.Text}'", item.Position);
//                    }
//                }
//            }

//            return targetMemberedType;
//        }
//    }
//}