using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using JiteLang.Main.Builder.Instructions;
using JiteLang.Main.Builder.Operands;
using JiteLang.Main.CodeAnalysis.Types;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Main.LangParser.Types;
using JiteLang.Main.LangParser.Types.Predefined;
using JiteLang.Main.Shared;
using JiteLang.Main.Visitor.Syntax;
using JiteLang.Main.Visitor.Type.Scope;
using JiteLang.Syntax;

namespace JiteLang.Main.Visitor.Type
{
    internal class TypeVisitor// : ITypeVisitor
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

        private void AddError(string errorMessage, in SyntaxPosition position)
        {
            var errorText = $"{errorMessage}   On {position.GetPosText()}";
            _diagnostics.Add(errorText);
        }

        private void VisitDefaultBlock(SyntaxNode item, TypeScope scope)
        {
            switch (item.Kind)
            {
                case SyntaxKind.VariableDeclaration:
                    VisitVariableDeclaration((VariableDeclarationSyntax)item, scope);
                    break;

                case SyntaxKind.CallExpression:
                    VisitCallExpression((CallExpressionSyntax)item, scope);
                    break;

                case SyntaxKind.ReturnStatement:
                    VisitReturnStatement((ReturnStatementSyntax)item, scope);
                    break;

                case SyntaxKind.IfStatement:
                    VisitIfStatement((IfStatementSyntax)item, scope);
                    break;

                case SyntaxKind.WhileStatement:
                    VisitWhileStatement((WhileStatementSyntax)item, scope);
                    break;

                case SyntaxKind.AssignmentExpression:
                    VisitAssignmentExpression((AssignmentExpressionSyntax)item, scope);
                    break;

                default:
                    throw new UnreachableException();
            }
        }

        #region Declarations

        public virtual void VisitNamespaceDeclaration(NamespaceDeclarationSyntax namespaceDeclarationSyntax, TypeScope scope)
        {
            foreach (var classDeclaration in namespaceDeclarationSyntax.Body.Members)
            {
                VisitClassDeclaration(classDeclaration, scope);
            }
        }

