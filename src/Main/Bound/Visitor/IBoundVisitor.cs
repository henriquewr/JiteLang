using JiteLang.Main.Bound.Statements.Declaration;
using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Bound.Statements;

namespace JiteLang.Main.Bound.Visitor
{
    internal interface IBoundVisitor<TNamespaceDeclaration, TClassDeclaration, TMethodDeclaration, TVariableDeclaration, TAssignmentExpression, TExpression, TStatement, TLiteralExpression, TParameterDeclaration, TScope>
    {
        #region Declarations
        TNamespaceDeclaration VisitNamespaceDeclaration(BoundNamespaceDeclaration root, TScope scope);
        TClassDeclaration VisitClassDeclaration(BoundClassDeclaration classDeclaration, TScope scope);
        TMethodDeclaration VisitMethodDeclaration(BoundMethodDeclaration methodDeclaration, TScope scope);
        TVariableDeclaration VisitVariableDeclaration(BoundVariableDeclaration variableDeclaration, TScope scope);
        TParameterDeclaration VisitMethodParameter(BoundParameterDeclaration parameterDeclaration, TScope scope);
        #endregion Declarations

        #region Statements
        TStatement VisitReturnStatement(BoundReturnStatement returnStatement, TScope scope);
        TStatement VisitIfStatement(BoundIfStatement ifStatement, TScope scope);
        TStatement VisitElseStatement(BoundStatement elseStatement, TScope scope);
        TStatement VisitWhileStatement(BoundWhileStatement whileStatement, TScope scope);
        #endregion Statements

        #region Expressions
        TAssignmentExpression VisitAssignmentExpression(BoundAssignmentExpression assignmentExpression, TScope scope);
        TExpression VisitExpression(BoundExpression expression, TScope scope);
        TLiteralExpression VisitLiteralExpression(BoundLiteralExpression  literalExpression, TScope scope);
        TExpression VisitMemberExpression(BoundMemberExpression memberExpression, TScope scope);
        TExpression VisitUnaryExpression(BoundUnaryExpression unaryExpression, TScope scope);
        TExpression VisitCastExpression(BoundCastExpression castExpression, TScope scope);
        TExpression VisitBinaryExpression(BoundBinaryExpression binaryExpression, TScope scope);
        TExpression VisitLogicalExpression(BoundLogicalExpression logicalExpression, TScope scope);
        TExpression VisitIdentifierExpression(BoundIdentifierExpression identifierExpression, TScope scope);
        TExpression VisitCallExpression(BoundCallExpression callExpression, TScope scope);
        #endregion Expressions
    }

    internal interface IBuiltVisitor<TScope>
    {
        #region Declarations
        void VisitNamespaceDeclaration(BoundNamespaceDeclaration root, TScope scope);
        void VisitClassDeclaration(BoundClassDeclaration classDeclaration, TScope scope);
        void VisitMethodDeclaration(BoundMethodDeclaration methodDeclaration, TScope scope);
        void VisitVariableDeclaration(BoundVariableDeclaration variableDeclaration, TScope scope);
        void VisitMethodParameter(BoundParameterDeclaration nparameterDeclaration, TScope scope);
        #endregion Declarations

        #region Statements
        void VisitReturnStatement(BoundReturnStatement returnStatement, TScope scope);
        void VisitIfStatement(BoundIfStatement ifStatement, TScope scope);
        void VisitElseStatement(BoundStatement elseStatement, TScope scope);
        void VisitWhileStatement(BoundWhileStatement whileStatement, TScope scope);
        #endregion Statements

        #region Expressions
        void VisitAssignmentExpression(BoundAssignmentExpression assignmentExpression, TScope scope);
        void VisitExpression(BoundExpression expression, TScope scope);
        void VisitLiteralExpression(BoundLiteralExpression literalExpression, TScope scope);
        void VisitMemberExpression(BoundMemberExpression memberExpression, TScope scope);
        void VisitUnaryExpression(BoundUnaryExpression unaryExpression, TScope scope);
        void VisitCastExpression(BoundCastExpression castExpression, TScope scope);
        void VisitBinaryExpression(BoundBinaryExpression binaryExpression, TScope scope);
        void VisitLogicalExpression(BoundLogicalExpression logicalExpression, TScope scope);
        void VisitIdentifierExpression(BoundIdentifierExpression identifierExpression, TScope scope);
        void VisitCallExpression(BoundCallExpression callExpression, TScope scope);
        #endregion Expressions
    }
}
