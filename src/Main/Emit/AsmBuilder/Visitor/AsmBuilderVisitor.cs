﻿using JiteLang.Main.AsmBuilder.Builder;
using JiteLang.Main.AsmBuilder.Instructions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using JiteLang.Main.Emit.Tree.Statements.Declarations;
using JiteLang.Main.Shared;
using JiteLang.Main.Emit.Tree.Statements;
using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.Emit.Tree;
using JiteLang.Main.Emit.AsmBuilder.Builder.Abstractions;
using JiteLang.Main.Emit.AsmBuilder.Operands;
using JiteLang.Main.Emit.Tree.Utils;
using JiteLang.Main.Shared.Type;
using System.Linq;
using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.PredefinedExternMethods.PredefinedExternMethods;
using JiteLang.Main.PredefinedMethods;

namespace JiteLang.Main.Emit.AsmBuilder.Visitor
{
    internal class AsmBuilderVisitor
    {
        private readonly AssemblyBuilder _asmBuilder;
        private readonly IAssemblyBuilderAbstractions _asmBuilderAbstractions;

        public AsmBuilderVisitor(AssemblyBuilder asmBuilder, IAssemblyBuilderAbstractions asmBuilderAbstractions)
        {
            _asmBuilder = asmBuilder;
            _asmBuilderAbstractions = asmBuilderAbstractions;
        }

        #region Declarations
        public List<Instruction> VisitNamespaceDeclaration(EmitNamespaceDeclaration namespaceDeclaration)
        {
            var instructions = new List<Instruction>();

            foreach (var classDeclaration in namespaceDeclaration.Body.Members)
            {
                instructions.AddRange(VisitClassDeclaration(classDeclaration));
            }

            instructions.Add(_asmBuilder.Section(new Operand(".text")));

            instructions.Add(_asmBuilder.Extern(new Operand("ExitProcess")));
            instructions.Add(_asmBuilder.Extern(new Operand("VirtualAlloc")));
            instructions.Add(_asmBuilder.Extern(new Operand("GetStdHandle")));
            instructions.Add(_asmBuilder.Extern(new Operand("WriteFile")));

            instructions.AddRange(GenPredefinedMethods());

            instructions.Add(_asmBuilder.Global(new Operand("_start")));
            instructions.Add(_asmBuilder.Label(new Operand("_start")));
            instructions.Add(_asmBuilder.Call(new Operand("Main")));

            var code = Operand.Rbx;
            instructions.Add(_asmBuilder.Mov(code, Operand.Rax));
            instructions.AddRange(_asmBuilderAbstractions.Exit(code));
            instructions.Add(_asmBuilder.Ret(new Operand("0")));
            return instructions;
        }

        public List<Instruction> GenPredefinedMethods()
        {
            var instructions = new List<Instruction>();

            instructions.AddRange(Method_AllocateReadWriteMemory.GenerateInstructions(_asmBuilderAbstractions, _asmBuilder));

            return instructions;
        }

        public List<Instruction> VisitClassDeclaration(EmitClassDeclaration classDeclaration)
        {
            var instructions = new List<Instruction>();

            foreach (var item in classDeclaration.Body.Members)
            {
                switch (item.Kind)
                {
                    case EmitKind.FieldDeclaration:
                        instructions.AddRange(VisitFieldDeclaration((EmitFieldDeclaration)item));
                        break;
                    case EmitKind.MethodDeclaration:
                        instructions.AddRange(VisitMethodDeclaration((EmitMethodDeclaration)item));
                        break;
                    case EmitKind.ClassDeclaration:
                        instructions.AddRange(VisitClassDeclaration((EmitClassDeclaration)item));
                        break;
                    default:
                        throw new UnreachableException();
                }
            }

            return instructions;
        }

