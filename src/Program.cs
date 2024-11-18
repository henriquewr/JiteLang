using System;
using System.Collections.Generic;
using JiteLang.Main.Emit;
using JiteLang.Main.LangLexer;
using JiteLang.Main.LangParser;
using JiteLang.Main.Visitor.Type.Scope;
using JiteLang.Main.Bound;
using System.IO;
using JiteLang.Main.AsmBuilder.Builder;
using JiteLang.Main.AsmBuilder.Instructions;
using System.Diagnostics;
using Newtonsoft.Json;
using JiteLang.Main.Emit.AsmBuilder.Visitor;
using JiteLang.Main.Emit.Tree;
using JiteLang.Main.Emit.AsmBuilder.Builder.Abstractions;

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
            AlgumaCoisa a = null;
            Print(a);
            return 1;
        }

        public extern void Print(string value);
    }
}
""";

            Compile(text);
        }

        static void Compile(string text)
        {
            var lexed = new Lexer(text).Lex();

            var parsed = new Parser(lexed).Parse();

            var boundTree = new Binder(parsed).Bind(TypeScope.CreateGlobal());

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            var jsonTeste = Newtonsoft.Json.JsonConvert.SerializeObject(boundTree.Root, settings);

            foreach (var error in boundTree.Errors)
            {
                Console.WriteLine(error);
            }

            if (boundTree.HasErrors)
            {
                return;
            }

            AssemblyBuilder asmBuilder = new();
            AssemblyBuilderAbstractions_Windows asmBuilderAbstractions = new(asmBuilder);

            EmitTreeBuilderVisitor emitTreeBuilder = new();
            var emitTree = emitTreeBuilder.VisitNamespaceDeclaration(boundTree.Root, null);

            AsmBuilderVisitor asmBuilderVisitor = new(asmBuilder, asmBuilderAbstractions);
            var intructions = asmBuilderVisitor.VisitNamespaceDeclaration(emitTree);

            var optimized = Optimize(intructions); //make it better

            using StringWriter streamWriter = new();
            var asmEmiter = new AssemblyEmiter(streamWriter); 
            asmEmiter.EmitInstructions(optimized);

            var code = streamWriter.ToString();

            Console.WriteLine(code);
            AssembleLinkExecute(code);
        }

        static List<Instruction> Optimize(List<Instruction> instructions)
        {
            var optmiziedInstuctions = new List<Instruction>();

            for (int i = 0; i < instructions.Count; i++)
            {
                var item = instructions[i];
                if(item.Type is AsmInstructionType.Push && instructions[i + 1].Type is AsmInstructionType.Pop)
                {
                    var itemAsSingle = (SingleOperandInstruction)item;
                    var nextAsSingle = (SingleOperandInstruction)instructions[++i];

                    if (itemAsSingle.Operand.Value != nextAsSingle.Operand.Value)
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

        static void AssembleLinkExecute(string inputText)
        {
            //this is a test

            string asmFile = Path.GetTempFileName() + ".asm";
            File.WriteAllText(asmFile, inputText);

            string objFile = Path.ChangeExtension(asmFile, ".obj");
            string exeFile = Path.ChangeExtension(asmFile, ".exe");

            string nasmPath = @"nasm";
            string kernel32Path = @"C:\Program Files (x86)\Windows Kits\10\Lib\10.0.22621.0\um\x64\kernel32.lib";
            string user32Path = @"C:\Program Files (x86)\Windows Kits\10\Lib\10.0.22621.0\um\x64\User32.Lib";
            string linkerPath = @"H:\Microsoft Visual Studio 2022\VC\Tools\MSVC\14.42.34433\bin\Hostx64\x64\link.exe";

            Process nasmProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = nasmPath,
                    Arguments = $"-f win64 \"{asmFile}\" -o \"{objFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            nasmProcess.Start();
            string nasmError = nasmProcess.StandardError.ReadToEnd();
            nasmProcess.WaitForExit();

            if (nasmProcess.ExitCode != 0)
            {
                Console.WriteLine("Erro na compilação:");
                Console.WriteLine(nasmError);
                return;
            }

            Process linkProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = linkerPath,
                    Arguments = $"/OUT:\"{exeFile}\" /entry:_start /subsystem:console \"{objFile}\" \"{user32Path}\" \"{kernel32Path}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            linkProcess.Start();
            string linkError = linkProcess.StandardError.ReadToEnd();
            string linkOutput = linkProcess.StandardOutput.ReadToEnd();
            linkProcess.WaitForExit();

            if (linkProcess.ExitCode != 0)
            {
                Console.WriteLine("Erro na linkagem:");
                Console.WriteLine(linkError);
                return;
            }

            Console.WriteLine($"Executing code...{Environment.NewLine}");

            var codeProcess = Process.Start(exeFile);

            codeProcess.WaitForExit();

            string codeError = linkProcess.StandardError.ReadToEnd();
            string codeOutput = linkProcess.StandardOutput.ReadToEnd();

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"Exit code: {codeProcess.ExitCode} at {codeProcess.ExitTime.ToLongTimeString()}");
        }
    }
}