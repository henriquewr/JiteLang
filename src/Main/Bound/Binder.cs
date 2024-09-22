using JiteLang.Main.Builder;
using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Bound.Statements;
using JiteLang.Main.Bound.Statements.Declaration;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Main.LangParser.Types;
using JiteLang.Main.Shared;
using JiteLang.Syntax;
using System;
using System.Diagnostics;
using System.Linq;
using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Builder.Instructions;
using static System.Formats.Asn1.AsnWriter;
using System.Collections.Generic;

namespace JiteLang.Main.Bound
{
    internal class Binder
    {
        #region Declarations
        public virtual BoundNamespaceDeclaration BindNamespaceDeclaration(NamespaceDeclarationSyntax namespaceDeclaration)
        {
            var builtIdentifierExpression = BindIdentifierExpression(namespaceDeclaration.Identifier);

            BoundNamespaceDeclaration builtNamespaceDeclaration = new(builtIdentifierExpression, new());

            foreach (var classDeclaration in namespaceDeclaration.Body.Members)
            {
                builtNamespaceDeclaration.Body.Members.Add(BindClassDeclaration(classDeclaration));
            }

            return builtNamespaceDeclaration;
        }

        public virtual BoundClassDeclaration BindClassDeclaration(ClassDeclarationSyntax classDeclaration) 
        {
            var builtIdentifierExpression = BindIdentifierExpression(classDeclaration.Identifier);

            BoundClassDeclaration boundClassDeclaration = new(builtIdentifierExpression, new());

            foreach (var item in classDeclaration.Body.Members)
            {
                switch (item.Kind)
                {
                    case SyntaxKind.ClassDeclaration:
                        boundClassDeclaration.Body.Members.Add(BindClassDeclaration((ClassDeclarationSyntax)item));
                        break;
                    case SyntaxKind.MethodDeclaration:
                        boundClassDeclaration.Body.Members.Add(BindMethodDeclaration((MethodDeclarationSyntax)item));
                        break;
                    case SyntaxKind.VariableDeclaration:
                        boundClassDeclaration.Body.Members.Add(BindVariableDeclaration((VariableDeclarationSyntax)item));
                        break;
                    default:
                        throw new UnreachableException();
                }
            }

            return boundClassDeclaration;
        }

        public virtual BoundMethodDeclaration BindMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            var builtIdentifier = BindIdentifierExpression(methodDeclarationSyntax.Identifier);

            var methodParams = methodDeclarationSyntax.Params.Select(item => BindMethodParameter(item)).ToList();

            TypeSymbol returnType = ConvertType(methodDeclarationSyntax.ReturnType);

            BoundMethodDeclaration method = new(builtIdentifier,
                returnType,
                new(),
                methodDeclarationSyntax.Modifiers,
                methodParams
            );

            foreach (var item in methodDeclarationSyntax.Body.Members)
            {
                method.Body.Members.Add(BindDefaultBlock(item));
            }

            return method;
        }

        public virtual BoundParameterDeclaration BindMethodParameter(ParameterDeclarationSyntax parameterDeclarationSyntax)
        {
            var builtIdentifier = BindIdentifierExpression(parameterDeclarationSyntax.Identifier);
            TypeSymbol type = ConvertType(parameterDeclarationSyntax.Type);
            BoundParameterDeclaration builtParameter = new(builtIdentifier, type);
            return builtParameter;
        }

        public virtual BoundVariableDeclaration BindVariableDeclaration(VariableDeclarationSyntax variableDeclarationSyntax)
        {
            var builtIdentifier = BindIdentifierExpression(variableDeclarationSyntax.Identifier);
            TypeSymbol type = ConvertType(variableDeclarationSyntax.Type);
            BoundVariableDeclaration builtVariable = new(builtIdentifier, type);

            if (variableDeclarationSyntax.InitialValue is not null)
            {
                builtVariable.InitialValue = BindExpression(variableDeclarationSyntax.InitialValue);
            }

            return builtVariable;
        }

        #endregion Declarations

        #region Statements
        public virtual BoundStatement BindElseStatement(StatementSyntax elseStatementSyntax)
        {
            var boundElse = elseStatementSyntax.Kind switch
            {
                SyntaxKind.BlockStatement => BindElseBody((BlockStatement<SyntaxNode>)elseStatementSyntax),
                SyntaxKind.IfStatement => BindIfStatement((IfStatementSyntax)elseStatementSyntax),
                _ => throw new UnreachableException(),
            };

            return boundElse;

            BoundBlockStatement<BoundNode> BindElseBody(BlockStatement<SyntaxNode> elseBody)
            {
                var boundList = elseBody.Members.Select(x => BindDefaultBlock(x)).ToList();

                var boundBlock = new BoundBlockStatement<BoundNode>(boundList);

                return boundBlock;
            }
        }