        private List<Instruction> VisitExternMethodDeclaration(EmitMethodDeclaration methodDeclaration)
        {
            var instructions = new List<Instruction>();
            switch (methodDeclaration.Label.Name)
            {
                case Method_Print.C_Name:
                    var isWin = true;
                    if (isWin)
                    {
                        foreach (var item in methodDeclaration.Params)
                        {
                            instructions.AddRange(VisitMethodParameter(item));
                        }

                        var strVar = methodDeclaration.Params[0];

                        var pointerLocation = GetSizedRbpPosStr(methodDeclaration.Body.Variables[strVar.Name].InScopeStackLocation);
                        var lengthOperand = new Operand($"[rdx - {8}]");

                        var printInstructions = Method_Print.GenerateInstructions(_asmBuilder, new Operand(pointerLocation), lengthOperand, isWin);
                        instructions.AddRange(printInstructions);

                        return instructions;
                    }

                    throw new NotImplementedException();

                default:
                    throw new UnreachableException();
            }
        }

        public List<Instruction> VisitMethodDeclaration(EmitMethodDeclaration methodDeclaration)
        {
            if (methodDeclaration.Label.Name == Method_Print.C_Name)//.Modifiers.Any(x => x.Kind == Syntax.SyntaxKind.ExternKeyword))
            {
                return VisitExternMethodDeclaration(methodDeclaration);
            }

            List<Instruction> instructions = new()
            {
                _asmBuilder.Label(new Operand(methodDeclaration.Label.Name))
            };

            foreach (var item in methodDeclaration.Params)
            {
                instructions.AddRange(VisitMethodParameter(item));
            }

            var exitMethodLabel = _asmBuilder.Label(new Operand(methodDeclaration.LabelExit.Name));

            foreach (var item in methodDeclaration.Body.Members)
            {
                instructions.AddRange(VisitDefaultBlock(item));
            }
            
            // 1 to skip the label
            GenerateStackFrameWithExitIfNeeds(ref instructions, (methodDeclaration.UpperStackPosition, methodDeclaration.StackAllocatedBytes), 1, null, exitMethodLabel);

            instructions.Add(_asmBuilder.Ret(new Operand(methodDeclaration.UpperStackPosition - EmitMethodDeclaration.UpperStackInitialPos)));

            return instructions;
        }

        public List<Instruction> VisitMethodParameter(EmitParameterDeclaration parameterDeclaration)
        {
            var instructions = new List<Instruction>(0);

            return instructions;
        }

        public List<Instruction> VisitLocalDeclaration(EmitLocalDeclaration localDeclaration)
        {
            var instructions = new List<Instruction>();

            if (localDeclaration.InitialValue is not null)
            {
                instructions.AddRange(VisitExpression(localDeclaration.InitialValue));
                instructions.Add(_asmBuilder.Pop(Operand.Rax));

                var variable = localDeclaration.GetVariable();
                var loc = GetSizedRbpPosStr(variable.InScopeStackLocation);
                instructions.Add(_asmBuilder.Mov(new Operand(loc), Operand.Rax));
            }

            return instructions;
        }

        public List<Instruction> VisitFieldDeclaration(EmitFieldDeclaration fieldDeclaration)
        {
            var instructions = new List<Instruction>();

            return instructions;
        }

        #endregion Declarations

        #region Statements
        public List<Instruction> VisitReturnStatement(EmitReturnStatement returnStatement)
        {
            var instructions = new List<Instruction>();

            if (returnStatement.ReturnValue is not null)
            {
                instructions.AddRange(VisitExpression(returnStatement.ReturnValue));
                instructions.Add(_asmBuilder.Pop(Operand.Rax));
            }

            var method = returnStatement.GetMethod();

            instructions.Add(_asmBuilder.Jmp(new Operand(method.LabelExit.Name)));

            return instructions;
        }

        public List<Instruction> VisitIfStatement(EmitIfStatement ifStatement)
        {
            var ifEndLabel = new Operand(ifStatement.Condition.JumpIfFalse.Label.Name);
            var ifExitLabel = new Operand(ifStatement.LabelExit.Name);

            var instructions = VisitCondition(ifStatement.Condition);

            foreach (var item in ifStatement.Body.Members)
            {
                instructions.AddRange(VisitDefaultBlock(item));
            }

            if (!ifStatement.IsSingleIf)
            {
                instructions.Add(_asmBuilder.Jmp(ifExitLabel));
            }

            instructions.Add(_asmBuilder.Label(ifEndLabel));

            if (ifStatement.Else is not null)
            {
                instructions.AddRange(VisitElseStatement(ifStatement.Else));
            }

            return instructions;
        }

