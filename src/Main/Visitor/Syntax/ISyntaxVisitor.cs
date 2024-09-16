using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;

namespace JiteLang.Main.Visitor.Syntax
{
    internal interface ISyntaxVisitor<TNamespaceDeclaration, TClassDeclaration, TMethodDeclaration, TVariableDeclaration, TAssignmentExpression, TExpression, TStatement, TLiteralExpression, TParameterDeclaration, TScope>
    {
        #region Declarations
        TNamespaceDeclaration VisitNamespaceDeclaration(NamespaceDeclarationSyntax root, TScope scope);
        TClassDeclaration VisitClassDeclaration(ClassDeclarationSyntax classDeclaration, TScope scope);
        TMethodDeclaration VisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, TScope scope);
        TVariableDeclaration VisitVariableDeclaration(VariableDeclarationSyntax variableDeclarationSyntax, TScope scope);
        TParameterDeclaration VisitMethodParameter(ParameterDeclarationSyntax parameterDeclarationSyntax, TScope scope);
        #endregion Declarations

        #region Statements
        TStatement VisitReturnStatement(ReturnStatementSyntax returnStatementSyntax, TScope scope);
        TStatement VisitIfStatement(IfStatementSyntax ifStatementSyntax, TScope scope);
        TStatement VisitElseStatement(StatementSyntax elseStatementSyntax, TScope scope);
        TStatement VisitWhileStatement(WhileStatementSyntax whileStatementSyntax, TScope scope);
        #endregion Statements

        #region Expressions
        TAssignmentExpression VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, TScope scope);
        TExpression VisitExpression(ExpressionSyntax expressionSyntax, TScope scope);
        TLiteralExpression VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, TScope scope);
        TExpression VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, TScope scope);
        TExpression VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, TScope scope);
        TExpression VisitCastExpression(CastExpressionSyntax castExpressionSyntax, TScope scope);
        TExpression VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, TScope scope);
        TExpression VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, TScope scope);
        TExpression VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, TScope scope);
        TExpression VisitCallExpression(CallExpressionSyntax callExpressionSyntax, TScope scope);
        #endregion Expressions
    }

    internal interface ISyntaxVisitor<TScope>
    {
        #region Declarations
        void VisitNamespaceDeclaration(NamespaceDeclarationSyntax root, TScope scope);
        void VisitClassDeclaration(ClassDeclarationSyntax classDeclaration, TScope scope);
        void VisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, TScope scope);
        void VisitVariableDeclaration(VariableDeclarationSyntax variableDeclarationSyntax, TScope scope);
        void VisitMethodParameter(ParameterDeclarationSyntax parameterDeclarationSyntax, TScope scope);
        #endregion Declarations

        #region Statements
        void VisitReturnStatement(ReturnStatementSyntax returnStatementSyntax, TScope scope);
        void VisitIfStatement(IfStatementSyntax ifStatementSyntax, TScope scope);
        void VisitElseStatement(StatementSyntax elseStatementSyntax, TScope scope);
        void VisitWhileStatement(WhileStatementSyntax whileStatementSyntax, TScope scope);
        #endregion Statements

        #region Expressions
        void VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, TScope scope);
        void VisitExpression(ExpressionSyntax expressionSyntax, TScope scope);
        void VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, TScope scope);
        void VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, TScope scope);
        void VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, TScope scope);
        void VisitCastExpression(CastExpressionSyntax castExpressionSyntax, TScope scope);
        void VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, TScope scope);
        void VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, TScope scope);
        void VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, TScope scope);
        void VisitCallExpression(CallExpressionSyntax callExpressionSyntax, TScope scope);
        #endregion Expressions
    }
}
