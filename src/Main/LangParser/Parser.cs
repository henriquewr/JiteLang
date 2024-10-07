using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using JiteLang.Main.LangLexer.Token;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Main.Shared;
using JiteLang.Syntax;
using JiteLang.Utilities;

namespace JiteLang.Main.LangParser
{
    internal class Parser
    {
        private readonly ControllableArray<TokenInfo> _tokens;

        private readonly ParsedSyntaxTree _parsedSyntaxTree;

        public Parser(List<TokenInfo> tokens)
        {
            _tokens = new ControllableArray<TokenInfo>(tokens, default);

            _parsedSyntaxTree = new ParsedSyntaxTree();
        }

        private void AddErrorMessage(string errorMessage, in SyntaxPosition position)
        {
            var errorText = $"{errorMessage}   On {position.GetPosText()}";
            _parsedSyntaxTree.Errors.Add(errorText);
        }

        private bool Expect(SyntaxKind tokenKind, out TokenInfo previousToken, string errorMessage)
        {
            previousToken = _tokens.Current;
            _tokens.Advance();
            
            if (previousToken.Kind != tokenKind)
            {
                AddErrorMessage(errorMessage, previousToken.Position);
                return false;
            }

            return true;
        }

        private bool Expect(Func<TokenInfo, bool> isValidFunc, out TokenInfo previousToken, string errorMessage)
        {
            previousToken = _tokens.Current;
            _tokens.Advance();

            var isValid = isValidFunc(previousToken);

            if (!isValid)
            {
                AddErrorMessage(errorMessage, previousToken.Position);
            }

            return isValid;
        }

        private bool Expect(SyntaxKind tokenKind, string errorMessage)
        {
            return Expect(tokenKind, out _, errorMessage);
        }

        private bool AdvanceIf(Func<TokenInfo, bool> advanceFunc, out TokenInfo token)
        {
            _tokens.PeekNext(out token);

            var advance = advanceFunc(token);

            if (advance)
            {
                _tokens.Advance();
                return true;
            }

            return false;
        }

        private bool AdvanceIfIs(SyntaxKind tokenKind, out TokenInfo token)
        {
            _tokens.PeekNext(out token);
            if (token.Kind == tokenKind)
            {
                _tokens.Advance();
                return true;
            }

            return false;
        }

        private bool AdvanceIfIs(SyntaxKind tokenKind)
        {
            return AdvanceIfIs(tokenKind, out _);
        }

        public ParsedSyntaxTree Parse()
        {
            _parsedSyntaxTree.Root = ParseNamespaceDeclaration();
            return _parsedSyntaxTree;
        }

        private SyntaxNode ParseDefaultMembers(TokenInfo token)
        {
            switch (token.Kind)
            {
                case SyntaxKind.IdentifierToken:
                    var expr = ParseAssignmentExprOrCallExpr();
                    Expect(SyntaxKind.SemiColon, "Expected semicolon");
                    return expr;

                case SyntaxKind.ReturnKeyword:
                    return ParseReturn();
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement();

                case SyntaxKind.WhileKeyword:
                    return ParseWhileStatement();

                default:
                    if (SyntaxFacts.IsPredefinedType(token.Kind))
                    {
                        return ParsePredefinedTypeVarDeclaration();
                    }
                    _tokens.Advance();
                    break;
            }

            throw new UnreachableException();
        }

        private BlockStatement<TResult> ParseBlockStatement<TResult>(Func<TokenInfo, TResult> parseMembersFunc) where TResult : SyntaxNode
        {
            BlockStatement<TResult> blockStatement = new();
            Expect(SyntaxKind.OpenBraceToken, "Expected '{'");

            while (_tokens.Current.Kind != SyntaxKind.CloseBraceToken)
            {
                var member = parseMembersFunc(_tokens.Current);
                blockStatement.Members.Add(member);
            }

            Expect(SyntaxKind.CloseBraceToken, "Expected '}'");

            return blockStatement;
        }

