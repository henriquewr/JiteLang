using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JiteLang.Main.Builder;
using JiteLang.Main.Bound;
using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Bound.Statements;
using JiteLang.Main.Bound.Statements.Declaration;
using JiteLang.Main.Visitor.Type.Scope;
using JiteLang.Syntax;
using JiteLang.Main.Bound.TypeResolvers;

namespace JiteLang.Main.Visitor.Type
{
    internal class TypeVisitor /*: ITypeVisitor*/
    {
        //protected readonly SyntaxVisitor _typeResolver;

        //public TypeVisitor(SyntaxVisitor typeResolver)
        //{
        //    _typeResolver = typeResolver;
        //}

        private readonly ICollection<string> _diagnostics;
        public TypeVisitor(ICollection<string> diagnostics)
        {
            _diagnostics = diagnostics;
        }

        private BoundMethodDeclaration? _currentMethod = null;

        private void AddError(string errorMessage, in SyntaxPosition position)
        {
            var errorText = $"{errorMessage}   On {position.GetPosText()}";
            _diagnostics.Add(errorText);
        }

        private void VisitDefaultBlock(BoundNode item, TypeScope scope)
        {
            switch (item.Kind)
            {
                case BoundKind.VariableDeclaration:
                    VisitVariableDeclaration((BoundVariableDeclaration)item, scope);
                    break;

                case BoundKind.CallExpression:
                    VisitCallExpression((BoundCallExpression)item, scope);
                    break;

                case BoundKind.ReturnStatement:
                    VisitReturnStatement((BoundReturnStatement)item, scope);
                    break;

                case BoundKind.IfStatement:
                    VisitIfStatement((BoundIfStatement)item, scope);
                    break;

                case BoundKind.WhileStatement:
                    VisitWhileStatement((BoundWhileStatement)item, scope);
                    break;

                case BoundKind.AssignmentExpression:
                    VisitAssignmentExpression((BoundAssignmentExpression)item, scope);
                    break;

                default:
                    throw new UnreachableException();
            }
        }

        #region Declarations
        public virtual void VisitNamespaceDeclaration(BoundNamespaceDeclaration namespaceDeclaration, TypeScope scope)
        {
            foreach (var classDeclaration in namespaceDeclaration.Body.Members)
            {
                VisitClassDeclaration(classDeclaration, scope);
            }
        }

