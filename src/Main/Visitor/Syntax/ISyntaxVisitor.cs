using System.Collections.Generic;
using JiteLang.Main.LangParser;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Main.LangParser.SyntaxTree;

namespace JiteLang.Main.Visitor.Syntax
{
    internal interface ISyntaxVisitor<TNamespaceDeclaration, TClassDeclaration, TMethodDeclaration, TVariableDeclaration, TAssignmentExpression, TExpression, TStatement, TLiteralExpression, TParameterDeclaration>
    {
        TNamespaceDeclaration VisitNamespaceDeclaration(NamespaceDeclarationSyntax root, Scope context);
        TClassDeclaration VisitClassDeclaration(ClassDeclarationSyntax classDeclaration, Scope context);
        TMethodDeclaration VisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, Scope context);
        TParameterDeclaration VisitMethodParameter(ParameterDeclarationSyntax parameterDeclarationSyntax, Scope context);
        TVariableDeclaration VisitVariableDeclaration(VariableDeclarationSyntax variableDeclarationSyntax, Scope context);
        TStatement VisitReturnStatement(ReturnStatementSyntax returnStatementSyntax, Scope context);
        TStatement VisitIfStatement(IfStatementSyntax ifStatementSyntax, Scope context);
        TAssignmentExpression VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, Scope context);
        TExpression VisitExpression(ExpressionSyntax expressionSyntax, Scope context);
        TLiteralExpression VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, Scope context);
        TExpression VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, Scope context);
        TExpression VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, Scope context);
        TExpression VisitCastExpression(CastExpressionSyntax castExpressionSyntax, Scope context);
        TExpression VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, Scope context);
        TExpression VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, Scope context);
        TExpression VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, Scope context);
        TExpression VisitCallExpression(CallExpressionSyntax callExpressionSyntax, Scope context);
    }
}