        public virtual BoundStatement BindWhileStatement(WhileStatementSyntax whileStatementSyntax)
        {
            var boundCondition = BindExpression(whileStatementSyntax.Condition);

            var boundList = whileStatementSyntax.Body.Members.Select(x => BindDefaultBlock(x)).ToList();
            var boundBlock = new BoundBlockStatement<BoundNode>(boundList);

            var boundWhile = new BoundWhileStatement(boundCondition, boundBlock);

            return boundWhile;
        }

        public virtual BoundStatement BindReturnStatement(ReturnStatementSyntax returnStatementSyntax)
        {
            var builtReturn = new BoundReturnStatement()
            {
                Position = returnStatementSyntax.Position
            };

            if (returnStatementSyntax.ReturnValue is not null)
            {
                builtReturn.ReturnValue = BindExpression(returnStatementSyntax.ReturnValue);
            }

            return builtReturn;
        }

        public virtual BoundStatement BindIfStatement(IfStatementSyntax ifStatementSyntax)
        {
            var builtCondition = BindExpression(ifStatementSyntax.Condition);

            BoundIfStatement boundIfStmt = new(builtCondition, new())
            {
                Position = ifStatementSyntax.Position
            };

            foreach (var item in ifStatementSyntax.Body.Members)
            {
                boundIfStmt.Body.Members.Add(BindDefaultBlock(item));
            }

            if(ifStatementSyntax.Else is not null)
            {
                boundIfStmt.Else = BindElseStatement(ifStatementSyntax.Else);
            }

            return boundIfStmt;
        }
        #endregion Statements

