using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Bound.Statements.Declaration;
using JiteLang.Main.Bound.Statements;
using JiteLang.Main.Bound;
using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.Emit.Tree.Statements.Declarations;
using JiteLang.Main.Emit.Tree.Statements;
using System;
using System.Diagnostics;
using System.Linq;
using JiteLang.Main.Shared;
using System.Collections.Generic;
using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.PredefinedMethods;
using JiteLang.Main.Shared.Modifiers;
using System.Reflection.Metadata;
using JiteLang.Main.Shared.Type;
using System.Data.Common;

namespace JiteLang.Main.Emit.Tree
{
    internal class EmitTreeBuilderVisitor
    {
        private readonly Dictionary<string, EmitClassDeclaration> _typesDeclaration = new();

        private EmitMethodDeclaration? _currentMethod = null;

        #region Declarations
        public EmitNamespaceDeclaration VisitNamespaceDeclaration(BoundNamespaceDeclaration root, EmitNode? parent)
        {
            EmitNamespaceDeclaration emitNamespace = new(parent!, root.Identifier.Text);

            foreach (var classDeclaration in root.Body.Members)
            {
                emitNamespace.Body.Members.Add(VisitClassDeclaration(classDeclaration, emitNamespace.Body));
            }
            
            return emitNamespace;
        }

        public EmitClassDeclaration VisitClassDeclaration(BoundClassDeclaration classDeclaration, EmitNode parent)
        {
            EmitClassDeclaration emitClass = new(parent, classDeclaration.Type, classDeclaration.Identifier.Text);

            _typesDeclaration[emitClass.GetFullName()] = emitClass;

            foreach (var item in classDeclaration.Body.Members)
            {
                switch (item.Kind)
                {
                    case BoundKind.FieldDeclaration:
                        emitClass.Body.Members.Add(VisitFieldDeclaration((BoundFieldDeclaration)item, emitClass.Body));
                        break;
                    case BoundKind.MethodDeclaration:
                        emitClass.Body.Members.Add(VisitMethodDeclaration((BoundMethodDeclaration)item, emitClass.Body));
                        break;
                    case BoundKind.ClassDeclaration:
                        emitClass.Body.Members.Add(VisitClassDeclaration((BoundClassDeclaration)item, emitClass.Body));
                        break;
                    default:
                        throw new UnreachableException();
                }
            }

            return emitClass;
        }

        //private EmitMethodDeclaration VisitExternMethodDeclaration(BoundMethodDeclaration methodDeclaration, CodeScope newScope)
        //{
        //    //the method scope is created in the visit class declaration to make it scopeless
        //    var instructions = new List<Instruction>();

        //    switch (methodDeclaration.Identifier.Text)
        //    {
        //        case Method_Print.C_Name:

        //            foreach (var item in methodDeclaration.Params)
        //            {
        //                instructions.AddRange(VisitMethodParameter(item, newScope));
        //            }

        //            var strVar = newScope.Variables.First();

        //            var pointerLocation = newScope.GetSizedRbpPosStr(strVar.Key);
        //            newScope.GetVariable(strVar.Key, out int offset);

        //            var lengthOperand = new Operand($"[rsi - {8}]");

        //            var printInstructions = Method_Print.GenerateInstructions(_asmBuilder, new Operand(pointerLocation), lengthOperand);
        //            instructions.AddRange(printInstructions);

        //            return instructions;

        //        default:
        //            throw new UnreachableException();
        //    }
        //}

        public EmitMethodDeclaration VisitMethodDeclaration(BoundMethodDeclaration methodDeclaration, EmitNode parent)
        {
            if (methodDeclaration.Modifiers.HasFlag(Modifier.Extern))
            {
                //return VisitExternMethodDeclaration(methodDeclaration, newScope);
            }

            EmitMethodDeclaration emitMethodDeclaration = new(parent, methodDeclaration.Identifier.Text)
            {
                IsInitializer = methodDeclaration.IsInitializer
            };

            (emitMethodDeclaration.Modifiers, emitMethodDeclaration.AccessModifiers) = (methodDeclaration.Modifiers, methodDeclaration.AccessModifiers);

            _currentMethod = emitMethodDeclaration;

            if (!methodDeclaration.Modifiers.HasFlag(Modifier.Static))
            {
                EmitParameterDeclaration emitParam = new(emitMethodDeclaration, "this");
                var classDeclaration = emitMethodDeclaration.GetFirstOrDefaultOfType<EmitClassDeclaration>()!;

                AddParameter(emitMethodDeclaration!, emitParam.Name, classDeclaration.Type);

                emitMethodDeclaration.Params.Add(emitParam);
            }

            foreach (var item in methodDeclaration.Params)
            {
                emitMethodDeclaration.Params.Add(VisitMethodParameter(item, emitMethodDeclaration.Body));
            }

            foreach (var item in methodDeclaration.Body.Members)
            {
                emitMethodDeclaration.Body.Members.Add(VisitDefaultBlock(item, emitMethodDeclaration.Body));
            }

            _currentMethod = null;
            return emitMethodDeclaration;
        }

