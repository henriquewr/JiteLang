﻿//using System.Collections.Generic;
//using System.Diagnostics;
//using System;
//using System.Linq;
//using JiteLang.Main.AsmBuilder.Scope;
//using JiteLang.Main.Bound.Visitor;
//using JiteLang.Main.Bound.Statements.Declaration;
//using JiteLang.Main.Bound.Statements;
//using JiteLang.Main.Bound.Expressions;
//using JiteLang.Main.Bound;
//using JiteLang.Main.Shared;
//using System.Collections.Frozen;
//using JiteLang.Main.AsmBuilder.Builder;
//using JiteLang.Main.AsmBuilder.Instructions;
//using JiteLang.Main.PredefinedExternMethods;
//using JiteLang.Main.Emit.AsmBuilder.Builder.Abstractions;
//using JiteLang.Main.Emit.AsmBuilder.Operands;

//namespace JiteLang.Main.AsmBuilder.Visitor
//{
//    internal class AsmBuilderVisitorOLD : IBoundVisitor<List<Instruction>,
//        List<Instruction>,
//        List<Instruction>,
//        List<Instruction>,
//        List<Instruction>,
//        List<Instruction>,
//        List<Instruction>,
//        List<Instruction>,
//        List<Instruction>,
//        CodeScope>
//    {

//        private readonly AssemblyBuilder _asmBuilder;
//        private readonly IAssemblyBuilderAbstractions _asmBuilderAbstractions;

//        private int _labelCount = 0;
//        private const string ELSE_PREFIX = "to_else";
//        private const string ENDIF_PREFIX = "if_end";

//        private const string WHILE_START_PREFIX = "while_start";
//        private const string WHILE_END_PREFIX = "while_end";

//        private const string EXIT_METHOD_PREFIX = "exit_method";

//        private Instruction? _returnTo;

//        private const int MethodUpperStackInitialPos = 8; //skip return address

//        public AsmBuilderVisitorOLD(AssemblyBuilder asmBuilder, IAssemblyBuilderAbstractions asmBuilderAbstractions)
//        {
//            _asmBuilder = asmBuilder;
//            _asmBuilderAbstractions = asmBuilderAbstractions;
//        }

//        #region Declarations
//        public List<Instruction> VisitNamespaceDeclaration(BoundNamespaceDeclaration root, CodeScope scope)
//        {
//            var instructions = new List<Instruction>();

//            foreach (var classDeclaration in root.Body.Members)
//            {
//                instructions.AddRange(VisitClassDeclaration(classDeclaration, scope));
//            }

//            instructions.Add(_asmBuilder.Section(new Operand(".text")));
//            instructions.Add(_asmBuilder.Global(new Operand("_start")));
//            instructions.Add(_asmBuilder.Label(new Operand("_start")));
//            instructions.Add(_asmBuilder.Call(new Operand("Main")));

//            var code = Operand.Rbx;
//            instructions.Add(_asmBuilder.Mov(code, Operand.Rax));
//            instructions.AddRange(_asmBuilderAbstractions.Exit(code));

//            return instructions;
//        }
        
//        public List<Instruction> VisitClassDeclaration(BoundClassDeclaration classDeclaration, CodeScope scope)
//        {
//            var instructions = new List<Instruction>();

//            var newScope = new CodeScope(scope);

//            var methods = classDeclaration.Body.Members.Where(x => x.Kind == BoundKind.MethodDeclaration)
//                .Cast<BoundMethodDeclaration>().ToFrozenDictionary(k => k.Identifier.Text, v => CreateMethodScope(v, scope));

//            foreach (var item in classDeclaration.Body.Members)
//            {
//                switch (item.Kind)
//                {
//                    case BoundKind.VariableDeclaration:
//                        instructions.AddRange(VisitVariableDeclaration((BoundVariableDeclaration)item, newScope));
//                        break;
//                    case BoundKind.MethodDeclaration:
//                        var method = (BoundMethodDeclaration)item;
//                        var methodScope = methods[method.Identifier.Text];

//                        instructions.AddRange(VisitMethodDeclaration(method, methodScope));
//                        break;
//                    case BoundKind.ClassDeclaration:
//                        instructions.AddRange(VisitClassDeclaration((BoundClassDeclaration)item, newScope));
//                        break;
//                    default:
//                        throw new UnreachableException();
//                }
//            }

