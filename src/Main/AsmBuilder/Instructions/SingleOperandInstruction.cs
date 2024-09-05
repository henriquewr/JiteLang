﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.Builder.Operands;

namespace JiteLang.Main.Builder.Instructions
{
    internal class SingleOperandInstruction : Instruction
    {
        public SingleOperandInstruction(AsmInstructionType type, Operand operand)
        {
            Type = type;
            Operand = operand;
        }

        public Operand Operand { get; set; }
    }
}
