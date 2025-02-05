using JiteLang.Main.AsmBuilder.Instructions;
using JiteLang.Main.Emit.AsmBuilder.Operands;

namespace JiteLang.Main.AsmBuilder.Builder
{
    internal partial class AssemblyBuilder
    {
        public SingleOperandInstruction Push(Operand pushTo)
        {
            var push = new SingleOperandInstruction(AsmInstructionType.Push, pushTo);
            return push;
        }

        public SingleOperandInstruction Pop(Operand popTo)
        {
            var pop = new SingleOperandInstruction(AsmInstructionType.Pop, popTo);
            return pop;
        }

        public SingleOperandInstruction Label(Operand labelName)
        {
            var label = new SingleOperandInstruction(AsmInstructionType.Label, labelName);
            return label;
        }

        public SingleOperandInstruction Call(Operand call)
        {
            var callInstr = new SingleOperandInstruction(AsmInstructionType.Call, call);
            return callInstr;
        }

        public SingleOperandInstruction Section(Operand name)
        {
            var section = new SingleOperandInstruction(AsmInstructionType.Section, name);
            return section;
        }
        public SingleOperandInstruction Extern(Operand name)
        {
            var section = new SingleOperandInstruction(AsmInstructionType.Extern, name);
            return section;
        }
        public SingleOperandInstruction Global(Operand name)
        {
            var global = new SingleOperandInstruction(AsmInstructionType.Global, name);
            return global;
        } 
        
        public SingleOperandInstruction Idiv(Operand name)
        {
            var idiv = new SingleOperandInstruction(AsmInstructionType.Idiv, name);
            return idiv;
        }

        public SingleOperandInstruction Ret(Operand value)
        {
            return new SingleOperandInstruction(AsmInstructionType.Ret, value);
        }  
        
        public SingleOperandInstruction Je(Operand name)
        {
            return new SingleOperandInstruction(AsmInstructionType.Je, name);
        }
        public SingleOperandInstruction Jne(Operand name)
        {
            return new SingleOperandInstruction(AsmInstructionType.Jne, name);
        }
        public SingleOperandInstruction Jmp(Operand name)
        {
            return new SingleOperandInstruction(AsmInstructionType.Jmp, name);
        }
        public SingleOperandInstruction Jg(Operand name)
        {
            return new SingleOperandInstruction(AsmInstructionType.Jg, name);
        }
        public SingleOperandInstruction Jge(Operand name)
        {
            return new SingleOperandInstruction(AsmInstructionType.Jge, name);
        }
        public SingleOperandInstruction Jl(Operand name)
        {
            return new SingleOperandInstruction(AsmInstructionType.Jl, name);
        }
        public SingleOperandInstruction Jle(Operand name)
        {
            return new SingleOperandInstruction(AsmInstructionType.Jle, name);
        }
        public SingleOperandInstruction Sete(Operand operand)
        {
            return new SingleOperandInstruction(AsmInstructionType.Sete, operand);
        }
        public SingleOperandInstruction Setne(Operand operand)
        {
            return new SingleOperandInstruction(AsmInstructionType.Setne, operand);
        }
        public SingleOperandInstruction Setle(Operand operand)
        {
            return new SingleOperandInstruction(AsmInstructionType.Setle, operand);
        }  
        public SingleOperandInstruction Setl(Operand operand)
        {
            return new SingleOperandInstruction(AsmInstructionType.Setl, operand);
        }  
        public SingleOperandInstruction Setge(Operand operand)
        {
            return new SingleOperandInstruction(AsmInstructionType.Setge, operand);
        }   
        public SingleOperandInstruction Setg(Operand operand)
        {
            return new SingleOperandInstruction(AsmInstructionType.Setg, operand);
        }
    }
}
