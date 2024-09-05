using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Builder.Instructions
{
    internal abstract class Instruction
    {
        public virtual AsmInstructionType Type { get; set; }
    }
}
