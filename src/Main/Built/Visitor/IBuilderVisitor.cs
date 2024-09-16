//using System.Collections.Generic;
//using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
//using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
//using JiteLang.Main.LangParser.SyntaxTree;
//using JiteLang.Main.Builder.Instructions;
//using JiteLang.Main.Built.Statements.Declaration;
//using JiteLang.Main.Built.Expressions;
//using JiteLang.Main.Builder;
//using JiteLang.Main.Shared;

//namespace JiteLang.Main.Built.Visitor
//{
//    internal interface IBuilderVisitor
//    {
//        BuiltNamespaceDeclaration VisitNamespaceDeclaration(NamespaceDeclarationSyntax namespaceDeclarationSyntax, Scope context);
//        BuiltClassDeclaration VisitClassDeclaration(ClassDeclarationSyntax classDeclarationSyntax, Scope context);
//        BuiltMethodDeclaration VisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, Scope context);
//        BuiltVariableDeclaration VisitVariableDeclaration(VariableDeclarationSyntax variableDeclarationSyntax, Scope context);
//        BuiltAssignmentExpression VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, Scope context);
//        BuiltExpression VisitExpression(ExpressionSyntax expressionSyntax, Scope context);
//        BuiltLiteralExpression VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, Scope context);
//        BuiltExpression VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, Scope context);
//        BuiltExpression VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, Scope context);
//        BuiltExpression VisitCastExpression(CastExpressionSyntax castExpressionSyntax, Scope context);
//        BuiltExpression VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, Scope context);
//        BuiltExpression VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, Scope context);
//        BuiltExpression VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, Scope context);
//    }
//}
