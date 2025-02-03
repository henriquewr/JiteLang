using JiteLang.Main.Emit.AsmBuilder.Operands;

namespace JiteLang.Main.AsmBuilder.Instructions
{
    internal class DoubleOperandInstruction : Instruction
    {
        protected override string GetDebuggerDisplay()
        {
            return $"{Type} {Left.Value}, {Right.Value}";
        }
        public DoubleOperandInstruction(AsmInstructionType type, Operand left, Operand right)
        {
            Type = type;
            Left = left;
            Right = right;
        }

        public Operand Left { get; set; }
        public Operand Right { get; set; }
    }
}
