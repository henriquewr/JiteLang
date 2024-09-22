using System;
using System.Diagnostics;
using JiteLang.Main.Builder;
using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Bound.Statements;
using JiteLang.Main.Bound.Statements.Declaration;
using JiteLang.Main.LangParser;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Syntax;

namespace JiteLang.Main.Bound.Visitor
{
    internal class BoundVisitor : IBoundVisitor<BoundNamespaceDeclaration,
        BoundClassDeclaration,
        BoundMethodDeclaration,
        BoundVariableDeclaration,
        BoundAssignmentExpression,
        BoundExpression,
        BoundStatement,
        BoundLiteralExpression,
        BoundParameterDeclaration,
        object>
    {
        #region Declarations
        public virtual BoundNamespaceDeclaration VisitNamespaceDeclaration(BoundNamespaceDeclaration root, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundClassDeclaration VisitClassDeclaration(BoundClassDeclaration classDeclaration, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundMethodDeclaration VisitMethodDeclaration(BoundMethodDeclaration methodDeclaration, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundParameterDeclaration VisitMethodParameter(BoundParameterDeclaration parameterDeclaration, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundVariableDeclaration VisitVariableDeclaration(BoundVariableDeclaration variableDeclaration, object scope)
        {
            throw new NotImplementedException();
        }
        #endregion Declarations

        #region Statements
        public virtual BoundStatement VisitReturnStatement(BoundReturnStatement returnStatement, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundStatement VisitWhileStatement(BoundWhileStatement whileStatement, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundStatement VisitIfStatement(BoundIfStatement ifStatement, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundStatement VisitElseStatement(BoundStatement elseStatement, object scope)
        {
            throw new NotImplementedException();
        }
        #endregion Statements

        #region Expressions
        public virtual BoundExpression VisitExpression(BoundExpression expression, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundAssignmentExpression VisitAssignmentExpression(BoundAssignmentExpression assignmentExpression, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundExpression VisitBinaryExpression(BoundBinaryExpression binaryExpression, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundExpression VisitCallExpression(BoundCallExpression callExpression, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundExpression VisitCastExpression(BoundCastExpression castExpression, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundExpression VisitIdentifierExpression(BoundIdentifierExpression identifierExpression, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundLiteralExpression VisitLiteralExpression(BoundLiteralExpression literalExpression, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundExpression VisitLogicalExpression(BoundLogicalExpression logicalExpression, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundExpression VisitMemberExpression(BoundMemberExpression memberExpression, object scope)
        {
            throw new NotImplementedException();
        }

        public virtual BoundExpression VisitUnaryExpression(BoundUnaryExpression unaryExpression, object scope)
        {
            throw new NotImplementedException();
        }
        #endregion Expressions
    }
}