//            return instructions;

//            static CodeScope CreateMethodScope(BoundMethodDeclaration methodDeclaration, CodeScope scope) // make the methods scopeless
//            {
//                var allocatedValue = methodDeclaration.Body.Members.Where(x => x.Kind == BoundKind.VariableDeclaration).Count() * 8;

//                CodeScope newScope = new(scope, allocatedValue)
//                {
//                    UpperStackPosition = MethodUpperStackInitialPos
//                };

//                Dictionary<string, CodeMethodParameter> methodParams = new();
//                foreach (var item in methodDeclaration.Params)
//                {
//                    methodParams.Add(item.Identifier.Text, new(item.Type));
//                }
//                scope.AddMethod(methodDeclaration.Identifier.Text, methodDeclaration.ReturnType, methodParams);

//                return newScope;
//            }
//        }

//        private List<Instruction> VisitExternMethodDeclaration(BoundMethodDeclaration methodDeclaration, CodeScope newScope)
//        {
//            //the method scope is created in the visit class declaration to make it scopeless
//            var instructions = new List<Instruction>();

//            switch (methodDeclaration.Identifier.Text)
//            {
//                case Method_Print.C_Name:

//                    foreach (var item in methodDeclaration.Params)
//                    {
//                        instructions.AddRange(VisitMethodParameter(item, newScope));
//                    }

//                    var strVar = newScope.Variables.First();

//                    var pointerLocation = newScope.GetSizedRbpPosStr(strVar.Key);
//                    newScope.GetVariable(strVar.Key, out int offset);

//                    var lengthOperand = new Operand($"[rsi - {8}]");

//                    var printInstructions = Method_Print.GenerateInstructions(_asmBuilder, new Operand(pointerLocation), lengthOperand, false);
//                    instructions.AddRange(printInstructions);

//                    return instructions;

//                default:
//                    throw new UnreachableException();
//            }
//        }

//        public List<Instruction> VisitMethodDeclaration(BoundMethodDeclaration methodDeclaration, CodeScope newScope)
//        {
//            //the method scope is created in the visit class declaration to make it scopeless

//            if (methodDeclaration.Modifiers.Any(x => x.Kind == Syntax.SyntaxKind.ExternKeyword))
//            {
//                return VisitExternMethodDeclaration(methodDeclaration, newScope);
//            }

//            List<Instruction> instructions = new()
//            {
//                _asmBuilder.Label(new Operand(methodDeclaration.Identifier.Text))
//            };

//            foreach (var item in methodDeclaration.Params)
//            {
//                instructions.AddRange(VisitMethodParameter(item, newScope));
//            }

//            var upperStack = newScope.UpperStackPosition - MethodUpperStackInitialPos;
//            newScope.HasStackFrame = newScope.BytesAllocated != 0 || upperStack != 0;

//            var returnMethodLabel = new Operand(GenerateExitMethodLabel(methodDeclaration.Identifier.Text));

//            UsingReturn(_asmBuilder.Jmp(returnMethodLabel), () =>
//            {
//                foreach (var item in methodDeclaration.Body.Members)
//                {
//                    instructions.AddRange(VisitDefaultBlock(item, newScope, out bool isReturn));
//                }
//            });

//            // 1 to skip the label
//            GenerateStackFrameWithExitIfNeeds(ref instructions, newScope, 1, null, _asmBuilder.Label(returnMethodLabel));

//            instructions.Add(_asmBuilder.Ret(new Operand(upperStack)));

//            return instructions;
//        }

//        public List<Instruction> VisitMethodParameter(BoundParameterDeclaration parameterDeclaration, CodeScope scope)
//        {
//            var instructions = new List<Instruction>();

//            scope.AddVariable(parameterDeclaration.Identifier.Text, parameterDeclaration.Type, true);

//            return instructions;
//        }

//        public List<Instruction> VisitVariableDeclaration(BoundVariableDeclaration variableDeclaration, CodeScope scope)
//        {
//            var instructions = new List<Instruction>();

//            var variable = scope.AddVariable(variableDeclaration.Identifier.Text, variableDeclaration.Type, false);

