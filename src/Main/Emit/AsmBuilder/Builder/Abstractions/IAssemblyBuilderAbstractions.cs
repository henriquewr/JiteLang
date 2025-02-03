using JiteLang.Main.AsmBuilder.Instructions;
using JiteLang.Main.Emit.AsmBuilder.Operands;
using System.Collections.Generic;

namespace JiteLang.Main.Emit.AsmBuilder.Builder.Abstractions
{
    internal interface IAssemblyBuilderAbstractions
    {
        List<Instruction> String(string str);
        List<Instruction> AllocateReadWriteMemory(Operand size);
        List<Instruction> Exit(in Operand code);
    }
}
