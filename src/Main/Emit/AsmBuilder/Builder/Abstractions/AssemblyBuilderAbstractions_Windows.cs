using JiteLang.Main.AsmBuilder.Builder;
using JiteLang.Main.AsmBuilder.Instructions;
using JiteLang.Main.Emit.AsmBuilder.Operands;
using System;
using System.Collections.Generic;
using System.Text;

namespace JiteLang.Main.Emit.AsmBuilder.Builder.Abstractions
{
    internal class AssemblyBuilderAbstractions_Windows : IAssemblyBuilderAbstractions
    {
        private readonly AssemblyBuilder _asmBuilder;
        public AssemblyBuilderAbstractions_Windows(AssemblyBuilder asmBuilder)
        {
            _asmBuilder = asmBuilder;
        }

        public List<Instruction> String(string str)
        {
            const int MetadataLength = 8;

            var strLength = str.Length + 1; //string strLength + null terminator

            var instructions = AllocateMemory(new Operand(strLength + MetadataLength));
            
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
            return AllocateMemory(size, flAllocationType: 0x3000, permissions: 0x04);
        }

        private List<Instruction> AllocateMemory(Operand size, uint flAllocationType = 0x3000, uint permissions = 0x04)
        {
            /*
                Permissions:
                    PAGE_NOACCESS	0x01
                    PAGE_READONLY: 0x02
                    PAGE_READWRITE: 0x04
                    PAGE_WRITECOPY: 0x08
                    PAGE_EXECUTE: 0x10
                    PAGE_EXECUTE_READ: 0x20
                    PAGE_EXECUTE_READWRITE: 0x40
                    PAGE_EXECUTE_WRITECOPY: 0x80

                flAllocationType:
                    MEM_COMMIT:	 0x1000
                    MEM_RESERVE: 0x2000
                    MEM_RESET: 0x8000
                    MEM_TOP_DOWN: 0x100000	
             */
            var instructions = new List<Instruction>
            {
                _asmBuilder.Sub(Operand.Rsp, new Operand(32)),
                _asmBuilder.Xor(Operand.Rcx, Operand.Rcx),
                _asmBuilder.Mov(Operand.Rdx, size),
                _asmBuilder.Mov(Operand.R8, new Operand(flAllocationType.ToString())),
                _asmBuilder.Mov(Operand.R9, new Operand(permissions.ToString())),
                _asmBuilder.Call(new Operand("VirtualAlloc")),
                _asmBuilder.Add(Operand.Rsp, new Operand(32)),
            };

            return instructions;
        }

        public List<Instruction> Exit(in Operand code)
        {
            var instructions = new List<Instruction> 
            {
                _asmBuilder.Mov(Operand.Rcx, code),
                _asmBuilder.Call(new Operand("ExitProcess")),
            };

            return instructions;
        }
    }
}