        public EmitParameterDeclaration VisitMethodParameter(BoundParameterDeclaration parameterDeclaration, EmitNode parent)
        {
            AddParameter(_currentMethod!, parameterDeclaration.Identifier.Text, parameterDeclaration.Type);

            EmitParameterDeclaration parameter = new(parent, parameterDeclaration.Identifier.Text);

            return parameter;
        }

        private static CodeLocal AddParameter(EmitMethodDeclaration method, string text, TypeSymbol type)
        {
            method.UpperStackPosition += 8;
            CodeLocal local = new(method.UpperStackPosition, type);
            method.Body.Variables.Add(text, local);

            return local;
        }

        public EmitLocalDeclaration VisitLocalDeclaration(BoundLocalDeclaration localDeclaration, EmitNode parent)
        {
            if (_currentMethod is null)
            {
                throw new NullReferenceException();
            }

            var declarable = GetNearestLocalDeclarable(parent);

            _currentMethod.StackAllocatedBytes -= 8;

            EmitLocalDeclaration emitVariableDeclaration = new(parent, localDeclaration.Identifier.Text);

            if (localDeclaration.InitialValue is not null)
            {
                emitVariableDeclaration.InitialValue = VisitExpression(localDeclaration.InitialValue, emitVariableDeclaration);
            }

            declarable.Variables.Add(localDeclaration.Identifier.Text, new CodeLocal(_currentMethod.StackAllocatedBytes, localDeclaration.Type));

            return emitVariableDeclaration;
        }

        public EmitFieldDeclaration VisitFieldDeclaration(BoundFieldDeclaration fieldDeclaration, EmitNode parent)
        {
            EmitFieldDeclaration emitFieldDeclaration = new(parent, fieldDeclaration.Identifier.Text);

            (emitFieldDeclaration.Modifiers, emitFieldDeclaration.AccessModifiers) = (fieldDeclaration.Modifiers, fieldDeclaration.AccessModifiers);

            var declarable = GetNearestFieldDeclarable(parent);

            var location = declarable.Variables.Count * 8;

            declarable.Variables.Add(emitFieldDeclaration.Name, new CodeField(fieldDeclaration.Type, location));

            return emitFieldDeclaration;
        }
        #endregion Declarations

        #region Statements
        public EmitReturnStatement VisitReturnStatement(BoundReturnStatement returnStatement, EmitNode parent)
        {
            EmitReturnStatement emitReturnStatement = new(parent);

            if (returnStatement.ReturnValue is not null)
            {
                emitReturnStatement.ReturnValue = VisitExpression(returnStatement.ReturnValue, emitReturnStatement);
            }

            return emitReturnStatement;
        }

        public EmitIfStatement VisitIfStatement(BoundIfStatement ifStatement, EmitNode parent, EmitLabelStatement? labelExit)
        {
            var condition = VisitExpression(ifStatement.Condition, parent);

            EmitIfStatement emitIf = new(parent, condition, null!);
            labelExit ??= EmitLabelStatement.Create(emitIf, "ifExit");
            emitIf.LabelExit = labelExit;

            condition.Parent = emitIf;

            foreach (var item in ifStatement.Body.Members)
            {
                emitIf.Body.Members.Add(VisitDefaultBlock(item, emitIf.Body));
            }

            if (ifStatement.Else is not null)
            {
                emitIf.Else = VisitElseStatement(ifStatement.Else, parent, emitIf.LabelExit);
            }

            return emitIf;
        }

