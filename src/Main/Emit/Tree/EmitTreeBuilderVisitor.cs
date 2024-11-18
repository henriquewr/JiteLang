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

namespace JiteLang.Main.Emit.Tree
{
    internal class EmitTreeBuilderVisitor
    {
        private EmitMethodDeclaration? _currentMethod = null;

        #region Declarations
        public EmitNamespaceDeclaration VisitNamespaceDeclaration(BoundNamespaceDeclaration root, EmitNode? parent)
        {
            EmitNamespaceDeclaration emitNamespace = new(parent!);

            foreach (var classDeclaration in root.Body.Members)
            {
                emitNamespace.Body.Members.Add(VisitClassDeclaration(classDeclaration, emitNamespace.Body));
            }
            
            return emitNamespace;
        }

        public EmitClassDeclaration VisitClassDeclaration(BoundClassDeclaration classDeclaration, EmitNode parent)
        {
            EmitClassDeclaration emitClass = new(parent, classDeclaration.Identifier.Text);

            foreach (var item in classDeclaration.Body.Members)
            {
                switch (item.Kind)
                {
                    case BoundKind.VariableDeclaration:
                        throw new NotImplementedException();
                        emitClass.Body.Members.Add(VisitVariableDeclaration((BoundVariableDeclaration)item, emitClass));
                        break;
                    case BoundKind.MethodDeclaration:
                        emitClass.Body.Members.Add(VisitMethodDeclaration((BoundMethodDeclaration)item, emitClass));
                        break;
                    case BoundKind.ClassDeclaration:
                        emitClass.Body.Members.Add(VisitClassDeclaration((BoundClassDeclaration)item, emitClass));
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
            if (methodDeclaration.Modifiers.Any(x => x.Kind == Syntax.SyntaxKind.ExternKeyword))
            {
                //return VisitExternMethodDeclaration(methodDeclaration, newScope);
            }

            EmitMethodDeclaration emitMethodDeclaration = new(parent, methodDeclaration.Identifier.Text);

            _currentMethod = emitMethodDeclaration;

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
            if (_currentMethod is null)
            {
                throw new NullReferenceException();
            }
            _currentMethod.UpperStackPosition += 8;
            _currentMethod.Body.Variables.Add(parameterDeclaration.Identifier.Text, new(_currentMethod.UpperStackPosition, parameterDeclaration.Type));

            EmitParameterDeclaration parameter = new(parent, parameterDeclaration.Identifier.Text);

            return parameter;
        }

        public EmitVariableDeclaration VisitVariableDeclaration(BoundVariableDeclaration variableDeclaration, EmitNode parent)
        {
            if (_currentMethod is null)
            {
                throw new NullReferenceException();
            }

            var declarable = GetNearestVarDeclarable(parent);

            _currentMethod.StackAllocatedBytes -= 8;

            EmitVariableDeclaration emitVariableDeclaration = new(parent, variableDeclaration.Identifier.Text);

            if (variableDeclaration.InitialValue is not null)
            {
                emitVariableDeclaration.InitialValue = VisitExpression(variableDeclaration.InitialValue, emitVariableDeclaration);
            }

            declarable.Variables.Add(variableDeclaration.Identifier.Text, new(_currentMethod.StackAllocatedBytes, variableDeclaration.Type));

            return emitVariableDeclaration;
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

            EmitBlockStatement<EmitNode> VisitElseBody(BoundBlockStatement<BoundNode> elseBody, EmitNode parent)
            {
                EmitBlockStatement<EmitNode> emitBlock = new(parent);

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
            throw new NotImplementedException();
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
        #endregion Expressions

        private EmitNode VisitDefaultBlock(BoundNode item, EmitNode parent)
        {
            switch (item.Kind)
            {
                case BoundKind.VariableDeclaration:
                    return VisitVariableDeclaration((BoundVariableDeclaration)item, parent);

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

        private static CodeVariable GetVariable(EmitNode node, string name)
        {
            var current = GetNearestVarDeclarable(node, out var currentDeclarableNode);
           
            while (current != null)
            {
                if (current.Variables.TryGetValue(name, out var value))
                {
                    return value;
                }

                current = GetNearestVarDeclarable(currentDeclarableNode.Parent, out currentDeclarableNode);
            }

            throw new UnreachableException();
        }

        private static IVarDeclarable<CodeVariable> GetNearestVarDeclarable(EmitNode node)
        {
            return GetNearestVarDeclarable(node, out _);
        }

        private static IVarDeclarable<CodeVariable> GetNearestVarDeclarable(EmitNode node, out EmitNode declarableNode)
        {
            declarableNode = node;

            while (declarableNode != null)
            {
                if (declarableNode is IVarDeclarable<CodeVariable> declarable)
                {
                    return declarable;
                }

                declarableNode = declarableNode.Parent;
            }

            throw new UnreachableException();
        }
    }
}
