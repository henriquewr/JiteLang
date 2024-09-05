using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.Builder.Instructions
{
    internal enum AsmInstructionType
    {
        None,
        Mov,
        Movzx,
        Lea,
        Sub,
        Add,
        Imul,
        Or,
        And,
        Cmp,
        Test,

        Idiv,
        Push,
        Pop,
        Label,
        Section,
        Global,
        Call,
        Ret,
        Je,
        Jne,
        Jmp,
        Sete,
        Setne,

        Syscall,
        Cqo,
        Leave,
    }
}
