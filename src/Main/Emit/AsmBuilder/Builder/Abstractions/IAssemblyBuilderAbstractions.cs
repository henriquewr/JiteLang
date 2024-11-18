using JiteLang.Main.AsmBuilder.Instructions;
using JiteLang.Main.Emit.AsmBuilder.Operands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Emit.AsmBuilder.Builder.Abstractions
{
    internal interface IAssemblyBuilderAbstractions
    {
        List<Instruction> String(string str);
        List<Instruction> Exit(in Operand code);
    }
}
