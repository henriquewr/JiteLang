using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.AsmBuilder.Instructions
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
        Xor,
        And,
        Cmp,
        Test,

        Idiv,
        Push,
        Pop,
        Label,
        Section,
        Global,
        Extern,
        Call,
        Ret,
        Je,
        Jne,
        Jmp,
        Sete,
        Setne,
        Setle,
        Setl,
        Setge,
        Setg,

        Syscall,
        Cqo,
        Leave,
    }
}