        public List<Instruction> VisitElseStatement(EmitElseStatement elseStatement)
        {
            var elseInstructions = elseStatement.Else.Kind switch
            {
                EmitKind.BlockStatement => VisitElseBody((EmitBlockStatement<EmitNode, CodeLocal>)elseStatement.Else),
                EmitKind.IfStatement => VisitIfStatementFromElse((EmitIfStatement)elseStatement.Else),
                _ => throw new UnreachableException(),
            };

            return elseInstructions;

            List<Instruction> VisitElseBody(EmitBlockStatement<EmitNode, CodeLocal> elseBody)
            {
                List<Instruction> instructions = new();

                foreach (var item in elseBody.Members)
                {
                    instructions.AddRange(VisitDefaultBlock(item));
                }

                Operand labelExit = new(elseStatement.LabelExit.Name);
                instructions.Add(_asmBuilder.Label(labelExit));

                return instructions;
            }

            List<Instruction> VisitIfStatementFromElse(EmitIfStatement elseIf)
            {
                var instructions = VisitIfStatement(elseIf);

                if (elseIf.Else is null)
                {
                    Operand labelExit = new(elseStatement.LabelExit.Name);
                    instructions.Add(_asmBuilder.Label(labelExit));
                }

                return instructions;
            }
        }

        public List<Instruction> VisitWhileStatement(EmitWhileStatement whileStatement)
        {
            var whileStartLabelOp = new Operand(whileStatement.JumpStart.Label.Name);
            var whileEndLabelOp = new Operand(whileStatement.Condition.JumpIfFalse.Label.Name);

            var instructions = new List<Instruction>
            {
                _asmBuilder.Label(whileStartLabelOp)
            };

            instructions.AddRange(VisitCondition(whileStatement.Condition));
        
            foreach (var item in whileStatement.Body.Members)
            {
                instructions.AddRange(VisitDefaultBlock(item));
            }

            instructions.Add(_asmBuilder.Jmp(whileStartLabelOp));

            instructions.Add(_asmBuilder.Label(whileEndLabelOp));

            return instructions;
        }
        #endregion Statements

        #region Expressions
        public List<Instruction> VisitExpression(EmitExpression expression)
        {
            var expressionInstructions = expression.Kind switch
            {
                EmitKind.LiteralExpression => VisitLiteralExpression((EmitLiteralExpression)expression),
                EmitKind.MemberExpression => VisitMemberExpression((EmitMemberExpression)expression),
                EmitKind.UnaryExpression => VisitUnaryExpression((EmitUnaryExpression)expression),
                EmitKind.CastExpression => VisitCastExpression((EmitCastExpression)expression),
                EmitKind.BinaryExpression => VisitBinaryExpression((EmitBinaryExpression)expression),
                EmitKind.LogicalExpression => VisitLogicalExpression((EmitLogicalExpression)expression),
                EmitKind.IdentifierExpression => VisitIdentifierExpression((EmitIdentifierExpression)expression),
                EmitKind.CallExpression => VisitValuableCallExpression((EmitCallExpression)expression),
                EmitKind.AssignmentExpression => VisitAssignmentExpression((EmitAssignmentExpression)expression),
                EmitKind.NewExpression => VisitNewExpression((EmitNewExpression)expression),
                _ => throw new UnreachableException(),
            };

            return expressionInstructions;
        }

