using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using JiteLang.Main.AsmBuilder.Visitor;
using JiteLang.Main.Builder.AsmBuilder;
using JiteLang.Main.Builder.Instructions;
using JiteLang.Main.Emit;
using JiteLang.Main.LangLexer;
using JiteLang.Main.LangParser;
using JiteLang.Main.AsmBuilder;

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
                    return Galo(10000);
                }

                public int Galo(int galo)
                {
                    if(galo == 0)
                    {
                        return 3;
                    }

                    return Galo(galo - 1);
                }
            }
        }
        """
            ;

            Compile(text);
        }

        static void Compile(string text)
        {
            var lexer = new Lexer(text);
            var lexed = lexer.Lex();

            var parser = new Parser(lexed);
            var parsed = parser.Parse();

            foreach (var error in parsed.Errors)
            { 
                Console.WriteLine(error);
            }

            var jsonTeste = JsonConvert.SerializeObject(parsed.Root);

            //var typeChecker = new TypeCheckerVisitor(new TypeVisitor(new SyntaxVisitor()));
            //typeChecker.CheckNamespaceDeclaration(parsed.Root);

            //var visitor2 = new BuilderVisitor();
            //var builtNamespace = visitor2.VisitNamespaceDeclaration(parsed.Root, context);

            var asmBuilder = new AssemblyBuilder();
            var asmBuilderAbstractions = new AssemblyBuilderAbstractions(asmBuilder);
            var asmBuilderVisitor = new AsmBuilderVisitor(asmBuilder, asmBuilderAbstractions);
            var intructions = asmBuilderVisitor.VisitNamespaceDeclaration(parsed.Root, Scope.CreateGlobal());
            var optimized = Optimize(intructions);




            var asmEmiter = new AssemblyEmiter(Console.Out);
            asmEmiter.EmitInstructions(optimized);
        }

        static IList<Instruction> Optimize(IList<Instruction> instructions)
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