        public EmitElseStatement VisitElseStatement(BoundElseStatement elseStatement, EmitNode parent, EmitLabelStatement labelExit)
        {
            EmitElseStatement emitElse = new(parent, null!, labelExit);
            emitElse.Else = elseStatement.Else.Kind switch
            {
                BoundKind.BlockStatement => VisitElseBody((BoundBlockStatement<BoundNode>)elseStatement.Else, emitElse),
                BoundKind.IfStatement => VisitIfStatement((BoundIfStatement)elseStatement.Else, emitElse, emitElse.LabelExit),
                _ => throw new UnreachableException(),
            };
            
            return emitElse;

            EmitBlockStatement<EmitNode, CodeLocal> VisitElseBody(BoundBlockStatement<BoundNode> elseBody, EmitNode parent)
            {
                EmitBlockStatement<EmitNode, CodeLocal> emitBlock = new(parent);

                foreach (var item in elseBody.Members)
                {
                    emitBlock.Members.Add(VisitDefaultBlock(item, emitBlock));
                }

                return emitBlock;
            }
        }

        public EmitWhileStatement VisitWhileStatement(BoundWhileStatement whileStatement, EmitNode parent)
        {
            var condition = VisitExpression(whileStatement.Condition, parent);
            EmitWhileStatement emitWhile = new(parent, condition);
            condition.Parent = emitWhile;

            foreach (var item in whileStatement.Body.Members)
            {
                emitWhile.Body.Members.Add(VisitDefaultBlock(item, emitWhile.Body));
            }

            return emitWhile;
        }
        #endregion Statements

        #region Expressions
        public EmitExpression VisitExpression(BoundExpression expression, EmitNode parent)
        {
            EmitExpression expressionInstructions = expression.Kind switch
            {
                BoundKind.LiteralExpression => VisitLiteralExpression((BoundLiteralExpression)expression, parent),
                BoundKind.MemberExpression => VisitMemberExpression((BoundMemberExpression)expression, parent),
                BoundKind.UnaryExpression => VisitUnaryExpression((BoundUnaryExpression)expression, parent),
                BoundKind.CastExpression => VisitCastExpression((BoundCastExpression)expression, parent),
                BoundKind.BinaryExpression => VisitBinaryExpression((BoundBinaryExpression)expression, parent),
                BoundKind.LogicalExpression => VisitLogicalExpression((BoundLogicalExpression)expression, parent),
                BoundKind.IdentifierExpression => VisitIdentifierExpression((BoundIdentifierExpression)expression, parent),
                BoundKind.CallExpression => VisitCallExpression((BoundCallExpression)expression, parent),
                BoundKind.AssignmentExpression => VisitAssignmentExpression((BoundAssignmentExpression)expression, parent),
                BoundKind.NewExpression => VisitNewExpression((BoundNewExpression)expression, parent),
                _ => throw new UnreachableException(),
            };

            return expressionInstructions;
        }

        public EmitBinaryExpression VisitBinaryExpression(BoundBinaryExpression binaryExpression, EmitNode parent)
        {
            var left = VisitExpression(binaryExpression.Left, parent);
            var right = VisitExpression(binaryExpression.Right, parent);

            EmitBinaryExpression emitBinaryExpression = new(parent, left, binaryExpression.Operation, right);

            left.Parent = emitBinaryExpression;
            right.Parent = emitBinaryExpression;

            return emitBinaryExpression;
        }

        public EmitIdentifierExpression VisitIdentifierExpression(BoundIdentifierExpression identifierExpression, EmitNode parent)
        {
            EmitIdentifierExpression emitIdentifier = new(parent, identifierExpression.Text);

            return emitIdentifier;
        }

        public EmitLogicalExpression VisitLogicalExpression(BoundLogicalExpression logicalExpression, EmitNode parent)
        {
            var left = VisitExpression(logicalExpression.Left, parent);
            var right = VisitExpression(logicalExpression.Right, parent);

            EmitLogicalExpression emitLogicalExpression = new(parent, left, logicalExpression.Operation, right);

            left.Parent = emitLogicalExpression;
            right.Parent = emitLogicalExpression;

            return emitLogicalExpression;
        }

        public EmitLiteralExpression VisitLiteralExpression(BoundLiteralExpression literalExpression, EmitNode parent)
        {
            EmitLiteralExpression emitLiteralExpression = new(parent, literalExpression.Value);

            return emitLiteralExpression;
        }

        public EmitMemberExpression VisitMemberExpression(BoundMemberExpression memberExpression, EmitNode parent)
        {
            EmitMemberExpression emitMemberExpr = new(parent, null!, null!);
            emitMemberExpr.Left = VisitExpression(memberExpression.Left, emitMemberExpr);
            emitMemberExpr.Right = VisitIdentifierExpression(memberExpression.Right, emitMemberExpr);

            return emitMemberExpr;
        }

