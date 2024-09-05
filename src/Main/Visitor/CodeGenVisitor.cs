//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using JiteLang.Main.Emit;
//using JiteLang.Main.LangParser;
//using JiteLang.Main.LangParser.SyntaxNodes;
//using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
//using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
//using JiteLang.Main.LangParser.SyntaxTree;
//using JiteLang.Main.Visitor.Syntax;
//using JiteLang.Syntax;

//namespace JiteLang.Main.Visitor
//{
//    internal class CodeGenVisitor : SyntaxVisitor
//    {
//        private readonly AssemblyEmiter _assemblyEmiter;

//        public CodeGenVisitor(AssemblyEmiter assemblyEmiter) 
//        {
//            _assemblyEmiter = assemblyEmiter;
//        }

//        public override IList<SyntaxNode> VisitNamespaceDeclaration(NamespaceDeclarationSyntax root, Context context)
//        {
//            var body = base.VisitNamespaceDeclaration(root, context);
//            _assemblyEmiter.Setup();
//            _assemblyEmiter.Exit();
//            return body;
//        }

//        public override IList<SyntaxNode> VisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, Context context)
//        {
//            IList<SyntaxNode> method = null!;

//            _assemblyEmiter.Label(methodDeclarationSyntax.Identifier.Text);
//            //method = base.VisitMethodDeclaration(methodDeclarationSyntax);
//            //_assemblyEmiter.Pop("rax");

//            _assemblyEmiter.NewStackFrame(() =>
//            {
//                var delarations = methodDeclarationSyntax.Body.Members.Count(x => x.Kind == SyntaxKind.VariableDeclaration);
//                _assemblyEmiter.Sub("rbp", $"{delarations * 8}");
//                method = base.VisitMethodDeclaration(methodDeclarationSyntax, context);
//                _assemblyEmiter.Pop("rax");
//            });

//            _assemblyEmiter.Ret();
//            return method;
//        }
//        public override VariableDeclarationSyntax VisitVariableDeclaration(VariableDeclarationSyntax variableDeclarationSyntax, Context context)
//        {
//            var variable = new Context.Variable(_assemblyEmiter.StackPtr, variableDeclarationSyntax.Type);
//            context.Variables.Add(variableDeclarationSyntax.Identifier.Text, variable);
//            //_assemblyEmiter.Push($"QWORD [rsp + {(_assemblyEmiter.StackPtr - variable.StackLocation) * 8}]");


//            if (variableDeclarationSyntax.InitialValue is not null)
//            {
//                variableDeclarationSyntax.InitialValue = base.VisitExpression(variableDeclarationSyntax.InitialValue, context);
//                _assemblyEmiter.Pop("rax");
//                _assemblyEmiter.StackPtr++;
//                _assemblyEmiter.Mov($"QWORD [rbp - {(_assemblyEmiter.StackPtr - variable.StackLocation) * 8}]", "rax");
//            }

//            return variableDeclarationSyntax;
//        }

//        public override AssignmentExpressionSyntax VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, Context context)
//        {
//            assignmentExpressionSyntax.Right = base.VisitExpression(assignmentExpressionSyntax.Right, context);
//            assignmentExpressionSyntax.Left = base.VisitExpression(assignmentExpressionSyntax.Left, context);

//            _assemblyEmiter.Pop("rax");
//            _assemblyEmiter.Pop("rbx");
//            _assemblyEmiter.Mov("[rbx]", "rax");

//            return assignmentExpressionSyntax;
//        }

//        public override ExpressionSyntax VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, Context context)
//        {
//            var visited = base.VisitIdentifierExpression(identifierExpressionSyntax, context);

//            var variable = context.Variables[identifierExpressionSyntax.Text];

//            _assemblyEmiter.Lea("rax", $"[rbp - {(_assemblyEmiter.StackPtr - variable.StackLocation) * 8}]");
//            //_assemblyEmiter.Push($"QWORD [rbp - {(_assemblyEmiter.StackPtr - variable.StackLocation) * 8}]");
//            _assemblyEmiter.Push("rax");

//            return visited;
//        }

//        public override LiteralExpressionSyntax VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, Context context)
//        {
//            var literalValue = literalExpressionSyntax.Value;

//            switch (literalValue.Kind)
//            {
//                case SyntaxKind.StringLiteralToken:
//                    throw new NotImplementedException();
//                    break;
//                case SyntaxKind.CharLiteralToken:
//                    throw new NotImplementedException();
//                    break;
//                case SyntaxKind.IntLiteralToken:
//                    _assemblyEmiter.EmitIntLiteral(((SyntaxTokenWithValue<int>)literalValue).Value);
//                    break;
//                case SyntaxKind.LongLiteralToken:
//                    _assemblyEmiter.EmitLongLiteral(((SyntaxTokenWithValue<long>)literalValue).Value);
//                    break;
//                case SyntaxKind.FalseLiteralToken:
//                    throw new NotImplementedException();
//                    break;
//                case SyntaxKind.TrueLiteralToken:
//                    throw new NotImplementedException();
//                    break;
//                default:
//                    throw new NotImplementedException();
//                    break;
//            }

//            return literalExpressionSyntax;
//        }

//        public override ExpressionSyntax VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, Context context)
//        {
//            var leftCode = VisitExpression(binaryExpressionSyntax.Left, context);
//            var rightCode = VisitExpression(binaryExpressionSyntax.Right, context);

//            _assemblyEmiter.Pop("rbx");
//            _assemblyEmiter.Pop("rax");

//            var operation = binaryExpressionSyntax.Operation;

//            string resultIsIn;

//            switch (operation)
//            {
//                case SyntaxKind.PlusToken:
//                    _assemblyEmiter.Add("rax", "rbx");
//                    resultIsIn = "rax";
//                    break;
//                case SyntaxKind.MinusToken:
//                    _assemblyEmiter.Sub("rax", "rbx");
//                    resultIsIn = "rax";
//                    break;   
//                case SyntaxKind.AsteriskToken:
//                    _assemblyEmiter.IMul("rax", "rbx");
//                    resultIsIn = "rax";
//                    break;     
//                case SyntaxKind.SlashToken:
//                    _assemblyEmiter.Push("rdx");

//                    _assemblyEmiter.IDivComp("rax", "rbx");

//                    _assemblyEmiter.Pop("rdx");
//                    resultIsIn = "rax";
//                    break;
//                case SyntaxKind.PercentToken:
//                    _assemblyEmiter.Push("rax");

//                    _assemblyEmiter.IDivComp("rax", "rbx");

//                    _assemblyEmiter.Pop("rax");
//                    resultIsIn = "rdx";
//                    break;
//                default:
//                    throw new UnreachableException();
//                    break;
//            }

//            _assemblyEmiter.Push(resultIsIn);

//            return binaryExpressionSyntax;
//        }
//    }
//}
