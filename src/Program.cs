using Newtonsoft.Json;
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
                public string Teste(int a, long b)
                {
                    return "galo2";
                }

                public string Main()
                {
                    return Teste(1, "");
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

            var jsonTeste = JsonConvert.SerializeObject(parsed.Root);

            new TypeVisitor(parsed.Errors).VisitNamespaceDeclaration(parsed.Root, TypeScope.CreateGlobal());

            foreach (var error in parsed.Errors)
            {
                Console.WriteLine(error);
            }


            //var typeChecker = new TypeCheckerVisitor(new TypeVisitor());
            //typeChecker.CheckNamespaceDeclaration(parsed.Root);

            //var visitor2 = new BuilderVisitor();
            //var builtNamespace = visitor2.VisitNamespaceDeclaration(parsed.Root, context);

            var asmBuilder = new AssemblyBuilder();
            var asmBuilderAbstractions = new AssemblyBuilderAbstractions(asmBuilder);
            var asmBuilderVisitor = new AsmBuilderVisitor(asmBuilder, asmBuilderAbstractions);
            var intructions = asmBuilderVisitor.VisitNamespaceDeclaration(parsed.Root, CodeScope.CreateGlobal());

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
