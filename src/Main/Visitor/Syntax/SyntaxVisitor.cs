using System.Collections.Generic;
using System.Diagnostics;
using JiteLang.Main.AsmBuilder;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Syntax;

namespace JiteLang.Main.Visitor.Syntax
{
    internal class SyntaxVisitor : ISyntaxVisitor<IList<SyntaxNode>,
        ClassDeclarationSyntax, 
        MethodDeclarationSyntax, 
        VariableDeclarationSyntax, 
        AssignmentExpressionSyntax, 
        ExpressionSyntax, 
        StatementSyntax,
        LiteralExpressionSyntax,
        ParameterDeclarationSyntax>
    {
        public SyntaxVisitor()
        {
        }

        public virtual IList<SyntaxNode> VisitNamespaceDeclaration(NamespaceDeclarationSyntax root, Scope context)
        {
            var newNodes = new List<SyntaxNode>();

            foreach (var classDeclaration in root.Body.Members)
            {
                newNodes.Add(VisitClassDeclaration(classDeclaration, context));
            }

            return newNodes;
        }

        public virtual ClassDeclarationSyntax VisitClassDeclaration(ClassDeclarationSyntax classDeclaration, Scope context)
        {
            var newNodes = new List<SyntaxNode>();

            var newContext = new Scope(context);

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

        public virtual MethodDeclarationSyntax VisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, Scope context)
        {
            var newContext = new Scope(context);

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


        public ParameterDeclarationSyntax VisitMethodParameter(ParameterDeclarationSyntax parameterDeclarationSyntax, Scope context)
        {
            return parameterDeclarationSyntax;
        }

        public virtual VariableDeclarationSyntax VisitVariableDeclaration(VariableDeclarationSyntax variableDeclarationSyntax, Scope context)
        {
            if (variableDeclarationSyntax.InitialValue is not null)
            {
                variableDeclarationSyntax.InitialValue = VisitExpression(variableDeclarationSyntax.InitialValue, context);
            }

            return variableDeclarationSyntax;
        }

        public StatementSyntax VisitReturnStatement(ReturnStatementSyntax returnStatementSyntax, Scope context)
        {
            if (returnStatementSyntax.ReturnValue is not null)
            {
                returnStatementSyntax.ReturnValue = VisitExpression(returnStatementSyntax.ReturnValue, context);
            }

            return returnStatementSyntax;
        }

        public StatementSyntax VisitIfStatement(IfStatementSyntax ifStatementSyntax, Scope context)
        {
            ifStatementSyntax.Condition = VisitExpression(ifStatementSyntax.Condition, context);

            var newContext = new Scope(context);

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

        public virtual AssignmentExpressionSyntax VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, Scope context)
        {
            assignmentExpressionSyntax.Right = VisitExpression(assignmentExpressionSyntax.Right, context);
            assignmentExpressionSyntax.Left = VisitExpression(assignmentExpressionSyntax.Left, context);
            return assignmentExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitExpression(ExpressionSyntax expressionSyntax, Scope context)
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

        public virtual LiteralExpressionSyntax VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, Scope context)
        {
            return literalExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, Scope context)
        {
            return VisitExpression(memberExpressionSyntax, context);
        }

        public virtual ExpressionSyntax VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, Scope context)
        {
            return VisitExpression(unaryExpressionSyntax, context);
        }
          
        public virtual ExpressionSyntax VisitCastExpression(CastExpressionSyntax castExpressionSyntax, Scope context)
        {
            castExpressionSyntax.Value = VisitExpression(castExpressionSyntax.Value, context);

            return castExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, Scope context)
        {
            binaryExpressionSyntax.Left = VisitExpression(binaryExpressionSyntax.Left, context);
            binaryExpressionSyntax.Right = VisitExpression(binaryExpressionSyntax.Right, context);

            return binaryExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, Scope context)
        {
            logicalExpressionSyntax.Left = VisitExpression(logicalExpressionSyntax.Left, context);
            logicalExpressionSyntax.Right = VisitExpression(logicalExpressionSyntax.Right, context);

            return logicalExpressionSyntax;
        }

        public virtual ExpressionSyntax VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, Scope context)
        {
            return identifierExpressionSyntax;
        }

        public ExpressionSyntax VisitCallExpression(CallExpressionSyntax callExpressionSyntax, Scope context)
        {
            return callExpressionSyntax;
        }
    }
}
