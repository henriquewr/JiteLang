using JiteLang.Main.AsmBuilder.Builder;
using JiteLang.Main.AsmBuilder.Instructions;
using JiteLang.Main.AsmBuilder.Operands;
using JiteLang.Main.Bound;
using System.Collections.Generic;

namespace JiteLang.Main.PredefinedExternMethods
{
    internal class Method_Print : IPredefinedExternMethod
    {
        public const string C_Name = "Print";
        public string Name => C_Name;

        public static readonly TypeSymbol S_ReturnType = PredefinedTypeSymbol.Void;
        public TypeSymbol ReturnType => PredefinedTypeSymbol.Void;


        public static readonly List<TypeSymbol> S_ParamsTypes = new() { PredefinedTypeSymbol.String };
        public List<TypeSymbol> ParamsTypes => S_ParamsTypes;

        public Method_Print()
        {

        }

        public static List<Instruction> GenerateInstructions(AssemblyBuilder asmBuilder, Operand pointer, Operand length)
        {
            List<Instruction> instructions = new()
            {
                asmBuilder.Label(new Operand(C_Name)),

                asmBuilder.Push(Operand.Rbp),
                asmBuilder.Mov(Operand.Rbp, Operand.Rsp),

                asmBuilder.Mov(Operand.Rax, new Operand("1")),
                asmBuilder.Mov(Operand.Rdi, new Operand("1")),

                asmBuilder.Mov(Operand.Rsi, pointer),
                asmBuilder.Mov(Operand.Rdx, length),

                asmBuilder.Syscall(),

                asmBuilder.Leave(),

                asmBuilder.Ret(new Operand("8")),
            };

            return instructions;
        }
    }
}
