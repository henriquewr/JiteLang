using System;
using System.Collections.Generic;
using System.Diagnostics;
using JiteLang.Main.LangParser;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Main.LangParser.Types;
using JiteLang.Main.LangParser.Types.Predefined;
using JiteLang.Main.Visitor.Syntax;
using JiteLang.Syntax;

namespace JiteLang.Main.Visitor.Type
{
    internal class TypeVisitor : ITypeVisitor
    {
        protected readonly SyntaxVisitor _syntaxVisitor;

        public TypeVisitor(SyntaxVisitor syntaxVisitor) 
        {
            _syntaxVisitor = syntaxVisitor;
        }

        public virtual TypeSyntax? VisitExpression(ExpressionSyntax expressionSyntax, Scope context)
        {
            var visitedExpr = _syntaxVisitor.VisitExpression(expressionSyntax, context);

            switch (visitedExpr.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return VisitLiteralExpression((LiteralExpressionSyntax)visitedExpr, context);
                case SyntaxKind.MemberExpression:
                    return VisitMemberExpression((MemberExpressionSyntax)visitedExpr, context);
                case SyntaxKind.UnaryExpression:
                    return VisitUnaryExpression((UnaryExpressionSyntax)visitedExpr, context);
                case SyntaxKind.CastExpression:        
                    return VisitCastExpression((CastExpressionSyntax)visitedExpr, context);
                case SyntaxKind.BinaryExpression:
                    return VisitBinaryExpression((BinaryExpressionSyntax)visitedExpr, context);
                case SyntaxKind.LogicalExpression:
                    return VisitLogicalExpression((LogicalExpressionSyntax)visitedExpr, context);
                case SyntaxKind.IdentifierExpression:
                    return VisitIdentifierExpression((IdentifierExpressionSyntax)visitedExpr, context);
                default:
                    throw new UnreachableException();
            }
        }

        public virtual TypeSyntax VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, Scope context)
        {
            var visitedLiteralExpr = _syntaxVisitor.VisitLiteralExpression(literalExpressionSyntax, context);

            return SyntaxFactory.PredefinedTypeFromLiteral(visitedLiteralExpr.Value.Kind, default);
        }

        public virtual TypeSyntax VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, Scope context)
        {
            var visitedMemberExpr = _syntaxVisitor.VisitMemberExpression(memberExpressionSyntax, context);

            throw new NotImplementedException();
        }

        public virtual TypeSyntax VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, Scope context)
        {
            var visitedUnaryExpr = _syntaxVisitor.VisitUnaryExpression(unaryExpressionSyntax, context);

            throw new NotImplementedException();
        }
        public virtual TypeSyntax VisitCastExpression(CastExpressionSyntax castExpressionSyntax, Scope context)
        {
            var visitedValue = _syntaxVisitor.VisitExpression(castExpressionSyntax.Value, context);

            throw new NotImplementedException();
        }
        public virtual TypeSyntax? VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, Scope context)
        {
            var leftNodes = new Stack<BinaryExpressionSyntax>();

            ExpressionSyntax current = binaryExpressionSyntax;

            while (current.Kind == SyntaxKind.BinaryExpression)
            {
                var binaryExpr = (BinaryExpressionSyntax)current;
                leftNodes.Push(binaryExpr);
                current = binaryExpr.Left;
            }

            TypeSyntax? currentType = null;

            while (leftNodes.TryPop(out var binaryExprNode))
            {
                var visitedRight = VisitExpression(binaryExprNode.Right, context);
                var visitedLeft = VisitExpression(binaryExprNode.Left, context);

                if (visitedRight is null || visitedLeft is null)
                {
                    return null;    
                }

                if (visitedRight.IsPreDefined && visitedLeft.IsPreDefined)
                {
                    var right = (PredefinedTypeSyntax)visitedRight;
                    var left = (PredefinedTypeSyntax)visitedLeft;
                    if(left.Keyword.Kind != right.Keyword.Kind)
                    {
                        return null;
                    }
                    currentType = right;// GetResultType(left, right, binaryExprNode.Operation);
                }
            }

            return currentType;


            //static PredefinedTypeSyntax GetResultType(PredefinedTypeSyntax left, PredefinedTypeSyntax right, SyntaxKind operation)
            //{
            //    var anyIsString = left.Keyword.Kind == SyntaxKind.StringKeyword || right.Keyword.Kind == SyntaxKind.StringKeyword;
            //    var anyIsLong = left.Keyword.Kind == SyntaxKind.LongKeyword || right.Keyword.Kind == SyntaxKind.LongKeyword;
            //    var anyIsInt = left.Keyword.Kind == SyntaxKind.IntKeyword || right.Keyword.Kind == SyntaxKind.IntKeyword;

            //    switch (operation)
            //    {
            //        case SyntaxKind.PlusToken:
            //            if (anyIsString)
            //            {
            //                return SyntaxFactory.PredefinedType(SyntaxKind.StringKeyword);
            //            }

            //            if (anyIsInt)
            //            {
            //                return anyIsLong ? SyntaxFactory.PredefinedType(SyntaxKind.LongKeyword) : SyntaxFactory.PredefinedType(SyntaxKind.IntKeyword);
            //            }
            //            break;

            //        case SyntaxKind.MinusToken:
            //            if (anyIsInt)
            //            {
            //                return anyIsLong ? SyntaxFactory.PredefinedType(SyntaxKind.LongKeyword) : SyntaxFactory.PredefinedType(SyntaxKind.IntKeyword);
            //            }
            //            break;

            //        case SyntaxKind.AsteriskToken:
            //            if (anyIsInt)
            //            {
            //                return anyIsLong ? SyntaxFactory.PredefinedType(SyntaxKind.LongKeyword) : SyntaxFactory.PredefinedType(SyntaxKind.IntKeyword);
            //            }
            //            break;

            //    }

            //    throw new InvalidCastException();
            //}
        }

        public virtual TypeSyntax VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, Scope context)
        {
            var visitedLeft = VisitExpression(logicalExpressionSyntax.Left, context);
            var visitedRight = VisitExpression(logicalExpressionSyntax.Right, context);

            throw new NotImplementedException();
        }

        public virtual TypeSyntax VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, Scope context)
        {
            var visitedIdentifierExpr = _syntaxVisitor.VisitIdentifierExpression(identifierExpressionSyntax, context);

            throw new NotImplementedException();
        }
    }
}