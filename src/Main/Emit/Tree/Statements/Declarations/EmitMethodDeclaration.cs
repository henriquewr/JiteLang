using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.Shared.Modifiers;
using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.Emit.Tree.Statements.Declarations
{
    internal class EmitMethodDeclaration : EmitDeclaration
    {
        public override EmitKind Kind => EmitKind.MethodDeclaration;
        public EmitMethodDeclaration(EmitNode? parent, string name, EmitBlockStatement<EmitNode, CodeLocal> body, DelegateTypeSymbol delegateType) : base(parent, name)
        {
            Label = new EmitLabelStatement(this, name);
            LabelExit = EmitLabelStatement.Create(this, $"exit_{name}");
            Body = body;
            Params = new();
            Type = delegateType;
        }

        public const int UpperStackInitialPos = 8; //skip return address

        public EmitLabelStatement Label { get; set; }
        public EmitLabelStatement LabelExit { get; set; }

        public int StackAllocatedBytes { get; set; }
        public int UpperStackPosition { get; set; } = UpperStackInitialPos;
        public EmitBlockStatement<EmitNode, CodeLocal> Body { get; set; }
        public List<EmitParameterDeclaration> Params { get; set; }
        public Modifier Modifiers { get; set; }
        public AccessModifier AccessModifiers { get; set; }
        public bool IsInitializer { get; set; }
        public DelegateTypeSymbol Type { get; set; }

        public override void SetParent()
        {
            Body.Parent = this;

            Label.Parent = this;

            LabelExit.Parent = this;

            foreach (var param in Params)
            {
                param.Parent = Body;
            }
        }

        public override void SetParentRecursive()
        {
            Body.Parent = this;
            Body.SetParentRecursive();

            Label.Parent = this;
            Label.SetParentRecursive();

            LabelExit.Parent = this;
            LabelExit.SetParentRecursive();

            foreach (var param in Params)
            {
                param.Parent = Body;
                param.Parent.SetParentRecursive();
            }
        }
    }
}