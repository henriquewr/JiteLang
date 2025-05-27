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
using JiteLang.Main.LangParser.Types;
using JiteLang.Main.Shared;
using JiteLang.Syntax;
using JiteLang.Utilities;

namespace JiteLang.Main.LangParser
{
    internal class Parser
    {
        private readonly ControllableArray<TokenInfo> _tokens;
        private readonly HashSet<string> _errors;

        public Parser(List<TokenInfo> tokens)
        {
            _tokens = new ControllableArray<TokenInfo>(tokens, default);
            _errors = new();
        }

        private void AddErrorMessage(string errorMessage, in SyntaxPosition position)
        {
            var errorText = $"{errorMessage}   On {position.GetPosText()}";
            _errors.Add(errorText);
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

        private (TokenInfo Token, int Position) PeekWhile(Func<TokenInfo, bool> willPeek)
        {
            (TokenInfo Token, int Position) result;
            result.Position = 1;

            do
            {
                if(!_tokens.PeekNext(out result.Token, result.Position))
                {
                    throw new IndexOutOfRangeException();
                }

                result.Position++;
            } while (willPeek(result.Token));

            return result;
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
            var root = ParseNamespaceDeclaration();

            var parsedSyntaxTree = new ParsedSyntaxTree(_errors, root);

            root.SetParentRecursive();

            return parsedSyntaxTree;
        }

        private SyntaxNode ParseDefaultMembers(TokenInfo token)
        {
            switch (token.Kind)
            {
                case SyntaxKind.IdentifierToken:
                    return ParseAssignmentExprOrCallExprOrVarDeclaration();

                case SyntaxKind.ReturnKeyword:
                    return ParseReturn();
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement();

                case SyntaxKind.WhileKeyword:
                    return ParseWhileStatement();

                default:
                    if (SyntaxFacts.IsPredefinedType(token.Kind))
                    {
                        return ParseLocalDeclaration();
                    }
                    _tokens.Advance();
                    break;
            }

            throw new UnreachableException();
        }

        private BlockStatement<TResult> ParseBlockStatement<TResult>(Func<TokenInfo, TResult> parseMembersFunc) where TResult : SyntaxNode
        {
            Expect(SyntaxKind.OpenBraceToken, "Expected '{'");

            var blockMembers = new List<TResult>();

            while (_tokens.Current.Kind != SyntaxKind.CloseBraceToken)
            {
                var member = parseMembersFunc(_tokens.Current);
                blockMembers.Add(member);
            }

            Expect(SyntaxKind.CloseBraceToken, "Expected '}'");

            BlockStatement<TResult> blockStatement = new(blockMembers);

            return blockStatement;
        }

        private NamespaceDeclarationSyntax ParseNamespaceDeclaration()
        {
            _tokens.Advance();
            Expect(SyntaxKind.IdentifierToken, out var namespaceIdentifier, "Expected identifier name");

            var body = ParseBlockStatement<ClassDeclarationSyntax>(ParseNamespaceMembers);

            NamespaceDeclarationSyntax parsed = new(SyntaxFactory.Identifier(namespaceIdentifier), body);

            return parsed;

            ClassDeclarationSyntax ParseNamespaceMembers(TokenInfo token)
            {
                switch (token.Kind)
                {
                    case SyntaxKind.ClassKeyword:
                        return ParseClassDeclaration();
                    default:
                        throw new UnreachableException();
                }
            }
        }

        private ClassDeclarationSyntax ParseClassDeclaration()
        {
            _tokens.Advance();
            Expect(SyntaxKind.IdentifierToken, out var classIdentifier, "Expected identifier name");

            var classBody = ParseBlockStatement(ParseClassMembers);

            ClassDeclarationSyntax parsed = new(SyntaxFactory.Identifier(classIdentifier), classBody);

            return parsed;

            SyntaxNode ParseClassMembers(TokenInfo token)
            {
                switch (token.Kind)
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.PrivateKeyword:
                        return ParseMethodOrFieldDeclaration();
                    case SyntaxKind.ClassKeyword:
                        return ParseClassDeclaration();

                    default:
                        throw new NotImplementedException();
                        // syntax error probably
                        //if (SyntaxFacts.IsPredefinedType(token.Kind))
                        //{
                        //    return ParseFieldDeclaration();
                        //}
                        //_tokens.Advance();
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

        private SyntaxNode ParseMethodOrFieldDeclaration()
        {
            var modifiers = ParseModifiers();
            _tokens.PeekNext(out var token, 2);

            if (token.Kind == SyntaxKind.OpenParenToken)
            {
                var methodDeclaration = ParseMethodDeclaration(modifiers);
                return methodDeclaration;
            }
            else
            {
                var variable = ParseVariableDeclaration();

                var fieldDeclaration = new FieldDeclarationSyntax(modifiers, SyntaxFactory.Identifier(variable.IdentifierToken), SyntaxFactory.Type(variable.TypeToken), variable.InitialValue);

                return fieldDeclaration;
            }
        }

        private MethodDeclarationSyntax ParseMethodDeclaration(List<SyntaxToken> modifiers)
        {
            _tokens.Advance(out var returnType);
            Expect(SyntaxKind.IdentifierToken, out var methodIdentifier, "Expected identifier name");

            var returnTypeSyntax = SyntaxFactory.Type(returnType);
            var identifier = SyntaxFactory.Identifier(methodIdentifier);

            var methodParams = ParseMethodParams();

            var isExtern = modifiers.Any(x => x.Kind == SyntaxKind.ExternKeyword);

            BlockStatement<SyntaxNode> methodBody;

            if (isExtern) //Extern methods cannot have a body
            {
                methodBody = new(new());
                Expect(SyntaxKind.SemiColon, "Extern method declaration must end with semicolon");
            }
            else
            {
                methodBody = ParseBlockStatement<SyntaxNode>(ParseDefaultMembers);
            }

            MethodDeclarationSyntax methodDeclaration = new(identifier, returnTypeSyntax, methodBody, methodParams, modifiers);

            return methodDeclaration;
        }

        private List<TItem> ParseParenthesisExpr<TItem>(Func<TItem> parseItemFunc)
        {
            Expect(SyntaxKind.OpenParenToken, "Expected '('");

            var items = new List<TItem>();

            while (_tokens.Current.Kind != SyntaxKind.CloseParenToken)
            {
                var item = parseItemFunc();
                items.Add(item);
            }

            Expect(SyntaxKind.CloseParenToken, "Expected ')'");

            return items;
        }

        private List<ParameterDeclarationSyntax> ParseMethodParams()
        {
            var @params = ParseParenthesisExpr(ParseParam);

            return @params;

            ParameterDeclarationSyntax ParseParam()
            {
                _tokens.Advance(out var token);

                Expect(SyntaxKind.IdentifierToken, out var identifier, "Expected identifier name");

                var ideToken = SyntaxFactory.Identifier(identifier);
                var type = SyntaxFactory.Type(token);

                var paramDeclaration = new ParameterDeclarationSyntax(ideToken, type);

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

            var ifStmtElse = TryParseElseStatement();

            var ifStmt = new IfStatementSyntax(condition, ifBody, ifStmtElse);

            return ifStmt;

            ElseStatementSyntax? TryParseElseStatement()
            {
                if (_tokens.Current.Kind == SyntaxKind.ElseKeyword)
                {
                    _tokens.Advance();

                    return ParseElseStatement();
                }

                return null;
            }
        }

        private ElseStatementSyntax ParseElseStatement()
        {
            StatementSyntax @else;

            if (_tokens.Current.Kind == SyntaxKind.IfKeyword)
            {
                @else = ParseIfStatement();
            }
            else
            {
                @else = ParseBlockStatement<SyntaxNode>(ParseDefaultMembers);
            }

            var elseStmt = new ElseStatementSyntax(@else);

            return elseStmt;
        }

        private WhileStatementSyntax ParseWhileStatement()
        {
            _tokens.Advance();

            Expect(SyntaxKind.OpenParenToken, "Expected '('");
            var condition = ParseExpr();
            Expect(SyntaxKind.CloseParenToken, "Expected ')'");

            var body = ParseBlockStatement<SyntaxNode>(ParseDefaultMembers);

            var whileStmt = new WhileStatementSyntax(condition, body);

            return whileStmt;
        }

        private ReturnStatementSyntax ParseReturn()
        {
            _tokens.Advance(out var retToken);

            ExpressionSyntax? returnValue = null;

            if (_tokens.Current.Kind != SyntaxKind.SemiColon)
            {
                returnValue = ParseExpr();
            }

            Expect(SyntaxKind.SemiColon, "Expected semicolon");

            var returnStmp = new ReturnStatementSyntax(returnValue)
            {
                Position = retToken.Position
            };

            return returnStmp;
        }

        private LocalDeclarationSyntax ParseLocalDeclaration()
        {
            var variable = ParseVariableDeclaration();

            var localDeclaration = new LocalDeclarationSyntax(SyntaxFactory.Identifier(variable.IdentifierToken), SyntaxFactory.Type(variable.TypeToken), variable.InitialValue);

            return localDeclaration;
        }

        private (TokenInfo IdentifierToken, TokenInfo TypeToken, ExpressionSyntax? InitialValue) ParseVariableDeclaration()
        {
            _tokens.Advance(out var token);

            Expect(SyntaxKind.IdentifierToken, out var identifier, "Expected identifier name");

            (TokenInfo IdentifierToken, TokenInfo TypeToken, ExpressionSyntax? InitialValue) result = (identifier, token, null);

            if (_tokens.Current.Kind == SyntaxKind.SemiColon)
            {
                _tokens.Advance();
                //No value
                return result;
            }

            Expect(SyntaxKind.EqualsToken, out _, "Expected equals token following identifier in variable declaration");

            result.InitialValue = ParseExpr();

            Expect(SyntaxKind.SemiColon, "Expected semicolon");

            return result;
        }

        private SyntaxNode ParseAssignmentExprOrCallExprOrVarDeclaration()
        {
            _tokens.PeekNext(out var nextToken);

            switch (nextToken.Kind)
            {
                case SyntaxKind.OpenParenToken:
                    /*
                        Identifier => OpenParen
                        Method()
                       */

                    return ParseWithSemicolon(ParseCallExpression);

                case SyntaxKind.IdentifierToken:
                    /*
                        Identifier => Identifier
                        UserType variable ...
                       */

                    return ParseLocalDeclaration();

                case SyntaxKind.DotToken:
                    /*
                        Identifier => Dot
                        variable.Something
                       */

                    var (lastToken, position) = PeekWhile(token => token.Kind == SyntaxKind.DotToken || token.Kind == SyntaxKind.IdentifierToken);

                    if (lastToken.Kind == SyntaxKind.OpenParenToken)
                    {
                        return ParseWithSemicolon(ParseCallExpression);
                    }
                    else if (SyntaxFacts.IsAssignmentOperator(lastToken.Kind))
                    {
                        return ParseWithSemicolon(ParseAssignmentExpr);
                    }

                    throw new UnreachableException();
                default:
                    if (!SyntaxFacts.IsAssignmentOperator(nextToken.Kind))
                    {
                        throw new UnreachableException();
                    }

                    /*
                        Identifier => Equals
                        variable = ...
                      */

                    return ParseWithSemicolon(ParseAssignmentExpr);
            }

            SyntaxNode ParseWithSemicolon(Func<SyntaxNode> func)
            {
                var result = func();
                Expect(SyntaxKind.SemiColon, "Expected semicolon");
                return result;
            }
        }

        #region Expressions
        private ExpressionSyntax ParseExpr() 
        {
            var parsed = ParseLogicalOrExpr();
            return parsed;
        }

        private ExpressionSyntax ParseAssignmentExpr()
        {
            var left = ParsePrimaryExpr();

            Expect(currentToken => {
                return SyntaxFacts.IsAssignmentOperator(currentToken.Kind);
            }, out var operationToken, "Expected assignment operator");

            var value = ParseExpr();

            var assignment = new AssignmentExpressionSyntax(left, operationToken.Kind, value);

            return assignment;
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
                    var type = SyntaxFactory.Type(token);
                    _tokens.Advance();
                    Expect(SyntaxKind.CloseParenToken, "Expected ')'");

                    var castExprValue = ParseUnaryOrCastExpr();
                    var castExpr = new CastExpressionSyntax(castExprValue, type);

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

        private ExpressionSyntax ParseMemberExpression(ExpressionSyntax leftExpr)
        {
            while(_tokens.Current.Kind == SyntaxKind.DotToken)
            {
                _tokens.Advance();
                Expect(SyntaxKind.IdentifierToken, out var identifierToken, "Expected identifier");

                var rightExpr = SyntaxFactory.Identifier(identifierToken);

                leftExpr = new MemberExpressionSyntax(leftExpr, rightExpr);
            }

            return leftExpr;
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

        private NewExpressionSyntax ParseNewExpr()
        {
            _tokens.Advance(out var typeToken);

            var type = SyntaxFactory.Type(typeToken);

            Expect(SyntaxKind.OpenParenToken, "Expected '('");

            var args = ParseArgs();
            var newExpr = new NewExpressionSyntax(type, args);
            newExpr.Position = typeToken.Position;

            Expect(SyntaxKind.CloseParenToken, "Expected ')'");

            return newExpr;
            
            List<ExpressionSyntax> ParseArgs()
            {
                List<ExpressionSyntax> args = new();

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

        private ExpressionSyntax ParsePrimaryExpr()
        {
            _tokens.Advance(out var currentToken);

            switch (currentToken.Kind)
            {
                case SyntaxKind.IdentifierToken:
                {
                    var identifier = SyntaxFactory.Identifier(currentToken);

                    if (_tokens.Current.Kind == SyntaxKind.DotToken)
                    {
                        var memberExpr = ParseMemberExpression(identifier);
                        return memberExpr;
                    }

                    return identifier;
                }

                case SyntaxKind.NewKeyword:
                    return ParseNewExpr();

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

        #endregion Expressions
    }
}
