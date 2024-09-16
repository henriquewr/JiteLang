using System.Collections.Generic;
using System.Diagnostics;
using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Main.Shared;
using JiteLang.Syntax;

namespace JiteLang.Main.Visitor.Syntax
{
    internal class SyntaxVisitor<TScope> : ISyntaxVisitor<IList<SyntaxNode>,
        ClassDeclarationSyntax, 
        MethodDeclarationSyntax, 
        VariableDeclarationSyntax, 
        AssignmentExpressionSyntax, 
        ExpressionSyntax, 
        StatementSyntax,
        LiteralExpressionSyntax,
        ParameterDeclarationSyntax, 
        CodeScope>
    {
        public SyntaxVisitor()
        {
        }


        #region Declarations
        public virtual IList<SyntaxNode> VisitNamespaceDeclaration(NamespaceDeclarationSyntax root, CodeScope context)
        {
            var newNodes = new List<SyntaxNode>();

            foreach (var classDeclaration in root.Body.Members)
            {
                newNodes.Add(VisitClassDeclaration(classDeclaration, context));
            }

            return newNodes;
        }

        public virtual ClassDeclarationSyntax VisitClassDeclaration(ClassDeclarationSyntax classDeclaration, CodeScope context)
        {
            var newNodes = new List<SyntaxNode>();

            var newContext = new CodeScope(context);

            foreach (var item in classDeclaration.Body.Members)
            {
                switch (item.Kind)
                {
                    case SyntaxKind.ClassDeclaration:
                        newNodes.Add(VisitClassDeclaration((ClassDeclarationSyntax)item, newContext));
                        break;
                    case SyntaxKind.MethodDeclaration:
                        newNodes.Add(VisitMethodDeclaration((MethodDeclarationSyntax)item, newContext));
                        break;
                    case SyntaxKind.VariableDeclaration:
                        newNodes.Add(VisitVariableDeclaration((VariableDeclarationSyntax)item, newContext));
                        break;
                    default:
                        throw new UnreachableException();
                }
            }

            classDeclaration.Body.Members = newNodes;

            return classDeclaration;
        }

        public virtual MethodDeclarationSyntax VisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, CodeScope context)
        {
            var newContext = new CodeScope(context);

            var newParameters = new List<ParameterDeclarationSyntax>();
            foreach (var item in methodDeclarationSyntax.Params)
            {
                newParameters.Add(VisitMethodParameter(item, newContext));
            }
            methodDeclarationSyntax.Params = newParameters;

            var newNodes = new List<SyntaxNode>();
            foreach (var item in methodDeclarationSyntax.Body.Members)
            {
                switch (item.Kind)
                {
                    case SyntaxKind.MethodDeclaration:
                        newNodes.Add(VisitMethodDeclaration((MethodDeclarationSyntax)item, newContext));
                        break;
                    case SyntaxKind.VariableDeclaration:
                        newNodes.Add(VisitVariableDeclaration((VariableDeclarationSyntax)item, newContext));
                        break;       
                    case SyntaxKind.ReturnStatement:
                        newNodes.Add(VisitReturnStatement((ReturnStatementSyntax)item, newContext));
                        break;
                    case SyntaxKind.AssignmentExpression:
                        newNodes.Add(VisitAssignmentExpression((AssignmentExpressionSyntax)item, newContext));
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
            methodDeclarationSyntax.Body.Members = newNodes;

            return methodDeclarationSyntax;
        }

        public virtual ParameterDeclarationSyntax VisitMethodParameter(ParameterDeclarationSyntax parameterDeclarationSyntax, CodeScope context)
        {
            return parameterDeclarationSyntax;
        }

        public virtual VariableDeclarationSyntax VisitVariableDeclaration(VariableDeclarationSyntax variableDeclarationSyntax, CodeScope context)
        {
            if (variableDeclarationSyntax.InitialValue is not null)
            {
                variableDeclarationSyntax.InitialValue = VisitExpression(variableDeclarationSyntax.InitialValue, context);
            }

            return variableDeclarationSyntax;
        }
        #endregion Declarations

        #region Statements
        public virtual StatementSyntax VisitElseStatement(StatementSyntax elseStatementSyntax, CodeScope scope)
        {
            throw new System.NotImplementedException();
        }

        public virtual StatementSyntax VisitWhileStatement(WhileStatementSyntax whileStatementSyntax, CodeScope scope)
        {
            throw new System.NotImplementedException();
        }

        public virtual StatementSyntax VisitReturnStatement(ReturnStatementSyntax returnStatementSyntax, CodeScope context)
        {
            if (returnStatementSyntax.ReturnValue is not null)
            {
                returnStatementSyntax.ReturnValue = VisitExpression(returnStatementSyntax.ReturnValue, context);
            }

            return returnStatementSyntax;
        }

        public virtual StatementSyntax VisitIfStatement(IfStatementSyntax ifStatementSyntax, CodeScope context)
        {
            ifStatementSyntax.Condition = VisitExpression(ifStatementSyntax.Condition, context);

            var newContext = new CodeScope(context);

            var newNodes = new List<SyntaxNode>();
            foreach (var item in ifStatementSyntax.Body.Members)
            {
                switch (item.Kind)
                {
                    case SyntaxKind.MethodDeclaration:
                        newNodes.Add(VisitMethodDeclaration((MethodDeclarationSyntax)item, newContext));
                        break;
                    case SyntaxKind.VariableDeclaration:
                        newNodes.Add(VisitVariableDeclaration((VariableDeclarationSyntax)item, newContext));
                        break;
                    case SyntaxKind.ReturnStatement:
                        newNodes.Add(VisitReturnStatement((ReturnStatementSyntax)item, newContext));
                        break;
                    case SyntaxKind.AssignmentExpression:
                        newNodes.Add(VisitAssignmentExpression((AssignmentExpressionSyntax)item, newContext));
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
            ifStatementSyntax.Body.Members = newNodes;

            return ifStatementSyntax;
        }
        #endregion Statements

        #region Expressions
        public virtual AssignmentExpressionSyntax VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, CodeScope context)
        {
            assignmentExpressionSyntax.Right = VisitExpression(assignmentExpressionSyntax.Right, context);
            assignmentExpressionSyntax.Left = VisitExpression(assignmentExpressionSyntax.Left, context);
            return assignmentExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitExpression(ExpressionSyntax expressionSyntax, CodeScope context)
        {
            switch (expressionSyntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return VisitLiteralExpression((LiteralExpressionSyntax)expressionSyntax, context);
                case SyntaxKind.MemberExpression:
                    return VisitMemberExpression((MemberExpressionSyntax)expressionSyntax, context);
                case SyntaxKind.UnaryExpression:
                    return VisitUnaryExpression((UnaryExpressionSyntax)expressionSyntax, context);
                case SyntaxKind.CastExpression:
                    return VisitCastExpression((CastExpressionSyntax)expressionSyntax, context);
                case SyntaxKind.BinaryExpression:
                    return VisitBinaryExpression((BinaryExpressionSyntax)expressionSyntax, context);
                case SyntaxKind.LogicalExpression:
                    return VisitLogicalExpression((LogicalExpressionSyntax)expressionSyntax, context);
                case SyntaxKind.IdentifierExpression:
                    return VisitIdentifierExpression((IdentifierExpressionSyntax)expressionSyntax, context);                
                case SyntaxKind.AssignmentExpression:
                    return VisitAssignmentExpression((AssignmentExpressionSyntax)expressionSyntax, context);
                default:
                    throw new UnreachableException();
            }
        }

        public virtual LiteralExpressionSyntax VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, CodeScope context)
        {
            return literalExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, CodeScope context)
        {
            return VisitExpression(memberExpressionSyntax, context);
        }

        public virtual ExpressionSyntax VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, CodeScope context)
        {
            return VisitExpression(unaryExpressionSyntax, context);
        }
          
        public virtual ExpressionSyntax VisitCastExpression(CastExpressionSyntax castExpressionSyntax, CodeScope context)
        {
            castExpressionSyntax.Value = VisitExpression(castExpressionSyntax.Value, context);

            return castExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, CodeScope context)
        {
            binaryExpressionSyntax.Left = VisitExpression(binaryExpressionSyntax.Left, context);
            binaryExpressionSyntax.Right = VisitExpression(binaryExpressionSyntax.Right, context);

            return binaryExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, CodeScope context)
        {
            logicalExpressionSyntax.Left = VisitExpression(logicalExpressionSyntax.Left, context);
            logicalExpressionSyntax.Right = VisitExpression(logicalExpressionSyntax.Right, context);

            return logicalExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, CodeScope context)
        {
            return identifierExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitCallExpression(CallExpressionSyntax callExpressionSyntax, CodeScope context)
        {
            return callExpressionSyntax;
        }
        #endregion Expressions
    }
}