//            if (variableDeclaration.InitialValue is not null)
//            {
//                instructions.AddRange(VisitExpression(variableDeclaration.InitialValue, scope));
//                instructions.Add(_asmBuilder.Pop(Operand.Rax));

//                var loc = scope.GetRbpPosStr(variableDeclaration.Identifier.Text);

//                instructions.Add(_asmBuilder.Mov(new Operand(loc), Operand.Rax));
//            }

//            return instructions;
//        }
//        #endregion Declarations

//        #region Statements
//        public List<Instruction> VisitReturnStatement(BoundReturnStatement returnStatement, CodeScope scope)
//        {
//            var instructions = new List<Instruction>();

//            if (returnStatement.ReturnValue is not null)
//            {
//                instructions.AddRange(VisitExpression(returnStatement.ReturnValue, scope));
//            }

//            instructions.Add(_asmBuilder.Pop(Operand.Rax));

//            if (_returnTo is null)
//            {
//                throw new InvalidOperationException("No return defined");
//            }

//            instructions.Add(_returnTo);
            
//            return instructions;
//        }

//        public List<Instruction> VisitIfStatement(BoundIfStatement ifStatement, CodeScope scope)
//        {
//            var instructions = new List<Instruction>();

//            instructions.AddRange(VisitExpression(ifStatement.Condition, scope));
//            instructions.Add(_asmBuilder.Pop(Operand.Rax));
//            instructions.Add(_asmBuilder.Test(Operand.Rax, Operand.Rax));

//            var toElseLabelOperand = new Operand(GenerateIfLabel(ELSE_PREFIX));
//            var endIfLabelOperand = new Operand(GenerateIfLabel(ENDIF_PREFIX));

//            instructions.Add(_asmBuilder.Je(toElseLabelOperand));

//            int stackFrameInitPos = instructions.Count;
//            int? stackFrameEndPos = null;

//            var allocatedValue = ifStatement.Body.Members.Where(x => x.Kind == BoundKind.VariableDeclaration).Count() * 8;
//            var newScope = new CodeScope(scope, allocatedValue);

//            newScope.HasStackFrame = newScope.BytesAllocated != 0;

//            foreach (var item in ifStatement.Body.Members)
//            {
//                instructions.AddRange(VisitDefaultBlock(item, newScope, out bool isReturn));
//                if (isReturn)
//                {
//                    stackFrameEndPos = instructions.Count;
//                }
//            }

//            if (newScope.HasStackFrame)
//            {
//                GenerateStackFrame(ref instructions, newScope, stackFrameInitPos, stackFrameEndPos, null);
//            }

//            instructions.Add(_asmBuilder.Jmp(endIfLabelOperand));
//            instructions.Add(_asmBuilder.Label(toElseLabelOperand));

//            if (ifStatement.Else is not null)
//            {
//                instructions.AddRange(VisitElseStatement(ifStatement.Else, scope));
//            }

//            instructions.Add(_asmBuilder.Label(endIfLabelOperand));

//            return instructions;
//        }

//        public List<Instruction> VisitElseStatement(BoundElseStatement elseStatement, CodeScope scope)
//        {
//            var newScope = new CodeScope(scope);

//            var elseInstructions = elseStatement.Else.Kind switch
//            {
//                BoundKind.BlockStatement => VisitElseBody((BoundBlockStatement<BoundNode>)elseStatement.Else, newScope),
//                BoundKind.IfStatement => VisitIfStatement((BoundIfStatement)elseStatement.Else, newScope),
//                _ => throw new UnreachableException(),
//            };

//            return elseInstructions;

//            List<Instruction> VisitElseBody(BoundBlockStatement<BoundNode> elseBody, CodeScope elseScope) 
//            {
//                var instructions = new List<Instruction>();

//                int stackFrameInitPos = instructions.Count;
//                int? stackFrameEndPos = null;

//                var allocatedValue = elseBody.Members.Where(x => x.Kind == BoundKind.VariableDeclaration).Count() * 8;
//                var newElseScope = new CodeScope(elseScope, allocatedValue);

//                newElseScope.HasStackFrame = newElseScope.BytesAllocated != 0;

//                foreach (var item in elseBody.Members)
//                {
//                    instructions.AddRange(VisitDefaultBlock(item, newElseScope, out bool isReturn));
//                    if (isReturn)
//                    {
//                        stackFrameEndPos = instructions.Count;
//                    }
//                }

