using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.Builder.Instructions;

namespace JiteLang.Main.Builder.AsmBuilder
{
    internal partial class AssemblyBuilder
    {
        public NoOperandInstruction Syscall()
        {
            return new NoOperandInstruction(AsmInstructionType.Syscall);
        }

        public NoOperandInstruction Cqo()
        {
            return new NoOperandInstruction(AsmInstructionType.Cqo);
        }

        public NoOperandInstruction Leave()
        {
            return new NoOperandInstruction(AsmInstructionType.Leave);
        }
    }
}
