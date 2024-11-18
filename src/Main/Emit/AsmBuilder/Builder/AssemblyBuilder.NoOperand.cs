using JiteLang.Main.AsmBuilder.Instructions;

namespace JiteLang.Main.AsmBuilder.Builder
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
