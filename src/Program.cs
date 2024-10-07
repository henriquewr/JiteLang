using System;
using System.Collections.Generic;
using JiteLang.Main.AsmBuilder.Visitor;
using JiteLang.Main.Emit;
using JiteLang.Main.LangLexer;
using JiteLang.Main.LangParser;
using JiteLang.Main.Visitor.Type;
using JiteLang.Main.Visitor.Type.Scope;
using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Bound;
using System.IO;
using JiteLang.Main.AsmBuilder.Builder;
using JiteLang.Main.AsmBuilder.Instructions;

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
        public void Main()
        {
            string value = "i cant believe this string works";
            Print(value);
        }

        public extern void Print(string value);
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

            var builtNamespace = new Binder().BindNamespaceDeclaration(parsed.Root);
            var boundTree = new BoundSyntaxTree(builtNamespace, parsed.Errors);

            new TypeVisitor(boundTree.Errors).VisitNamespaceDeclaration(boundTree.Root, TypeScope.CreateGlobal());

            var jsonTeste = Newtonsoft.Json.JsonConvert.SerializeObject(boundTree.Root);

            foreach (var error in boundTree.Errors)
            {
                Console.WriteLine(error);
            }

            if (boundTree.HasErrors)
            {
                return;
            }

            AssemblyBuilder asmBuilder = new();
            AssemblyBuilderAbstractions asmBuilderAbstractions = new(asmBuilder);
            AsmBuilderVisitor asmBuilderVisitor = new(asmBuilder, asmBuilderAbstractions);
            var intructions = asmBuilderVisitor.VisitNamespaceDeclaration(builtNamespace, CodeScope.CreateGlobal());

            var optimized = Optimize(intructions); //make it better

            using StringWriter streamWriter = new();
            var asmEmiter = new AssemblyEmiter(streamWriter); 
            asmEmiter.EmitInstructions(optimized);

            Console.WriteLine(streamWriter.ToString());
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
                else if (item.Type is AsmInstructionType.Mov)
                {
                    var itemAsDouble = (DoubleOperandInstruction)item;

                    if (itemAsDouble.Left.Value != itemAsDouble.Right.Value)
                    {
                        optmiziedInstuctions.Add(item);
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