        private NamespaceDeclarationSyntax ParseNamespaceDeclaration()
        {
            _tokens.Advance();
            Expect(SyntaxKind.IdentifierToken, out var namespaceIdentifier, "Expected identifier name");

            var identifier = SyntaxFactory.Identifier(namespaceIdentifier);
            identifier.Position = namespaceIdentifier.Position;

            NamespaceDeclarationSyntax parsed = new(identifier);
            parsed.Body = ParseBlockStatement<ClassDeclarationSyntax>(ParseNamespaceMembers);

            return parsed;


            ClassDeclarationSyntax ParseNamespaceMembers(TokenInfo token)
            {
                switch (token.Kind)
                {
                    case SyntaxKind.ClassKeyword:
                        return ParseClassDeclaration();
                }

                throw new UnreachableException();
            }
        }

        private ClassDeclarationSyntax ParseClassDeclaration()
        {
            _tokens.Advance();
            Expect(SyntaxKind.IdentifierToken, out var classIdentifier, "Expected identifier name");
            var identifier = SyntaxFactory.Identifier(classIdentifier);
            ClassDeclarationSyntax parsed = new(identifier);

            parsed.Body = ParseBlockStatement(ParseClassMembers);
            return parsed;

            SyntaxNode ParseClassMembers(TokenInfo token)
            {
                switch (token.Kind)
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.PrivateKeyword:
                        return ParseMethodOrVariableDeclaration();
                    case SyntaxKind.ClassKeyword:
                        return ParseClassDeclaration();

                    default:
                        if (SyntaxFacts.IsPredefinedType(token.Kind))
                        {
                            return ParsePredefinedTypeVarDeclaration();
                        }
                        _tokens.Advance();
                        break;
                }

                throw new UnreachableException();
            }
        }

        private List<SyntaxToken> ParseModifiers()
        {
            var modifiers = new List<SyntaxToken>();
            Expect(token => SyntaxFacts.IsAccessModifier(token.Kind), out var token, "Expected acess modifier");
            modifiers.Add(SyntaxFactory.Token(token));

            while (SyntaxFacts.IsModifier(_tokens.Current.Kind))
            {
                modifiers.Add(SyntaxFactory.Token(_tokens.Current));
                _tokens.Advance();
            }

            return modifiers;
        }

        private SyntaxNode ParseMethodOrVariableDeclaration()
        {
            var modifiers = ParseModifiers();
            _tokens.PeekNext(out var token, 2);


            if(token.Kind == SyntaxKind.OpenParenToken)
            {
                var methodDeclaration = ParseMethodDeclaration(modifiers);
                return methodDeclaration;
            }
            else
            {
                var varDeclaration = ParsePredefinedTypeVarDeclaration();
                varDeclaration.Modifiers.AddRange(modifiers);
                return varDeclaration;
            }
        }

        private MethodDeclarationSyntax ParseMethodDeclaration(List<SyntaxToken> modifiers)
        {
            _tokens.Advance(out var returnType);
            Expect(SyntaxKind.IdentifierToken, out var methodIdentifier, "Expected identifier name");

            var returnTypeSyntax = SyntaxFactory.PredefinedType(returnType);
            var identifier = SyntaxFactory.Identifier(methodIdentifier);
            MethodDeclarationSyntax methodDeclaration = new(identifier, returnTypeSyntax, modifiers);

            methodDeclaration.Params = ParseMethodParams();

            var isExtern = methodDeclaration.Modifiers.Any(x => x.Kind == SyntaxKind.ExternKeyword);

            if (isExtern) //Extern methods cannot have a body
            {
                Expect(SyntaxKind.SemiColon, "Extern method declaration must end with semicolon");
            }
            else
            {
                methodDeclaration.Body = ParseBlockStatement<SyntaxNode>(ParseDefaultMembers);
            }

            return methodDeclaration;
        }

