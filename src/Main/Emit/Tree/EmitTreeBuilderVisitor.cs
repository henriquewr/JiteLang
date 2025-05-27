using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Bound;
using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Bound.Statements;
using JiteLang.Main.Bound.Statements.Declaration;
using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.Emit.Tree.Statements;
using JiteLang.Main.Emit.Tree.Statements.Declarations;
using JiteLang.Main.Emit.Tree.Utils;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using JiteLang.Main.PredefinedMethods;
using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Modifiers;
using JiteLang.Main.Shared.Type;
using JiteLang.Main.Visitor.Type.Scope;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JiteLang.Main.Emit.Tree
{
    internal class EmitTreeBuilderVisitor
    {
        private readonly Dictionary<string, EmitClassDeclaration> _typesDeclaration = new();

        #region Declarations
        public EmitNamespaceDeclaration VisitNamespaceDeclaration(BoundNamespaceDeclaration root, EmitNode? parent)
        {
            EmitNamespaceDeclaration emitNamespace = new(parent, root.Identifier.Text, null!);
            emitNamespace.Body = new EmitBlockStatement<EmitClassDeclaration, CodeVariable>(emitNamespace, new(root.Body.Members.Count));

            foreach (var classDeclaration in root.Body.Members)
            {
                emitNamespace.Body.Members.Add(VisitClassDeclaration(classDeclaration, emitNamespace.Body));
            }

            emitNamespace.SetParent();
            emitNamespace.Body.SetParent();

            return emitNamespace;
        }

        public EmitClassDeclaration VisitClassDeclaration(BoundClassDeclaration classDeclaration, EmitNode parent)
        {
            EmitClassDeclaration emitClass = new(parent, classDeclaration.Type, classDeclaration.Identifier.Text, null!);
            emitClass.Body = new EmitBlockStatement<EmitNode, CodeField>(emitClass, new(classDeclaration.Body.Members.Count));

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

            emitClass.SetParent();
            emitClass.Body.SetParent();

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

            EmitMethodDeclaration emitMethodDeclaration = new(parent, methodDeclaration.Identifier.Text, null!, methodDeclaration.GetDelegateType())
            {
                IsInitializer = methodDeclaration.IsInitializer,
                Modifiers = methodDeclaration.Modifiers,
                AccessModifiers = methodDeclaration.AccessModifiers,
            };

            emitMethodDeclaration.Body = new EmitBlockStatement<EmitNode, CodeLocal>(emitMethodDeclaration, new(methodDeclaration.Body.Members.Count));

            if (!methodDeclaration.Modifiers.HasFlag(Modifier.Static))
            {
                EmitParameterDeclaration emitParam = new(emitMethodDeclaration.Body, "this");
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

            emitMethodDeclaration.SetParent();
            emitMethodDeclaration.Body.SetParent();

            return emitMethodDeclaration;
        }

        public EmitParameterDeclaration VisitMethodParameter(BoundParameterDeclaration parameterDeclaration, EmitNode parent)
        {
            var method = parent.GetFirstOrDefaultOfType<EmitMethodDeclaration>()!;
            AddParameter(method, parameterDeclaration.Identifier.Text, parameterDeclaration.Type);

            EmitParameterDeclaration parameter = new(parent, parameterDeclaration.Identifier.Text);
            parameter.SetParent();
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
            var method = parent.GetFirstOrDefaultOfType<EmitMethodDeclaration>()!;

            var declarable = GetNearestLocalDeclarable(parent);

            method.StackAllocatedBytes -= 8;

            EmitLocalDeclaration emitVariableDeclaration = new(parent, localDeclaration.Identifier.Text);

            if (localDeclaration.InitialValue is not null)
            {
                emitVariableDeclaration.InitialValue = VisitExpression(localDeclaration.InitialValue, emitVariableDeclaration);
            }

            declarable.Variables.Add(localDeclaration.Identifier.Text, new CodeLocal(method.StackAllocatedBytes, localDeclaration.Type));

            return emitVariableDeclaration;
        }

        public EmitFieldDeclaration VisitFieldDeclaration(BoundFieldDeclaration fieldDeclaration, EmitNode parent)
        {
            EmitFieldDeclaration emitFieldDeclaration = new(parent, fieldDeclaration.Identifier.Text)
            {
                Modifiers = fieldDeclaration.Modifiers,
                AccessModifiers = fieldDeclaration.AccessModifiers,
            };

            var declarable = GetNearestFieldDeclarable(parent);

            var location = declarable.Variables.Count * 8;

            declarable.Variables.Add(emitFieldDeclaration.Name, new CodeField(fieldDeclaration.Type, location));
            emitFieldDeclaration.SetParent();

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

            emitReturnStatement.SetParent();

            return emitReturnStatement;
        }

        public EmitIfStatement VisitIfStatement(BoundIfStatement ifStatement, EmitNode parent, EmitLabelStatement? labelExit)
        {
            EmitIfStatement emitIf = new(parent, null!, null!, null!);

            var condition = VisitExpression(ifStatement.Condition, emitIf);
            var jumpStmt = new EmitJumpStatement(emitIf, null!);
            jumpStmt.Label = EmitLabelStatement.Create(jumpStmt, "ifEnd");
            emitIf.Condition = new EmitCondition(condition, jumpStmt);
            emitIf.Condition.Condition.SetParent();
            emitIf.Condition.JumpIfFalse.SetParent();

            emitIf.Body = new EmitBlockStatement<EmitNode, CodeLocal>(emitIf, new(ifStatement.Body.Members.Count));
            labelExit ??= EmitLabelStatement.Create(emitIf, "ifExit");
            emitIf.LabelExit = labelExit;

            foreach (var item in ifStatement.Body.Members)
            {
                emitIf.Body.Members.Add(VisitDefaultBlock(item, emitIf.Body));
            }

            if (ifStatement.Else is not null)
            {
                emitIf.Else = VisitElseStatement(ifStatement.Else, emitIf, emitIf.LabelExit);
            }

            emitIf.SetParent();
            emitIf.Body.SetParent();

            return emitIf;
        }

        public EmitElseStatement VisitElseStatement(BoundElseStatement elseStatement, EmitNode parent, EmitLabelStatement labelExit)
        {
            EmitElseStatement emitElse = new(parent, null!, labelExit);
            emitElse.Else = elseStatement.Else.Kind switch
            {
                BoundKind.BlockStatement => VisitElseBody((BoundBlockStatement<BoundNode, TypeLocal>)elseStatement.Else),
                BoundKind.IfStatement => VisitIfStatement((BoundIfStatement)elseStatement.Else, emitElse, emitElse.LabelExit),
                _ => throw new UnreachableException(),
            };
            emitElse.SetParent();
            return emitElse;

            EmitBlockStatement<EmitNode, CodeLocal> VisitElseBody(BoundBlockStatement<BoundNode, TypeLocal> elseBody)
            {
                EmitBlockStatement<EmitNode, CodeLocal> emitBlock = new(emitElse, new(elseBody.Members.Count));

                foreach (var item in elseBody.Members)
                {
                    emitBlock.Members.Add(VisitDefaultBlock(item, emitBlock));
                }

                emitBlock.SetParent();

                return emitBlock;
            }
        }

        public EmitWhileStatement VisitWhileStatement(BoundWhileStatement whileStatement, EmitNode parent)
        {
            EmitWhileStatement emitWhile = new(parent, null!, null!);

            var jumpStmt = new EmitJumpStatement(emitWhile, null!);
            jumpStmt.Label = EmitLabelStatement.Create(jumpStmt, "whileEnd");
            var condition = VisitExpression(whileStatement.Condition, emitWhile);
            emitWhile.Condition = new EmitCondition(condition, jumpStmt);
            emitWhile.Condition.Condition.SetParent();
            emitWhile.Condition.JumpIfFalse.SetParent();
            emitWhile.Body = new(emitWhile, new(whileStatement.Body.Members.Count));

            foreach (var item in whileStatement.Body.Members)
            {
                emitWhile.Body.Members.Add(VisitDefaultBlock(item, emitWhile.Body));
            }

            emitWhile.SetParent();
            emitWhile.Body.SetParent();

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
            EmitBinaryExpression emitBinaryExpression = new(parent, null!, binaryExpression.Operation, null!, binaryExpression.Type);
            emitBinaryExpression.Left = VisitExpression(binaryExpression.Left, emitBinaryExpression);
            emitBinaryExpression.Right = VisitExpression(binaryExpression.Right, emitBinaryExpression);
            emitBinaryExpression.SetParent();
            return emitBinaryExpression;
        }

        public EmitIdentifierExpression VisitIdentifierExpression(BoundIdentifierExpression identifierExpression, EmitNode parent)
        {
            EmitIdentifierExpression emitIdentifier = new(parent, identifierExpression.Text, identifierExpression.Type);
            emitIdentifier.SetParent();
            return emitIdentifier;
        }

        public EmitLogicalExpression VisitLogicalExpression(BoundLogicalExpression logicalExpression, EmitNode parent)
        {
            EmitLogicalExpression emitLogicalExpression = new(parent, null!, logicalExpression.Operation, null!, logicalExpression.Type);

            emitLogicalExpression.Left = VisitExpression(logicalExpression.Left, emitLogicalExpression);
            emitLogicalExpression.Right = VisitExpression(logicalExpression.Right, emitLogicalExpression);
            emitLogicalExpression.SetParent();
            return emitLogicalExpression;
        }

        public EmitLiteralExpression VisitLiteralExpression(BoundLiteralExpression literalExpression, EmitNode parent)
        {
            EmitLiteralExpression emitLiteralExpression = new(parent, literalExpression.Value);
            emitLiteralExpression.SetParent();
            return emitLiteralExpression;
        }

        public EmitMemberExpression VisitMemberExpression(BoundMemberExpression memberExpression, EmitNode parent)
        {
            EmitMemberExpression emitMemberExpr = new(parent, null!, null!, memberExpression.Type);
            emitMemberExpr.Left = VisitExpression(memberExpression.Left, emitMemberExpr);
            emitMemberExpr.Right = VisitIdentifierExpression(memberExpression.Right, emitMemberExpr);

            emitMemberExpr.SetParent();

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
            EmitCallExpression emitCallExpression = new(parent, null!, null!);
            emitCallExpression.Caller = VisitExpression(callExpression.Caller, emitCallExpression);
            emitCallExpression.Args = callExpression.Args.Select(x => VisitExpression(x, emitCallExpression)).ToList();
            emitCallExpression.SetParent();
            return emitCallExpression;
        }

        public EmitAssignmentExpression VisitAssignmentExpression(BoundAssignmentExpression assignmentExpression, EmitNode parent)
        {
            EmitAssignmentExpression emitAssingnment = new(parent, null!, null!);
            emitAssingnment.Left = VisitExpression(assignmentExpression.Left, emitAssingnment);

            switch (assignmentExpression.Operator)
            {
                case BoundKind.EqualsToken:
                    emitAssingnment.Right = VisitExpression(assignmentExpression.Right, emitAssingnment);
                    break;
                
                default:
                    throw new NotImplementedException();
            }

            emitAssingnment.SetParent();

            return emitAssingnment;
        }

        public EmitNewExpression VisitNewExpression(BoundNewExpression newExpression, EmitNode parent)
        {
            EmitNewExpression newExpr = new(parent, newExpression.Type, null!);

            newExpr.Initializer = new EmitBlockStatement<EmitNode, CodeLocal>(newExpr, new(1));

            var callExpr = new EmitCallExpression(newExpr.Initializer, null!, new(1));

            EmitLiteralExpression typeSize = new(null, new ConstantValue(default, newExpression.Type.Size));

            callExpr.Args.Add(Method_AllocateReadWriteMemory.Call(callExpr, typeSize));

            var typeDeclaration = _typesDeclaration[newExpression.Type.FullText];
            callExpr.Caller = new EmitIdentifierExpression(callExpr, typeDeclaration.Initializer.Label.Name, typeDeclaration.Initializer.Type);

            newExpr.Initializer.Members.Add(callExpr);

            newExpr.SetParentRecursive();

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