//                if (newElseScope.HasStackFrame)
//                {
//                    GenerateStackFrame(ref instructions, newElseScope, stackFrameInitPos, stackFrameEndPos, null);
//                }

//                return instructions;
//            }
//        }

//        public List<Instruction> VisitWhileStatement(BoundWhileStatement whileStatement, CodeScope scope)
//        {
//            var whileStartLabelOp = new Operand(GenerateWhileLabel("start"));
//            var whileEndLabelOp = new Operand(GenerateWhileLabel("end"));
//            var whileReturnLabelOp = new Operand(GenerateWhileLabel("ret"));

//            var instructions = new List<Instruction>
//            {
//                _asmBuilder.Label(whileStartLabelOp)
//            };
//            instructions.AddRange(VisitExpression(whileStatement.Condition, scope));
//            instructions.Add(_asmBuilder.Pop(Operand.Rax));
//            instructions.Add(_asmBuilder.Test(Operand.Rax, Operand.Rax));

//            instructions.Add(_asmBuilder.Je(whileEndLabelOp));

//            int stackFrameInitPos = instructions.Count;
//            int? stackFrameEndPos = null;

//            var allocatedValue = whileStatement.Body.Members.Where(x => x.Kind == BoundKind.VariableDeclaration).Count() * 8;
//            var newScope = new CodeScope(scope, allocatedValue);
//            newScope.HasStackFrame = newScope.BytesAllocated != 0;

//            bool hasReturn = false;
//            UsingReturn(_asmBuilder.Jmp(whileReturnLabelOp), () =>
//            {
//                foreach (var item in whileStatement.Body.Members)
//                {
//                    instructions.AddRange(VisitDefaultBlock(item, newScope, out hasReturn));
//                    if (hasReturn)
//                    {
//                        stackFrameEndPos = instructions.Count;
//                    }
//                }
//            });

//            if (!stackFrameEndPos.HasValue)
//            {
//                stackFrameEndPos = instructions.Count;
//            }

//            instructions.Add(_asmBuilder.Jmp(whileStartLabelOp));

//            instructions.Add(_asmBuilder.Label(whileEndLabelOp));

//            if (newScope.HasStackFrame)
//            {
//                GenerateStackFrameCustomLeave(ref instructions, newScope, stackFrameInitPos, stackFrameEndPos, () => {
//                    if (_returnTo is null)
//                    {
//                        throw new InvalidOperationException("No return defined");
//                    }

//                    var instr = new List<Instruction>
//                    {
//                        _asmBuilder.Label(whileReturnLabelOp),
//                        _asmBuilder.Leave(),
//                        _returnTo
//                    };
//                    return instr;
//                });
//            }

//            return instructions;
//        }
//        #endregion Statements

//        #region Expressions
//        public List<Instruction> VisitExpression(BoundExpression expression, CodeScope scope)
//        {
//            var expressionInstructions = expression.Kind switch
//            {
//                BoundKind.LiteralExpression => VisitLiteralExpression((BoundLiteralExpression)expression, scope),
//                BoundKind.MemberExpression => VisitMemberExpression((BoundMemberExpression)expression, scope),
//                BoundKind.UnaryExpression => VisitUnaryExpression((BoundUnaryExpression)expression, scope),
//                BoundKind.CastExpression => VisitCastExpression((BoundCastExpression)expression, scope),
//                BoundKind.BinaryExpression => VisitBinaryExpression((BoundBinaryExpression)expression, scope),
//                BoundKind.LogicalExpression => VisitLogicalExpression((BoundLogicalExpression)expression, scope),
//                BoundKind.IdentifierExpression => VisitIdentifierExpression((BoundIdentifierExpression)expression, scope),
//                BoundKind.CallExpression => VisitCallExpression((BoundCallExpression)expression, scope),
//                BoundKind.AssignmentExpression => VisitAssignmentExpression((BoundAssignmentExpression)expression, scope),
//                _ => throw new UnreachableException(),
//            };

//            return expressionInstructions;
//        }

//        public List<Instruction> VisitBinaryExpression(BoundBinaryExpression binaryExpression, CodeScope scope)
//        {
//            var instructions = new List<Instruction>();