        public virtual void VisitClassDeclaration(BoundClassDeclaration classDeclaration, TypeScope scope)
        {
            var newScope = new TypeScope(scope);
            foreach (var item in classDeclaration.Body.Members)
            {
                switch (item.Kind)
                {
                    case BoundKind.ClassDeclaration:
                        VisitClassDeclaration((BoundClassDeclaration)item, newScope);
                        break;
                    case BoundKind.MethodDeclaration:
                        VisitMethodDeclaration((BoundMethodDeclaration)item, newScope);
                        break;
                    case BoundKind.VariableDeclaration:
                        VisitVariableDeclaration((BoundVariableDeclaration)item, newScope);
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
        }

        public virtual void VisitMethodDeclaration(BoundMethodDeclaration methodDeclaration, TypeScope scope)
        {
            var newScope = new TypeScope(scope);

            var @params = new Dictionary<string, TypeMethodParameter>();

            foreach (var item in methodDeclaration.Params)
            {
                var paramType = VisitMethodParameter(item, newScope);
                @params.Add(item.Identifier.Text, new(paramType));
            }

            scope.AddMethod(methodDeclaration.Identifier.Text, methodDeclaration.ReturnType, @params);

            _currentMethod = methodDeclaration;

            foreach (var item in methodDeclaration.Body.Members)
            {
                VisitDefaultBlock(item, newScope);
            }

            _currentMethod = null;
        }

        public virtual TypeSymbol VisitMethodParameter(BoundParameterDeclaration parameterDeclaration, TypeScope scope)
        {
            var paramType = parameterDeclaration.Type;
            scope.AddVariable(parameterDeclaration.Identifier.Text, paramType);

            return paramType;
        }

        public virtual TypeSymbol VisitVariableDeclaration(BoundVariableDeclaration variableDeclaration, TypeScope scope)
        {
            var variableType = variableDeclaration.Type;

            var variable = scope.AddVariable(variableDeclaration.Identifier.Text, variableType);

            if (variableDeclaration.InitialValue is not null)
            {
                var varValueType = VisitExpression(variableDeclaration.InitialValue, scope);
                if (varValueType.Text != variableType.Text)
                {
                    AddError($"Cannot implicit convert '{varValueType.Text}' to type '{variableType.Text}'", variableDeclaration.Position);
                }
            }

            return variableType;
        }
        #endregion Declarations

        #region Statements
        public virtual void VisitIfStatement(BoundIfStatement ifStatement, TypeScope scope)
        {
            VisitExpression(ifStatement.Condition, scope);

            var newScope = new TypeScope(scope);

            foreach (var item in ifStatement.Body.Members)
            {
                VisitDefaultBlock(item, newScope);
            }

            if (ifStatement.Else is not null)
            {
                VisitElseStatement(ifStatement.Else, scope);
            }
        }

        public virtual void VisitElseStatement(BoundStatement elseStatement, TypeScope scope)
        {
            var newScope = new TypeScope(scope);

            switch (elseStatement.Kind)
            {
                case BoundKind.BlockStatement:
                    VisitElseBody((BoundBlockStatement<BoundNode>)elseStatement, newScope);
                    break;
                case BoundKind.IfStatement:
                    VisitIfStatement((BoundIfStatement)elseStatement, newScope);
                    break;

                default:
                    break;
            }

            void VisitElseBody(BoundBlockStatement<BoundNode> elseBody, TypeScope elseScope)
            {
                var newElseScope = new TypeScope(elseScope);

                foreach (var item in elseBody.Members)
                {
                    VisitDefaultBlock(item, newElseScope);
                }
            }
        }

        public virtual void VisitReturnStatement(BoundReturnStatement returnStatement, TypeScope scope)
        {
            if (_currentMethod is null)
            {
                AddError("Misplaced return", returnStatement.Position);
            }

            if (returnStatement.ReturnValue is not null)
            {
                var returnType = VisitExpression(returnStatement.ReturnValue, scope);

                if (_currentMethod?.ReturnType.IsEqualsNotNone(returnType) == false)
                {
                    AddError($"Method {_currentMethod.Identifier.Text} must returns {_currentMethod.ReturnType.Text}", returnStatement.Position);
                }
            }
        }

        public virtual void VisitWhileStatement(BoundWhileStatement whileStatement, TypeScope scope)
        {
            VisitExpression(whileStatement.Condition, scope);

            var newScope = new TypeScope(scope);

            foreach (var item in whileStatement.Body.Members)
            {
                VisitDefaultBlock(item, newScope);
            }
        }

        #endregion Statements

        #region Expressions
        public virtual TypeSymbol VisitExpression(BoundExpression expression, TypeScope scope)
        {
            switch (expression.Kind)
            {
                case BoundKind.LiteralExpression:
                    return VisitLiteralExpression((BoundLiteralExpression)expression, scope);

                case BoundKind.MemberExpression:
                    return VisitMemberExpression((BoundMemberExpression)expression, scope);

                case BoundKind.UnaryExpression:
                    return VisitUnaryExpression((BoundUnaryExpression)expression, scope);

                case BoundKind.CastExpression:
                    return VisitCastExpression((BoundCastExpression)expression, scope);

                case BoundKind.BinaryExpression:
                    return VisitBinaryExpression((BoundBinaryExpression)expression, scope);

                case BoundKind.LogicalExpression:
                    return VisitLogicalExpression((BoundLogicalExpression)expression, scope);

                case BoundKind.IdentifierExpression:
                    return VisitIdentifierExpression((BoundIdentifierExpression)expression, scope);

                case BoundKind.CallExpression:
                    return VisitCallExpression((BoundCallExpression)expression, scope);

                case BoundKind.AssignmentExpression:
                    return VisitAssignmentExpression((BoundAssignmentExpression)expression, scope);

                default:
                    throw new UnreachableException();
            }
        }

        public virtual TypeSymbol VisitBinaryExpression(BoundBinaryExpression binaryExpression, TypeScope scope)
        {
            var leftType = VisitExpression(binaryExpression.Left, scope);
            var rightType = VisitExpression(binaryExpression.Right, scope);

            var resultType = BinaryExprTypeResolver.Resolve(leftType, binaryExpression.Operation, rightType);

            if (resultType == TypeSymbol.None)
            {
                AddError($"Cannot implicit convert '{leftType.Text}' to type '{rightType.Text}'", binaryExpression.Left.Position);
            }

            return resultType;
            //var operation = binaryExpression.Operation;

            //switch (operation)
            //{
            //    case BinaryOperatorKind.Plus:
                    
            //        break;
            //    case BinaryOperatorKind.Minus:

            //        break;
            //    case BinaryOperatorKind.Multiply:

            //        break;
            //    case BinaryOperatorKind.Divide:

            //        break;
            //    case BinaryOperatorKind.Modulus:

            //        break;
            //    default:
            //        throw new UnreachableException();
            //}
            //throw new UnreachableException();
        }

        public virtual TypeSymbol VisitLiteralExpression(BoundLiteralExpression literalExpression, TypeScope scope)
        {
            return literalExpression.Value.Type;
        }

        public virtual TypeSymbol VisitMemberExpression(BoundMemberExpression memberExpression, TypeScope scope)
        {
            throw new NotImplementedException();
        }

        public virtual TypeSymbol VisitUnaryExpression(BoundUnaryExpression unaryExpression, TypeScope scope)
        {
            throw new NotImplementedException();
        }

        public virtual TypeSymbol VisitCastExpression(BoundCastExpression castExpression, TypeScope scope)
        {
            throw new NotImplementedException();
        }

        public virtual TypeSymbol VisitCallExpression(BoundCallExpression callExpression, TypeScope scope)
        {
            var method = scope.GetMethod(((BoundIdentifierExpression)callExpression.Caller).Text);

            for (int i = 0; i < callExpression.Args.Count; i++)
            {
                var argItem = callExpression.Args[i];

                var argType = VisitExpression(argItem, scope);

                var paramItem = method.Params.ElementAtOrDefault(i);

                if (!argType.IsEqualsNotNone(paramItem.Value?.Type))
                {
                    AddError($"Expected type {argType.Text}", argItem.Position);
                }
            }

            return method.ReturnType;
        }

        public virtual TypeSymbol VisitLogicalExpression(BoundLogicalExpression logicalExpression, TypeScope scope)
        {
            var leftType = VisitExpression(logicalExpression.Left, scope);
            var rightType = VisitExpression(logicalExpression.Right, scope);

            var resultType = LogicalExprTypeResolver.Resolve(leftType, logicalExpression.Operation, rightType);

            if (resultType == TypeSymbol.None)
            {
                AddError($"Operator not defined for '{leftType.Text}' and '{rightType.Text}'", logicalExpression.Left.Position);
            }

            return resultType;
        }

        public virtual TypeSymbol VisitAssignmentExpression(BoundAssignmentExpression assignmentExpression, TypeScope scope)
        {
            var rightType = VisitExpression(assignmentExpression.Right, scope);
            var leftType = VisitExpression(assignmentExpression.Left, scope);

            if (!rightType.IsEqualsNotNone(leftType))
            {
                AddError($"Cannot implicit convert '{leftType.Text}' to type '{rightType.Text}'", assignmentExpression.Position);
                return TypeSymbol.None;
            }

            return rightType;
        }

        public virtual TypeSymbol VisitIdentifierExpression(BoundIdentifierExpression identifierExpression, TypeScope scope)
        {
            var variable = scope.GetVariable(identifierExpression.Text);
            return variable.Type;
        }

        #endregion Expressions
    }
}