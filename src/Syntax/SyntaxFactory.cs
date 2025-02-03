using System;
using System.Diagnostics;
using JiteLang.Main.LangLexer.Token;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Main.LangParser.Types;
using JiteLang.Main.LangParser.Types.Predefined;

namespace JiteLang.Syntax
{
    internal class SyntaxFactory
    {
        public SyntaxFactory() { }

        public static PredefinedTypeSyntax PredefinedTypeFromLiteral(in TokenInfo token)
        {
            return PredefinedTypeFromLiteral(token.Kind, token.Position);
        }

        public static PredefinedTypeSyntax PredefinedTypeFromLiteral(SyntaxKind literalKind, in SyntaxPosition position)
        {
            var keyword = SyntaxFacts.GetKeywordFromLiteral(literalKind);

            return PredefinedType(keyword, position);
        }

        public static PredefinedTypeSyntax PredefinedType(in TokenInfo token)
        {
            return PredefinedType(token.Kind, token.Position);
        }

        public static TypeSyntax Type(in TokenInfo token)
        {
            var isPredefined = SyntaxFacts.IsPredefinedType(token.Kind);
            if (isPredefined)
            {
                return PredefinedType(token);
            }
            var sytaxToken = Token(token);

            var typeSyntax = new TypeSyntax(sytaxToken);
            return typeSyntax;
        }

        public static PredefinedTypeSyntax PredefinedType(SyntaxKind keyword, SyntaxPosition position)
        {
            var text = SyntaxFacts.GetText(keyword);
            if (text is null)
            {
                throw new ArgumentException($"{keyword} is not a predefined type");
            }
           
            var token = new SyntaxToken(keyword, text, position);
            var predefined = new PredefinedTypeSyntax(token);

            return predefined;
        }

        public static VariableDeclarationSyntax DeclareVariable(in TokenInfo token, SyntaxNode parent, IdentifierExpressionSyntax identifier)
        {
            var type = Type(token);

            var varDeclaration = new VariableDeclarationSyntax(parent, identifier, type);

            return varDeclaration;
        }

        public static IdentifierExpressionSyntax Identifier(SyntaxNode parent, in TokenInfo token)
        {
            return Identifier(token.Text, parent, token.Position);
        }

        public static IdentifierExpressionSyntax Identifier(string text, SyntaxNode parent, in SyntaxPosition position)
        {
            var identifier = new IdentifierExpressionSyntax(parent, text, position);
            return identifier;
        }

        public static SyntaxToken TokenWithValue(in TokenInfo tokenInfo)
        {
            switch (tokenInfo.Kind)
            {
                case SyntaxKind.StringLiteralToken:
                    return new SyntaxTokenWithValue<string>(tokenInfo.Kind, tokenInfo.Text!, tokenInfo.StringValue!, tokenInfo.Position);
                case SyntaxKind.CharLiteralToken:
                    return new SyntaxTokenWithValue<char>(tokenInfo.Kind, tokenInfo.Text!, tokenInfo.CharValue, tokenInfo.Position);
                case SyntaxKind.IntLiteralToken:
                    return new SyntaxTokenWithValue<int>(tokenInfo.Kind, tokenInfo.Text!, tokenInfo.IntValue, tokenInfo.Position);
                case SyntaxKind.LongLiteralToken:
                    return new SyntaxTokenWithValue<long>(tokenInfo.Kind, tokenInfo.Text!, tokenInfo.LongValue, tokenInfo.Position);
    
                case SyntaxKind.FalseLiteralToken:
                case SyntaxKind.FalseKeyword:
                    return new SyntaxTokenWithValue<bool>(tokenInfo.Kind, tokenInfo.Text!, false, tokenInfo.Position);
                case SyntaxKind.TrueLiteralToken:
                case SyntaxKind.TrueKeyword:
                    return new SyntaxTokenWithValue<bool>(tokenInfo.Kind, tokenInfo.Text!, true, tokenInfo.Position);

                case SyntaxKind.NullLiteralToken:
                case SyntaxKind.NullKeyword:
                    return new SyntaxTokenWithValue<object?>(tokenInfo.Kind, tokenInfo.Text!, null, tokenInfo.Position);
                default:
                    throw new UnreachableException();
            }
        }

        public static SyntaxToken Token(in TokenInfo tokenInfo)
        {
            var token = new SyntaxToken(tokenInfo.Kind, tokenInfo.Text!, tokenInfo.Position);
            return token;
        }

        public static LiteralExpressionSyntax LiteralExpression(SyntaxNode parent, SyntaxToken tokenWithValue)
        {
            var literalExpression = new LiteralExpressionSyntax(parent, tokenWithValue);
            return literalExpression;
        }
    }
}