        private List<ParameterDeclarationSyntax> ParseMethodParams()
        {
            Expect(SyntaxKind.OpenParenToken, "Expected '('");

            var param = new List<ParameterDeclarationSyntax>();
            
            while (_tokens.Current.Kind != SyntaxKind.CloseParenToken)
            {
                var item = ParseParam();
                param.Add(item);
            }

            Expect(SyntaxKind.CloseParenToken, "Expected ')'");

            return param;

            ParameterDeclarationSyntax ParseParam()
            {
                _tokens.Advance(out var token);

                Expect(SyntaxKind.IdentifierToken, out var identifier, "Expected identifier name");

                var ideToken = SyntaxFactory.Identifier(identifier);
                var predefinedType = SyntaxFactory.PredefinedType(token);

                var paramDeclaration = new ParameterDeclarationSyntax(ideToken, predefinedType);

                if (_tokens.Current.Kind == SyntaxKind.CommaToken)
                {
                    _tokens.Advance();
                }

                return paramDeclaration;
            }
        }

        private IfStatementSyntax ParseIfStatement() 
        {
            _tokens.Advance();

            Expect(SyntaxKind.OpenParenToken, "Expected '('");
            var condition = ParseExpr();
            Expect(SyntaxKind.CloseParenToken, "Expected ')'");

            var ifBody = ParseBlockStatement<SyntaxNode>(ParseDefaultMembers);

            StatementSyntax? elseBody = TryParseElseStatement();

            var ifStmt = new IfStatementSyntax(condition, ifBody, elseBody);

            return ifStmt;

            StatementSyntax? TryParseElseStatement()
            {
                if (_tokens.Current.Kind == SyntaxKind.ElseKeyword)
                {
                    _tokens.Advance();

                    return ParseElseStatement();
                }

                return null;
            }
        }

        private StatementSyntax ParseElseStatement()
        {
            if (_tokens.Current.Kind == SyntaxKind.IfKeyword)
            {
                return ParseIfStatement();
            }

            var elseBody = ParseBlockStatement<SyntaxNode>(ParseDefaultMembers);

            return elseBody;
        }

        private WhileStatementSyntax ParseWhileStatement()
        {
            _tokens.Advance();

            Expect(SyntaxKind.OpenParenToken, "Expected '('");
            var condition = ParseExpr();
            Expect(SyntaxKind.CloseParenToken, "Expected ')'");

            var whileBody = ParseBlockStatement<SyntaxNode>(ParseDefaultMembers);

            var whileStmt = new WhileStatementSyntax(condition, whileBody);
            return whileStmt;
        }

        private ReturnStatementSyntax ParseReturn()
        {
            _tokens.Advance(out var retToken);

            var returnStmp = new ReturnStatementSyntax
            {
                Position = retToken.Position
            };

            if (_tokens.Current.Kind != SyntaxKind.SemiColon)
            {
                returnStmp.ReturnValue = ParseExpr();
            }

            Expect(SyntaxKind.SemiColon, "Expected semicolon");

            return returnStmp;
        }

        private VariableDeclarationSyntax ParsePredefinedTypeVarDeclaration()
        {
            _tokens.Advance(out var token);

            Expect(SyntaxKind.IdentifierToken, out var identifier, "Expected identifier name");

            var ideToken = SyntaxFactory.Identifier(identifier);
            var varDeclaration = SyntaxFactory.DeclareFromPredefined(token, ideToken);

            if (_tokens.Current.Kind == SyntaxKind.SemiColon) 
            {
                _tokens.Advance();
                //No value
                return varDeclaration;
            }

            Expect(SyntaxKind.EqualsToken, out _, "Expected equals token following identifier in variable declaration");

            varDeclaration.InitialValue = ParseExpr();

            Expect(SyntaxKind.SemiColon, "Expected semicolon");

            return varDeclaration;
        }

        private ExpressionSyntax ParseAssignmentExprOrCallExpr()
        {
            _tokens.PeekNext(out var nextToken);

            switch (nextToken.Kind)
            {
                case SyntaxKind.OpenParenToken:
                    return ParseCallExpression();

                default:
                    return ParseAssignmentExpr();
            }
        }