        public virtual void VisitClassDeclaration(ClassDeclarationSyntax classDeclarationSyntax, TypeScope scope)
        {
            var newScope = new TypeScope(scope);
            foreach (var item in classDeclarationSyntax.Body.Members)
            {
                switch (item.Kind)
                {
                    case SyntaxKind.ClassDeclaration:
                        VisitClassDeclaration((ClassDeclarationSyntax)item, newScope);
                        break;
                    case SyntaxKind.MethodDeclaration:
                        VisitMethodDeclaration((MethodDeclarationSyntax)item, newScope);
                        break;
                    case SyntaxKind.VariableDeclaration:
                        VisitVariableDeclaration((VariableDeclarationSyntax)item, newScope);
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
        }

        public virtual void VisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, TypeScope scope)
        {
            var newScope = new TypeScope(scope);

            var @params = new Dictionary<string, TypeMethodParameter>();

            foreach (var item in methodDeclarationSyntax.Params)
            {
                var paramType = VisitMethodParameter(item, newScope);
                @params.Add(item.Identifier.Text, new(paramType));
            }

            var returnType = FromSyntaxKind(((PredefinedTypeSyntax)methodDeclarationSyntax.ReturnType).Keyword.Kind);
            scope.AddMethod(methodDeclarationSyntax.Identifier.Text, returnType, @params);

            foreach (var item in methodDeclarationSyntax.Body.Members)
            {
                VisitDefaultBlock(item, newScope);
            }
        }

        public virtual LangType VisitMethodParameter(ParameterDeclarationSyntax parameterDeclarationSyntax, TypeScope scope)
        {
            var paramType = FromSyntaxKind(((PredefinedTypeSyntax)parameterDeclarationSyntax.Type).Keyword.Kind);
            scope.AddVariable(parameterDeclarationSyntax.Identifier.Text, paramType);

            return paramType;
        }

        public virtual LangType VisitVariableDeclaration(VariableDeclarationSyntax variableDeclarationSyntax, TypeScope scope)
        {
            var variableType = FromSyntaxKind(((PredefinedTypeSyntax)variableDeclarationSyntax.Type).Keyword.Kind);

            var variable = scope.AddVariable(variableDeclarationSyntax.Identifier.Text, variableType);

            if (variableDeclarationSyntax.InitialValue is not null)
            {
                var varValueType = VisitExpression(variableDeclarationSyntax.InitialValue, scope);
                if (varValueType.Kind != variableType.Kind)
                {
                    AddError($"Cannot implicit convert '{varValueType.Kind}' to type '{variableType.Kind}'", variableDeclarationSyntax.InitialValue.Position);
                }
            }

            return variableType;
        }

        #endregion Declarations

        #region Statements

        public virtual void VisitIfStatement(IfStatementSyntax ifStatementSyntax, TypeScope scope)
        {
            VisitExpression(ifStatementSyntax.Condition, scope);

            var newScope = new TypeScope(scope);

            foreach (var item in ifStatementSyntax.Body.Members)
            {
                VisitDefaultBlock(item, newScope);
            }

            if (ifStatementSyntax.Else is not null)
            {
                VisitElseStatement(ifStatementSyntax.Else, scope);
            }
        }

        public virtual void VisitElseStatement(StatementSyntax elseStatementSyntax, TypeScope scope)
        {
            {
                var newScope = new TypeScope(scope);

                switch (elseStatementSyntax.Kind)
                {
                    case SyntaxKind.BlockStatement:
                        VisitElseBody((BlockStatement<SyntaxNode>)elseStatementSyntax, newScope);
                        break;
                    case SyntaxKind.IfStatement:
                        VisitIfStatement((IfStatementSyntax)elseStatementSyntax, newScope);
                        break;

                    default:
                        break;
                }
            }

            void VisitElseBody(BlockStatement<SyntaxNode> elseBody, TypeScope elseScope)
            {
                var newElseScope = new TypeScope(elseScope);

                foreach (var item in elseBody.Members)
                {
                    VisitDefaultBlock(item, newElseScope);
                }
            }
        }

        public virtual void VisitReturnStatement(ReturnStatementSyntax returnStatementSyntax, TypeScope scope)
        {
            if (returnStatementSyntax.ReturnValue is not null)
            {
                VisitExpression(returnStatementSyntax.ReturnValue, scope);
            }
        }

        public virtual void VisitWhileStatement(WhileStatementSyntax whileStatementSyntax, TypeScope scope)
        {
            VisitExpression(whileStatementSyntax.Condition, scope);

            var newScope = new TypeScope(scope);

            foreach (var item in whileStatementSyntax.Body.Members)
            {
                VisitDefaultBlock(item, newScope);
            }
        }

        #endregion Statements

        #region Expressions
        public virtual LangType VisitExpression(ExpressionSyntax expressionSyntax, TypeScope scope)
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
                    return LangType.None;
            }
        }

        public virtual LangType VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, TypeScope scope)
        {
            VisitExpression(binaryExpressionSyntax.Left, scope);
            VisitExpression(binaryExpressionSyntax.Right, scope);

            var operation = binaryExpressionSyntax.Operation;

            switch (operation)
            {
                case SyntaxKind.PlusToken:

                    break;
                case SyntaxKind.MinusToken:

                    break;
                case SyntaxKind.AsteriskToken:

                    break;
                case SyntaxKind.SlashToken:

                    break;
                case SyntaxKind.PercentToken:

                    break;
                default:
                    throw new UnreachableException();
            }

            throw new UnreachableException();
        }

        public virtual LangType VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, TypeScope scope)
        {
            var literalValue = literalExpressionSyntax.Value;

            return FromSyntaxKind(literalValue.Kind);
        }

        public virtual LangType VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, TypeScope scope)
        {
            throw new NotImplementedException();
        }

        public virtual LangType VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, TypeScope scope)
        {
            throw new NotImplementedException();
        }

        public virtual LangType VisitCastExpression(CastExpressionSyntax castExpressionSyntax, TypeScope scope)
        {
            throw new NotImplementedException();
        }

        public virtual LangType VisitCallExpression(CallExpressionSyntax callExpressionSyntax, TypeScope scope)
        {
            var method = scope.GetMethod(((IdentifierExpressionSyntax)callExpressionSyntax.Caller).Text);

            for (int i = 0; i < callExpressionSyntax.Args.Count; i++)
            {
                var argItem = callExpressionSyntax.Args[i];

                var argType = VisitExpression(argItem, scope);

                var paramItem = method.Params.ElementAtOrDefault(i);

                if (!argType.IsEqualsNotNone(paramItem.Value?.Type)) 
                {
                    AddError($"Expected type {argType.GetStr()}", argItem.Position);
                }
            }

            return method.ReturnType;
        }

        public virtual LangType VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, TypeScope scope)
        {
            VisitExpression(logicalExpressionSyntax.Left, scope);
            VisitExpression(logicalExpressionSyntax.Right, scope);

            var operation = logicalExpressionSyntax.Operation;

            switch (operation)
            {
                case SyntaxKind.BarBarToken:
                    break;
                case SyntaxKind.AmpersandAmpersandToken:
                    break;
                case SyntaxKind.EqualsEqualsToken:
                    break;
                case SyntaxKind.NotEqualsToken:
                    break;
                case SyntaxKind.GreaterThanToken:
                    break;
                case SyntaxKind.GreaterThanEqualsToken:
                    break;
                case SyntaxKind.LessThanToken:
                    break;
                case SyntaxKind.LessThanEqualsToken:
                    break;

                default:
                    throw new NotImplementedException();
            }

            throw new NotImplementedException();

        }

        public virtual LangType VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, TypeScope scope)
        {
            //VisitExpression(assignmentExpressionSyntax.Right, scope);
            throw new NotImplementedException();

        }

        public virtual LangType VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, TypeScope scope)
        {
            var variable = scope.GetVariable(identifierExpressionSyntax.Text);
            return variable.Type;
        }

        #endregion Expressions
        private static LangType FromSyntaxKind(SyntaxKind syntaxKind)
        {
            switch (syntaxKind)
            {
                case SyntaxKind.StringLiteralToken:
                case SyntaxKind.StringKeyword:
                    return LangType.String;

                case SyntaxKind.CharLiteralToken:
                case SyntaxKind.CharKeyword:
                    return LangType.Char;

                case SyntaxKind.IntLiteralToken:
                case SyntaxKind.IntKeyword:
                    return LangType.Int;

                case SyntaxKind.LongLiteralToken:
                case SyntaxKind.LongKeyword:
                    return LangType.Long;

                case SyntaxKind.FalseLiteralToken:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.TrueLiteralToken:
                case SyntaxKind.TrueKeyword:
                    return LangType.Bool;

                default:
                    return LangType.None;
            }
        }
    }
}