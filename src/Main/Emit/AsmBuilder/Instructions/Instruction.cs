using System.Diagnostics;

namespace JiteLang.Main.AsmBuilder.Instructions
{
    [DebuggerDisplay("{GetDebuggerDisplay()}")]
    internal abstract class Instruction
    {
        protected abstract string GetDebuggerDisplay();

        public virtual AsmInstructionType Type { get; set; }
    }
}
