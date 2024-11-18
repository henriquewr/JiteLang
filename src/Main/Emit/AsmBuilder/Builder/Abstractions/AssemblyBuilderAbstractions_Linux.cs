using System;
using System.Collections.Generic;
using System.Text;
using JiteLang.Main.AsmBuilder.Builder;
using JiteLang.Main.AsmBuilder.Instructions;
using JiteLang.Main.Emit.AsmBuilder.Operands;

namespace JiteLang.Main.Emit.AsmBuilder.Builder.Abstractions
{
    internal class AssemblyBuilderAbstractions_Linux : IAssemblyBuilderAbstractions
    {
        private readonly AssemblyBuilder _asmBuilder;

        public AssemblyBuilderAbstractions_Linux(AssemblyBuilder asmBuilder)
        {
            _asmBuilder = asmBuilder;
        }

        public List<Instruction> String(string str)
        {
            //this function does not work for strange chars (alt + 1 for example)

            const int MetadataLength = 8;

            var strLength = str.Length + 1; //string strLength + null terminator

            var instructions = new List<Instruction>
            {
                _asmBuilder.Mov(Operand.Rdi, new Operand("0")),                        // address
                _asmBuilder.Mov(Operand.Rsi, new Operand(strLength + MetadataLength)), // memory strLength
                _asmBuilder.Mov(Operand.Rdx, new Operand("0x3")),                      // PROT_READ | PROT_WRITE
                _asmBuilder.Mov(Operand.R10, new Operand("0x22")),                     // MAP_PRIVATE | MAP_ANONYMOUS
                _asmBuilder.Mov(Operand.R8, new Operand("0")),                         // File descriptor
                _asmBuilder.Mov(Operand.R9, new Operand("0")),                         // Offset 
                _asmBuilder.Mov(Operand.Rax, new Operand("9")),                        // mmap
                _asmBuilder.Syscall(),


                _asmBuilder.Mov(new Operand($"qword [rax]"), new Operand(strLength)),
                _asmBuilder.Add(Operand.Rax, new Operand(MetadataLength))
            };

            int i = 0;
            int lengthAligned = str.Length / 8 * 8;

            var bytes = Encoding.UTF8.GetBytes(str);
            for (; i < lengthAligned; i += 8)
            {
                var value = BitConverter.ToUInt64(bytes, i);
                instructions.Add(_asmBuilder.Mov(Operand.Rbx, new Operand(value.ToString())));
                instructions.Add(_asmBuilder.Mov(new Operand($"[rax + {i}]"), Operand.Rbx));
            }

            for (; i < str.Length; i++)
            {
                instructions.Add(_asmBuilder.Mov(new Operand($"byte [rax + {i}]"), new Operand($"'{str[i]}'")));
            }

            instructions.Add(_asmBuilder.Mov(new Operand($"byte [rax + {i}]"), new Operand(0)));

            return instructions;
        }

        public List<Instruction> Exit(in Operand code)
        {
            var instructions = new List<Instruction>
            {
                _asmBuilder.Mov(Operand.Rax, new Operand("60")),
                _asmBuilder.Mov(Operand.Rdi, code),
                _asmBuilder.Syscall()
            };

            return instructions;
        }
    }
}
