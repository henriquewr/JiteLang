using JiteLang.Main.LangParser;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;

namespace JiteLang.Main.Visitor.Type
{
    internal interface ITypeVisitor
    {
        TypeSyntax? VisitExpression(ExpressionSyntax expressionSyntax, Scope context);
        TypeSyntax VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, Scope context);
        TypeSyntax VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, Scope context);
        TypeSyntax VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, Scope context);
        TypeSyntax VisitCastExpression(CastExpressionSyntax castExpressionSyntax, Scope context);
        TypeSyntax? VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, Scope context);
        TypeSyntax VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, Scope context);
        TypeSyntax VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, Scope context);
    }
}