//            instructions.AddRange(VisitExpression(binaryExpression.Left, scope));
//            instructions.AddRange(VisitExpression(binaryExpression.Right, scope));

//            var rightOperand = Operand.Rbx;
//            var leftOperand = Operand.Rax;

//            instructions.Add(_asmBuilder.Pop(rightOperand));
//            instructions.Add(_asmBuilder.Pop(leftOperand));

//            var operation = binaryExpression.Operation;

//            Operand resultIsIn;

//            switch (operation)
//            {
//                case BinaryOperatorKind.Plus:
//                    instructions.Add(_asmBuilder.Add(leftOperand, rightOperand));
//                    resultIsIn = leftOperand;
//                    break;
//                case BinaryOperatorKind.Minus:
//                    instructions.Add(_asmBuilder.Sub(leftOperand, rightOperand));
//                    resultIsIn = leftOperand;
//                    break;
//                case BinaryOperatorKind.Multiply:
//                    instructions.Add(_asmBuilder.Imul(leftOperand, rightOperand));
//                    resultIsIn = leftOperand;
//                    break;
//                case BinaryOperatorKind.Divide:
//                    instructions.Add(_asmBuilder.Push(Operand.Rdx));


//                    instructions.Add(_asmBuilder.Cqo());
//                    instructions.Add(_asmBuilder.Idiv(rightOperand));


//                    instructions.Add(_asmBuilder.Pop(Operand.Rdx));

//                    resultIsIn = Operand.Rax;
//                    break;
//                case BinaryOperatorKind.Modulus:
//                    instructions.Add(_asmBuilder.Push(Operand.Rax));


//                    instructions.Add(_asmBuilder.Cqo());
//                    instructions.Add(_asmBuilder.Idiv(rightOperand));


//                    instructions.Add(_asmBuilder.Pop(Operand.Rax));
//                    resultIsIn = Operand.Rdx;
//                    break;
//                default:
//                    throw new UnreachableException();
//            }

//            instructions.Add(_asmBuilder.Push(resultIsIn));

//            return instructions;
//        }
     
//        public List<Instruction> VisitIdentifierExpression(BoundIdentifierExpression identifierExpression, CodeScope scope)
//        {
//            var variable = scope.GetVariable(identifierExpression.Text);

//            var instructions = new List<Instruction>
//            {
//                _asmBuilder.Mov(Operand.Rax, new Operand(scope.GetRbpPosStr(identifierExpression.Text))),
//                _asmBuilder.Push(Operand.Rax),
//            };

//            return instructions;
//        }

//        public List<Instruction> VisitLogicalExpression(BoundLogicalExpression logicalExpression, CodeScope scope)
//        {
//            var instructions = new List<Instruction>();

//            instructions.AddRange(VisitExpression(logicalExpression.Left, scope));
//            instructions.AddRange(VisitExpression(logicalExpression.Right, scope));

//            var rightOperand = Operand.Rbx;
//            var leftOperand = Operand.Rax;

//            instructions.Add(_asmBuilder.Pop(rightOperand));
//            instructions.Add(_asmBuilder.Pop(leftOperand));

//            var operation = logicalExpression.Operation;

//            switch (operation)
//            {
//                case LogicalOperatorKind.OrOr:
//                    instructions.Add(_asmBuilder.Or(leftOperand, rightOperand));
//                    break;
//                case LogicalOperatorKind.AndAnd:
//                    instructions.Add(_asmBuilder.And(leftOperand, rightOperand));
//                    break;

//                case LogicalOperatorKind.EqualsEquals:
//                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
//                    instructions.Add(_asmBuilder.Sete(new Operand("al")));
//                    instructions.Add(_asmBuilder.Movzx(leftOperand, new Operand("al")));
//                    break;
//                case LogicalOperatorKind.NotEquals:
//                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
//                    instructions.Add(_asmBuilder.Setne(new Operand("al")));
//                    instructions.Add(_asmBuilder.Movzx(leftOperand, new Operand("al")));
//                    break;

//                case LogicalOperatorKind.GreaterThan:
//                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
//                    instructions.Add(_asmBuilder.Setg(new Operand("al")));
//                    instructions.Add(_asmBuilder.Movzx(leftOperand, new Operand("al")));
//                    break;
//                case LogicalOperatorKind.GreaterThanOrEquals:
//                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
//                    instructions.Add(_asmBuilder.Setge(new Operand("al")));
//                    instructions.Add(_asmBuilder.Movzx(leftOperand, new Operand("al")));
//                    break;

