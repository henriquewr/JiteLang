﻿using JiteLang.Main.AsmBuilder.Scope;
using JiteLang.Main.Shared;
using System.Collections.Generic;

namespace JiteLang.Main.Emit.Tree.Statements
{
    internal class EmitBlockStatement<TMembers> : EmitStatement,
        IVarDeclarable<CodeVariable> where TMembers : EmitNode
    {
        public override EmitKind Kind => EmitKind.BlockStatement;
        public EmitBlockStatement(EmitNode parent, List<TMembers> members, Dictionary<string, CodeVariable> variables) : base(parent)
        {
            Members = members;
            Variables = variables;
        }

        public EmitBlockStatement(EmitNode parent) : this(parent, new(), new())
        {
        }

        public List<TMembers> Members { get; set; }
        public Dictionary<string, CodeVariable> Variables { get; set; }
    }
}