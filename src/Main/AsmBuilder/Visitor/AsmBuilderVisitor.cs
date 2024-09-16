using System.Collections.Generic;
using System.Diagnostics;
using JiteLang.Main.Builder.Instructions;
using JiteLang.Main.Builder.AsmBuilder;
using System;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Syntax;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Main.Builder.Operands;
using JiteLang.Main.LangParser.SyntaxNodes;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using System.Linq;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Main.Shared;
using JiteLang.Main.Visitor.Syntax;
using JiteLang.Main.AsmBuilder.Scope;

namespace JiteLang.Main.AsmBuilder.Visitor
{
    internal class AsmBuilderVisitor : ISyntaxVisitor<List<Instruction>,
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

        private readonly Stack<Instruction> _returnTo = new();

        public AsmBuilderVisitor(AssemblyBuilder asmBuilder, AssemblyBuilderAbstractions asmBuilderAbstractions)
        {
            _asmBuilder = asmBuilder;
            _asmBuilderAbstractions = asmBuilderAbstractions;
        }
        #region Declarations
        public List<Instruction> VisitNamespaceDeclaration(NamespaceDeclarationSyntax root, CodeScope scope)
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
 
        public List<Instruction> VisitClassDeclaration(ClassDeclarationSyntax classDeclaration, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            var newScope = new CodeScope(scope);

            foreach (var item in classDeclaration.Body.Members)
            {
                switch (item.Kind)
                {
                    case SyntaxKind.ClassDeclaration:
                        instructions.AddRange(VisitClassDeclaration((ClassDeclarationSyntax)item, newScope));
                        break;
                    case SyntaxKind.MethodDeclaration:
                        instructions.AddRange(VisitMethodDeclaration((MethodDeclarationSyntax)item, newScope));
                        break;
                    case SyntaxKind.VariableDeclaration:
                        instructions.AddRange(VisitVariableDeclaration((VariableDeclarationSyntax)item, newScope));
                        break;
                    default:
                        throw new UnreachableException();
                }
            }

            return instructions;
        }

