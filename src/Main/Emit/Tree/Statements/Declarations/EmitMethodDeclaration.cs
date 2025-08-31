using JiteLang.Main.Emit.AsmBuilder.Scope;
using JiteLang.Main.Shared.Modifiers;
using JiteLang.Main.Shared.Type;
using JiteLang.Utilities;
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

        public EmitLabelStatement Label
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }
        public EmitLabelStatement LabelExit
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;
            }
        }

        public int StackAllocatedBytes { get; set; }
        public int UpperStackPosition { get; set; } = UpperStackInitialPos;
        public EmitBlockStatement<EmitNode, CodeLocal> Body
        {
            get;
            set
            {
                field = value;
                field?.Parent = this;

                if (Params is not null)
                {
                    foreach (var param in Params)
                    {
                        OnAdd(param);
                    }
                }
            }
        }

        protected void OnAdd(EmitParameterDeclaration item)
        {
            item.Parent = Body;
        }

        public NotifyAddList<EmitParameterDeclaration> Params
        {
            get;
            set
            {
                field?.OnAdd -= OnAdd;
                field = value;

                if (field is not null)
                {
                    field.OnAdd += OnAdd;

                    foreach (var member in Params)
                    {
                        OnAdd(member);
                    }
                }
            }
        }

        public Modifier Modifiers { get; set; }
        public AccessModifier AccessModifiers { get; set; }
        public bool IsInitializer { get; set; }
        public DelegateTypeSymbol Type { get; set; }
    }
}