        private ExpressionSyntax ParseAssignmentExpr()
        {
            _tokens.Advance(out var identifierToken);
            var left = SyntaxFactory.Identifier(identifierToken);

            Expect(currentToken =>  { 
                var kind = currentToken.Kind;

                switch (kind)
                {
                    case SyntaxKind.EqualsToken:
                    case SyntaxKind.PlusEqualsToken:
                    case SyntaxKind.MinusEqualsToken:
                    case SyntaxKind.AsteriskEqualsToken:
                    case SyntaxKind.SlashEqualsToken:
                    case SyntaxKind.PercentEqualsToken:
                        return true;

                    default:
                        return false;
                }

            }, out var operationToken, "Expected assignment operator");

            var value = ParseExpr();

            var assignment = new AssignmentExpressionSyntax(left, operationToken.Kind, value);

            return assignment;
        }

        private ExpressionSyntax ParseExpr() 
        {
            var parsed = ParseLogicalOrExpr();
            return parsed;
        }

        private ExpressionSyntax ParseLogicalOrExpr()
        {
            return ParseLogicalLeftExpr(currentKind =>
                currentKind == SyntaxKind.BarBarToken,
            ParseLogicalAndExpr);
        }

        private ExpressionSyntax ParseLogicalAndExpr()
        {
            return ParseLogicalLeftExpr(currentKind =>
                currentKind == SyntaxKind.AmpersandAmpersandToken,
            ParseBitwiseOrExpr);
        }

        private ExpressionSyntax ParseBitwiseOrExpr()
        {
            return ParseBitwiseXorExpr();
        }

        private ExpressionSyntax ParseBitwiseXorExpr()
        {
            return ParseBitwiseAndExpr();
        }

        private ExpressionSyntax ParseBitwiseAndExpr()
        {
            return ParseEqualityExpr();
        }

        private ExpressionSyntax ParseEqualityExpr()
        {
            var result = ParseLogicalLeftExpr(currentKind =>
                currentKind == SyntaxKind.EqualsEqualsToken || 
                currentKind == SyntaxKind.NotEqualsToken,
            ParseRelationalExpr);
            return result;
        }

        private ExpressionSyntax ParseRelationalExpr()
        {
            return ParseLogicalLeftExpr(currentKind => 
                currentKind == SyntaxKind.GreaterThanToken || 
                currentKind == SyntaxKind.GreaterThanEqualsToken || 
                currentKind == SyntaxKind.LessThanToken || 
                currentKind == SyntaxKind.LessThanEqualsToken, 
            ParseBitwiseShiftExpr);
        }

        private ExpressionSyntax ParseBitwiseShiftExpr()
        {
            return ParseAdditiveExpr();
        }

        private ExpressionSyntax ParseAdditiveExpr()
        {
            return ParseBinaryLeftExpr(currentKind => 
                currentKind == SyntaxKind.PlusToken || 
                currentKind == SyntaxKind.MinusToken, 
            ParseMultiplicativeExpr);
        }

        private ExpressionSyntax ParseMultiplicativeExpr()
        {
            return ParseBinaryLeftExpr(currentKind =>
                currentKind == SyntaxKind.AsteriskToken ||
                currentKind == SyntaxKind.SlashToken ||
                currentKind == SyntaxKind.PercentToken,
            ParseUnaryOrCastExpr);
        }

        private ExpressionSyntax ParseUnaryOrCastExpr()
        {
            if (_tokens.Current.Kind == SyntaxKind.OpenParenToken) 
            {
                _tokens.PeekNext(out var token);

                if (SyntaxFacts.IsPredefinedType(token.Kind))
                {
                    _tokens.Advance();
                    var predefinedType = SyntaxFactory.PredefinedType(token);
                    _tokens.Advance();
                    Expect(SyntaxKind.CloseParenToken, "Expected ')'");

                    var valueExpr = ParseUnaryOrCastExpr();
                    
                    var castExpr = new CastExpressionSyntax(valueExpr, predefinedType);
                    return castExpr;
                }
            }

            var expr = ParseCallExpression();
            return expr;
        }

