using JiteLang.Main.Emit.AsmBuilder.Operands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.AsmBuilder.Instructions
{
    internal class NoOperandInstruction : Instruction
    {
        protected override string GetDebuggerDisplay()
        {
            return $"{Type}";
        }
        public NoOperandInstruction(AsmInstructionType type)
        {
            Type = type;
        }
    }
}