        public List<Instruction> VisitBinaryExpression(EmitBinaryExpression binaryExpression)
        {
            var instructions = VisitExpression(binaryExpression.Left);
            instructions.AddRange(VisitExpression(binaryExpression.Right));

            var rightOperand = Operand.Rbx;
            var leftOperand = Operand.Rax;

            instructions.Add(_asmBuilder.Pop(rightOperand));
            instructions.Add(_asmBuilder.Pop(leftOperand));

            var operation = binaryExpression.Operation;

            Operand resultIsIn;

            switch (operation)
            {
                case BinaryOperatorKind.Plus:
                    instructions.Add(_asmBuilder.Add(leftOperand, rightOperand));
                    resultIsIn = leftOperand;
                    break;
                case BinaryOperatorKind.Minus:
                    instructions.Add(_asmBuilder.Sub(leftOperand, rightOperand));
                    resultIsIn = leftOperand;
                    break;
                case BinaryOperatorKind.Multiply:
                    instructions.Add(_asmBuilder.Imul(leftOperand, rightOperand));
                    resultIsIn = leftOperand;
                    break;
                case BinaryOperatorKind.Divide:
                    instructions.Add(_asmBuilder.Push(Operand.Rdx));


                    instructions.Add(_asmBuilder.Cqo());
                    instructions.Add(_asmBuilder.Idiv(rightOperand));


                    instructions.Add(_asmBuilder.Pop(Operand.Rdx));

                    resultIsIn = Operand.Rax;
                    break;
                case BinaryOperatorKind.Modulus:
                    instructions.Add(_asmBuilder.Push(Operand.Rax));


                    instructions.Add(_asmBuilder.Cqo());
                    instructions.Add(_asmBuilder.Idiv(rightOperand));


                    instructions.Add(_asmBuilder.Pop(Operand.Rax));
                    resultIsIn = Operand.Rdx;
                    break;
                default:
                    throw new UnreachableException();
            }

            instructions.Add(_asmBuilder.Push(resultIsIn));

            return instructions;
        }

        public List<Instruction> VisitReferenceIdentifierExpression(EmitIdentifierExpression identifierExpression)
        {
            var variable = identifierExpression.GetLocal()!;

            var instructions = new List<Instruction>
            {
                _asmBuilder.Push(new Operand(GetRbpPos(variable.InScopeStackLocation))),
            };

            return instructions;
        }

        public List<Instruction> VisitIdentifierExpression(EmitIdentifierExpression identifierExpression)
        {
            var local = identifierExpression.GetLocal()!;
         
            var localInstructions = new List<Instruction>
            {
                _asmBuilder.Mov(Operand.Rax, new Operand(GetRbpPos(local.InScopeStackLocation))),
                _asmBuilder.Push(Operand.Rax),
            };

            return localInstructions;
        }

        public List<Instruction> VisitLogicalExpression(EmitLogicalExpression logicalExpression)
        {
            var instructions = VisitExpression(logicalExpression.Left);
            instructions.AddRange(VisitExpression(logicalExpression.Right));

            var rightOperand = Operand.Rbx;
            var leftOperand = Operand.Rax;

            instructions.Add(_asmBuilder.Pop(rightOperand));
            instructions.Add(_asmBuilder.Pop(leftOperand));

            var operation = logicalExpression.Operation;

            Operand al = new("al");

            switch (operation)
            {
                case LogicalOperatorKind.OrOr:
                    instructions.Add(_asmBuilder.Or(leftOperand, rightOperand));
                    break;
                case LogicalOperatorKind.AndAnd:
                    instructions.Add(_asmBuilder.And(leftOperand, rightOperand));
                    break;

                case LogicalOperatorKind.EqualsEquals:
                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Sete(al));
                    instructions.Add(_asmBuilder.Movzx(leftOperand, al));
                    break;
                case LogicalOperatorKind.NotEquals:
                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Setne(al));
                    instructions.Add(_asmBuilder.Movzx(leftOperand, al));
                    break;