        #region Expressions
        public virtual BoundAssignmentExpression BindAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax)
        {
            var right = BindExpression(assignmentExpressionSyntax.Right);
            var left = BindExpression(assignmentExpressionSyntax.Left);

            var @operator = ToBoundKind(assignmentExpressionSyntax.Operator);

            BoundAssignmentExpression builtAssignmentExpr = new(left, @operator, right);

            return builtAssignmentExpr;
        }

        public virtual BoundExpression BindExpression(ExpressionSyntax expressionSyntax)
        {
            return expressionSyntax.Kind switch
            {
                SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax)expressionSyntax),
                SyntaxKind.MemberExpression => BindMemberExpression((MemberExpressionSyntax)expressionSyntax),
                SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax)expressionSyntax),
                SyntaxKind.CastExpression => BindCastExpression((CastExpressionSyntax)expressionSyntax),
                SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax)expressionSyntax),
                SyntaxKind.LogicalExpression => BindLogicalExpression((LogicalExpressionSyntax)expressionSyntax),
                SyntaxKind.IdentifierExpression => BindIdentifierExpression((IdentifierExpressionSyntax)expressionSyntax),
                SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax)expressionSyntax),
                SyntaxKind.CallExpression => BindCallExpression((CallExpressionSyntax)expressionSyntax),
                _ => throw new UnreachableException(),
            };
        }

        public virtual BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax)
        {
            ConstantValue constantValue;

            switch (literalExpressionSyntax.Value.Kind)
            {
                case SyntaxKind.StringLiteralToken:
                    var strTok = (SyntaxTokenWithValue<string>)literalExpressionSyntax.Value;
                    constantValue = new ConstantValue(ConstantValueKind.String, strTok.Position, strTok.Value);
                    break;
                case SyntaxKind.CharLiteralToken:
                    var charTok = (SyntaxTokenWithValue<char>)literalExpressionSyntax.Value;
                    constantValue = new ConstantValue(ConstantValueKind.Char, charTok.Position, charTok.Value);
                    break;
                case SyntaxKind.IntLiteralToken:
                    var intTok = (SyntaxTokenWithValue<int>)literalExpressionSyntax.Value;
                    constantValue = new ConstantValue(ConstantValueKind.Int, intTok.Position, intTok.Value);
                    break;
                case SyntaxKind.LongLiteralToken:
                    var longTok = (SyntaxTokenWithValue<long>)literalExpressionSyntax.Value;
                    constantValue = new ConstantValue(ConstantValueKind.Long, longTok.Position, longTok.Value);
                    break;
                case SyntaxKind.FalseLiteralToken:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.TrueLiteralToken:
                case SyntaxKind.TrueKeyword:
                    var boolTok = (SyntaxTokenWithValue<bool>)literalExpressionSyntax.Value;
                    constantValue = new ConstantValue(ConstantValueKind.Bool, boolTok.Position, boolTok.Value);
                    break;
                default:
                    throw new UnreachableException();
            }

            BoundLiteralExpression builtLiteralExpression = new(constantValue);

            return builtLiteralExpression;
        }

        public virtual BoundExpression BindMemberExpression(MemberExpressionSyntax memberExpressionSyntax)
        {
            throw new NotImplementedException();
        }

        public virtual BoundExpression BindUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax)
        {
            throw new NotImplementedException();
        }

        public virtual BoundExpression BindCastExpression(CastExpressionSyntax castExpressionSyntax)
        {
            throw new NotImplementedException();
        }

        public virtual BoundExpression BindBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax) 
        {
            var left = BindExpression(binaryExpressionSyntax.Left);
            var right = BindExpression(binaryExpressionSyntax.Right);

            BoundBinaryExpression builtBinaryExpression = new(left, binaryExpressionSyntax.Operation, right);

            return builtBinaryExpression;
        }

        public virtual BoundExpression BindLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax)
        {
            var left = BindExpression(logicalExpressionSyntax.Left);
            var right = BindExpression(logicalExpressionSyntax.Right);

            BoundLogicalExpression builtLogicalExpr = new(left, logicalExpressionSyntax.Operation, right);

            return builtLogicalExpr;
        }

        public virtual BoundExpression BindCallExpression(CallExpressionSyntax callExpressionSyntax)
        {
            var builtCaller = BindExpression(callExpressionSyntax.Caller);
            BoundCallExpression builtCallExpression = new(builtCaller);

            foreach (var item in callExpressionSyntax.Args)
            {
                var arg = BindExpression(item);
                builtCallExpression.Args.Add(arg);
            }

            return builtCallExpression;
        }

        public virtual BoundIdentifierExpression BindIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax)
        {
            BoundIdentifierExpression builtIdentifierExpression = new(identifierExpressionSyntax.Text, identifierExpressionSyntax.Position);
            return builtIdentifierExpression;
        }

        #endregion Expressions
        private static TypeSymbol ConvertType(TypeSyntax typeSyntax)
        {
            if (typeSyntax.IsPreDefined)
            {
                var predefinedTypeSymbol = PredefinedTypeSymbol.FromText(typeSyntax.Text);
                return predefinedTypeSymbol ?? throw new NullReferenceException();
            }

            TypeSymbol typeSymbol = new(typeSyntax.Text);

            return typeSymbol;
        }

        private BoundNode BindDefaultBlock(SyntaxNode item)
        {
            return item.Kind switch
            {
                SyntaxKind.VariableDeclaration => BindVariableDeclaration((VariableDeclarationSyntax)item),
                SyntaxKind.CallExpression => BindCallExpression((CallExpressionSyntax)item),
                SyntaxKind.ReturnStatement => BindReturnStatement((ReturnStatementSyntax)item),
                SyntaxKind.IfStatement => BindIfStatement((IfStatementSyntax)item),
                SyntaxKind.WhileStatement => BindWhileStatement((WhileStatementSyntax)item),
                SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax)item),
                _ => throw new UnreachableException(),
            };
        }

        private static BoundKind ToBoundKind(SyntaxKind syntaxKind)
        {
            var builtKind = syntaxKind switch
            {
                SyntaxKind.None => BoundKind.None,
                //SyntaxKind.NotToken => BoundKind.NotToken,
                SyntaxKind.EqualsToken => BoundKind.EqualsToken,
                //SyntaxKind.EqualsEqualsToken => BoundKind.EqualsEqualsToken,
                //SyntaxKind.NotEqualsToken => BoundKind.NotEqualsToken,
                //SyntaxKind.GreaterThanToken => BoundKind.GreaterThanToken,
                //SyntaxKind.GreaterThanEqualsToken => BoundKind.GreaterThanEqualsToken,
                //SyntaxKind.LessThanToken => BoundKind.LessThanToken,
                //SyntaxKind.LessThanEqualsToken => BoundKind.LessThanEqualsToken,
                //SyntaxKind.PlusToken => BoundKind.PlusToken,
                //SyntaxKind.PlusEqualsToken => BoundKind.PlusEqualsToken,
                //SyntaxKind.MinusToken => BoundKind.MinusToken,
                //SyntaxKind.MinusEqualsToken => BoundKind.MinusEqualsToken,
                //SyntaxKind.AsteriskToken => BoundKind.AsteriskToken,
                //SyntaxKind.AsteriskEqualsToken => BoundKind.AsteriskEqualsToken,
                //SyntaxKind.SlashToken => BoundKind.SlashToken,
                //SyntaxKind.SlashEqualsToken => BoundKind.SlashEqualsToken,
                //SyntaxKind.PercentToken => BoundKind.PercentToken,
                //SyntaxKind.PercentEqualsToken => BoundKind.PercentEqualsToken,
                //SyntaxKind.AmpersandToken => BoundKind.AmpersandToken,
                //SyntaxKind.AmpersandAmpersandToken => BoundKind.AmpersandAmpersandToken,
                //SyntaxKind.BarToken => BoundKind.BarToken,
                //SyntaxKind.BarBarToken => BoundKind.BarBarToken,
                //SyntaxKind.CaretToken => BoundKind.CaretToken,
                _ => throw new UnreachableException(),
            };

            return builtKind;
        }
    }
}
