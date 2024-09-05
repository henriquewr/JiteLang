using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.Builder.Instructions;
using JiteLang.Main.Builder.Operands;

namespace JiteLang.Main.Builder.AsmBuilder
{
    internal class AssemblyBuilderAbstractions
    {
        private readonly AssemblyBuilder _asmBuilder;

        public AssemblyBuilderAbstractions(AssemblyBuilder asmBuilder)
        {
            _asmBuilder = asmBuilder;
        }

        public IList<Instruction> String(string str)
        {
            //this function does not work for strange chars (alt + 1 for example)

            var length = str.Length + 1; //string length + null terminator

            var instructions = new List<Instruction>
            {
                _asmBuilder.Mov(new Operand("rdi"), new Operand("0")),        // address
                _asmBuilder.Mov(new Operand("rsi"), new Operand(length)),     // memory length
                _asmBuilder.Mov(new Operand("rdx"), new Operand("0x3")),      // PROT_READ | PROT_WRITE
                _asmBuilder.Mov(new Operand("r10"), new Operand("0x22")),     // MAP_PRIVATE | MAP_ANONYMOUS
                _asmBuilder.Mov(new Operand("r8"), new Operand("0")),         // File descriptor
                _asmBuilder.Mov(new Operand("r9"), new Operand("0")),         // Offset 
                _asmBuilder.Mov(new Operand("rax"), new Operand("9")),        // mmap
                _asmBuilder.Syscall(),
            };

            int i = 0;
            int lengthAligned = (str.Length / 8) * 8;

            var bytes = Encoding.UTF8.GetBytes(str);
            for (; i < lengthAligned; i += 8)
            {
                var value = BitConverter.ToUInt64(bytes, i);
                instructions.Add(_asmBuilder.Mov(new Operand($"rbx"), new Operand(value.ToString())));
                instructions.Add(_asmBuilder.Mov(new Operand($"[rax + {i}]"), new Operand("rbx")));
            }

            for (; i < str.Length; i++)
            {
                instructions.Add(_asmBuilder.Mov(new Operand($"byte [rax + {i}]"), new Operand($"'{str[i]}'")));
            }

            instructions.Add(_asmBuilder.Mov(new Operand($"byte [rax + {i}]"), new Operand(0)));

            return instructions;
        }

        public IList<Instruction> Exit(in Operand code)
        {
            var instructions = new List<Instruction>
            {
                _asmBuilder.Mov(new Operand("rax"), new Operand("60")),
                _asmBuilder.Mov(new Operand("rdi"), code),
                _asmBuilder.Syscall()
            };

            return instructions;
        }
    }
}
