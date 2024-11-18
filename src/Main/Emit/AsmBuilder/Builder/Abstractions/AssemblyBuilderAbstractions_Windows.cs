using JiteLang.Main.AsmBuilder.Builder;
using JiteLang.Main.AsmBuilder.Instructions;
using JiteLang.Main.Emit.AsmBuilder.Operands;
using JiteLang.Main.Shared.Type;
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

            var instructions = AllocateMemory(strLength + MetadataLength + 123);

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

        public List<Instruction> AllocateClass(TypeSymbol type)
        {
            var instructions = new List<Instruction>(); 

            return instructions;
        }

        public List<Instruction> AllocateMemory(int size, uint flAllocationType = 0x3000, uint permissions = 0x04)
        {
            /*
                Permissions:
                    PAGE_READWRITE: 0x04
                    PAGE_EXECUTE_READWRITE:	0x40
                    PAGE_NOACCESS:	0x01

                flAllocationType:
                    MEM_COMMIT:	 0x1000
                    MEM_RESERVE: 0x2000
                    MEM_RESET: 0x8000
                    MEM_TOP_DOWN: 0x100000	
             */
            var instructions = new List<Instruction>
            { 
                _asmBuilder.Xor(Operand.Rcx, Operand.Rcx),
                _asmBuilder.Mov(Operand.Rdx, new Operand(size.ToString())),
                _asmBuilder.Mov(Operand.R8, new Operand(flAllocationType.ToString())),
                _asmBuilder.Mov(Operand.R9, new Operand(permissions.ToString())),
                _asmBuilder.Call(new Operand("VirtualAlloc"))
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
