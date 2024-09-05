using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.Builder.Instructions;
using JiteLang.Main.Builder.Operands;

namespace JiteLang.Main.Builder.AsmBuilder
{
    internal partial class AssemblyBuilder
    {
        public DoubleOperandInstruction Mov(Operand left, Operand right)
        {
            var mov = new DoubleOperandInstruction(AsmInstructionType.Mov, left, right);
            return mov;
        }   
        public DoubleOperandInstruction Movzx(Operand left, Operand right)
        {
            var movzx = new DoubleOperandInstruction(AsmInstructionType.Movzx, left, right);
            return movzx;
        }

        public DoubleOperandInstruction Sub(Operand left, Operand right)
        {
            var sub = new DoubleOperandInstruction(AsmInstructionType.Sub, left, right);
            return sub;
        }

        public DoubleOperandInstruction Add(Operand left, Operand right)
        {
            var add = new DoubleOperandInstruction(AsmInstructionType.Add, left, right);
            return add;
        }

        public DoubleOperandInstruction Imul(Operand left, Operand right)
        {
            var imul = new DoubleOperandInstruction(AsmInstructionType.Imul, left, right);
            return imul;
        }

        public DoubleOperandInstruction Lea(Operand left, Operand right)
        {
            var lea = new DoubleOperandInstruction(AsmInstructionType.Lea, left, right);
            return lea;
        } 
        
        public DoubleOperandInstruction Or(Operand left, Operand right)
        {
            var or = new DoubleOperandInstruction(AsmInstructionType.Or, left, right);
            return or;
        }

        public DoubleOperandInstruction And(Operand left, Operand right)
        {
            var and = new DoubleOperandInstruction(AsmInstructionType.And, left, right);
            return and;
        }   
        
        public DoubleOperandInstruction Cmp(Operand left, Operand right)
        {
            var cmp = new DoubleOperandInstruction(AsmInstructionType.Cmp, left, right);
            return cmp;
        }
        public DoubleOperandInstruction Test(Operand left, Operand right)
        {
            var test = new DoubleOperandInstruction(AsmInstructionType.Test, left, right);
            return test;
        }
    }
}
