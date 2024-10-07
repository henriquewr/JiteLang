using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JiteLang.Main.AsmBuilder.Instructions;

namespace JiteLang.Main.Emit
{
    internal class AssemblyEmiter
    {
        private readonly TextWriter _outputWriter;

        public AssemblyEmiter(TextWriter outputWriter)
        {
            _outputWriter = outputWriter;
        }

        public void EmitInstructions(IEnumerable<Instruction> intructions)
        {
            foreach (var instruction in intructions)
            {
                var type = instruction.Type;

                if (instruction is DoubleOperandInstruction doubleOperandInstruction)
                {
                    var left = doubleOperandInstruction.Left;
                    var right = doubleOperandInstruction.Right;
                    switch (type)
                    {
                        case AsmInstructionType.Mov:
                            Mov(left.Value, right.Value);
                            break;

                        case AsmInstructionType.Movzx:
                            Movzx(left.Value, right.Value);
                            break;

                        case AsmInstructionType.Imul:
                            IMul(left.Value, right.Value);
                            break;

                        case AsmInstructionType.Sub:
                            Sub(left.Value, right.Value);
                            break;

                        case AsmInstructionType.Add:
                            Add(left.Value, right.Value);
                            break;

                        case AsmInstructionType.Lea:
                            Lea(left.Value, right.Value);
                            break;

                        case AsmInstructionType.Or:
                            Or(left.Value, right.Value);
                            break;

                        case AsmInstructionType.And:
                            And(left.Value, right.Value);
                            break;      
                        
                        case AsmInstructionType.Cmp:
                            Cmp(left.Value, right.Value);
                            break;

                        case AsmInstructionType.Test:
                            Test(left.Value, right.Value);
                            break;

                        default:
                            throw new UnreachableException();
                    }
                }
                else if(instruction is SingleOperandInstruction singleOperandInstruction)
                {
                    var operand = singleOperandInstruction.Operand;
                    switch (type)
                    {
                        case AsmInstructionType.Push:
                            Push(operand.Value);
                            break;
                        case AsmInstructionType.Pop:
                            Pop(operand.Value);
                            break;
                        case AsmInstructionType.Label:
                            Label(operand.Value);
                            break;
                        case AsmInstructionType.Section:
                            Section(operand.Value);
                            break;
                        case AsmInstructionType.Global:
                            Global(operand.Value);
                            break;

                        case AsmInstructionType.Call:
                            Call(operand.Value);
                            break;

                        case AsmInstructionType.Ret:
                            Ret(operand.Value);
                            break;
                        case AsmInstructionType.Idiv:
                            Idiv(operand.Value);
                            break;

                        case AsmInstructionType.Je:
                            Je(operand.Value);
                            break;

                        case AsmInstructionType.Jne:
                            Jne(operand.Value);
                            break;     
                        
                        case AsmInstructionType.Jmp:
                            Jmp(operand.Value);
                            break;

                        case AsmInstructionType.Sete:
                            Sete(operand.Value);
                            break;

                        case AsmInstructionType.Setne:
                            Setne(operand.Value);
                            break;

                        case AsmInstructionType.Setle:
                            Setle(operand.Value);
                            break;

                        case AsmInstructionType.Setl:
                            Setl(operand.Value);
                            break;

                        case AsmInstructionType.Setge:
                            Setge(operand.Value);
                            break;

                        case AsmInstructionType.Setg:
                            Setg(operand.Value);
                            break;

                        default:
                            throw new UnreachableException();
                    }
                }
                else if (instruction is NoOperandInstruction noOperandInstruction)
                {
                    switch (type)
                    {
                        case AsmInstructionType.Syscall:
                            Syscall();
                            break;

                        case AsmInstructionType.Cqo:
                            Cqo();
                            break; 
                            
                        case AsmInstructionType.Leave:
                            Leave();
                            break;     

                        default:
                            throw new UnreachableException();
                    }
                }
                else
                {
                    throw new UnreachableException();
                }
            }
        }

        #region DoubleOperand
        public void Add(string left, string right)
        {
            _outputWriter.WriteLine($"    add {left}, {right}");
        }
        public void Sub(string left, string right)
        {
            _outputWriter.WriteLine($"    sub {left}, {right}");
        }
        public void IMul(string left, string right)
        {
            _outputWriter.WriteLine($"    imul {left}, {right}");
        }
        public void And(string left, string right)
        {
            _outputWriter.WriteLine($"    and {left}, {right}");
        }
        public void Or(string left, string right)
        {
            _outputWriter.WriteLine($"    or {left}, {right}");
        }
        public void Mov(string left, string right)
        {
            _outputWriter.WriteLine($"    mov {left}, {right}");
        }
        public void Movzx(string left, string right)
        {
            _outputWriter.WriteLine($"    movzx {left}, {right}");
        }
        public void Test(string left, string right)
        {
            _outputWriter.WriteLine($"    test {left}, {right}");
        }
        public void Cmp(string left, string right)
        {
            _outputWriter.WriteLine($"    cmp {left}, {right}");
        }
        public void Lea(string left, string right)
        {
            _outputWriter.WriteLine($"    lea {left}, {right}");
        }
        #endregion DoubleOperand

        #region SingleOperand
        public void Push(string pushTo)
        {
            _outputWriter.WriteLine($"    push {pushTo}");
        }
        public void Pop(string popTo)
        {
            _outputWriter.WriteLine($"    pop {popTo}");
        }
        public void Idiv(string operand)
        {
            _outputWriter.WriteLine($"    idiv {operand}");
        }
        public void Sete(string operand)
        {
            _outputWriter.WriteLine($"    sete {operand}");
        }
        public void Setne(string operand)
        {
            _outputWriter.WriteLine($"    setne {operand}");
        }
        public void Setg(string operand)
        {
            _outputWriter.WriteLine($"    setg {operand}");
        }
        public void Setge(string operand)
        {
            _outputWriter.WriteLine($"    setge {operand}");
        }
        public void Setl(string operand)
        {
            _outputWriter.WriteLine($"    setl {operand}");
        }
        public void Setle(string operand)
        {
            _outputWriter.WriteLine($"    setle {operand}");
        }
        public void Label(string label)
        {
            _outputWriter.WriteLine($"{label}:");
        }
        public void Section(string section)
        {
            _outputWriter.WriteLine($"section {section}");
        }
        public void Global(string name)
        {
            _outputWriter.WriteLine($"    global {name}");
        }
        public void Call(string call)
        {
            _outputWriter.WriteLine($"    call {call}");
        }
        public void Je(string name)
        {
            _outputWriter.WriteLine($"    je {name}");
        }
        public void Jne(string name)
        {
            _outputWriter.WriteLine($"    jne {name}");
        }
        public void Jmp(string name)
        {
            _outputWriter.WriteLine($"    jmp {name}");
        }
        public void Ret(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "0")
            {
                _outputWriter.WriteLine($"    ret");
            }
            else
            {
                _outputWriter.WriteLine($"    ret {value}");
            }
        }
        #endregion SingleOperand

        #region NoOperand
        public void Cqo()
        {
            _outputWriter.WriteLine("    cqo");
        }
        public void Leave()
        {
            _outputWriter.WriteLine("    leave");
        }
        public void Syscall()
        {
            _outputWriter.WriteLine("    syscall");
        }
        #endregion NoOperand
    }
}
