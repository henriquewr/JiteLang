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

            var memoryLength = strLength + MetadataLength;

            var instructions = AllocateReadWriteMemory(new Operand(memoryLength));

            instructions.Add(_asmBuilder.Mov(new Operand($"qword [rax]"), new Operand(strLength)));
            instructions.Add(_asmBuilder.Add(Operand.Rax, new Operand(MetadataLength)));

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

        public List<Instruction> AllocateReadWriteMemory(Operand size)
        {
            return AllocateMemory(size, config: 0x22, permissions: 0x3);
        }

        private List<Instruction> AllocateMemory(Operand size, uint config = 0x22, uint permissions = 0x3)
        {
            /*
                Permissions:
                    PROT_NONE: 0x0
                    PROT_READ: 0x1
                    PROT_WRITE:	0x2
                    PROT_EXEC: 0x4

                Configs:
                    MAP_SHARED:           0x01
                    MAP_PRIVATE:          0x02
                    MAP_ANONYMOUS:        0x20
                    MAP_FIXED:            0x10
                    MAP_FIXED_NOREPLACE:  0x100000
                    MAP_POPULATE:         0x8000
                    MAP_NORESERVE:        0x40
                    MAP_LOCKED:           0x2000
                    MAP_HUGETLB:          0x40000
                    MAP_DENYWRITE:        0x0800
                    MAP_EXECUTABLE:       0x1000
                    MAP_GROWSDOWN:        0x0100
                    MAP_GROWSUP:          0x0200
                    MAP_NONBLOCK:         0x10000
                    MAP_STACK:            0x20000
                    MAP_SYNC:             0x80000
                    MAP_SHARED_VALIDATE:  0x03
             */

            List<Instruction> instructions = new()
            {
                _asmBuilder.Xor(Operand.Rdi, Operand.Rdi),
                _asmBuilder.Mov(Operand.Rsi, size),
                _asmBuilder.Mov(Operand.Rdx, new Operand(permissions.ToString())),
                _asmBuilder.Mov(Operand.R10, new Operand(config.ToString())),
                _asmBuilder.Xor(Operand.R8, Operand.R8),
                _asmBuilder.Xor(Operand.R9, Operand.R9),
                _asmBuilder.Mov(Operand.Rax, new Operand("9")),
                _asmBuilder.Syscall(),
            };

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
