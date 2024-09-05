using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.Builder.Operands;

namespace JiteLang.Main.Builder.Instructions
{
    internal class DoubleOperandInstruction : Instruction
    {
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