//                case LogicalOperatorKind.LessThan:
//                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
//                    instructions.Add(_asmBuilder.Setl(new Operand("al")));
//                    instructions.Add(_asmBuilder.Movzx(leftOperand, new Operand("al")));
//                    break;
//                case LogicalOperatorKind.LessThanOrEquals:
//                    instructions.Add(_asmBuilder.Cmp(leftOperand, rightOperand));
//                    instructions.Add(_asmBuilder.Setle(new Operand("al")));
//                    instructions.Add(_asmBuilder.Movzx(leftOperand, new Operand("al")));
//                    break;

//                default:
//                    throw new UnreachableException();
//            }

//            instructions.Add(_asmBuilder.Push(leftOperand));

//            return instructions;
//        }

//        public List<Instruction> VisitLiteralExpression(BoundLiteralExpression literalExpression, CodeScope scope)
//        {
//            var instructions = new List<Instruction>();

//            var literalValue = literalExpression.Value;

//            Operand operand;

//            switch (literalValue.Kind)
//            {
//                case ConstantValueKind.String:
//                    var stringValue = literalValue.StringValue!;
//                    instructions.AddRange(_asmBuilderAbstractions.String(stringValue));
//                    operand = Operand.Rax;
//                    break;
//                case ConstantValueKind.Char:
//                    var valueTxt = $"'{literalValue.CharValue}'";
//                    operand = new Operand(valueTxt);
//                    break;
//                case ConstantValueKind.Int:
//                    operand = new Operand(literalValue.IntValue.ToString());
//                    break;
//                case ConstantValueKind.Long:
//                    operand = new Operand(literalValue.LongValue.ToString());
//                    break;
//                case ConstantValueKind.Bool:
//                    var num = literalValue.BoolValue == true ? 1 : 0;
//                    operand = new Operand(num);
//                    break;
//                default:
//                    throw new NotImplementedException();
//            }

//            instructions.Add(_asmBuilder.Push(operand));

//            return instructions;
//        }

//        public List<Instruction> VisitMemberExpression(BoundMemberExpression memberExpression, CodeScope scope)
//        {
//            return VisitExpression(memberExpression, scope);
//        }

//        public List<Instruction> VisitUnaryExpression(BoundUnaryExpression unaryExpression, CodeScope scope)
//        {
//            return VisitExpression(unaryExpression, scope);
//        }

//        public List<Instruction> VisitCastExpression(BoundCastExpression castExpression, CodeScope scope)
//        {
//            return VisitExpression(castExpression.Value, scope);
//        }   
        
//        public List<Instruction> VisitCallExpression(BoundCallExpression callExpression, CodeScope scope)
//        {
//            var instructions = new List<Instruction>();

//            for (int i = callExpression.Args.Count - 1; i >= 0; i--)
//            {
//                var arg = callExpression.Args[i];

//                var visited = VisitExpression(arg, scope);
//                instructions.AddRange(visited);
//            }

//            instructions.Add(_asmBuilder.Call(new Operand(((BoundIdentifierExpression)callExpression.Caller).Text)));

//            return instructions;
//        }

//        public List<Instruction> VisitAssignmentExpression(BoundAssignmentExpression assignmentExpression, CodeScope scope)
//        {
//            var instructions = new List<Instruction>();

//            var location = VisitLocationExpression(assignmentExpression.Left, scope);

//            instructions.AddRange(VisitExpression(assignmentExpression.Right, scope));
//            instructions.Add(_asmBuilder.Pop(Operand.Rax));

//            var locationOperand = new Operand(location);
//            instructions.Add(_asmBuilder.Mov(locationOperand, Operand.Rax));

//            return instructions;
//        }
//        #endregion Expressions

//        private List<Instruction> VisitDefaultBlock(BoundNode item, CodeScope scope, out bool isReturn)
//        {
//            isReturn = false;

//            switch (item.Kind)
//            {
//                case BoundKind.VariableDeclaration:
//                    return VisitVariableDeclaration((BoundVariableDeclaration)item, scope);

//                case BoundKind.CallExpression:
//                    return VisitCallExpression((BoundCallExpression)item, scope);

