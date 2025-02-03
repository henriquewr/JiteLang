using JiteLang.Main.Emit.AsmBuilder.Operands;

namespace JiteLang.Main.AsmBuilder.Instructions
{
    internal class SingleOperandInstruction : Instruction
    {
        protected override string GetDebuggerDisplay()
        {
            return $"{Type} {Operand.Value}";
        }

        public SingleOperandInstruction(AsmInstructionType type, Operand operand)
        {
            Type = type;
            Operand = operand;
        }

        public Operand Operand { get; set; }
    }
}
