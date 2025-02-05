using JiteLang.Main.AsmBuilder.Builder;
using JiteLang.Main.AsmBuilder.Instructions;
using JiteLang.Main.Emit.AsmBuilder.Builder.Abstractions;
using JiteLang.Main.Emit.AsmBuilder.Operands;
using JiteLang.Main.Emit.Tree;
using JiteLang.Main.Emit.Tree.Expressions;
using JiteLang.Main.PredefinedExternMethods;
using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.PredefinedMethods
{
    internal class Method_AllocateReadWriteMemory : IPredefinedMethod
    {
        public const string C_Name = "AllocateReadWriteMemory";
        public string Name => C_Name;

        public static readonly TypeSymbol S_ReturnType = PredefinedTypeSymbol.Object;
        public TypeSymbol ReturnType => PredefinedTypeSymbol.Object;


        public static readonly List<TypeSymbol> S_ParamsTypes = new() { PredefinedTypeSymbol.Int };
        public List<TypeSymbol> ParamsTypes => S_ParamsTypes;

        public static EmitCallExpression Call(EmitNode parent, EmitExpression size)
        {
            EmitCallExpression call = new (parent, null!, new() { size });
            call.Caller = new EmitIdentifierExpression(call, C_Name);

            return call;
        }

        public Method_AllocateReadWriteMemory()
        {
        }

        // stack arg
        public static List<Instruction> GenerateInstructions(IAssemblyBuilderAbstractions asmBuilderAbs, AssemblyBuilder asmBuilder)
        {
            var instructions = new List<Instruction>
            {
                asmBuilder.Label(new Operand(C_Name)),
                asmBuilder.Push(Operand.Rbp),
                asmBuilder.Mov(Operand.Rbp, Operand.Rsp),
                asmBuilder.Sub(Operand.Rsp, new Operand(16)),
            };

            instructions.AddRange(asmBuilderAbs.AllocateReadWriteMemory(new Operand("[rbp + 16]")));

            instructions.Add(asmBuilder.Leave());
            instructions.Add(asmBuilder.Ret(new Operand(8)));

            return instructions;
        }
    }
}