        private ExpressionSyntax ParseCallExpression()
        {
            var possibleCaller = ParsePrimaryExpr();
            if (_tokens.Current.Kind == SyntaxKind.OpenParenToken)
            {
                _tokens.Advance(out var openParenToken);

                var args = ParseArgs();
                var callExpr = new CallExpressionSyntax(possibleCaller, args);

                Expect(SyntaxKind.CloseParenToken, "Expected ')'");

                return callExpr;
            }

            //isnt a caller
            return possibleCaller;

            List<ExpressionSyntax> ParseArgs()
            {
                var args = new List<ExpressionSyntax>();

                if (_tokens.Current.Kind == SyntaxKind.CloseParenToken)
                {
                    //no args
                    return args;
                }

                args.Add(ParseExpr());

                while (_tokens.Current.Kind == SyntaxKind.CommaToken)
                {
                    _tokens.Advance();
                    var parsed = ParseExpr();
                    args.Add(parsed);
                }

                return args;
            }
        }

        private ExpressionSyntax ParseLogicalLeftExpr(Func<SyntaxKind, bool> willAdvance, Func<ExpressionSyntax> getData, [CallerMemberName] string callerForDebugView = "doNotUse")
        {
            var left = getData();

            while (willAdvance(_tokens.Current.Kind))
            {
                _tokens.Advance(out var operationToken);
                var right = getData();
                left = new LogicalExpressionSyntax(left, Convert(operationToken.Kind), right);
            }

            return left;

            static LogicalOperatorKind Convert(SyntaxKind syntaxKind)
            {
                var operation = syntaxKind switch
                {
                    SyntaxKind.AmpersandAmpersandToken => LogicalOperatorKind.AndAnd,
                    SyntaxKind.BarBarToken => LogicalOperatorKind.OrOr,

                    SyntaxKind.EqualsEqualsToken => LogicalOperatorKind.EqualsEquals,
                    SyntaxKind.NotEqualsToken => LogicalOperatorKind.NotEquals,

                    SyntaxKind.GreaterThanToken => LogicalOperatorKind.GreaterThan,
                    SyntaxKind.GreaterThanEqualsToken => LogicalOperatorKind.GreaterThanOrEquals,

                    SyntaxKind.LessThanToken => LogicalOperatorKind.LessThan,
                    SyntaxKind.LessThanEqualsToken => LogicalOperatorKind.LessThanOrEquals,
                    _ => throw new UnreachableException()
                };
                return operation;
            }
        }

        private ExpressionSyntax ParseBinaryLeftExpr(Func<SyntaxKind, bool> willAdvance, Func<ExpressionSyntax> getData, [CallerMemberName] string callerForDebugView = "doNotUse")
        {
            var left = getData();

            while (willAdvance(_tokens.Current.Kind))
            {
                _tokens.Advance(out var operationToken);
                var right = getData();
                left = new BinaryExpressionSyntax(left, Convert(operationToken.Kind), right);
            }

            return left;

            static BinaryOperatorKind Convert(SyntaxKind syntaxKind)
            {
                var operation = syntaxKind switch
                {
                    SyntaxKind.AsteriskToken => BinaryOperatorKind.Multiply,
                    SyntaxKind.MinusToken => BinaryOperatorKind.Minus,
                    SyntaxKind.PercentToken => BinaryOperatorKind.Modulus,
                    SyntaxKind.PlusToken => BinaryOperatorKind.Plus,
                    SyntaxKind.SlashToken => BinaryOperatorKind.Divide,
                    _ => throw new UnreachableException()
                };
                return operation;
            }
        }

        private ExpressionSyntax ParsePrimaryExpr()
        {
            _tokens.Advance(out var currentToken);

            switch (currentToken.Kind)
            {
                case SyntaxKind.IdentifierToken:
                    return SyntaxFactory.Identifier(currentToken);

                case SyntaxKind.OpenParenToken:
                    var value = ParseExpr();
                    Expect(SyntaxKind.CloseParenToken, "Expected ')'");
                    return value;

                default:
                    var token = SyntaxFactory.TokenWithValue(currentToken);
                    var literalExpr = SyntaxFactory.LiteralExpression(token);
                    return literalExpr;
            }
        }
    }
}