//                case BoundKind.ReturnStatement:
//                    isReturn = true;
//                    return VisitReturnStatement((BoundReturnStatement)item, scope);

//                case BoundKind.IfStatement:
//                    return VisitIfStatement((BoundIfStatement)item, scope);

//                case BoundKind.WhileStatement:
//                    return VisitWhileStatement((BoundWhileStatement)item, scope);

//                case BoundKind.AssignmentExpression:
//                    return VisitAssignmentExpression((BoundAssignmentExpression)item, scope);

//                default:
//                    throw new UnreachableException();
//            }
//        }

//        private static string VisitLocationExpression(BoundExpression expression, CodeScope scope)
//        {
//            switch (expression.Kind)
//            {
//                case BoundKind.MemberExpression:
//                    return GetMemberLocation((BoundMemberExpression)expression, scope);
//                case BoundKind.IdentifierExpression:
//                    return GetIdentifierLocation((BoundIdentifierExpression)expression, scope);
//                default:
//                    throw new UnreachableException();
//            }

//            static string GetIdentifierLocation(BoundIdentifierExpression identifier, CodeScope scope)
//            {
//                var location = scope.GetSizedRbpPosStr(identifier.Text);

//                return location;
//            }

//            static string GetMemberLocation(BoundMemberExpression member, CodeScope scope)
//            {
//                throw new NotImplementedException();
//            }
//        }

//        private void UsingReturn(Instruction ret, Action action)
//        {
//            var lastRet = _returnTo;

//            _returnTo = ret;

//            action();

//            _returnTo = lastRet;
//        }

//        private void GenerateStackFrame(ref List<Instruction> instructions, CodeScope scope, int initPos, int? endPos, Func<IEnumerable<Instruction>>? beforeLeave = null)
//        {
//            GenerateStackFrameCustomLeave(ref instructions, scope, initPos, endPos, () =>
//            {
//                var instr = new List<Instruction>();
//                if (beforeLeave is not null)
//                {
//                    instr.AddRange(beforeLeave());
//                }
//                instr.Add(_asmBuilder.Leave());

//                return instr;
//            });
//        }

//        private void GenerateStackFrameCustomLeave(ref List<Instruction> instructions, CodeScope scope, int initPos, int? endPos, Func<IEnumerable<Instruction>> leave)
//        {
//            scope.HasStackFrame = true;

//            // backwards because the endpos will be affected by the insert
//            var endStackFrameInstructions = new List<Instruction>();
//            endStackFrameInstructions.AddRange(leave());

//            if (endPos.HasValue)
//            {
//                instructions.InsertRange(endPos.Value, endStackFrameInstructions);
//            }
//            else
//            {
//                instructions.AddRange(endStackFrameInstructions);
//            }

//            instructions.Insert(initPos++, _asmBuilder.Push(Operand.Rbp));
//            instructions.Insert(initPos++, _asmBuilder.Mov(Operand.Rbp, Operand.Rsp));

//            var needsSub = scope.BytesAllocated != 0;
//            if (needsSub)
//            {
//                instructions.Insert(initPos, _asmBuilder.Sub(Operand.Rsp, new Operand(Math.Abs(scope.BytesAllocated).ToString())));
//            }
//        }

//        private void GenerateStackFrameWithExitIfNeeds(ref List<Instruction> instructions, CodeScope scope, int initPos, int? endPos, Instruction beforeLeave)
//        {
//            if (scope.HasStackFrame)
//            {
//                GenerateStackFrame(ref instructions, scope, initPos, endPos, () =>
//                {
//                    var intr = new Instruction[]
//                    {
//                        beforeLeave,
//                    };
//                    return intr;
//                });
//            }
//            else
//            {
//                instructions.Add(beforeLeave);
//            }
//        }

//        private string GenerateIfLabel(string prefix)
//        {
//            return $"{prefix}_I_L_{_labelCount++}";
//        }

//        private string GenerateWhileLabel(string prefix)
//        {
//            return $"{prefix}_W_L_{_labelCount++}";
//        }

//        private string GenerateExitMethodLabel(string methodName)
//        {
//            return $"{EXIT_METHOD_PREFIX}_{methodName}_L_{_labelCount++}";
//        }
//    }
//}