                case LogicalOperatorKind.GreaterThan:
                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Setg(al));
                    instructions.Add(_asmBuilder.Movzx(leftOperand, al));
                    break;
                case LogicalOperatorKind.GreaterThanOrEquals:
                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Setge(al));
                    instructions.Add(_asmBuilder.Movzx(leftOperand, al));
                    break;

                case LogicalOperatorKind.LessThan:
                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Setl(al));
                    instructions.Add(_asmBuilder.Movzx(leftOperand, al));
                    break;
                case LogicalOperatorKind.LessThanOrEquals:
                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Setle(al));
                    instructions.Add(_asmBuilder.Movzx(leftOperand, al));
                    break;

                default:
                    throw new UnreachableException();
            }

            instructions.Add(_asmBuilder.Push(leftOperand));

            return instructions;
        }

        public List<Instruction> VisitJumpLogicalExpression(EmitLogicalExpression logicalExpression, Operand jumpIfFalse)
        {
            var instructions = VisitExpression(logicalExpression.Left);
            instructions.AddRange(VisitExpression(logicalExpression.Right));

            var rightOperand = Operand.Rbx;
            var leftOperand = Operand.Rax;

            instructions.Add(_asmBuilder.Pop(rightOperand));
            instructions.Add(_asmBuilder.Pop(leftOperand));

            var operation = logicalExpression.Operation;

            switch (operation)
            {
                case LogicalOperatorKind.OrOr:
                    instructions.Add(_asmBuilder.Or(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Je(jumpIfFalse));
                    break;
                case LogicalOperatorKind.AndAnd:
                    instructions.Add(_asmBuilder.And(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Je(jumpIfFalse));
                    break;

                case LogicalOperatorKind.EqualsEquals:
                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Jne(jumpIfFalse));
                    break;
                case LogicalOperatorKind.NotEquals:
                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Je(jumpIfFalse));
                    break;

                case LogicalOperatorKind.GreaterThan:
                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Jle(jumpIfFalse));
                    break;
                case LogicalOperatorKind.GreaterThanOrEquals:
                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Jl(jumpIfFalse));
                    break;

                case LogicalOperatorKind.LessThan:
                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Jge(jumpIfFalse));
                    break;
                case LogicalOperatorKind.LessThanOrEquals:
                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
                    instructions.Add(_asmBuilder.Jg(jumpIfFalse));
                    break;

                default:
                    throw new UnreachableException();
            }
            
            return instructions;
        }

        public List<Instruction> VisitLiteralExpression(EmitLiteralExpression literalExpression)
        {
            var instructions = new List<Instruction>();

            var literalValue = literalExpression.Value;

            Operand operand;

            switch (literalValue.Kind)
            {
                case ConstantValueKind.String:
                    var stringValue = literalValue.StringValue!;
                    instructions.AddRange(_asmBuilderAbstractions.String(stringValue));
                    operand = Operand.Rax;
                    break;
                case ConstantValueKind.Char:
                    var valueTxt = $"'{literalValue.CharValue}'";
                    operand = new Operand(valueTxt);
                    break;
                case ConstantValueKind.Int:
                    operand = new Operand(literalValue.IntValue.ToString());
                    break;
                case ConstantValueKind.Long:
                    operand = new Operand(literalValue.LongValue.ToString());
                    break;
                case ConstantValueKind.Bool:
                    var num = literalValue.BoolValue == true ? 1 : 0;
                    operand = new Operand(num);
                    break;

                case ConstantValueKind.Null:
                    operand = new Operand("0");
                    break;
                default:
                    throw new NotImplementedException();
            }

            instructions.Add(_asmBuilder.Push(operand));

            return instructions;
        }       
        
        public List<Instruction> VisitJumpLiteralExpression(EmitLiteralExpression literalExpression, Operand jumpIfFalse)
        {
            var instructions = new List<Instruction>();

            var literalValue = literalExpression.Value;

            switch (literalValue.Kind)
            {
                case ConstantValueKind.Bool:
                    if (literalValue.BoolValue == false)
                    {
                        instructions.Add(_asmBuilder.Jmp(jumpIfFalse));
                    }

                    return instructions;

                default:
                    throw new NotImplementedException();
            }
        }

        public List<Instruction> VisitReferenceMemberExpression(EmitMemberExpression memberExpression)
        {
            var instructions = VisitIdentifierExpression((EmitIdentifierExpression)memberExpression.Left);

            var memberedLeftType = ((MemberedTypeSymbol)memberExpression.Left.Type);

            var pos = -8;
            var right = memberedLeftType.Fields.FirstOrDefault(x => {
                pos += 8;
                return x.Name == memberExpression.Right.Text;
            });

            instructions.Add(_asmBuilder.Pop(Operand.Rax));
            instructions.Add(_asmBuilder.Add(Operand.Rax, new Operand(pos)));
            instructions.Add(_asmBuilder.Push(Operand.Rax));

            return instructions;
        }

        public List<Instruction> VisitMemberExpression(EmitMemberExpression memberExpression)
        {
            var instructions = VisitIdentifierExpression((EmitIdentifierExpression)memberExpression.Left);

            var memberedLeftType = ((MemberedTypeSymbol)memberExpression.Left.Type);

            var pos = -8;
            var right = memberedLeftType.Fields.FirstOrDefault(x => {
                pos += 8;
                return x.Name == memberExpression.Right.Text;
            });

            instructions.Add(_asmBuilder.Pop(Operand.Rax));
            instructions.Add(_asmBuilder.Push(new Operand($"qword [rax + {pos}]")));

            return instructions;
        }

        public List<Instruction> VisitUnaryExpression(EmitUnaryExpression unaryExpression)
        {
            throw new NotImplementedException();
        }

        public List<Instruction> VisitCastExpression(EmitCastExpression castExpression)
        {
            throw new NotImplementedException();
        }

        public List<Instruction> VisitValuableCallExpression(EmitCallExpression callExpression)
        {
            var instructions = VisitCallExpression(callExpression);
            instructions.Add(_asmBuilder.Push(Operand.Rax));
            return instructions;
        }

        public List<Instruction> VisitCallExpression(EmitCallExpression callExpression)
        {
            var instructions = new List<Instruction>();

            for (int i = callExpression.Args.Count - 1; i >= 0; i--)
            {
                var arg = callExpression.Args[i];

                var visited = VisitExpression(arg);
                instructions.AddRange(visited);
            }

            switch (callExpression.Caller.Kind)
            {
                case EmitKind.IdentifierExpression:
                    instructions.Add(_asmBuilder.Call(new Operand(((EmitIdentifierExpression)callExpression.Caller).Text)));
                    break;

                case EmitKind.MemberExpression:
                    var memberExpr = (EmitMemberExpression)callExpression.Caller;
                    instructions.AddRange(VisitExpression(memberExpr.Left));
                    instructions.Add(_asmBuilder.Call(new Operand(memberExpr.Right.Text)));
                    break;
                default:
                    throw new UnreachableException();
            }

            return instructions;
        }

        public List<Instruction> VisitAssignmentExpression(EmitAssignmentExpression assignmentExpression)
        {
            var instructions = new List<Instruction>();

            Func<Operand> getLocation;

            switch (assignmentExpression.Left.Kind)
            {
                case EmitKind.IdentifierExpression:
                    var local = ((EmitIdentifierExpression)assignmentExpression.Left).GetLocal()!;
                    var location = GetSizedRbpPosStr(local.InScopeStackLocation);

                    getLocation = () => new Operand(location);
                    break;
         
                case EmitKind.MemberExpression:
                    instructions.AddRange(VisitReferenceMemberExpression((EmitMemberExpression)assignmentExpression.Left));

                    getLocation = () => {
                        instructions.Add(_asmBuilder.Pop(Operand.Rax));
                        return new Operand($"[{Operand.Rax.Value}]");
                    };
                    break;
 
                default:
                    throw new UnreachableException();
            }

            instructions.AddRange(VisitExpression(assignmentExpression.Right));
            instructions.Add(_asmBuilder.Pop(Operand.Rbx));

            instructions.Add(_asmBuilder.Mov(getLocation(), Operand.Rbx));

            return instructions;
        }

        public List<Instruction> VisitNewExpression(EmitNewExpression newExpression)
        {
            var instructions = new List<Instruction>();

            foreach (var item in newExpression.Initializer.Members)
            {
                instructions.AddRange(VisitDefaultBlock(item));
            }

            instructions.Add(_asmBuilder.Push(Operand.Rax));

            return instructions;
        }

        #endregion Expressions

        public List<Instruction> VisitCondition(EmitCondition condition)
        {
            var jumpFalseLabel = new Operand(condition.JumpIfFalse.Label.Name);

            switch (condition.Condition.Kind)
            {
                case EmitKind.LiteralExpression:
                    return VisitJumpLiteralExpression((EmitLiteralExpression)condition.Condition, jumpFalseLabel);

                case EmitKind.LogicalExpression:
                    return VisitJumpLogicalExpression((EmitLogicalExpression)condition.Condition, jumpFalseLabel);

                default:
                    //slow path
                    var instructions = VisitExpression(condition.Condition);

                    instructions.Add(_asmBuilder.Pop(Operand.Rax));
                    instructions.Add(_asmBuilder.Test(Operand.Rax, Operand.Rax));

                    instructions.Add(_asmBuilder.Je(jumpFalseLabel));

                    return instructions;
            }
        }

        private List<Instruction> VisitDefaultBlock(EmitNode item)
        {
            switch (item.Kind)
            {
                case EmitKind.LocalDeclaration:
                    return VisitLocalDeclaration((EmitLocalDeclaration)item);

                case EmitKind.CallExpression:
                    return VisitCallExpression((EmitCallExpression)item);

                case EmitKind.ReturnStatement:
                    return VisitReturnStatement((EmitReturnStatement)item);

                case EmitKind.IfStatement:
                    return VisitIfStatement((EmitIfStatement)item);

                case EmitKind.WhileStatement:
                    return VisitWhileStatement((EmitWhileStatement)item);

                case EmitKind.AssignmentExpression:
                    return VisitAssignmentExpression((EmitAssignmentExpression)item);

                default:
                    throw new UnreachableException();
            }
        }

        private void GenerateStackFrame(ref List<Instruction> instructions, int bytesAllocated, int initPos, int? endPos, Func<IEnumerable<Instruction>>? beforeLeave = null)
        {
            GenerateStackFrameCustomLeave(ref instructions, bytesAllocated, initPos, endPos, () =>
            {
                var instr = new List<Instruction>();
                if (beforeLeave is not null)
                {
                    instr.AddRange(beforeLeave());
                }
                instr.Add(_asmBuilder.Leave());

                return instr;
            });
        }

        private void GenerateStackFrameCustomLeave(ref List<Instruction> instructions, int bytesAllocated, int initPos, int? endPos, Func<IEnumerable<Instruction>> leave)
        {
            // backwards because the endpos will be affected by the insert
            var endStackFrameInstructions = new List<Instruction>();
            endStackFrameInstructions.AddRange(leave());

            if (endPos.HasValue)
            {
                instructions.InsertRange(endPos.Value, endStackFrameInstructions);
            }
            else
            {
                instructions.AddRange(endStackFrameInstructions);
            }

            instructions.Insert(initPos++, _asmBuilder.Push(Operand.Rbp));
            instructions.Insert(initPos++, _asmBuilder.Mov(Operand.Rbp, Operand.Rsp));

            var needsSub = bytesAllocated != 0;
            if (needsSub)
            {
                instructions.Insert(initPos, _asmBuilder.Sub(Operand.Rsp, new Operand(Math.Abs(bytesAllocated).ToString())));
            }
        }

        private void GenerateStackFrameWithExitIfNeeds(ref List<Instruction> instructions, (int upper, int down) stack, int initPos, int? endPos, Instruction beforeLeave)
        {
            if (stack.upper > EmitMethodDeclaration.UpperStackInitialPos || stack.down != 0)
            {
                GenerateStackFrame(ref instructions, stack.down, initPos, endPos, () =>
                {
                    var intr = new Instruction[]
                    {
                        beforeLeave,
                    };
                    return intr;
                });
            }
            else
            {
                instructions.Add(beforeLeave);
            }
        }

        private static string GetRbpPos(int location)
        {
            var sign = location >= 0 ? '+' : '-';
            var strPos = $"[rbp {sign} {Math.Abs(location)}]";
            return strPos;
        }

        public string GetSizedRbpPosStr(int location)
        {
            var pos = GetRbpPos(location);
            var sizedPos = $"qword {pos}"; // TODO qword is not always the right size
            return sizedPos;
        }
    }
}