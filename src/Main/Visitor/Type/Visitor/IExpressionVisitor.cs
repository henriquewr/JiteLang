//using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
//using JiteLang.Main.Visitor.Type.Scope;

//namespace JiteLang.Main.Visitor.Type.Visitor
//{
//    internal interface IExpressionVisitor<T>
//    {
//        T VisitExpression(ExpressionSyntax expressionSyntax, TypeScope scope);
//        T VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, TypeScope scope);
//        T VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, TypeScope scope);
//        T VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, TypeScope scope);
//        T VisitCastExpression(CastExpressionSyntax castExpressionSyntax, TypeScope scope);
//        T VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, TypeScope scope);
//        T VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, TypeScope scope);
//        T VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, TypeScope scope);
//        T VisitCallExpression(CallExpressionSyntax callExpressionSyntax, TypeScope scope);
//        T VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, TypeScope scope);
//        T VisitNewExpression(NewExpressionSyntax newExpressionSyntax, TypeScope scope);
//    }
//}