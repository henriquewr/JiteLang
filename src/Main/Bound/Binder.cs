using JiteLang.Main.Bound.Context;
using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Bound.Statements;
using JiteLang.Main.Bound.Statements.Declaration;
using JiteLang.Main.Bound.TypeResolvers;
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
using JiteLang.Syntax;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JiteLang.Main.Bound
{
    internal class Binder
    {
        private readonly ParsedSyntaxTree _parsedSyntaxTree;

        private readonly HashSet<string> _diagnostics;
        private readonly BindingContext _bindingContext;

        private readonly Dictionary<string, TypeSymbol> _types;

        public Binder(ParsedSyntaxTree parsedSyntaxTree)
        {
            _parsedSyntaxTree = parsedSyntaxTree;

            _bindingContext = new BindingContext(new());

            _diagnostics = new(parsedSyntaxTree.Errors);
            _types = new();
        }

        private static string GetErrorText(string errorMessage, SyntaxPosition position)
        {
            var errorText = $"{errorMessage}   On {position.GetPosText()}";
            return errorText;
        }

        private void AddError(string errorMessage, SyntaxPosition position)
        {
            _diagnostics.Add(GetErrorText(errorMessage, position));
        }

        public virtual BoundTree Bind(TypeScope scope)
        {
            var namespaceDeclaration = BindNamespaceDeclaration(_parsedSyntaxTree.Root, scope);

            var boundTree = new BoundTree(namespaceDeclaration, _diagnostics);

            return boundTree;
        }

        #region Declarations
        public virtual BoundNamespaceDeclaration BindNamespaceDeclaration(NamespaceDeclarationSyntax namespaceDeclaration, TypeScope scope, BoundNode? parent = null)
        {
            BoundNamespaceDeclaration boundNamespaceDeclaration = new(parent, null!, null!);
            boundNamespaceDeclaration.Body = new BoundBlockStatement<BoundClassDeclaration, TypeVariable>(boundNamespaceDeclaration, new(namespaceDeclaration.Body.Members.Count));

            boundNamespaceDeclaration.Identifier = BindIdentifierExpression(namespaceDeclaration.Identifier, scope, boundNamespaceDeclaration);

            foreach (var item in namespaceDeclaration.Body.Members)
            {
                boundNamespaceDeclaration.Body.Members.Add(BindClassDeclaration(item, scope, boundNamespaceDeclaration.Body));
            }

            boundNamespaceDeclaration.SetParent();
            boundNamespaceDeclaration.Body.SetParent();

            return boundNamespaceDeclaration;
        }

        public virtual BoundClassDeclaration BindClassDeclaration(ClassDeclarationSyntax classDeclaration, TypeScope scope, BoundNode parent) 
        {
            var classType = CreateType(classDeclaration);

            var methods = classDeclaration.Body.Members.Where(x => x.Kind == SyntaxKind.MethodDeclaration)
                .Cast<MethodDeclarationSyntax>().ToFrozenDictionary(k => k.Identifier.Text, v => CreateMethodScope(v, scope));

            TypeScope newScope = new(scope);


            BoundClassDeclaration boundClassDeclaration = new
            (
                parent,
                classType,
                null!,
                null!
            );
            boundClassDeclaration.Body = new BoundBlockStatement<BoundNode, TypeField>(boundClassDeclaration, new(classDeclaration.Body.Members.Count));

            boundClassDeclaration.Identifier = BindIdentifierExpression(classDeclaration.Identifier, newScope, boundClassDeclaration);
            boundClassDeclaration.AddInitializer();

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

            boundClassDeclaration.Body.Variables = newScope.Variables.Select(x => new KeyValuePair<string, TypeField>(x.Key, (TypeField)x.Value)).ToDictionary();

            boundClassDeclaration.SetParent();
            boundClassDeclaration.Body.SetParent();

            return boundClassDeclaration;

            TypeScope CreateMethodScope(MethodDeclarationSyntax methodDeclaration, TypeScope scope) // make the methods scopeless
            {
                TypeScope newScope = new(scope);

                Dictionary<string, TypeMethodParameter> @params = new(methodDeclaration.Params.Count);

                var paramsTypes = new List<TypeSymbol>(methodDeclaration.Params.Count);

                foreach (var item in methodDeclaration.Params)
                {
                    var name = item.Identifier.Text;

                    var paramType = ResolveType(item.Type);
                    @params.Add(name, new(paramType, name));
                    paramsTypes.Add(paramType);
                    newScope.Variables.Add(name, new TypeLocal(paramType, name));
                }

                var methodType = DelegateTypeSymbol.Generate(methodDeclaration.Identifier.Text, ResolveType(methodDeclaration.ReturnType), paramsTypes);
                scope.AddMethod(methodDeclaration.Identifier.Text, methodType, @params);

                return newScope;
            }

            ClassTypeSymbol CreateType(ClassDeclarationSyntax classDeclaration)
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
                    var fieldSymbol = new FieldSymbol(field.Identifier.Text, ResolveType(field.Type));
                    typeSymbol.Fields.Add(fieldSymbol);
                }

                foreach (var method in methods)
                {
                    var returnType = ResolveType(method.ReturnType);
                    var mParams = method.Params.Select(p => ResolveType(p.Type));

                    var delegateTypeSymbol = DelegateTypeSymbol.Generate(method.Identifier.Text, returnType, mParams);

                    _types.Add(delegateTypeSymbol.FullText, delegateTypeSymbol);

                    typeSymbol.Methods.Add(new MethodSymbol(
                        method.Identifier.Text,
                        delegateTypeSymbol
                    ));
                }

                return typeSymbol;
            }
        }

        public virtual BoundMethodDeclaration BindMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, TypeScope newScope, BoundNode parent)
        {
            //the method scope is created in the visit class declaration to make it scopeless

            TypeSymbol returnType = ResolveType(methodDeclarationSyntax.ReturnType);
      
            BoundMethodDeclaration method = new
            (
                parent,
                null!,
                returnType,
                null!,
                null!
            );
            method.Body = new BoundBlockStatement<BoundNode, TypeLocal>(method, new(methodDeclarationSyntax.Body.Members.Count));
            method.Params = methodDeclarationSyntax.Params.Select(x => BindMethodParameter(x, newScope, method.Body)).ToList();

            method.Identifier = BindIdentifierExpression(methodDeclarationSyntax.Identifier, newScope, method);
            (method.Modifiers, method.AccessModifiers) = ConvertModifier(methodDeclarationSyntax.Modifiers);
            var isExtern = method.Modifiers.HasFlag(Modifier.Extern);

            if (isExtern)
            {
                CheckExternMethodDeclaration(methodDeclarationSyntax, newScope);
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

            method.Body.Variables = newScope.Variables.Select(x => new KeyValuePair<string, TypeLocal>(x.Key, (TypeLocal)x.Value)).ToDictionary();

            method.SetParent();
            method.Body.SetParent();

            return method;

            void CheckExternMethodDeclaration(MethodDeclarationSyntax methodDeclaration, TypeScope newScope)
            {
                var predefinedExternMethod = PredefinedExternMethodResolver.Resolve(methodDeclaration.Identifier.Text);

                if (predefinedExternMethod is null)
                {
                    AddError($"The extern method '{methodDeclaration.Identifier.Text}' does not exists", methodDeclaration.Identifier.Position);
                }
                else
                {
                    var isSameReturnType = returnType.IsEqualsNotError(predefinedExternMethod.ReturnType);
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

                            if (!externParamType.IsEqualsNotError(ResolveType(methodParam.Type)))
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

            BoundParameterDeclaration boundParameter = new
            (
                parent,
                null!, 
                type
            );
            boundParameter.Identifier = BindIdentifierExpression(parameterDeclarationSyntax.Identifier, scope, boundParameter);
            boundParameter.SetParent();
            return boundParameter;
        }

        public virtual BoundLocalDeclaration BindLocalDeclaration(LocalDeclarationSyntax localDeclarationSyntax, TypeScope scope, BoundNode parent)
        {
            var variableType = ResolveType(localDeclarationSyntax.Type);

            BoundLocalDeclaration boundLocal = new
            (
                parent,
                null!,
                variableType
            );
            boundLocal.Identifier = BindIdentifierExpression(localDeclarationSyntax.Identifier, scope, boundLocal);
            scope.AddVariable(boundLocal.Identifier.Text, boundLocal.Type);

            if (localDeclarationSyntax.InitialValue is not null)
            {
                boundLocal.InitialValue = BindExpression(localDeclarationSyntax.InitialValue, scope, boundLocal);

                if (!boundLocal.InitialValue.Type.Equals(variableType) && !_bindingContext.ConversionTable.TryGetImplicitConversion(boundLocal.InitialValue.Type, variableType, out var conversion))
                {
                    AddError($"Cannot implicit convert '{boundLocal.InitialValue.Type.FullText}' to type '{variableType.FullText}'", localDeclarationSyntax.Position);
                }
            }

            boundLocal.SetParent();

            return boundLocal;
        }

        public virtual BoundFieldDeclaration BindFieldDeclaration(FieldDeclarationSyntax fieldDeclarationSyntax, TypeScope scope, BoundNode parent)
        {
            var parentClass = parent.GetFirstOrDefaultOfType<BoundClassDeclaration>()!;

            TypeSymbol fieldType = ResolveType(fieldDeclarationSyntax.Type);

            BoundFieldDeclaration fieldDeclaration = new
            (
                parent,
                null!,
                fieldType
            );
            fieldDeclaration.Identifier = BindIdentifierExpression(fieldDeclarationSyntax.Identifier, scope, fieldDeclaration);

            (fieldDeclaration.Modifiers, fieldDeclaration.AccessModifiers) = ConvertModifier(fieldDeclarationSyntax.Modifiers);

            if (fieldDeclarationSyntax.InitialValue is not null)
            {
                BoundAssignmentExpression fieldInitializer = new
                (
                    parentClass.Initializer.Body,
                    null!,
                    BoundKind.EqualsToken,
                    null!
                );

                var left = new BoundMemberExpression
                (
                    fieldInitializer,
                    new BoundIdentifierExpression
                    (
                        null,
                        "this",
                        parentClass.Type
                    ),
                    null!,
                    null!
                );
                left.Right = BindIdentifierExpression(fieldDeclarationSyntax.Identifier, scope, left);
                left.Type = left.Right.Type;

                fieldInitializer.Left = left;

                fieldInitializer.Right = BindExpression(fieldDeclarationSyntax.InitialValue, scope, fieldInitializer);
                fieldInitializer.Type = fieldInitializer.Right.Type;

                if (!fieldInitializer.Right.Type.Equals(fieldType) && !_bindingContext.ConversionTable.TryGetImplicitConversion(fieldInitializer.Right.Type, fieldType, out var conversion))
                {
                    AddError($"Cannot implicit convert '{fieldInitializer.Right.Type.FullText}' to type '{fieldType.FullText}'", fieldDeclarationSyntax.Position);
                }

                //Dont add after the return
                parentClass.Initializer.Body.Members.Insert(parentClass.Initializer.Body.Members.Count - 1, fieldInitializer);
                parentClass.Initializer.SetParentRecursive();
            }

            fieldDeclaration.SetParentRecursive();

            return fieldDeclaration;
        }

        #endregion Declarations

        #region Statements
        public virtual BoundStatement BindIfStatement(IfStatementSyntax ifStatementSyntax, TypeScope scope, BoundNode parent)
        {
            TypeScope newScope = new(scope);

            BoundIfStatement boundIfStmt = new(parent, null!, null!);
            boundIfStmt.Body = new BoundBlockStatement<BoundNode, TypeLocal>(boundIfStmt, new(ifStatementSyntax.Body.Members.Count));

            boundIfStmt.Condition = BindExpression(ifStatementSyntax.Condition, scope, boundIfStmt);

            foreach (var item in ifStatementSyntax.Body.Members)
            {
                boundIfStmt.Body.Members.Add(BindDefaultBlock(item, newScope, boundIfStmt.Body));

                if (item.Kind == SyntaxKind.ReturnStatement)
                {
                    break;
                }
            }

            if (ifStatementSyntax.Else is not null)
            {
                boundIfStmt.Else = BindElseStatement(ifStatementSyntax.Else, scope, boundIfStmt);
            }

            boundIfStmt.Body.Variables = newScope.Variables.Select(x => new KeyValuePair<string, TypeLocal>(x.Key, (TypeLocal)x.Value)).ToDictionary();
            
            boundIfStmt.SetParent();
            boundIfStmt.Body.SetParent();

            return boundIfStmt;
        }

        public virtual BoundElseStatement BindElseStatement(ElseStatementSyntax elseStatementSyntax, TypeScope scope, BoundNode parent)
        {
            var elseStmt = new BoundElseStatement(parent, null!);

            elseStmt.Else = elseStatementSyntax.Else.Kind switch
            {
                SyntaxKind.BlockStatement => BindElseBody((BlockStatement<SyntaxNode>)elseStatementSyntax.Else),
                SyntaxKind.IfStatement => BindIfStatement((IfStatementSyntax)elseStatementSyntax.Else, scope, elseStmt), //TODO: maybe use the 'newScope' for if else
                _ => throw new UnreachableException(),
            };
            elseStmt.SetParent();

            return elseStmt;

            BoundBlockStatement<BoundNode, TypeLocal> BindElseBody(BlockStatement<SyntaxNode> elseBody)
            {
                TypeScope newScope = new(scope);

                BoundBlockStatement<BoundNode, TypeLocal> boundBlock = new(elseStmt, new(elseBody.Members.Count));

                foreach (var item in elseBody.Members)
                {
                    boundBlock.Members.Add(BindDefaultBlock(item, newScope, boundBlock));

                    if (item.Kind == SyntaxKind.ReturnStatement)
                    {
                        break;
                    }
                }

                boundBlock.Variables = newScope.Variables.Select(x => new KeyValuePair<string, TypeLocal>(x.Key, (TypeLocal)x.Value)).ToDictionary();

                boundBlock.SetParent();

                return boundBlock;
            }
        }

        public virtual BoundStatement BindWhileStatement(WhileStatementSyntax whileStatementSyntax, TypeScope scope, BoundNode parent)
        {
            TypeScope newScope = new(scope);

            var boundWhile = new BoundWhileStatement
            (
                parent,
                null!,
                null!
            );
            boundWhile.Body = new BoundBlockStatement<BoundNode, TypeLocal>(boundWhile, new(whileStatementSyntax.Body.Members.Count));
            boundWhile.Condition = BindExpression(whileStatementSyntax.Condition, scope, boundWhile);

            foreach (var item in whileStatementSyntax.Body.Members)
            {
                boundWhile.Body.Members.Add(BindDefaultBlock(item, newScope, boundWhile.Body));

                if (item.Kind == SyntaxKind.ReturnStatement)
                {
                    break;
                }
            }

            boundWhile.Body.Variables = newScope.Variables.Select(x => new KeyValuePair<string, TypeLocal>(x.Key, (TypeLocal)x.Value)).ToDictionary();

            boundWhile.SetParent();
            boundWhile.Body.SetParent();

            return boundWhile;
        }

        public virtual BoundStatement BindReturnStatement(ReturnStatementSyntax returnStatementSyntax, TypeScope scope, BoundNode parent)
        {
            var method = parent.GetFirstOrDefaultOfType<BoundMethodDeclaration>();

            if (method is null)
            {
                AddError("Misplaced return", returnStatementSyntax.Position);
            }

            var boundReturn = new BoundReturnStatement(parent);

            if (returnStatementSyntax.ReturnValue is not null)
            {
                boundReturn.ReturnValue = BindExpression(returnStatementSyntax.ReturnValue, scope, boundReturn);

                if (method is not null && !boundReturn.ReturnValue.Type.Equals(method.ReturnType) && _bindingContext.ConversionTable.TryGetImplicitConversion(boundReturn.ReturnValue.Type, method.ReturnType, out var conversion))
                {
                    AddError($"Cannot implicit convert '{boundReturn.ReturnValue.Type.FullText}' to type '{method.ReturnType.FullText}'", returnStatementSyntax.Position);
                }
            }

            boundReturn.SetParent();

            return boundReturn;
        }

        #endregion Statements

        #region Expressions
        public virtual BoundAssignmentExpression BindAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            var @operator = ToBoundKind(assignmentExpressionSyntax.Operator);
            BoundAssignmentExpression builtAssignmentExpr = new(parent, null!, @operator, null!);

            builtAssignmentExpr.Right = BindExpression(assignmentExpressionSyntax.Right, scope, builtAssignmentExpr);
            builtAssignmentExpr.Left = BindExpression(assignmentExpressionSyntax.Left, scope, builtAssignmentExpr);
            builtAssignmentExpr.SetParent();

            if (!builtAssignmentExpr.Right.Type.Equals(builtAssignmentExpr.Left.Type) && !_bindingContext.ConversionTable.TryGetImplicitConversion(builtAssignmentExpr.Right.Type, builtAssignmentExpr.Left.Type, out var conversion))
            {
                AddError($"Cannot implicit convert '{builtAssignmentExpr.Right.Type.FullText}' to type '{builtAssignmentExpr.Left.Type.FullText}'", assignmentExpressionSyntax.Position);
            }

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
                SyntaxKind.IdentifierExpression => BindThisOrIdentifierExpression((IdentifierExpressionSyntax)expressionSyntax, scope, parent),
                SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax)expressionSyntax, scope, parent),
                SyntaxKind.CallExpression => BindCallExpression((CallExpressionSyntax)expressionSyntax, scope, parent),
                SyntaxKind.NewExpression => BindNewExpression((NewExpressionSyntax)expressionSyntax, scope, parent),
                _ => throw new UnreachableException(),
            };
        }

        public virtual BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, TypeScope scope, BoundNode parent)
        {
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
            builtLiteralExpression.SetParentRecursive();
            return builtLiteralExpression;
        }

        public virtual BoundExpression BindMemberExpression(MemberExpressionSyntax memberExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            BoundMemberExpression boundMemberExpr = new(parent, null!, null!, null!);

            boundMemberExpr.Left = BindExpression(memberExpressionSyntax.Left, scope, boundMemberExpr);

            var targetMember = ((MemberedTypeSymbol)boundMemberExpr.Left.Type).GetTypedMembers().FirstOrDefault(x => x.Name == memberExpressionSyntax.Right.Text);

            if (targetMember is null)
            {
                AddError($"The type '{boundMemberExpr.Left.Type.FullText}' does not have any member called '{memberExpressionSyntax.Right.Text}'", memberExpressionSyntax.Right.Position);
            }

            BoundIdentifierExpression builtIdentifierExpression = new(parent, memberExpressionSyntax.Right.Text, targetMember?.Type ?? ErrorTypeSymbol.Instance);
            builtIdentifierExpression.SetParent();

            boundMemberExpr.Right = builtIdentifierExpression;
            boundMemberExpr.Type = boundMemberExpr.Right.Type;

            boundMemberExpr.SetParent();

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
            BoundBinaryExpression boundBinaryExrp = new
            (
                parent,
                null!,
                binaryExpressionSyntax.Operation,
                null!,
                null!
            );

            boundBinaryExrp.Left = BindExpression(binaryExpressionSyntax.Left, scope, boundBinaryExrp);
            boundBinaryExrp.Right = BindExpression(binaryExpressionSyntax.Right, scope, boundBinaryExrp);

            boundBinaryExrp.Type = BinaryExprTypeResolver.Resolve(boundBinaryExrp.Left.Type, boundBinaryExrp.Operation, boundBinaryExrp.Right.Type);
            boundBinaryExrp.SetParent();
            if (boundBinaryExrp.Type.IsError())
            {
                AddError($"Cannot implicit convert '{boundBinaryExrp.Left.Type.FullText}' to type '{boundBinaryExrp.Right.Type.FullText}'", binaryExpressionSyntax.Left.Position);
            }

            return boundBinaryExrp;
        }

        public virtual BoundExpression BindLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            BoundLogicalExpression boundLogicalExpr = new
            (
                parent,
                null!,
                logicalExpressionSyntax.Operation,
                null!,
                null!
            );

            boundLogicalExpr.Left = BindExpression(logicalExpressionSyntax.Left, scope, boundLogicalExpr);
            boundLogicalExpr.Right = BindExpression(logicalExpressionSyntax.Right, scope, boundLogicalExpr);

            boundLogicalExpr.Type = LogicalExprTypeResolver.Resolve(boundLogicalExpr.Left.Type, boundLogicalExpr.Operation, boundLogicalExpr.Right.Type);

            if (boundLogicalExpr.Type.IsError())
            {
                AddError($"Operator not defined for '{boundLogicalExpr.Left.Type.FullText}' and '{boundLogicalExpr.Right.Type.FullText}'", logicalExpressionSyntax.Left.Position);
            }
            boundLogicalExpr.SetParent();
            return boundLogicalExpr;
        }

        public virtual BoundExpression BindCallExpression(CallExpressionSyntax callExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            BoundCallExpression builtCallExpression = new(parent, null!, null!);
            builtCallExpression.Caller = BindExpression(callExpressionSyntax.Caller, scope, builtCallExpression);
            builtCallExpression.Args = callExpressionSyntax.Args.Select(item => BindExpression(item, scope, builtCallExpression)).ToList();

            if (builtCallExpression.Type is not DelegateTypeSymbol methodType)
            {
                AddError($"The type {builtCallExpression.Type.FullText} is not a method type", callExpressionSyntax.Position);
            }
            else
            {
                if (methodType.Parameters.Count == builtCallExpression.Args.Count)
                {
                    for (int i = 0; i < methodType.Parameters.Count; i++)
                    {
                        var providedArg = builtCallExpression.Args[i];

                        var expectedArgType = methodType.Parameters[i].Type;

                        if (!expectedArgType.Equals(providedArg.Type) && !_bindingContext.ConversionTable.TryGetImplicitConversion(providedArg.Type, expectedArgType, out var conversion))
                        {
                            var argSyntax = callExpressionSyntax.Args[i];

                            AddError($"Argument {i + 1} cannot be implicitly converted from type '{providedArg.Type.FullText}' to type '{expectedArgType.FullText}'", argSyntax.Position);
                            break;
                        }
                    }
                }
                else
                {
                    AddError($"The method '{methodType.Text}' expect {methodType.Parameters.Count} arguments", callExpressionSyntax.Caller.Position);
                }
            }

            builtCallExpression.SetParent();

            return builtCallExpression;
        }

        public virtual BoundIdentifierExpression BindIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            var identifier = scope.GetIdentifier(identifierExpressionSyntax.Text);

            if (identifier is null)
            {
                //AddError($"The identifier {identifierExpressionSyntax.Text} does not exist in the current context", identifierExpressionSyntax.Position);
            }

            BoundIdentifierExpression builtIdentifierExpression = new(parent, identifierExpressionSyntax.Text, identifier?.Type ?? ErrorTypeSymbol.Instance);
            builtIdentifierExpression.SetParent();
            return builtIdentifierExpression;
        }
        
        private BoundExpression BindThisOrIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            var method = parent.GetFirstOrDefaultOfType<BoundMethodDeclaration>();

            var identifier = scope.GetIdentifier(identifierExpressionSyntax.Text);
            
            if (identifier is not null || method is null || method.Modifiers.HasFlag(Modifier.Static))
            {
                return BindIdentifierExpression(identifierExpressionSyntax, scope, parent);
            }

            var classDeclaration = method.GetFirstOrDefaultOfType<BoundClassDeclaration>()!;

            var fieldSymbol = classDeclaration.Type.Fields.FirstOrDefault(x => x.Name == identifierExpressionSyntax.Text);

            if (fieldSymbol is null)
            {
                AddError($"The identifier {identifierExpressionSyntax.Text} does not exist in the current context", identifierExpressionSyntax.Position);
            }

            var resultType = fieldSymbol?.Type ?? ErrorTypeSymbol.Instance;

            var boundMemberExpr = new BoundMemberExpression
            (
                parent,
                new BoundIdentifierExpression(null, "this", classDeclaration.Type),
                new BoundIdentifierExpression(null, identifierExpressionSyntax.Text, resultType),
                resultType!
            );

            boundMemberExpr.SetParentRecursive();

            return boundMemberExpr;
        }

        public virtual BoundExpression BindNewExpression(NewExpressionSyntax newExpressionSyntax, TypeScope scope, BoundNode parent)
        {
            var targetMemberedType = (MemberedTypeSymbol)ResolveType(newExpressionSyntax.Type);
            var boundNewExpr = new BoundNewExpression(parent, targetMemberedType, null!);

            boundNewExpr.Args = newExpressionSyntax.Args.Select(arg => BindExpression(arg, scope, boundNewExpr)).ToList();

            var argsTypes = boundNewExpr.Args.Select(x => x.Type).ToList();

            var avaliableCtors = targetMemberedType.GetCtors(argsTypes.Count).ToList();

            if (avaliableCtors.Count == 0)
            {
                AddError($"The type '{targetMemberedType.Text}' does not have any constructor with {argsTypes.Count} arguments", newExpressionSyntax.Position);
            }
            else
            {
                var targetCtor = targetMemberedType.GetMatchingCtor(_bindingContext, argsTypes);
                if (targetCtor is null)
                {
                    foreach (var avaliableCtor in avaliableCtors)
                    {
                        var hasError = false;

                        for (int i = 0; i < boundNewExpr.Args.Count; i++)
                        {
                            var providedArg = boundNewExpr.Args[i];

                            var expectedArgType = avaliableCtor.Parameters[i].Type;

                            if (!expectedArgType.Equals(providedArg.Type) && !_bindingContext.ConversionTable.TryGetImplicitConversion(providedArg.Type, expectedArgType, out var conversion))
                            {
                                hasError = true;
                                var argSyntax = newExpressionSyntax.Args[i];
                                AddError($"Argument {i + 1} cannot be implicitly converted from type '{providedArg.Type.FullText}' to type '{expectedArgType.FullText}'", argSyntax.Position);
                            }
                        }

                        if (hasError)
                        {
                            break;
                        }
                    }
                }
            }

            boundNewExpr.SetParentRecursive();

            return boundNewExpr;
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
                SyntaxKind.LocalDeclaration => BindLocalDeclaration((LocalDeclarationSyntax)item, scope, parent),
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
                SyntaxKind.EqualsToken => BoundKind.EqualsToken,
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