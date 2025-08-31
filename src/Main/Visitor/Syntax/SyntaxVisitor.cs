using System.Collections.Generic;
using System.Diagnostics;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Main.Shared;
using JiteLang.Syntax;

namespace JiteLang.Main.Visitor.Syntax
{
    internal class SyntaxVisitor<TScope, TVariableKey,
        TVariable,
        TMethodKey,
        TMethod> : ISyntaxVisitor<IList<SyntaxNode>,
        ClassDeclarationSyntax, 
        MethodDeclarationSyntax, 
        FieldDeclarationSyntax, 
        LocalDeclarationSyntax, 
        AssignmentExpressionSyntax, 
        ExpressionSyntax, 
        StatementSyntax,
        LiteralExpressionSyntax,
        ParameterDeclarationSyntax,
        TScope>
        where TScope : IScope<TVariable, TMethod, TScope>, new()
    {
        public SyntaxVisitor()
        {
        }


        #region Declarations
        public virtual IList<SyntaxNode> VisitNamespaceDeclaration(NamespaceDeclarationSyntax root, TScope scope)
        {
            var newNodes = new List<SyntaxNode>();

            foreach (var classDeclaration in root.Body.Members)
            {
                newNodes.Add(VisitClassDeclaration(classDeclaration, scope));
            }

            return newNodes;
        }

        public virtual ClassDeclarationSyntax VisitClassDeclaration(ClassDeclarationSyntax classDeclaration, TScope scope)
        {
            var newNodes = new List<SyntaxNode>();

            var newScope = NewScope(scope);
 
            foreach (var item in classDeclaration.Body.Members)
            {
                switch (item.Kind)
                {
                    case SyntaxKind.ClassDeclaration:
                        newNodes.Add(VisitClassDeclaration((ClassDeclarationSyntax)item, newScope));
                        break;
                    case SyntaxKind.MethodDeclaration:
                        newNodes.Add(VisitMethodDeclaration((MethodDeclarationSyntax)item, newScope));
                        break;
                    case SyntaxKind.FieldDeclaration:
                        newNodes.Add(VisitFieldDeclaration((FieldDeclarationSyntax)item, newScope));
                        break;
                    default:
                        throw new UnreachableException();
                }
            }

            classDeclaration.Body.Members = new(newNodes);

            return classDeclaration;
        }

        public virtual MethodDeclarationSyntax VisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, TScope scope)
        {
            var newScope = NewScope(scope);

            var newParameters = new List<ParameterDeclarationSyntax>();
            foreach (var item in methodDeclarationSyntax.Params)
            {
                newParameters.Add(VisitMethodParameter(item, newScope));
            }
            methodDeclarationSyntax.Params = new(newParameters);

            var newNodes = new List<SyntaxNode>();
            foreach (var item in methodDeclarationSyntax.Body.Members)
            {
                switch (item.Kind)
                {
                    case SyntaxKind.MethodDeclaration:
                        newNodes.Add(VisitMethodDeclaration((MethodDeclarationSyntax)item, newScope));
                        break;
                    case SyntaxKind.LocalDeclaration:
                        newNodes.Add(VisitLocalDeclaration((LocalDeclarationSyntax)item, newScope));
                        break;       
                    case SyntaxKind.ReturnStatement:
                        newNodes.Add(VisitReturnStatement((ReturnStatementSyntax)item, newScope));
                        break;
                    case SyntaxKind.AssignmentExpression:
                        newNodes.Add(VisitAssignmentExpression((AssignmentExpressionSyntax)item, newScope));
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
            methodDeclarationSyntax.Body.Members = new(newNodes);

            return methodDeclarationSyntax;
        }

        public virtual ParameterDeclarationSyntax VisitMethodParameter(ParameterDeclarationSyntax parameterDeclarationSyntax, TScope scope)
        {
            return parameterDeclarationSyntax;
        }

        public virtual FieldDeclarationSyntax VisitFieldDeclaration(FieldDeclarationSyntax fieldDeclarationSyntax, TScope scope)
        {
            if (fieldDeclarationSyntax.InitialValue is not null)
            {
                fieldDeclarationSyntax.InitialValue = VisitExpression(fieldDeclarationSyntax.InitialValue, scope);
            }

            return fieldDeclarationSyntax;
        }

        public virtual LocalDeclarationSyntax VisitLocalDeclaration(LocalDeclarationSyntax localDeclarationSyntax, TScope scope)
        {
            if (localDeclarationSyntax.InitialValue is not null)
            {
                localDeclarationSyntax.InitialValue = VisitExpression(localDeclarationSyntax.InitialValue, scope);
            }

            return localDeclarationSyntax;
        }
        #endregion Declarations

        #region Statements
        public virtual StatementSyntax VisitElseStatement(StatementSyntax elseStatementSyntax, TScope scope)
        {
            throw new System.NotImplementedException();
        }

        public virtual StatementSyntax VisitWhileStatement(WhileStatementSyntax whileStatementSyntax, TScope scope)
        {
            throw new System.NotImplementedException();
        }

        public virtual StatementSyntax VisitReturnStatement(ReturnStatementSyntax returnStatementSyntax, TScope scope)
        {
            if (returnStatementSyntax.ReturnValue is not null)
            {
                returnStatementSyntax.ReturnValue = VisitExpression(returnStatementSyntax.ReturnValue, scope);
            }

            return returnStatementSyntax;
        }

        public virtual StatementSyntax VisitIfStatement(IfStatementSyntax ifStatementSyntax, TScope scope)
        {
            ifStatementSyntax.Condition = VisitExpression(ifStatementSyntax.Condition, scope);

            var newScope = NewScope(scope);

            var newNodes = new List<SyntaxNode>();
            foreach (var item in ifStatementSyntax.Body.Members)
            {
                switch (item.Kind)
                {
                    case SyntaxKind.MethodDeclaration:
                        newNodes.Add(VisitMethodDeclaration((MethodDeclarationSyntax)item, newScope));
                        break;
                    case SyntaxKind.LocalDeclaration:
                        newNodes.Add(VisitLocalDeclaration((LocalDeclarationSyntax)item, newScope));
                        break;
                    case SyntaxKind.ReturnStatement:
                        newNodes.Add(VisitReturnStatement((ReturnStatementSyntax)item, newScope));
                        break;
                    case SyntaxKind.AssignmentExpression:
                        newNodes.Add(VisitAssignmentExpression((AssignmentExpressionSyntax)item, newScope));
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
            ifStatementSyntax.Body.Members = new(newNodes);

            return ifStatementSyntax;
        }
        #endregion Statements

        #region Expressions
        public virtual AssignmentExpressionSyntax VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, TScope scope)
        {
            assignmentExpressionSyntax.Right = VisitExpression(assignmentExpressionSyntax.Right, scope);
            assignmentExpressionSyntax.Left = VisitExpression(assignmentExpressionSyntax.Left, scope);
            return assignmentExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitExpression(ExpressionSyntax expressionSyntax, TScope scope)
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
                case SyntaxKind.AssignmentExpression:
                    return VisitAssignmentExpression((AssignmentExpressionSyntax)expressionSyntax, scope);
                default:
                    throw new UnreachableException();
            }
        }

        public virtual LiteralExpressionSyntax VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, TScope scope)
        {
            return literalExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, TScope scope)
        {
            return VisitExpression(memberExpressionSyntax, scope);
        }

        public virtual ExpressionSyntax VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, TScope scope)
        {
            return VisitExpression(unaryExpressionSyntax, scope);
        }
          
        public virtual ExpressionSyntax VisitCastExpression(CastExpressionSyntax castExpressionSyntax, TScope scope)
        {
            castExpressionSyntax.Value = VisitExpression(castExpressionSyntax.Value, scope);

            return castExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, TScope scope)
        {
            binaryExpressionSyntax.Left = VisitExpression(binaryExpressionSyntax.Left, scope);
            binaryExpressionSyntax.Right = VisitExpression(binaryExpressionSyntax.Right, scope);

            return binaryExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, TScope scope)
        {
            logicalExpressionSyntax.Left = VisitExpression(logicalExpressionSyntax.Left, scope);
            logicalExpressionSyntax.Right = VisitExpression(logicalExpressionSyntax.Right, scope);

            return logicalExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, TScope scope)
        {
            return identifierExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitCallExpression(CallExpressionSyntax callExpressionSyntax, TScope scope)
        {
            return callExpressionSyntax;
        }
        #endregion Expressions


        protected virtual TScope NewScope(TScope parent)
        {
            TScope newScope = new()
            {
                Parent = parent
            };
            return newScope;
        }
    }
}