        public List<Instruction> VisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, CodeScope scope)
        {
            const int UpperInitialPos = 8; //skip return address

            List<Instruction> instructions = new()
            {
                _asmBuilder.Label(new Operand(methodDeclarationSyntax.Identifier.Text))
            };

            var allocatedValue = methodDeclarationSyntax.Body.Members.Where(x => x.Kind == SyntaxKind.VariableDeclaration).Count() * 8;

            CodeScope newScope = new(scope, allocatedValue)
            {
                UpperStackPosition = UpperInitialPos
            };

            Dictionary<string, CodeMethodParameter> methodParams = new();
            foreach (var item in methodDeclarationSyntax.Params)
            {
                methodParams.Add(item.Identifier.Text, new(item.Type));
                instructions.AddRange(VisitMethodParameter(item, newScope));
            }
            scope.AddMethod(methodDeclarationSyntax.Identifier.Text, methodDeclarationSyntax.ReturnType, methodParams);

            var upperStack = newScope.UpperStackPosition - UpperInitialPos;
            newScope.HasStackFrame = newScope.BytesAllocated != 0 || upperStack != 0;


            var returnMethodLabel = new Operand(GenerateExitMethodLabel(methodDeclarationSyntax.Identifier.Text));


            _returnTo.Push(_asmBuilder.Jmp(returnMethodLabel));
            foreach (var item in methodDeclarationSyntax.Body.Members)
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
            return instructions;
        }

        public List<Instruction> VisitMethodParameter(ParameterDeclarationSyntax parameterDeclarationSyntax, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            scope.AddVariable(parameterDeclarationSyntax.Identifier.Text, parameterDeclarationSyntax.Type, true);

            return instructions;
        }

        public List<Instruction> VisitVariableDeclaration(VariableDeclarationSyntax variableDeclarationSyntax, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            var variable = scope.AddVariable(variableDeclarationSyntax.Identifier.Text, variableDeclarationSyntax.Type, false);

            if (variableDeclarationSyntax.InitialValue is not null)
            {
                instructions.AddRange(VisitExpression(variableDeclarationSyntax.InitialValue, scope));
                var rax = new Operand("rax");
                instructions.Add(_asmBuilder.Pop(rax));

                var loc = scope.GetRbpPosStr(variableDeclarationSyntax.Identifier.Text);

                instructions.Add(_asmBuilder.Mov(new Operand(loc), rax));
            }

            return instructions;
        }
        #endregion Declarations

        #region Statements
        public List<Instruction> VisitReturnStatement(ReturnStatementSyntax returnStatementSyntax, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            if (returnStatementSyntax.ReturnValue is not null)
            {
                instructions.AddRange(VisitExpression(returnStatementSyntax.ReturnValue, scope));
            }

            instructions.Add(_asmBuilder.Pop(new Operand("rax")));

            var returnTo = _returnTo.Pop();

            instructions.Add(returnTo);

            return instructions;
        }

        public List<Instruction> VisitIfStatement(IfStatementSyntax ifStatementSyntax, CodeScope scope)
        {
            var instructions = new List<Instruction>();
            instructions.AddRange(VisitExpression(ifStatementSyntax.Condition, scope));

            var rax = new Operand("rax");
            instructions.Add(_asmBuilder.Pop(rax));
            instructions.Add(_asmBuilder.Test(rax, rax));

            var toElseLabelOperand = new Operand(GenerateIfLabel(ELSE_PREFIX));
            var endIfLabelOperand = new Operand(GenerateIfLabel(ENDIF_PREFIX));

            instructions.Add(_asmBuilder.Je(toElseLabelOperand));

            int stackFrameInitPos = instructions.Count;
            int? stackFrameEndPos = null;

            var allocatedValue = ifStatementSyntax.Body.Members.Where(x => x.Kind == SyntaxKind.VariableDeclaration).Count() * 8;
            var newScope = new CodeScope(scope, allocatedValue);

            newScope.HasStackFrame = newScope.BytesAllocated != 0;

            foreach (var item in ifStatementSyntax.Body.Members)
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

            if (ifStatementSyntax.Else is not null)
            {
                instructions.AddRange(VisitElseStatement(ifStatementSyntax.Else, scope));
            }

            instructions.Add(_asmBuilder.Label(endIfLabelOperand));

            return instructions;
        }

        public List<Instruction> VisitElseStatement(StatementSyntax elseStatementSyntax, CodeScope scope)
        {
            {
                var instructions = new List<Instruction>();
                var newScope = new CodeScope(scope);

                switch (elseStatementSyntax.Kind)
                {
                    case SyntaxKind.BlockStatement:
                        instructions.AddRange(VisitElseBody((BlockStatement<SyntaxNode>)elseStatementSyntax, newScope));
                        break;
                    case SyntaxKind.IfStatement:
                        instructions.AddRange(VisitIfStatement((IfStatementSyntax)elseStatementSyntax, newScope));
                        break;

                    default:
                        break;
                }

                return instructions;
            }

            IList<Instruction> VisitElseBody(BlockStatement<SyntaxNode> elseBody, CodeScope elseScope) 
            {
                var instructions = new List<Instruction>();

                int stackFrameInitPos = instructions.Count;
                int? stackFrameEndPos = null;

                var allocatedValue = elseBody.Members.Where(x => x.Kind == SyntaxKind.VariableDeclaration).Count() * 8;
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

        public List<Instruction> VisitWhileStatement(WhileStatementSyntax whileStatementSyntax, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            var whileStartLabelOp = new Operand(GenerateWhileLabel(WHILE_START_PREFIX));
            var whileEndLabelOp = new Operand(GenerateWhileLabel(WHILE_END_PREFIX));

            instructions.Add(_asmBuilder.Label(whileStartLabelOp));


            instructions.AddRange(VisitExpression(whileStatementSyntax.Condition, scope));
            var rax = new Operand("rax");
            instructions.Add(_asmBuilder.Pop(rax));
            instructions.Add(_asmBuilder.Test(rax, rax));

            instructions.Add(_asmBuilder.Je(whileEndLabelOp));


            int stackFrameInitPos = instructions.Count;
            int? stackFrameEndPos = null;

            var allocatedValue = whileStatementSyntax.Body.Members.Where(x => x.Kind == SyntaxKind.VariableDeclaration).Count() * 8;
            var newScope = new CodeScope(scope, allocatedValue);
            newScope.HasStackFrame = newScope.BytesAllocated != 0;

            foreach (var item in whileStatementSyntax.Body.Members)
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
        public List<Instruction> VisitExpression(ExpressionSyntax expressionSyntax, CodeScope scope)
        {
            var expression = expressionSyntax.Kind switch
            {
                SyntaxKind.LiteralExpression => VisitLiteralExpression((LiteralExpressionSyntax)expressionSyntax, scope),
                SyntaxKind.MemberExpression => VisitMemberExpression((MemberExpressionSyntax)expressionSyntax, scope),
                SyntaxKind.UnaryExpression => VisitUnaryExpression((UnaryExpressionSyntax)expressionSyntax, scope),
                SyntaxKind.CastExpression => VisitCastExpression((CastExpressionSyntax)expressionSyntax, scope),
                SyntaxKind.BinaryExpression => VisitBinaryExpression((BinaryExpressionSyntax)expressionSyntax, scope),
                SyntaxKind.LogicalExpression => VisitLogicalExpression((LogicalExpressionSyntax)expressionSyntax, scope),
                SyntaxKind.IdentifierExpression => VisitIdentifierExpression((IdentifierExpressionSyntax)expressionSyntax, scope),
                SyntaxKind.CallExpression => VisitCallExpression((CallExpressionSyntax)expressionSyntax, scope),
                SyntaxKind.AssignmentExpression => VisitAssignmentExpression((AssignmentExpressionSyntax)expressionSyntax, scope),
                _ => throw new UnreachableException(),
            };

            return expression;
        }

        public List<Instruction> VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            instructions.AddRange(VisitExpression(binaryExpressionSyntax.Left, scope));
            instructions.AddRange(VisitExpression(binaryExpressionSyntax.Right, scope));

            instructions.Add(_asmBuilder.Pop(new Operand("rbx")));
            instructions.Add(_asmBuilder.Pop(new Operand("rax")));

            var operation = binaryExpressionSyntax.Operation;

            string resultIsIn;

            switch (operation)
            {
                case SyntaxKind.PlusToken:
                    instructions.Add(_asmBuilder.Add(new Operand("rax"), new Operand("rbx")));
                    resultIsIn = "rax";
                    break;
                case SyntaxKind.MinusToken:
                    instructions.Add(_asmBuilder.Sub(new Operand("rax"), new Operand("rbx")));
                    resultIsIn = "rax";
                    break;
                case SyntaxKind.AsteriskToken:
                    instructions.Add(_asmBuilder.Imul(new Operand("rax"), new Operand("rbx")));
                    resultIsIn = "rax";
                    break;
                case SyntaxKind.SlashToken:
                    instructions.Add(_asmBuilder.Push(new Operand("rdx")));


                    instructions.Add(_asmBuilder.Cqo());
                    instructions.Add(_asmBuilder.Idiv(new Operand("rbx")));


                    instructions.Add(_asmBuilder.Pop(new Operand("rdx")));

                    resultIsIn = "rax";
                    break;
                case SyntaxKind.PercentToken:
                    instructions.Add(_asmBuilder.Push(new Operand("rax")));


                    instructions.Add(_asmBuilder.Cqo());
                    instructions.Add(_asmBuilder.Idiv(new Operand("rbx")));


                    instructions.Add(_asmBuilder.Pop(new Operand("rax")));
                    resultIsIn = "rdx";
                    break;
                default:
                    throw new UnreachableException();
                    break;
            }

            instructions.Add(_asmBuilder.Push(new Operand(resultIsIn)));

            return instructions;
        }
     
        public List<Instruction> VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, CodeScope scope)
        {
            var instructions = new List<Instruction>
            {
                _asmBuilder.Mov(new Operand("rax"), new Operand(scope.GetRbpPosStr(identifierExpressionSyntax.Text))),
                _asmBuilder.Push(new Operand("rax"))
            };

            return instructions;
        }

        public List<Instruction> VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            instructions.AddRange(VisitExpression(logicalExpressionSyntax.Left, scope));
            instructions.AddRange(VisitExpression(logicalExpressionSyntax.Right, scope));

            instructions.Add(_asmBuilder.Pop(new Operand("rbx")));
            instructions.Add(_asmBuilder.Pop(new Operand("rax")));

            var operation = logicalExpressionSyntax.Operation;

            switch (operation)
            {
                case SyntaxKind.BarBarToken:
                    instructions.Add(_asmBuilder.Or(new Operand("rax"), new Operand("rbx")));
                    break;
                case SyntaxKind.AmpersandAmpersandToken:
                    instructions.Add(_asmBuilder.And(new Operand("rax"), new Operand("rbx")));
                    break;

                case SyntaxKind.EqualsEqualsToken:
                    instructions.Add(_asmBuilder.Cmp(new Operand("rax"), new Operand("rbx")));
                    instructions.Add(_asmBuilder.Sete(new Operand("al")));
                    instructions.Add(_asmBuilder.Movzx(new Operand("rax"), new Operand("al")));
                    break;
                case SyntaxKind.NotEqualsToken:
                    instructions.Add(_asmBuilder.Cmp(new Operand("rax"), new Operand("rbx")));
                    instructions.Add(_asmBuilder.Setne(new Operand("al")));
                    instructions.Add(_asmBuilder.Movzx(new Operand("rax"), new Operand("al")));
                    break;

                //case SyntaxKind.GreaterThanToken:
                //    break;
                //case SyntaxKind.GreaterThanEqualsToken:
                //    break; 
                //case SyntaxKind.LessThanToken:
                //    break; 
                //case SyntaxKind.LessThanEqualsToken:
                //    break; 

                default:
                    throw new NotImplementedException();
            }

            instructions.Add(_asmBuilder.Push(new Operand("rax")));

            return instructions;
        }

        public List<Instruction> VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            var literalValue = literalExpressionSyntax.Value;

            switch (literalValue.Kind)
            {
                case SyntaxKind.StringLiteralToken:
                    var strTok = (SyntaxTokenWithValue<string>)literalValue;
                    instructions.AddRange(_asmBuilderAbstractions.String(strTok.Value));
                    instructions.Add(_asmBuilder.Push(new Operand("rax")));
                    break;
                case SyntaxKind.CharLiteralToken:
                    var charTok = (SyntaxTokenWithValue<char>)literalValue;
                    var valueTxt = $"'{charTok.Value}'";
                    instructions.Add(_asmBuilder.Push(new Operand(valueTxt)));
                    break;
                case SyntaxKind.IntLiteralToken:
                    instructions.Add(_asmBuilder.Push(new Operand(((SyntaxTokenWithValue<int>)literalValue).Value.ToString())));
                    break;
                case SyntaxKind.LongLiteralToken:
                    instructions.Add(_asmBuilder.Push(new Operand(((SyntaxTokenWithValue<long>)literalValue).Value.ToString())));
                    break;
                case SyntaxKind.FalseLiteralToken:
                case SyntaxKind.FalseKeyword:
                    instructions.Add(_asmBuilder.Push(new Operand("0")));
                    break;
                case SyntaxKind.TrueLiteralToken:
                case SyntaxKind.TrueKeyword:
                    instructions.Add(_asmBuilder.Push(new Operand("1")));
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }

            return instructions;
        }

        public List<Instruction> VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, CodeScope scope)
        {
            return VisitExpression(memberExpressionSyntax, scope);
        }

        public List<Instruction> VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, CodeScope scope)
        {
            return VisitExpression(unaryExpressionSyntax, scope);
        }

        public List<Instruction> VisitCastExpression(CastExpressionSyntax castExpressionSyntax, CodeScope scope)
        {
            return VisitExpression(castExpressionSyntax.Value, scope);
        }   
        
        public List<Instruction> VisitCallExpression(CallExpressionSyntax callExpressionSyntax, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            for (int i = callExpressionSyntax.Args.Count - 1; i >= 0; i--)
            {
                var arg = callExpressionSyntax.Args[i];

                var visited = VisitExpression(arg, scope);
                instructions.AddRange(visited);
            }

            instructions.Add(_asmBuilder.Call(new Operand(((IdentifierExpressionSyntax)callExpressionSyntax.Caller).Text)));
            instructions.Add(_asmBuilder.Push(new Operand("rax")));

            return instructions;
        }

        public List<Instruction> VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, CodeScope scope)
        {
            var instructions = new List<Instruction>();

            var location = VisitLocationExpression(assignmentExpressionSyntax.Left, scope);

            instructions.AddRange(VisitExpression(assignmentExpressionSyntax.Right, scope));

            instructions.Add(_asmBuilder.Pop(new Operand("rax")));

            instructions.Add(_asmBuilder.Mov(new Operand(location), new Operand("rax")));

            return instructions;
        }
        #endregion Expressions

        private List<Instruction> VisitDefaultBlock(SyntaxNode item, CodeScope scope, out bool isReturn)
        {
            isReturn = false;

            switch (item.Kind)
            {
                case SyntaxKind.VariableDeclaration:
                    return VisitVariableDeclaration((VariableDeclarationSyntax)item, scope);

                case SyntaxKind.CallExpression:
                    return VisitCallExpression((CallExpressionSyntax)item, scope);

                case SyntaxKind.ReturnStatement:
                    isReturn = true;
                    return VisitReturnStatement((ReturnStatementSyntax)item, scope);

                case SyntaxKind.IfStatement:
                    return VisitIfStatement((IfStatementSyntax)item, scope);

                case SyntaxKind.WhileStatement:
                    return VisitWhileStatement((WhileStatementSyntax)item, scope);

                case SyntaxKind.AssignmentExpression:
                    return VisitAssignmentExpression((AssignmentExpressionSyntax)item, scope);

                default:
                    throw new UnreachableException();
            }
        }

        private static string VisitLocationExpression(ExpressionSyntax expressionSyntax, CodeScope scope)
        {
            switch (expressionSyntax.Kind)
            {
                case SyntaxKind.MemberExpression:
                    return GetMemberLocation((MemberExpressionSyntax)expressionSyntax, scope);
                case SyntaxKind.IdentifierExpression:
                    return GetIdentifierLocation((IdentifierExpressionSyntax)expressionSyntax, scope);
                default:
                    throw new UnreachableException();
            }

            static string GetIdentifierLocation(IdentifierExpressionSyntax identifier, CodeScope scope)
            {
                var location = scope.GetRbpPosStr(identifier.Text);

                return location;
            }

            static string GetMemberLocation(MemberExpressionSyntax member, CodeScope scope)
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