        public EmitUnaryExpression VisitUnaryExpression(BoundUnaryExpression unaryExpression, EmitNode parent)
        {
            throw new NotImplementedException();
        }

        public EmitCastExpression VisitCastExpression(BoundCastExpression castExpression, EmitNode parent)
        {
            throw new NotImplementedException();
        }

        public EmitCallExpression VisitCallExpression(BoundCallExpression callExpression, EmitNode parent)
        {
            var caller = VisitExpression(callExpression.Caller, parent);
            var args = callExpression.Args.Select(x => VisitExpression(x, parent)).ToList();
            EmitCallExpression emitCallExpression = new(parent, caller, args);
            return emitCallExpression;
        }

        public EmitAssignmentExpression VisitAssignmentExpression(BoundAssignmentExpression assignmentExpression, EmitNode parent)
        {
            var left = VisitExpression(assignmentExpression.Left, null!);
            EmitAssignmentExpression emitAssingnment = new(parent, left, null!);
            left.Parent = emitAssingnment;

            switch (assignmentExpression.Operator)
            {
                case BoundKind.EqualsToken:
                    emitAssingnment.Right = VisitExpression(assignmentExpression.Right, emitAssingnment);
                    break;
                
                default:
                    throw new NotImplementedException();
            }

            return emitAssingnment;
        }

        public EmitNewExpression VisitNewExpression(BoundNewExpression newExpression, EmitNode parent)
        {
            var typeDeclaration = _typesDeclaration[newExpression.Type.FullText];

            EmitNewExpression newExpr = new(parent, newExpression.Type);

            var callInitIdentifier = new EmitIdentifierExpression(newExpr.Initializer, typeDeclaration.Initializer.Label.Name);
            EmitLiteralExpression typeSize = new(null!, new ConstantValue(default, newExpression.Type.Size * 10));
            newExpr.Initializer.Members.Add(new EmitCallExpression(newExpr.Initializer, callInitIdentifier, new() { Method_AllocateReadWriteMemory.Call(newExpr.Initializer, typeSize) }));

            return newExpr;
        }

        #endregion Expressions

        private EmitNode VisitDefaultBlock(BoundNode item, EmitNode parent)
        {
            switch (item.Kind)
            {
                case BoundKind.LocalDeclaration:
                    return VisitLocalDeclaration((BoundLocalDeclaration)item, parent);

                case BoundKind.CallExpression:
                    return VisitCallExpression((BoundCallExpression)item, parent);

                case BoundKind.ReturnStatement:
                    return VisitReturnStatement((BoundReturnStatement)item, parent);

                case BoundKind.IfStatement:
                    return VisitIfStatement((BoundIfStatement)item, parent, null);

                case BoundKind.WhileStatement:
                    return VisitWhileStatement((BoundWhileStatement)item, parent);

                case BoundKind.AssignmentExpression:
                    return VisitAssignmentExpression((BoundAssignmentExpression)item, parent);

                default:
                    throw new UnreachableException();
            }
        }

        private static CodeLocal GetLocal(EmitNode node, string name)
        {
            var current = GetNearestLocalDeclarable(node, out var currentDeclarableNode);
           
            while (current != null)
            {
                if (current.Variables.TryGetValue(name, out var value))
                {
                    return value;
                }

                current = GetNearestLocalDeclarable(currentDeclarableNode.Parent, out currentDeclarableNode);
            }

            throw new UnreachableException();
        }

        private static IVarDeclarable<CodeLocal> GetNearestLocalDeclarable(EmitNode node)
        {
            return GetNearestLocalDeclarable(node, out _);
        }

        private static IVarDeclarable<CodeLocal> GetNearestLocalDeclarable(EmitNode node, out EmitNode declarableNode)
        {
            declarableNode = node;

            while (declarableNode != null)
            {
                if (declarableNode is IVarDeclarable<CodeLocal> declarable)
                {
                    return declarable;
                }

                declarableNode = declarableNode.Parent;
            }

            throw new UnreachableException();
        }

        private static IVarDeclarable<CodeField> GetNearestFieldDeclarable(EmitNode node)
        {
            return GetNearestFieldDeclarable(node, out _);
        }

        private static IVarDeclarable<CodeField> GetNearestFieldDeclarable(EmitNode node, out EmitNode declarableNode)
        {
            declarableNode = node;

            while (declarableNode != null)
            {
                if (declarableNode is IVarDeclarable<CodeField> declarable)
                {
                    return declarable;
                }

                declarableNode = declarableNode.Parent;
            }

            throw new UnreachableException();
        }
    }
}
