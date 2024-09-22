using System;
using System.Collections.Generic;
using JiteLang.Main.AsmBuilder.Visitor;
using JiteLang.Main.Builder.AsmBuilder;
using JiteLang.Main.Builder.Instructions;
using JiteLang.Main.Emit;
using JiteLang.Main.LangLexer;
using JiteLang.Main.LangParser;
using JiteLang.Main.Visitor.Type;
using JiteLang.Main.Visitor.Type.Scope;
using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Bound;

namespace JiteLang
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var text = """
namespace Teste
{
    class AlgumaCoisa
    {
        public int Main()
        {
            int test = 1;

            while(test <= 200)
            {
                test = test + 1;
            }

            return test;
        }
    }
}
""";

            Compile(text);
        }

        static void Compile(string text)
        {
            var lexer = new Lexer(text);
            var lexed = lexer.Lex();

            var parser = new Parser(lexed);
            var parsed = parser.Parse();

            var jsonTeste = Newtonsoft.Json.JsonConvert.SerializeObject(parsed.Root);

            var builtNamespace = new Binder().BindNamespaceDeclaration(parsed.Root);
            var boundTree = new BoundSyntaxTree(builtNamespace, parsed.Errors);

            new TypeVisitor(boundTree.Errors).VisitNamespaceDeclaration(boundTree.Root, TypeScope.CreateGlobal());

            foreach (var error in boundTree.Errors)
            {
                Console.WriteLine(error);
            }

            if (boundTree.HasErrors)
            {
                return;
            }

            var asmBuilder = new AssemblyBuilder();
            var asmBuilderAbstractions = new AssemblyBuilderAbstractions(asmBuilder);
            var asmBuilderVisitor = new AsmBuilderVisitor(asmBuilder, asmBuilderAbstractions);
            var intructions = asmBuilderVisitor.VisitNamespaceDeclaration(builtNamespace, CodeScope.CreateGlobal());

            var optimized = Optimize(intructions);

            var asmEmiter = new AssemblyEmiter(Console.Out);
            asmEmiter.EmitInstructions(optimized);
        }

        static List<Instruction> Optimize(IList<Instruction> instructions)
        {
            var optmiziedInstuctions = new List<Instruction>();

            for (int i = 0; i < instructions.Count; i++)
            {
                var item = instructions[i];
                if(item.Type is AsmInstructionType.Push && instructions[i + 1].Type is AsmInstructionType.Pop)
                {
                    var itemAsSingle = (SingleOperandInstruction)item;
                    var nextAsSingle = (SingleOperandInstruction)instructions[++i];

                    if(itemAsSingle.Operand.Value != nextAsSingle.Operand.Value)
                    {
                        optmiziedInstuctions.Add(new DoubleOperandInstruction(AsmInstructionType.Mov, nextAsSingle.Operand, itemAsSingle.Operand));
                    }
                }
                else
                {
                    optmiziedInstuctions.Add(item);
                }
            }

            return optmiziedInstuctions;
        }
    }
}
