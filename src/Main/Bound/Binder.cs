﻿using JiteLang.Main.Bound.Context;
using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Bound.Statements;
using JiteLang.Main.Bound.Statements.Declaration;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Main.LangParser.Types;
using JiteLang.Main.PredefinedExternMethods.PredefinedExternMethods;
using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Modifiers;
using JiteLang.Main.Shared.Type;
using JiteLang.Main.Shared.Type.Members;
using JiteLang.Main.Shared.Type.Members.Method;
using JiteLang.Main.Visitor.Type.Scope;
using JiteLang.Main.Visitor.Type.Visitor;
using JiteLang.Syntax;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace JiteLang.Main.Bound
{
    internal class Binder
    {
        private BoundMethodDeclaration? _currentMethod = null;

        private readonly ExpressionVisitor _expressionVisitor;

        private readonly ParsedSyntaxTree _parsedSyntaxTree;

        private readonly HashSet<string> _diagnostics;
        private readonly BindingContext _bindingContext;

        private readonly Dictionary<string, TypeSymbol> _types;

        public Binder(ParsedSyntaxTree parsedSyntaxTree, ExpressionVisitor expressionVisitor)
        {
            _parsedSyntaxTree = parsedSyntaxTree;

            _bindingContext = new BindingContext(new());

            _diagnostics = new(parsedSyntaxTree.Errors);
            _expressionVisitor = expressionVisitor;
            _types = new();
        }

        public Binder(ParsedSyntaxTree parsedSyntaxTree)
        {
            _parsedSyntaxTree = parsedSyntaxTree;

            _bindingContext = new BindingContext(new());

            _diagnostics = new HashSet<string>(parsedSyntaxTree.Errors);
            _expressionVisitor = new(_bindingContext, AddError, ResolveType);
            _types = new();
        }

        private void AddError(string errorMessage, SyntaxPosition position)
        {
            var errorText = $"{errorMessage}   On {position.GetPosText()}";
            _diagnostics.Add(errorText);
        }

        public virtual BoundTree Bind(TypeScope scope)
        {
            var namespaceDeclaration = BindNamespaceDeclaration(_parsedSyntaxTree.Root, scope, null);

            var boundTree = new BoundTree(namespaceDeclaration, _diagnostics);

            return boundTree;
        }

        #region Declarations
        public virtual BoundNamespaceDeclaration BindNamespaceDeclaration(NamespaceDeclarationSyntax namespaceDeclaration, TypeScope scope, BoundNode parent)
        {
            var builtIdentifierExpression = BindIdentifierExpression(namespaceDeclaration.Identifier, scope, parent);
            BoundNamespaceDeclaration builtNamespaceDeclaration = new(parent, builtIdentifierExpression);
            builtNamespaceDeclaration.Identifier.Parent = builtNamespaceDeclaration;

            foreach (var classDeclaration in namespaceDeclaration.Body.Members)
            {
                builtNamespaceDeclaration.Body.Members.Add(BindClassDeclaration(classDeclaration, scope, builtNamespaceDeclaration.Body));
            }

            return builtNamespaceDeclaration;
        }

        public virtual BoundClassDeclaration BindClassDeclaration(ClassDeclarationSyntax classDeclaration, TypeScope scope, BoundNode parent) 
        {
            var classType = CreateType(classDeclaration);

            var methods = classDeclaration.Body.Members.Where(x => x.Kind == SyntaxKind.MethodDeclaration)
                .Cast<MethodDeclarationSyntax>().ToFrozenDictionary(k => k.Identifier.Text, v => CreateMethodScope(v, scope, null));

            TypeScope newScope = new(scope);

            var builtIdentifierExpression = BindIdentifierExpression(classDeclaration.Identifier, newScope, null);
            BoundClassDeclaration boundClassDeclaration = new(parent, classType, builtIdentifierExpression);
            boundClassDeclaration.Identifier.Parent = boundClassDeclaration;

            foreach (var item in classDeclaration.Body.Members)
            {
                switch (item.Kind)
                {
                    case SyntaxKind.ClassDeclaration:
                        boundClassDeclaration.Body.Members.Add(BindClassDeclaration((ClassDeclarationSyntax)item, newScope, boundClassDeclaration.Body));
                        break;
                    case SyntaxKind.MethodDeclaration:
                        var method = (MethodDeclarationSyntax)item;
                        var methodScope = methods[method.Identifier.Text];

                        boundClassDeclaration.Body.Members.Add(BindMethodDeclaration(method, methodScope, boundClassDeclaration.Body));
                        break;
                    case SyntaxKind.FieldDeclaration:
                        boundClassDeclaration.Body.Members.Add(BindFieldDeclaration((FieldDeclarationSyntax)item, newScope, boundClassDeclaration.Body));
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
            boundClassDeclaration.Body.Variables = newScope.Variables;
            return boundClassDeclaration;


            TypeScope CreateMethodScope(MethodDeclarationSyntax methodDeclaration, TypeScope scope, BoundNode parent) // make the methods scopeless
            {
                TypeScope newScope = new(scope);

                Dictionary<string, TypeMethodParameter> @params = new(methodDeclaration.Params.Count);

                var methodTypeName = methodDeclaration.Identifier.Text + methodDeclaration.ReturnType.Text;

                foreach (var item in methodDeclaration.Params)
                {
                    var name = item.Identifier.Text;

                    var param = BindMethodParameter(item, newScope, parent);
                    @params.Add(name, new(param.Type, name));
                    newScope.Variables.Add(name, new(param.Type, name));
                    methodTypeName += param.Type.Text;
                }

                var delegateMethodType = (DelegateTypeSymbol)_types[methodTypeName];

                scope.AddMethod(methodDeclaration.Identifier.Text, delegateMethodType, @params);
                return newScope;
            }

            TypeSymbol CreateType(ClassDeclarationSyntax classDeclaration)
            {
                var methods = classDeclaration.Body.Members.Where(x => x.Kind == SyntaxKind.MethodDeclaration).Cast<MethodDeclarationSyntax>();
                var fields = classDeclaration.Body.Members.Where(x => x.Kind == SyntaxKind.FieldDeclaration).Cast<FieldDeclarationSyntax>();

                var classFullName = classDeclaration.GetFullName();

                var typeSymbol = new ClassTypeSymbol(classFullName, classDeclaration.Identifier.Text, new List<FieldSymbol>(), new List<MethodSymbol>());

                if (!_types.TryAdd(typeSymbol.Text, typeSymbol))
                {
                    AddError($"The type {typeSymbol.Text} is already defined", classDeclaration.Position);
                }

                foreach (var field in fields)
                {
                    var fieldSymbol = new FieldSymbol(field.Identifier.Text, ResolveType(field.Variable.Type));
                    typeSymbol.Fields.Add(fieldSymbol);
                }

                foreach (var method in methods)
                {
                    var mParams = method.Params.Select(p => new ParameterSymbol(ResolveType(p.Type))).ToImmutableList();
                    var returnType = ResolveType(method.ReturnType);
                    var methodTypeName = mParams.Aggregate(method.Identifier.Text + returnType.Text, (acc, item) => acc + item.Type.Text);

                    if (!_types.TryGetValue(methodTypeName, out var methodType))
                    {
                        methodType = new DelegateTypeSymbol(methodTypeName, methodTypeName, returnType, mParams);
                        _types.Add(methodTypeName, methodType);
                    }

                    typeSymbol.Methods.Add(new MethodSymbol(
                        method.Identifier.Text,
                        (DelegateTypeSymbol)methodType
                    ));
                }

                return typeSymbol;
            }
        }

        public virtual BoundMethodDeclaration BindMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, TypeScope newScope, BoundNode parent)
        {
            //the method scope is created in the visit class declaration to make it scopeless

            TypeSymbol returnType = ResolveType(methodDeclarationSyntax.ReturnType);

            BoundMethodDeclaration method = new(parent,
                BindIdentifierExpression(methodDeclarationSyntax.Identifier, newScope, null!),
                returnType,
                null!
            );
            (method.Modifiers, method.AccessModifiers) = ConvertModifier(methodDeclarationSyntax.Modifiers);
            method.Identifier.Parent = method;
            method.Params = methodDeclarationSyntax.Params.Select(x => BindMethodParameter(x, newScope, method.Body)).ToList();

            var isExtern = method.Modifiers.HasFlag(Modifier.Extern);

            _currentMethod = method;

            if (isExtern)
            {
                CheckExternMethodDeclaration(method, newScope);
            }
            else
            {
                foreach (var item in methodDeclarationSyntax.Body.Members)
                {
                    method.Body.Members.Add(BindDefaultBlock(item, newScope, method.Body));

                    if (item.Kind == SyntaxKind.ReturnStatement)
                    {
                        break;
                    }
                }
            }

            method.Body.Variables = newScope.Variables;

            _currentMethod = null;

            return method;

            void CheckExternMethodDeclaration(BoundMethodDeclaration methodDeclaration, TypeScope newScope)
            {
                var predefinedExternMethod = PredefinedExternMethodResolver.Resolve(methodDeclaration.Identifier.Text);

                if (predefinedExternMethod is null)
                {
                    AddError($"The extern method '{methodDeclaration.Identifier.Text}' does not exists", methodDeclaration.Identifier.Position);
                }
                else
                {
                    var isSameReturnType = methodDeclaration.ReturnType.IsEqualsNotError(predefinedExternMethod.ReturnType);
                    if (!isSameReturnType)
                    {
                        AddError($"The extern method '{methodDeclaration.Identifier.Text}' must returns '{predefinedExternMethod.ReturnType.Text}'", methodDeclaration.Identifier.Position);
                    }

                    var paramsTypes = methodDeclaration.Params.Select(x => x.Type);

                    if (methodDeclaration.Params.Count != predefinedExternMethod.ParamsTypes.Count)
                    {
                        AddError($"Expected {predefinedExternMethod.ParamsTypes.Count} parameters, but got {methodDeclaration.Params.Count}", methodDeclaration.Identifier.Position);
                    }
                    else
                    {
                        for (int i = 0; i < predefinedExternMethod.ParamsTypes.Count; i++)
                        {
                            var externParamType = predefinedExternMethod.ParamsTypes[i];

                            var methodParam = methodDeclaration.Params[i];

                            if (!externParamType.IsEqualsNotError(methodParam.Type))
                            {
                                AddError($"Expected type {externParamType.Text}", methodParam.Position);
                            }
                        }
                    }
                }
            }
        }

        public virtual BoundParameterDeclaration BindMethodParameter(ParameterDeclarationSyntax parameterDeclarationSyntax, TypeScope scope, BoundNode parent)
        {
            TypeSymbol type = ResolveType(parameterDeclarationSyntax.Type);

            BoundParameterDeclaration builtParameter = new(
                parent,
                BindIdentifierExpression(parameterDeclarationSyntax.Identifier, scope, null), 
                type
            );

            builtParameter.Identifier.Parent = builtParameter;

            return builtParameter;
        }

        public virtual BoundLocalDeclaration BindLocalDeclaration(VariableDeclarationSyntax variableDeclarationSyntax, TypeScope scope, BoundNode parent)
        {
            TypeSymbol variableType = ResolveType(variableDeclarationSyntax.Type);
            BoundLocalDeclaration builtVariable = new(
                parent,
                BindIdentifierExpression(variableDeclarationSyntax.Identifier, scope, null),
                variableType
            );
            builtVariable.Identifier.Parent = builtVariable;

            scope.AddVariable(builtVariable.Identifier.Text, variableType);

            if (variableDeclarationSyntax.InitialValue is not null)
            {
                var varValueType = _expressionVisitor.VisitExpression(variableDeclarationSyntax.InitialValue, scope);
                builtVariable.InitialValue = BindExpression(variableDeclarationSyntax.InitialValue, scope, parent);

                if(!varValueType.Equals(variableType) && !_bindingContext.ConversionTable.TryGetImplicitConversion(varValueType, variableType, out var conversion))
                {
                    AddError($"Cannot implicit convert '{varValueType.Text}' to type '{variableType.Text}'", variableDeclarationSyntax.Position);
                }
            }

            return builtVariable;
        }

        public virtual BoundFieldDeclaration BindFieldDeclaration(FieldDeclarationSyntax fieldDeclarationSyntax, TypeScope scope, BoundNode parent)
        {
            var parentClass = (BoundClassDeclaration)parent.Parent!;

            TypeSymbol fieldType = ResolveType(fieldDeclarationSyntax.Variable.Type);

            var identifier = BindIdentifierExpression(fieldDeclarationSyntax.Variable.Identifier, scope, null!);
            BoundFieldDeclaration fieldDeclaration = new(parent, identifier, fieldType);
            identifier.Parent = fieldDeclaration;

            (fieldDeclaration.Modifiers, fieldDeclaration.AccessModifiers) = ConvertModifier(fieldDeclarationSyntax.Modifiers);

            if (fieldDeclarationSyntax.Variable.InitialValue is not null)
            {
                var fieldValueType = _expressionVisitor.VisitExpression(fieldDeclarationSyntax.Variable.InitialValue, scope);
                var initialValue = BindExpression(fieldDeclarationSyntax.Variable.InitialValue, scope, parent);

                if (!fieldValueType.Equals(fieldType) && !_bindingContext.ConversionTable.TryGetImplicitConversion(fieldValueType, fieldType, out var conversion))
                {
                    AddError($"Cannot implicit convert '{fieldValueType.Text}' to type '{fieldType.Text}'", fieldDeclarationSyntax.Variable.Position);
                }

                BoundMemberExpression memberExpression = new(parentClass.Initializer, null!, identifier);
                memberExpression.Left = new BoundIdentifierExpression(memberExpression, "this", default);

                BoundAssignmentExpression fieldInitializer = new(parentClass.Initializer, memberExpression, BoundKind.EqualsToken, initialValue);

                //Dont add after the return
                parentClass.Initializer.Body.Members.Insert(parentClass.Initializer.Body.Members.Count - 1, fieldInitializer);
            }

            return fieldDeclaration;
        }

        #endregion Declarations

        #region Statements
        public virtual BoundStatement BindIfStatement(IfStatementSyntax ifStatementSyntax, TypeScope scope, BoundNode parent)
        {
            TypeScope newScope = new(scope);
            
            BoundIfStatement boundIfStmt = new(parent, BindExpression(ifStatementSyntax.Condition, scope, parent))
            {
                Position = ifStatementSyntax.Position
            };

            foreach (var item in ifStatementSyntax.Body.Members)
            {
                boundIfStmt.Body.Members.Add(BindDefaultBlock(item, newScope, boundIfStmt.Body));

                if (item.Kind == SyntaxKind.ReturnStatement)
                {
                    break;
                }
            }

            boundIfStmt.Body.Variables = newScope.Variables;

            if (ifStatementSyntax.Else is not null)
            {
                boundIfStmt.Else = new(parent, BindElseStatement(ifStatementSyntax.Else, scope, parent));
            }

            return boundIfStmt;
        }

        public virtual BoundStatement BindElseStatement(ElseStatementSyntax elseStatementSyntax, TypeScope scope, BoundNode parent)
        {
            var boundElse = elseStatementSyntax.Else.Kind switch
            {
                SyntaxKind.BlockStatement => BindElseBody((BlockStatement<SyntaxNode>)elseStatementSyntax.Else),
                SyntaxKind.IfStatement => BindIfStatement((IfStatementSyntax)elseStatementSyntax.Else, scope, parent), //TODO: maybe use the 'newScope' for if else
                _ => throw new UnreachableException(),
            };

            return boundElse;

            BoundBlockStatement<BoundNode> BindElseBody(BlockStatement<SyntaxNode> elseBody)
            {
                TypeScope newScope = new(scope);
                BoundBlockStatement<BoundNode> boundBlock = new(parent);

                foreach (var item in elseBody.Members)
                {
                    boundBlock.Members.Add(BindDefaultBlock(item, newScope, boundBlock));

                    if (item.Kind == SyntaxKind.ReturnStatement)
                    {
                        break;
                    }
                }
                boundBlock.Variables = newScope.Variables;
                return boundBlock;
            }
        }

        public virtual BoundStatement BindWhileStatement(WhileStatementSyntax whileStatementSyntax, TypeScope scope, BoundNode parent)
        {
            var boundCondition = BindExpression(whileStatementSyntax.Condition, scope, parent);

            TypeScope newScope = new(scope);

            var boundWhile = new BoundWhileStatement(
                parent,
                boundCondition
            );

            foreach (var item in whileStatementSyntax.Body.Members)
            {
                boundWhile.Body.Members.Add(BindDefaultBlock(item, newScope, boundWhile.Body));

                if (item.Kind == SyntaxKind.ReturnStatement)
                {
                    break;
                }
            }

            boundWhile.Body.Variables = newScope.Variables;

            return boundWhile;
        }

        public virtual BoundStatement BindReturnStatement(ReturnStatementSyntax returnStatementSyntax, TypeScope scope, BoundNode parent)
        {
            var builtReturn = new BoundReturnStatement(parent)
            {
                Position = returnStatementSyntax.Position
            };

            if (_currentMethod is null)
            {
                AddError("Misplaced return", builtReturn.Position);
            }

            if (returnStatementSyntax.ReturnValue is not null)
            {
                builtReturn.ReturnValue = BindExpression(returnStatementSyntax.ReturnValue, scope, parent);

                var returnType = _expressionVisitor.VisitExpression(returnStatementSyntax.ReturnValue, scope);

                if (_currentMethod?.ReturnType.IsEqualsNotError(returnType) == false)
                {
                    AddError($"Method {_currentMethod.Identifier.Text} must returns {_currentMethod.ReturnType.Text}", builtReturn.Position);
                }
            }

            return builtReturn;
        }

        #endregion Statements

        #region Expressions
        public virtual BoundAssignmentExpression BindAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            _expressionVisitor.VisitAssignmentExpression(assignmentExpressionSyntax, scope);

            var right = BindExpression(assignmentExpressionSyntax.Right, scope, parent);
            var left = BindExpression(assignmentExpressionSyntax.Left, scope, parent);

            var @operator = ToBoundKind(assignmentExpressionSyntax.Operator);

            BoundAssignmentExpression builtAssignmentExpr = new(parent, left, @operator, right);

            return builtAssignmentExpr;
        }

        public virtual BoundExpression BindExpression(ExpressionSyntax expressionSyntax, TypeScope scope, BoundNode parent)
        {
            return expressionSyntax.Kind switch
            {
                SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax)expressionSyntax, scope, parent),
                SyntaxKind.MemberExpression => BindMemberExpression((MemberExpressionSyntax)expressionSyntax, scope, parent),
                SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax)expressionSyntax, scope, parent),
                SyntaxKind.CastExpression => BindCastExpression((CastExpressionSyntax)expressionSyntax, scope, parent),
                SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax)expressionSyntax, scope, parent),
                SyntaxKind.LogicalExpression => BindLogicalExpression((LogicalExpressionSyntax)expressionSyntax, scope, parent),
                SyntaxKind.IdentifierExpression => BindIdentifierExpression((IdentifierExpressionSyntax)expressionSyntax, scope, parent),
                SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax)expressionSyntax, scope, parent),
                SyntaxKind.CallExpression => BindCallExpression((CallExpressionSyntax)expressionSyntax, scope, parent),
                SyntaxKind.NewExpression => BindNewExpression((NewExpressionSyntax)expressionSyntax, scope, parent),
                _ => throw new UnreachableException(),
            };
        }

        public virtual BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            _expressionVisitor.VisitLiteralExpression(literalExpressionSyntax, scope);

            ConstantValue constantValue;

            switch (literalExpressionSyntax.Value.Kind)
            {
                case SyntaxKind.StringLiteralToken:
                    var strTok = (SyntaxTokenWithValue<string>)literalExpressionSyntax.Value;
                    constantValue = new ConstantValue(strTok.Position, strTok.Value);
                    break;
                case SyntaxKind.CharLiteralToken:
                    var charTok = (SyntaxTokenWithValue<char>)literalExpressionSyntax.Value;
                    constantValue = new ConstantValue(charTok.Position, charTok.Value);
                    break;
                case SyntaxKind.IntLiteralToken:
                    var intTok = (SyntaxTokenWithValue<int>)literalExpressionSyntax.Value;
                    constantValue = new ConstantValue(intTok.Position, intTok.Value);
                    break;
                case SyntaxKind.LongLiteralToken:
                    var longTok = (SyntaxTokenWithValue<long>)literalExpressionSyntax.Value;
                    constantValue = new ConstantValue(longTok.Position, longTok.Value);
                    break;
                case SyntaxKind.FalseLiteralToken:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.TrueLiteralToken:
                case SyntaxKind.TrueKeyword:
                    var boolTok = (SyntaxTokenWithValue<bool>)literalExpressionSyntax.Value;
                    constantValue = new ConstantValue(boolTok.Position, boolTok.Value);
                    break;
                case SyntaxKind.NullLiteralToken:
                case SyntaxKind.NullKeyword:
                    var nullTok = (SyntaxTokenWithValue<object?>)literalExpressionSyntax.Value;
                    constantValue = new ConstantValue(nullTok.Position);
                    break;
                default:
                    throw new UnreachableException();
            }

            BoundLiteralExpression builtLiteralExpression = new(parent, constantValue);

            return builtLiteralExpression;
        }

        public virtual BoundExpression BindMemberExpression(MemberExpressionSyntax memberExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            _expressionVisitor.VisitMemberExpression(memberExpressionSyntax, scope);

            BoundMemberExpression boundMemberExpr = new(parent, null!, null!);
            boundMemberExpr.Left = BindExpression(memberExpressionSyntax.Left, scope, boundMemberExpr);
            boundMemberExpr.Right = BindIdentifierExpression(memberExpressionSyntax.Right, scope, boundMemberExpr);

            return boundMemberExpr;
        }

        public virtual BoundExpression BindUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            throw new NotImplementedException();
        }

        public virtual BoundExpression BindCastExpression(CastExpressionSyntax castExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            throw new NotImplementedException();
        }

        public virtual BoundExpression BindBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, TypeScope scope, BoundNode parent) 
        {
            _expressionVisitor.VisitBinaryExpression(binaryExpressionSyntax, scope);

            var left = BindExpression(binaryExpressionSyntax.Left, scope, parent);
            var right = BindExpression(binaryExpressionSyntax.Right, scope, parent);

            BoundBinaryExpression builtBinaryExpression = new(parent, left, binaryExpressionSyntax.Operation, right);

            return builtBinaryExpression;
        }

        public virtual BoundExpression BindLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            _expressionVisitor.VisitLogicalExpression(logicalExpressionSyntax, scope);

            var left = BindExpression(logicalExpressionSyntax.Left, scope, parent);
            var right = BindExpression(logicalExpressionSyntax.Right, scope, parent);

            BoundLogicalExpression builtLogicalExpr = new(parent, left, logicalExpressionSyntax.Operation, right);

            return builtLogicalExpr;
        }

        public virtual BoundExpression BindCallExpression(CallExpressionSyntax callExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            var methodReturnType = _expressionVisitor.VisitCallExpression(callExpressionSyntax, scope);

            var builtCaller = BindExpression(callExpressionSyntax.Caller, scope, parent);
            BoundCallExpression builtCallExpression = new(parent, builtCaller);

            if (methodReturnType.IsError())
            {
                return builtCallExpression;
            }

            var methodType = (DelegateTypeSymbol)_expressionVisitor.VisitExpression(callExpressionSyntax.Caller, scope);

            if (methodType.Parameters.Count == callExpressionSyntax.Args.Count)
            {
                for (int i = 0; i < callExpressionSyntax.Args.Count; i++)
                {
                    var item = callExpressionSyntax.Args[i];

                    var arg = BindExpression(item, scope, parent);

                    builtCallExpression.Args.Add(arg);
                }
            }

            return builtCallExpression;
        }

        public virtual BoundIdentifierExpression BindIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            BoundIdentifierExpression builtIdentifierExpression = new(parent, identifierExpressionSyntax.Text, identifierExpressionSyntax.Position);
            return builtIdentifierExpression;
        }
        
        public virtual BoundExpression BindNewExpression(NewExpressionSyntax newExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            var targetType = (MemberedTypeSymbol)_expressionVisitor.VisitNewExpression(newExpressionSyntax, scope);

            var typeDeclaration = (ClassTypeSymbol)_types[targetType.Text];
            
            var newExpr = new BoundNewExpression(parent, targetType, null!);

            var hasCtor = targetType.Constructors.Any(x => x.Parameters.Count == newExpressionSyntax.Args.Count);

            if (hasCtor)
            {
                var args = new List<BoundExpression>(newExpressionSyntax.Args.Count);
                for (int i = 0; i < newExpressionSyntax.Args.Count; i++)
                {
                    var item = newExpressionSyntax.Args[i];

                    var itemType = _expressionVisitor.VisitExpression(item, scope);

                    var arg = BindExpression(item, scope, parent);
                    args.Add(arg);
                }
            }

            return newExpr;
        }

        #endregion Expressions
        protected virtual TypeSymbol ResolveType(TypeSyntax typeSyntax)
        {
            if (typeSyntax.IsPreDefined)
            {
                var predefinedTypeSymbol = PredefinedTypeSymbol.FromText(typeSyntax.Text) ?? throw new NullReferenceException();
                return predefinedTypeSymbol;
            }

            if (_types.TryGetValue(typeSyntax.Text, out var typeSymbol))
            {
                return typeSymbol;
            }

            typeSymbol = _types.First(x => x.Value.Text == typeSyntax.Text).Value;

            return typeSymbol;
        }

        protected virtual BoundNode BindDefaultBlock(SyntaxNode item, TypeScope scope, BoundNode parent)
        {
            return item.Kind switch
            {
                SyntaxKind.VariableDeclaration => BindLocalDeclaration((VariableDeclarationSyntax)item, scope, parent),
                SyntaxKind.CallExpression => BindCallExpression((CallExpressionSyntax)item, scope, parent),
                SyntaxKind.ReturnStatement => BindReturnStatement((ReturnStatementSyntax)item, scope, parent),
                SyntaxKind.IfStatement => BindIfStatement((IfStatementSyntax)item, scope, parent),
                SyntaxKind.WhileStatement => BindWhileStatement((WhileStatementSyntax)item, scope, parent),
                SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax)item, scope, parent),
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

        private static (Modifier Modifiers, AccessModifier AccessModifiers) ConvertModifier(List<SyntaxToken> modifiers)
        {
            (Modifier Modifiers, AccessModifier AccessModifiers) result = (Modifier.None, AccessModifier.None);

            foreach (var token in modifiers)
            {
                var acessModifier = SyntaxFacts.GetAccessModifier(token.Kind);
                if (acessModifier != AccessModifier.None)
                {
                    result.AccessModifiers |= acessModifier;
                    continue;
                }
                var modifier = SyntaxFacts.GetModifier(token.Kind);
                if (modifier != Modifier.None)
                {
                    result.Modifiers |= modifier;
                    continue;
                }
            }

            return result;
        }
    }
}