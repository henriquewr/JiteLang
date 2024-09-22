using System.Collections.Generic;
using System.Diagnostics;
using JiteLang.Main.Builder.Instructions;
using JiteLang.Main.Builder.AsmBuilder;
using System;
using JiteLang.Main.Builder.Operands;
using System.Linq;
using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Bound.Visitor;
using JiteLang.Main.Bound.Statements.Declaration;
using JiteLang.Main.Bound.Statements;
using JiteLang.Main.Bound.Expressions;
using JiteLang.Main.Bound;
using JiteLang.Main.Builder;
using JiteLang.Main.Shared;

namespace JiteLang.Main.AsmBuilder.Visitor
{
    internal class AsmBuilderVisitor : IBoundVisitor<List<Instruction>,
        List<Instruction>,
        List<Instruction>,
        List<Instruction>,
        List<Instruction>,
        List<Instruction>,
        List<Instruction>,
        List<Instruction>,
        List<Instruction>,
        CodeScope>
    {
        private readonly AssemblyBuilder _asmBuilder;
        private readonly AssemblyBuilderAbstractions _asmBuilderAbstractions;

        private int _labelCount = 0;
        private const string ELSE_PREFIX = "to_else";
        private const string ENDIF_PREFIX = "if_end";

        private const string WHILE_START_PREFIX = "while_start";
        private const string WHILE_END_PREFIX = "while_end";

        private const string EXIT_METHOD_PREFIX = "exit_method";

        private Instruction? _returnTo;

        public AsmBuilderVisitor(AssemblyBuilder asmBuilder, AssemblyBuilderAbstractions asmBuilderAbstractions)
        {
            _asmBuilder = asmBuilder;
            _asmBuilderAbstractions = asmBuilderAbstractions;
        }

        #region Declarations
        public List<Instruction> VisitNamespaceDeclaration(BoundNamespaceDeclaration root, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            foreach (var classDeclaration in root.Body.Members)
            {
                instructions.AddRange(VisitClassDeclaration(classDeclaration, scope));
            }

            instructions.Add(_asmBuilder.Section(new Operand(".text")));
            instructions.Add(_asmBuilder.Global(new Operand("_start")));
            instructions.Add(_asmBuilder.Label(new Operand("_start")));
            instructions.Add(_asmBuilder.Call(new Operand("Main")));

            var code = new Operand("rbx");
            instructions.Add(_asmBuilder.Mov(code, new Operand("rax")));
            instructions.AddRange(_asmBuilderAbstractions.Exit(code));

            return instructions;
        }
 
        public List<Instruction> VisitClassDeclaration(BoundClassDeclaration classDeclaration, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            var newScope = new CodeScope(scope);

            foreach (var item in classDeclaration.Body.Members)
            {
                switch (item.Kind)
                {
                    case BoundKind.ClassDeclaration:
                        instructions.AddRange(VisitClassDeclaration((BoundClassDeclaration)item, newScope));
                        break;
                    case BoundKind.MethodDeclaration:
                        instructions.AddRange(VisitMethodDeclaration((BoundMethodDeclaration)item, newScope));
                        break;
                    case BoundKind.VariableDeclaration:
                        instructions.AddRange(VisitVariableDeclaration((BoundVariableDeclaration)item, newScope));
                        break;
                    default:
                        throw new UnreachableException();
                }
            }

            return instructions;
        }

        public List<Instruction> VisitMethodDeclaration(BoundMethodDeclaration methodDeclaration, CodeScope scope)
        {
            const int UpperInitialPos = 8; //skip return address

            List<Instruction> instructions = new()
            {
                _asmBuilder.Label(new Operand(methodDeclaration.Identifier.Text))
            };

            var allocatedValue = methodDeclaration.Body.Members.Where(x => x.Kind == BoundKind.VariableDeclaration).Count() * 8;

            CodeScope newScope = new(scope, allocatedValue)
            {
                UpperStackPosition = UpperInitialPos
            };

            Dictionary<string, CodeMethodParameter> methodParams = new();
            foreach (var item in methodDeclaration.Params)
            {
                methodParams.Add(item.Identifier.Text, new(item.Type));
                instructions.AddRange(VisitMethodParameter(item, newScope));
            }
            scope.AddMethod(methodDeclaration.Identifier.Text, methodDeclaration.ReturnType, methodParams);

            var upperStack = newScope.UpperStackPosition - UpperInitialPos;
            newScope.HasStackFrame = newScope.BytesAllocated != 0 || upperStack != 0;


            var returnMethodLabel = new Operand(GenerateExitMethodLabel(methodDeclaration.Identifier.Text));

            _returnTo = _asmBuilder.Jmp(returnMethodLabel);
            foreach (var item in methodDeclaration.Body.Members)
            {
                instructions.AddRange(VisitDefaultBlock(item, newScope, out bool isReturn));
            }

            if (newScope.HasStackFrame)
            {
                // 1 to skip the label
                GenerateStackFrame(ref instructions, newScope, 1, null, () =>
                {
                    var intr = new List<Instruction>
                    {
                        _asmBuilder.Label(returnMethodLabel),
                    };
                    return intr;
                });
            }
            else
            {
                instructions.Add(_asmBuilder.Label(returnMethodLabel));
            }
            
            instructions.Add(_asmBuilder.Ret(new Operand(upperStack.ToString())));
            _returnTo = null;
            return instructions;
        }

        public List<Instruction> VisitMethodParameter(BoundParameterDeclaration parameterDeclaration, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            scope.AddVariable(parameterDeclaration.Identifier.Text, parameterDeclaration.Type, true);

            return instructions;
        }

        public List<Instruction> VisitVariableDeclaration(BoundVariableDeclaration variableDeclaration, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            var variable = scope.AddVariable(variableDeclaration.Identifier.Text, variableDeclaration.Type, false);

            if (variableDeclaration.InitialValue is not null)
            {
                instructions.AddRange(VisitExpression(variableDeclaration.InitialValue, scope));
                var rax = new Operand("rax");
                instructions.Add(_asmBuilder.Pop(rax));

                var loc = scope.GetRbpPosStr(variableDeclaration.Identifier.Text);

                instructions.Add(_asmBuilder.Mov(new Operand(loc), rax));
            }

            return instructions;
        }
        #endregion Declarations

        #region Statements
        public List<Instruction> VisitReturnStatement(BoundReturnStatement returnStatement, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            if (returnStatement.ReturnValue is not null)
            {
                instructions.AddRange(VisitExpression(returnStatement.ReturnValue, scope));
            }

            instructions.Add(_asmBuilder.Pop(new Operand("rax")));

            if(_returnTo is null)
            {
                throw new InvalidOperationException("No return defined"); // set in the VisitMethod 
            }

            instructions.Add(_returnTo);

            return instructions;
        }

        public List<Instruction> VisitIfStatement(BoundIfStatement ifStatement, CodeScope scope)
        {
            var instructions = new List<Instruction>();
            instructions.AddRange(VisitExpression(ifStatement.Condition, scope));

            var rax = new Operand("rax");
            instructions.Add(_asmBuilder.Pop(rax));
            instructions.Add(_asmBuilder.Test(rax, rax));

            var toElseLabelOperand = new Operand(GenerateIfLabel(ELSE_PREFIX));
            var endIfLabelOperand = new Operand(GenerateIfLabel(ENDIF_PREFIX));

            instructions.Add(_asmBuilder.Je(toElseLabelOperand));

            int stackFrameInitPos = instructions.Count;
            int? stackFrameEndPos = null;

            var allocatedValue = ifStatement.Body.Members.Where(x => x.Kind == BoundKind.VariableDeclaration).Count() * 8;
            var newScope = new CodeScope(scope, allocatedValue);

            newScope.HasStackFrame = newScope.BytesAllocated != 0;

            foreach (var item in ifStatement.Body.Members)
            {
                instructions.AddRange(VisitDefaultBlock(item, newScope, out bool isReturn));
                if (isReturn)
                {
                    stackFrameEndPos = instructions.Count;
                }
            }

            if (newScope.HasStackFrame)
            {
                GenerateStackFrame(ref instructions, newScope, stackFrameInitPos, stackFrameEndPos, null);
            }

            instructions.Add(_asmBuilder.Jmp(endIfLabelOperand));
            instructions.Add(_asmBuilder.Label(toElseLabelOperand));

            if (ifStatement.Else is not null)
            {
                instructions.AddRange(VisitElseStatement(ifStatement.Else, scope));
            }

            instructions.Add(_asmBuilder.Label(endIfLabelOperand));

            return instructions;
        }

        public List<Instruction> VisitElseStatement(BoundStatement elseStatement, CodeScope scope)
        {
            var newScope = new CodeScope(scope);

            var elseInstructions = elseStatement.Kind switch
            {
                BoundKind.BlockStatement => VisitElseBody((BoundBlockStatement<BoundNode>)elseStatement, newScope),
                BoundKind.IfStatement => VisitIfStatement((BoundIfStatement)elseStatement, newScope),
                _ => throw new UnreachableException(),
            };

            return elseInstructions;

            List<Instruction> VisitElseBody(BoundBlockStatement<BoundNode> elseBody, CodeScope elseScope) 
            {
                var instructions = new List<Instruction>();

                int stackFrameInitPos = instructions.Count;
                int? stackFrameEndPos = null;

                var allocatedValue = elseBody.Members.Where(x => x.Kind == BoundKind.VariableDeclaration).Count() * 8;
                var newElseScope = new CodeScope(elseScope, allocatedValue);

                newElseScope.HasStackFrame = newElseScope.BytesAllocated != 0;

                foreach (var item in elseBody.Members)
                {
                    instructions.AddRange(VisitDefaultBlock(item, newElseScope, out bool isReturn));
                    if (isReturn)
                    {
                        stackFrameEndPos = instructions.Count;
                    }
                }

                if (newElseScope.HasStackFrame)
                {
                    GenerateStackFrame(ref instructions, newElseScope, stackFrameInitPos, stackFrameEndPos, null);
                }

                return instructions;
            }
        }

        public List<Instruction> VisitWhileStatement(BoundWhileStatement whileStatement, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            var whileStartLabelOp = new Operand(GenerateWhileLabel(WHILE_START_PREFIX));
            var whileEndLabelOp = new Operand(GenerateWhileLabel(WHILE_END_PREFIX));

            instructions.Add(_asmBuilder.Label(whileStartLabelOp));


            instructions.AddRange(VisitExpression(whileStatement.Condition, scope));
            var rax = new Operand("rax");
            instructions.Add(_asmBuilder.Pop(rax));
            instructions.Add(_asmBuilder.Test(rax, rax));

            instructions.Add(_asmBuilder.Je(whileEndLabelOp));


            int stackFrameInitPos = instructions.Count;
            int? stackFrameEndPos = null;

            var allocatedValue = whileStatement.Body.Members.Where(x => x.Kind == BoundKind.VariableDeclaration).Count() * 8;
            var newScope = new CodeScope(scope, allocatedValue);
            newScope.HasStackFrame = newScope.BytesAllocated != 0;

            foreach (var item in whileStatement.Body.Members)
            {
                instructions.AddRange(VisitDefaultBlock(item, newScope, out bool isReturn));
                if (isReturn)
                {
                    stackFrameEndPos = instructions.Count;
                }
            }

            if (!stackFrameEndPos.HasValue)
            {
                stackFrameEndPos = instructions.Count;
            }

            instructions.Add(_asmBuilder.Jmp(whileStartLabelOp));

            instructions.Add(_asmBuilder.Label(whileEndLabelOp));

            if (newScope.HasStackFrame)
            {
                GenerateStackFrame(ref instructions, newScope, stackFrameInitPos, stackFrameEndPos, null);
            }

            return instructions;
        }
        #endregion Statements

        #region Expressions
        public List<Instruction> VisitExpression(BoundExpression expression, CodeScope scope)
        {
            var expressionInstructions = expression.Kind switch
            {
                BoundKind.LiteralExpression => VisitLiteralExpression((BoundLiteralExpression)expression, scope),
                BoundKind.MemberExpression => VisitMemberExpression((BoundMemberExpression)expression, scope),
                BoundKind.UnaryExpression => VisitUnaryExpression((BoundUnaryExpression)expression, scope),
                BoundKind.CastExpression => VisitCastExpression((BoundCastExpression)expression, scope),
                BoundKind.BinaryExpression => VisitBinaryExpression((BoundBinaryExpression)expression, scope),
                BoundKind.LogicalExpression => VisitLogicalExpression((BoundLogicalExpression)expression, scope),
                BoundKind.IdentifierExpression => VisitIdentifierExpression((BoundIdentifierExpression)expression, scope),
                BoundKind.CallExpression => VisitCallExpression((BoundCallExpression)expression, scope),
                BoundKind.AssignmentExpression => VisitAssignmentExpression((BoundAssignmentExpression)expression, scope),
                _ => throw new UnreachableException(),
            };

            return expressionInstructions;
        }

        public List<Instruction> VisitBinaryExpression(BoundBinaryExpression binaryExpression, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            instructions.AddRange(VisitExpression(binaryExpression.Left, scope));
            instructions.AddRange(VisitExpression(binaryExpression.Right, scope));

            instructions.Add(_asmBuilder.Pop(new Operand("rbx")));
            instructions.Add(_asmBuilder.Pop(new Operand("rax")));

            var operation = binaryExpression.Operation;

            string resultIsIn;

            switch (operation)
            {
                case BinaryOperatorKind.Plus:
                    instructions.Add(_asmBuilder.Add(new Operand("rax"), new Operand("rbx")));
                    resultIsIn = "rax";
                    break;
                case BinaryOperatorKind.Minus:
                    instructions.Add(_asmBuilder.Sub(new Operand("rax"), new Operand("rbx")));
                    resultIsIn = "rax";
                    break;
                case BinaryOperatorKind.Multiply:
                    instructions.Add(_asmBuilder.Imul(new Operand("rax"), new Operand("rbx")));
                    resultIsIn = "rax";
                    break;
                case BinaryOperatorKind.Divide:
                    instructions.Add(_asmBuilder.Push(new Operand("rdx")));


                    instructions.Add(_asmBuilder.Cqo());
                    instructions.Add(_asmBuilder.Idiv(new Operand("rbx")));


                    instructions.Add(_asmBuilder.Pop(new Operand("rdx")));

                    resultIsIn = "rax";
                    break;
                case BinaryOperatorKind.Modulus:
                    instructions.Add(_asmBuilder.Push(new Operand("rax")));


                    instructions.Add(_asmBuilder.Cqo());
                    instructions.Add(_asmBuilder.Idiv(new Operand("rbx")));


                    instructions.Add(_asmBuilder.Pop(new Operand("rax")));
                    resultIsIn = "rdx";
                    break;
                default:
                    throw new UnreachableException();
            }

            instructions.Add(_asmBuilder.Push(new Operand(resultIsIn)));

            return instructions;
        }
     
        public List<Instruction> VisitIdentifierExpression(BoundIdentifierExpression identifierExpression, CodeScope scope)
        {
            var instructions = new List<Instruction>
            {
                _asmBuilder.Mov(new Operand("rax"), new Operand(scope.GetRbpPosStr(identifierExpression.Text))),
                _asmBuilder.Push(new Operand("rax"))
            };

            return instructions;
        }

        public List<Instruction> VisitLogicalExpression(BoundLogicalExpression logicalExpression, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            instructions.AddRange(VisitExpression(logicalExpression.Left, scope));
            instructions.AddRange(VisitExpression(logicalExpression.Right, scope));

            instructions.Add(_asmBuilder.Pop(new Operand("rbx")));
            instructions.Add(_asmBuilder.Pop(new Operand("rax")));

            var operation = logicalExpression.Operation;

            switch (operation)
            {
                case LogicalOperatorKind.OrOr:
                    instructions.Add(_asmBuilder.Or(new Operand("rax"), new Operand("rbx")));
                    break;
                case LogicalOperatorKind.AndAnd:
                    instructions.Add(_asmBuilder.And(new Operand("rax"), new Operand("rbx")));
                    break;

                case LogicalOperatorKind.EqualsEquals:
                    instructions.Add(_asmBuilder.Cmp(new Operand("rax"), new Operand("rbx")));
                    instructions.Add(_asmBuilder.Sete(new Operand("al")));
                    instructions.Add(_asmBuilder.Movzx(new Operand("rax"), new Operand("al")));
                    break;
                case LogicalOperatorKind.NotEquals:
                    instructions.Add(_asmBuilder.Cmp(new Operand("rax"), new Operand("rbx")));
                    instructions.Add(_asmBuilder.Setne(new Operand("al")));
                    instructions.Add(_asmBuilder.Movzx(new Operand("rax"), new Operand("al")));
                    break;

                case LogicalOperatorKind.GreaterThan:
                    instructions.Add(_asmBuilder.Cmp(new Operand("rax"), new Operand("rbx")));
                    instructions.Add(_asmBuilder.Setg(new Operand("al")));
                    instructions.Add(_asmBuilder.Movzx(new Operand("rax"), new Operand("al")));
                    break;
                case LogicalOperatorKind.GreaterThanOrEquals:
                    instructions.Add(_asmBuilder.Cmp(new Operand("rax"), new Operand("rbx")));
                    instructions.Add(_asmBuilder.Setge(new Operand("al")));
                    instructions.Add(_asmBuilder.Movzx(new Operand("rax"), new Operand("al")));
                    break;

                case LogicalOperatorKind.LessThan:
                    instructions.Add(_asmBuilder.Cmp(new Operand("rax"), new Operand("rbx")));
                    instructions.Add(_asmBuilder.Setl(new Operand("al")));
                    instructions.Add(_asmBuilder.Movzx(new Operand("rax"), new Operand("al")));
                    break;
                case LogicalOperatorKind.LessThanOrEquals:
                    instructions.Add(_asmBuilder.Cmp(new Operand("rax"), new Operand("rbx")));
                    instructions.Add(_asmBuilder.Setle(new Operand("al")));
                    instructions.Add(_asmBuilder.Movzx(new Operand("rax"), new Operand("al")));
                    break;

                default:
                    throw new UnreachableException();
            }

            instructions.Add(_asmBuilder.Push(new Operand("rax")));

            return instructions;
        }

        public List<Instruction> VisitLiteralExpression(BoundLiteralExpression literalExpression, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            var literalValue = literalExpression.Value;

            switch (literalValue.Kind)
            {
                case ConstantValueKind.String:
                    instructions.AddRange(_asmBuilderAbstractions.String(literalValue.StringValue!));
                    instructions.Add(_asmBuilder.Push(new Operand("rax")));
                    break;
                case ConstantValueKind.Char:
                    var valueTxt = $"'{literalValue.CharValue}'";
                    instructions.Add(_asmBuilder.Push(new Operand(valueTxt)));
                    break;
                case ConstantValueKind.Int:
                    instructions.Add(_asmBuilder.Push(new Operand((literalValue.IntValue.ToString()))));
                    break;
                case ConstantValueKind.Long:
                    instructions.Add(_asmBuilder.Push(new Operand((literalValue.LongValue.ToString()))));
                    break;
                case ConstantValueKind.Bool:
                    var num = literalValue.BoolValue == true ? 1 : 0;
                    instructions.Add(_asmBuilder.Push(new Operand(num)));
                    break;
                default:
                    throw new NotImplementedException();
            }

            return instructions;
        }

        public List<Instruction> VisitMemberExpression(BoundMemberExpression memberExpression, CodeScope scope)
        {
            return VisitExpression(memberExpression, scope);
        }

        public List<Instruction> VisitUnaryExpression(BoundUnaryExpression unaryExpression, CodeScope scope)
        {
            return VisitExpression(unaryExpression, scope);
        }

        public List<Instruction> VisitCastExpression(BoundCastExpression castExpression, CodeScope scope)
        {
            return VisitExpression(castExpression.Value, scope);
        }   
        
        public List<Instruction> VisitCallExpression(BoundCallExpression callExpression, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            for (int i = callExpression.Args.Count - 1; i >= 0; i--)
            {
                var arg = callExpression.Args[i];

                var visited = VisitExpression(arg, scope);
                instructions.AddRange(visited);
            }

            instructions.Add(_asmBuilder.Call(new Operand(((BoundIdentifierExpression)callExpression.Caller).Text)));
            instructions.Add(_asmBuilder.Push(new Operand("rax")));

            return instructions;
        }

        public List<Instruction> VisitAssignmentExpression(BoundAssignmentExpression assignmentExpression, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            var location = VisitLocationExpression(assignmentExpression.Left, scope);

            instructions.AddRange(VisitExpression(assignmentExpression.Right, scope));

            instructions.Add(_asmBuilder.Pop(new Operand("rax")));

            instructions.Add(_asmBuilder.Mov(new Operand(location), new Operand("rax")));

            return instructions;
        }
        #endregion Expressions

        private List<Instruction> VisitDefaultBlock(BoundNode item, CodeScope scope, out bool isReturn)
        {
            isReturn = false;

            switch (item.Kind)
            {
                case BoundKind.VariableDeclaration:
                    return VisitVariableDeclaration((BoundVariableDeclaration)item, scope);

                case BoundKind.CallExpression:
                    return VisitCallExpression((BoundCallExpression)item, scope);

                case BoundKind.ReturnStatement:
                    isReturn = true;
                    return VisitReturnStatement((BoundReturnStatement)item, scope);

                case BoundKind.IfStatement:
                    return VisitIfStatement((BoundIfStatement)item, scope);

                case BoundKind.WhileStatement:
                    return VisitWhileStatement((BoundWhileStatement)item, scope);

                case BoundKind.AssignmentExpression:
                    return VisitAssignmentExpression((BoundAssignmentExpression)item, scope);

                default:
                    throw new UnreachableException();
            }
        }

        private static string VisitLocationExpression(BoundExpression expression, CodeScope scope)
        {
            switch (expression.Kind)
            {
                case BoundKind.MemberExpression:
                    return GetMemberLocation((BoundMemberExpression)expression, scope);
                case BoundKind.IdentifierExpression:
                    return GetIdentifierLocation((BoundIdentifierExpression)expression, scope);
                default:
                    throw new UnreachableException();
            }

            static string GetIdentifierLocation(BoundIdentifierExpression identifier, CodeScope scope)
            {
                var location = scope.GetRbpPosStr(identifier.Text);

                return location;
            }

            static string GetMemberLocation(BoundMemberExpression member, CodeScope scope)
            {
                throw new NotImplementedException();
            }
        }

        private void GenerateStackFrame(ref List<Instruction> instructions, CodeScope scope, int initPos, int? endPos, Func<IEnumerable<Instruction>>? beforeLeave = null)
        {
            var rbp = new Operand("rbp");
            var rsp = new Operand("rsp");

            scope.HasStackFrame = true;

            // backwards because the endpos will be affected by the insert
            var endStackFrameInstructions = new List<Instruction>();
            if (beforeLeave is not null)
            {
                endStackFrameInstructions.AddRange(beforeLeave());
            }
            endStackFrameInstructions.Add(_asmBuilder.Leave());

            if (endPos.HasValue)
            {
                instructions.InsertRange(endPos.Value, endStackFrameInstructions);
            }
            else
            {
                instructions.AddRange(endStackFrameInstructions);
            }


            instructions.Insert(initPos++, _asmBuilder.Push(rbp));
            instructions.Insert(initPos++, _asmBuilder.Mov(rbp, rsp));

            var needsSub = scope.BytesAllocated != 0;
            if (needsSub)
            {
                instructions.Insert(initPos, _asmBuilder.Sub(rsp, new Operand(Math.Abs(scope.BytesAllocated).ToString())));
            }
        }

        private string GenerateIfLabel(string prefix)
        {
            return $"{prefix}_I_L_{_labelCount++}";
        }  
        
        private string GenerateWhileLabel(string prefix)
        {
            return $"{prefix}_W_L_{_labelCount++}";
        }

        private string GenerateExitMethodLabel(string methodName)
        {
            return $"{EXIT_METHOD_PREFIX}_{methodName}_L_{_labelCount++}";
        }
    }
}
