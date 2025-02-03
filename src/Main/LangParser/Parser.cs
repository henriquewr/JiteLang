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
            var root = ParseNamespaceDeclaration(null);

            var parsedSyntaxTree = new ParsedSyntaxTree(_errors, root);

            return parsedSyntaxTree;
        }

        private SyntaxNode ParseDefaultMembers(SyntaxNode parent, TokenInfo token)
        {
            switch (token.Kind)
            {
                case SyntaxKind.IdentifierToken:
                    return ParseAssignmentExprOrCallExprOrVarDeclaration(parent);

                case SyntaxKind.ReturnKeyword:
                    return ParseReturn(parent);
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement(parent);

                case SyntaxKind.WhileKeyword:
                    return ParseWhileStatement(parent);

                default:
                    if (SyntaxFacts.IsPredefinedType(token.Kind))
                    {
                        return ParseVarDeclaration(parent);
                    }
                    _tokens.Advance();
                    break;
            }

            throw new UnreachableException();
        }

        private BlockStatement<TResult> ParseBlockStatement<TResult>(SyntaxNode bodyParent, Func<SyntaxNode, TokenInfo, TResult> parseMembersFunc) where TResult : SyntaxNode
        {
            BlockStatement<TResult> blockStatement = new(bodyParent);
            Expect(SyntaxKind.OpenBraceToken, "Expected '{'");

            while (_tokens.Current.Kind != SyntaxKind.CloseBraceToken)
            {
                var member = parseMembersFunc(blockStatement, _tokens.Current);
                blockStatement.Members.Add(member);
            }

            Expect(SyntaxKind.CloseBraceToken, "Expected '}'");

            return blockStatement;
        }

        private NamespaceDeclarationSyntax ParseNamespaceDeclaration(SyntaxNode? parent)
        {
            _tokens.Advance();
            Expect(SyntaxKind.IdentifierToken, out var namespaceIdentifier, "Expected identifier name");

            var identifier = SyntaxFactory.Identifier(null!, namespaceIdentifier);
            identifier.Position = namespaceIdentifier.Position;

            NamespaceDeclarationSyntax parsed = new(parent!, identifier);
            identifier.Parent = parsed;
            parsed.Body = ParseBlockStatement<ClassDeclarationSyntax>(parsed, ParseNamespaceMembers);

            return parsed;


            ClassDeclarationSyntax ParseNamespaceMembers(SyntaxNode parent, TokenInfo token)
            {
                switch (token.Kind)
                {
                    case SyntaxKind.ClassKeyword:
                        return ParseClassDeclaration(parent);
                }

                throw new UnreachableException();
            }
        }

        private ClassDeclarationSyntax ParseClassDeclaration(SyntaxNode parent)
        {
            _tokens.Advance();
            Expect(SyntaxKind.IdentifierToken, out var classIdentifier, "Expected identifier name");

            var identifier = SyntaxFactory.Identifier(null!, classIdentifier);
            ClassDeclarationSyntax parsed = new(parent, identifier);
            identifier.Parent = parsed;

            parsed.Body = ParseBlockStatement(parsed, ParseClassMembers);
            return parsed;

            SyntaxNode ParseClassMembers(SyntaxNode thisParent, TokenInfo token)
            {
                switch (token.Kind)
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.PrivateKeyword:
                        return ParseMethodOrFieldDeclaration(thisParent);
                    case SyntaxKind.ClassKeyword:
                        return ParseClassDeclaration(thisParent);

                    default:
                        if (SyntaxFacts.IsPredefinedType(token.Kind))
                        {
                            return ParseVarDeclaration(thisParent);
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

        private SyntaxNode ParseMethodOrFieldDeclaration(SyntaxNode parent)
        {
            var modifiers = ParseModifiers();
            _tokens.PeekNext(out var token, 2);


            if(token.Kind == SyntaxKind.OpenParenToken)
            {
                var methodDeclaration = ParseMethodDeclaration(parent, modifiers);
                return methodDeclaration;
            }
            else
            {
                var varDeclaration = ParseVarDeclaration(null!);
                FieldDeclarationSyntax fieldDeclaration = new(parent, varDeclaration, modifiers);
                varDeclaration.Parent = fieldDeclaration;

                return fieldDeclaration;
            }
        }

        private MethodDeclarationSyntax ParseMethodDeclaration(SyntaxNode parent, List<SyntaxToken> modifiers)
        {
            _tokens.Advance(out var returnType);
            Expect(SyntaxKind.IdentifierToken, out var methodIdentifier, "Expected identifier name");

            var returnTypeSyntax = SyntaxFactory.Type(returnType);
            var identifier = SyntaxFactory.Identifier(null!, methodIdentifier);
            MethodDeclarationSyntax methodDeclaration = new(parent, identifier, returnTypeSyntax, modifiers);
            identifier.Parent = methodDeclaration;

            methodDeclaration.Params = ParseMethodParams(methodDeclaration);

            var isExtern = methodDeclaration.Modifiers.Any(x => x.Kind == SyntaxKind.ExternKeyword);

            if (isExtern) //Extern methods cannot have a body
            {
                Expect(SyntaxKind.SemiColon, "Extern method declaration must end with semicolon");
            }
            else
            {
                methodDeclaration.Body = ParseBlockStatement<SyntaxNode>(methodDeclaration, ParseDefaultMembers);
            }

            return methodDeclaration;
        }

        private List<TItem> ParseParenthesisExpr<TItem>(SyntaxNode parent, Func<SyntaxNode, TItem> parseItemFunc)
        {
            Expect(SyntaxKind.OpenParenToken, "Expected '('");

            var items = new List<TItem>();

            while (_tokens.Current.Kind != SyntaxKind.CloseParenToken)
            {
                var item = parseItemFunc(parent);
                items.Add(item);
            }

            Expect(SyntaxKind.CloseParenToken, "Expected ')'");

            return items;
        }

        private List<ParameterDeclarationSyntax> ParseMethodParams(SyntaxNode parent)
        {
            var @params = ParseParenthesisExpr(parent, ParseParam);

            return @params;

            ParameterDeclarationSyntax ParseParam(SyntaxNode thisParent)
            {
                _tokens.Advance(out var token);

                Expect(SyntaxKind.IdentifierToken, out var identifier, "Expected identifier name");

                var ideToken = SyntaxFactory.Identifier(null!, identifier);
                var type = SyntaxFactory.Type(token);

                var paramDeclaration = new ParameterDeclarationSyntax(thisParent, ideToken, type);
                ideToken.Parent = paramDeclaration;

                if (_tokens.Current.Kind == SyntaxKind.CommaToken)
                {
                    _tokens.Advance();
                }

                return paramDeclaration;
            }
        }

        private IfStatementSyntax ParseIfStatement(SyntaxNode parent) 
        {
            _tokens.Advance();

            Expect(SyntaxKind.OpenParenToken, "Expected '('");
            var condition = ParseExpr(null!);
            Expect(SyntaxKind.CloseParenToken, "Expected ')'");

            var ifStmt = new IfStatementSyntax(parent, condition);
            ifStmt.Body = ParseBlockStatement<SyntaxNode>(ifStmt, ParseDefaultMembers);
            condition.Parent = ifStmt;

            ifStmt.Else = TryParseElseStatement(parent);

            return ifStmt;

            StatementSyntax? TryParseElseStatement(SyntaxNode parent)
            {
                if (_tokens.Current.Kind == SyntaxKind.ElseKeyword)
                {
                    _tokens.Advance();

                    return ParseElseStatement(parent);
                }

                return null;
            }
        }

        private StatementSyntax ParseElseStatement(SyntaxNode parent)
        {
            if (_tokens.Current.Kind == SyntaxKind.IfKeyword)
            {
                return ParseIfStatement(parent);
            }

            var elseBody = ParseBlockStatement<SyntaxNode>(parent, ParseDefaultMembers);

            return elseBody;
        }

        private WhileStatementSyntax ParseWhileStatement(SyntaxNode parent)
        {
            _tokens.Advance();

            Expect(SyntaxKind.OpenParenToken, "Expected '('");
            var condition = ParseExpr(null!);
            Expect(SyntaxKind.CloseParenToken, "Expected ')'");

            var whileStmt = new WhileStatementSyntax(parent, condition);
            condition.Parent = whileStmt;

            whileStmt.Body = ParseBlockStatement<SyntaxNode>(whileStmt, ParseDefaultMembers);

            return whileStmt;
        }

        private ReturnStatementSyntax ParseReturn(SyntaxNode parent)
        {
            _tokens.Advance(out var retToken);

            var returnStmp = new ReturnStatementSyntax(parent)
            {
                Position = retToken.Position
            };

            if (_tokens.Current.Kind != SyntaxKind.SemiColon)
            {
                returnStmp.ReturnValue = ParseExpr(returnStmp);
            }

            Expect(SyntaxKind.SemiColon, "Expected semicolon");

            return returnStmp;
        }

        private VariableDeclarationSyntax ParseVarDeclaration(SyntaxNode parent)
        {
            _tokens.Advance(out var token);

            Expect(SyntaxKind.IdentifierToken, out var identifier, "Expected identifier name");

            var ideToken = SyntaxFactory.Identifier(null!, identifier);
            var varDeclaration = SyntaxFactory.DeclareVariable(token, parent, ideToken);
            if (_tokens.Current.Kind == SyntaxKind.SemiColon) 
            {
                _tokens.Advance();
                //No value
                return varDeclaration;
            }

            Expect(SyntaxKind.EqualsToken, out _, "Expected equals token following identifier in variable declaration");

            varDeclaration.InitialValue = ParseExpr(varDeclaration);

            Expect(SyntaxKind.SemiColon, "Expected semicolon");

            return varDeclaration;
        }

        private SyntaxNode ParseAssignmentExprOrCallExprOrVarDeclaration(SyntaxNode parent)
        {
            _tokens.PeekNext(out var nextToken);

            switch (nextToken.Kind)
            {
                case SyntaxKind.OpenParenToken:
                    /*
                        Identifier => OpenParen
                        Method()
                       */

                    return ParseWithSemicolon(() => ParseCallExpression(parent));

                case SyntaxKind.IdentifierToken:
                    /*
                        Identifier => Identifier
                        UserType variable ...
                       */

                    return ParseVarDeclaration(parent);

                case SyntaxKind.DotToken:
                    /*
                        Identifier => Dot
                        variable.Something
                       */

                    var (lastToken, position) = PeekWhile(token => token.Kind == SyntaxKind.DotToken || token.Kind == SyntaxKind.IdentifierToken);

                    if (lastToken.Kind == SyntaxKind.OpenParenToken)
                    {
                        return ParseWithSemicolon(() => ParseCallExpression(parent));
                    }
                    else if (SyntaxFacts.IsAssignmentOperator(lastToken.Kind))
                    {
                        return ParseWithSemicolon(() => ParseAssignmentExpr(parent));
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

                    return ParseWithSemicolon(() => ParseAssignmentExpr(parent));
            }

            SyntaxNode ParseWithSemicolon(Func<SyntaxNode> func)
            {
                var result = func();
                Expect(SyntaxKind.SemiColon, "Expected semicolon");
                return result;
            }
        }

        #region Expressions
        private ExpressionSyntax ParseExpr(SyntaxNode parent) 
        {
            var parsed = ParseLogicalOrExpr(parent);
            return parsed;
        }

        private ExpressionSyntax ParseAssignmentExpr(SyntaxNode parent)
        {
            var assignment = new AssignmentExpressionSyntax(parent, null!, default, null!);

            var left = ParsePrimaryExpr(assignment);

            Expect(currentToken => {
                return SyntaxFacts.IsAssignmentOperator(currentToken.Kind);
            }, out var operationToken, "Expected assignment operator");

            var value = ParseExpr(assignment);

            assignment.Left = left;
            assignment.Operator = operationToken.Kind;
            assignment.Right = value;

            return assignment;
        }

        private ExpressionSyntax ParseLogicalOrExpr(SyntaxNode parent)
        {
            return ParseLogicalLeftExpr(currentKind =>
                currentKind == SyntaxKind.BarBarToken,
            parent,
            ParseLogicalAndExpr);
        }

        private ExpressionSyntax ParseLogicalAndExpr(SyntaxNode parent)
        {
            return ParseLogicalLeftExpr(currentKind =>
                currentKind == SyntaxKind.AmpersandAmpersandToken,
            parent,
            ParseBitwiseOrExpr);
        }

        private ExpressionSyntax ParseBitwiseOrExpr(SyntaxNode parent)
        {
            return ParseBitwiseXorExpr(parent);
        }

        private ExpressionSyntax ParseBitwiseXorExpr(SyntaxNode parent)
        {
            return ParseBitwiseAndExpr(parent);
        }

        private ExpressionSyntax ParseBitwiseAndExpr(SyntaxNode parent)
        {
            return ParseEqualityExpr(parent);
        }

        private ExpressionSyntax ParseEqualityExpr(SyntaxNode parent)
        {
            var result = ParseLogicalLeftExpr(currentKind =>
                currentKind == SyntaxKind.EqualsEqualsToken || 
                currentKind == SyntaxKind.NotEqualsToken,
            parent,
            ParseRelationalExpr);
            return result;
        }

        private ExpressionSyntax ParseRelationalExpr(SyntaxNode parent)
        {
            return ParseLogicalLeftExpr(currentKind => 
                currentKind == SyntaxKind.GreaterThanToken || 
                currentKind == SyntaxKind.GreaterThanEqualsToken || 
                currentKind == SyntaxKind.LessThanToken || 
                currentKind == SyntaxKind.LessThanEqualsToken,
            parent,
            ParseBitwiseShiftExpr);
        }

        private ExpressionSyntax ParseBitwiseShiftExpr(SyntaxNode parent)
        {
            return ParseAdditiveExpr(parent);
        }

        private ExpressionSyntax ParseAdditiveExpr(SyntaxNode parent)
        {
            return ParseBinaryLeftExpr(currentKind => 
                currentKind == SyntaxKind.PlusToken || 
                currentKind == SyntaxKind.MinusToken,
            parent,
            ParseMultiplicativeExpr);
        }

        private ExpressionSyntax ParseMultiplicativeExpr(SyntaxNode parent)
        {
            return ParseBinaryLeftExpr(currentKind =>
                currentKind == SyntaxKind.AsteriskToken ||
                currentKind == SyntaxKind.SlashToken ||
                currentKind == SyntaxKind.PercentToken,
            parent,
            ParseUnaryOrCastExpr);
        }

        private ExpressionSyntax ParseUnaryOrCastExpr(SyntaxNode parent)
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

                    var castExpr = new CastExpressionSyntax(parent, null!, type);
                    castExpr.Value = ParseUnaryOrCastExpr(castExpr);

                    return castExpr;
                }
            }

            var expr = ParseCallExpression(parent);
            return expr;
        }

        private ExpressionSyntax ParseCallExpression(SyntaxNode parent)
        {
            var possibleCaller = ParsePrimaryExpr(parent);
            if (_tokens.Current.Kind == SyntaxKind.OpenParenToken)
            {
                _tokens.Advance(out var openParenToken);

                var callExpr = new CallExpressionSyntax(parent, possibleCaller);
                possibleCaller.Parent = callExpr;
                callExpr.Args = ParseArgs(callExpr);

                Expect(SyntaxKind.CloseParenToken, "Expected ')'");

                return callExpr;
            }

            //isnt a caller
            return possibleCaller;

            List<ExpressionSyntax> ParseArgs(SyntaxNode thisParent)
            {
                var args = new List<ExpressionSyntax>();

                if (_tokens.Current.Kind == SyntaxKind.CloseParenToken)
                {
                    //no args
                    return args;
                }

                args.Add(ParseExpr(thisParent));

                while (_tokens.Current.Kind == SyntaxKind.CommaToken)
                {
                    _tokens.Advance();
                    var parsed = ParseExpr(thisParent);
                    args.Add(parsed);
                }

                return args;
            }
        }

        private ExpressionSyntax ParseIdentifierMemberExpression(SyntaxNode parent)
        {
            Expect(SyntaxKind.IdentifierToken, out var currentToken, "Expected identifier");

            var identifier = SyntaxFactory.Identifier(parent, currentToken);

            return ParseMemberExpression(parent, identifier);
        }

        private ExpressionSyntax ParseMemberExpression(SyntaxNode parent, ExpressionSyntax leftExpr)
        {
            while(_tokens.Current.Kind == SyntaxKind.DotToken)
            {
                _tokens.Advance();
                Expect(SyntaxKind.IdentifierToken, out var identifierToken, "Expected identifier");

                var rightExpr = SyntaxFactory.Identifier(null!, identifierToken);

                leftExpr = new MemberExpressionSyntax(parent, leftExpr, rightExpr);
                rightExpr.Parent = leftExpr;

                parent = leftExpr;
            }

            return leftExpr;
        }

        private ExpressionSyntax ParseLogicalLeftExpr(Func<SyntaxKind, bool> willAdvance, SyntaxNode parent, Func<SyntaxNode, ExpressionSyntax> getData, [CallerMemberName] string callerForDebugView = "doNotUse")
        {
            var left = getData(parent);

            while (willAdvance(_tokens.Current.Kind))
            {
                _tokens.Advance(out var operationToken);
                var right = getData(parent);
                left = new LogicalExpressionSyntax(parent, left, Convert(operationToken.Kind), right);
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

        private ExpressionSyntax ParseBinaryLeftExpr(Func<SyntaxKind, bool> willAdvance, SyntaxNode parent, Func<SyntaxNode, ExpressionSyntax> getData, [CallerMemberName] string callerForDebugView = "doNotUse")
        {
            var left = getData(parent);

            while (willAdvance(_tokens.Current.Kind))
            {
                _tokens.Advance(out var operationToken);
                var right = getData(parent);
                left = new BinaryExpressionSyntax(parent, left, Convert(operationToken.Kind), right);
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

        private NewExpressionSyntax ParseNewExpr(SyntaxNode parent)
        {
            _tokens.Advance(out var typeToken);

            var type = SyntaxFactory.Type(typeToken);

            Expect(SyntaxKind.OpenParenToken, "Expected '('");

            var newExpr = new NewExpressionSyntax(parent, type, null!);

            newExpr.Args = ParseArgs(newExpr);

            Expect(SyntaxKind.CloseParenToken, "Expected ')'");

            return newExpr;
            
            List<ExpressionSyntax> ParseArgs(SyntaxNode parent)
            {
                List<ExpressionSyntax> args = new();

                if (_tokens.Current.Kind == SyntaxKind.CloseParenToken)
                {
                    //no args
                    return args;
                }

                args.Add(ParseExpr(parent));

                while (_tokens.Current.Kind == SyntaxKind.CommaToken)
                {
                    _tokens.Advance();
                    var parsed = ParseExpr(parent);
                    args.Add(parsed);
                }

                return args;
            }
        }

        private ExpressionSyntax ParsePrimaryExpr(SyntaxNode parent)
        {
            _tokens.Advance(out var currentToken);

            switch (currentToken.Kind)
            {
                case SyntaxKind.IdentifierToken:
                {
                    var identifier = SyntaxFactory.Identifier(null!, currentToken);

                    if (_tokens.Current.Kind == SyntaxKind.DotToken)
                    {
                        var memberExpr = ParseMemberExpression(parent, identifier);
                        identifier.Parent = memberExpr;
                        return memberExpr;
                    }

                    identifier.Parent = parent;
                    return identifier;
                }

                case SyntaxKind.NewKeyword:
                    return ParseNewExpr(parent);

                case SyntaxKind.OpenParenToken:
                    var value = ParseExpr(parent);
                    Expect(SyntaxKind.CloseParenToken, "Expected ')'");
                    return value;

                default:
                    var token = SyntaxFactory.TokenWithValue(currentToken);
                    var literalExpr = SyntaxFactory.LiteralExpression(parent, token);
                    return literalExpr;
            }
        }

#endregion Expressions
    }
}
