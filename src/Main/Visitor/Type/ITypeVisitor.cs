using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.Types;
using JiteLang.Main.Shared;
using JiteLang.Main.Visitor.Type.Scope;

namespace JiteLang.Main.Visitor.Type
{
    internal interface ITypeVisitor
    {
        TypeSyntax? VisitExpression(ExpressionSyntax expressionSyntax, TypeScope context);
        TypeSyntax VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, TypeScope context);
        TypeSyntax VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, TypeScope context);
        TypeSyntax VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, TypeScope context);
        TypeSyntax VisitCastExpression(CastExpressionSyntax castExpressionSyntax, TypeScope context);
        TypeSyntax? VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, TypeScope context);
        TypeSyntax VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, TypeScope context);
        TypeSyntax VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, TypeScope context);
    }
}
