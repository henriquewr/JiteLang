using System;
using System.Collections.Generic;
using JiteLang.Main.LangParser.Types;

namespace JiteLang.Main.LangParser
{
    internal class Scope
    {
        public Scope(Scope parent, Dictionary<string, Variable> variables, int bytesAllocated)
        {
            Parent = parent;
            Variables = variables;
            BytesAllocated = bytesAllocated;
        }

        public Scope(Scope parent) : this(parent, new Dictionary<string, Variable>(), 0) 
        {
        }

        public Scope(Scope parent, int bytesAllocated) : this(parent, new Dictionary<string, Variable>(), bytesAllocated)
        {
        }

        public static Scope CreateGlobal()
        {
            return new Scope(null!);
        }

        public Scope? Parent { get; set; }
        public Dictionary<string, Variable> Variables { get; set; }

        public bool HasStackFrame { get; set; }
        public int BytesAllocated { get; init; }
        public int UpperStackPosition { get; set; }

        private int _downStackPosition;
        public int DownStackPosition
        {
            get
            {
                return _downStackPosition;
            }
            set
            {
                if (BytesAllocated == 0)
                {
                    throw new InvalidOperationException("This scope cant allocate");
                }

                if (value > BytesAllocated)
                {
                    throw new InvalidOperationException($"This scope can only allocate {BytesAllocated} bytes");
                }

                _downStackPosition = value;
            }
        }

        public string GetRbpPosStr(string key)
        {
            var variable = GetVariable(key, out int offset);
            var sign = offset >= 0 ? '+' : '-';
            var strPos = $"[rbp {sign} {Math.Abs(offset)}]";
            return strPos;
        }

        public Variable GetVariable(string key)
        {
            return GetVariable(key, out _);
        }

        public Variable GetVariable(string key, out int stackOffsetToVariable)
        {
            const int PushRbpLength = 8;

            var currentContext = this;

            stackOffsetToVariable = 0;

            while (currentContext != null)
            {
                if (currentContext.Variables.TryGetValue(key, out var variable))
                {
                    stackOffsetToVariable += variable.InScopeStackLocation;

                    return variable;
                }

                var needsToCountBytesAllocated = currentContext.HasStackFrame;

                if (needsToCountBytesAllocated)
                {
                    stackOffsetToVariable += PushRbpLength; // exit scope
                }

                currentContext = currentContext.Parent;
                EatScopeWithoutStackFrame(ref currentContext!);

                if (needsToCountBytesAllocated)
                {
                    stackOffsetToVariable += currentContext.BytesAllocated; // top of stack frame of that scope
                }
            }

            throw new KeyNotFoundException($"Variable '{key}' not found in the current or parent scopes.");

            void EatScopeWithoutStackFrame(ref Scope? scope)
            {
                while (scope?.Parent is not null && scope.HasStackFrame == false)
                {
                    scope = currentContext?.Parent;
                }

                if (scope is null)
                {
                    throw new KeyNotFoundException($"Variable '{key}' not found in the current or parent scopes.");
                }
            }
        }

        public Variable AddVariable(string key, TypeSyntax type, bool isPositiveStackLocation)
        {
            int stackLocation;
            if (isPositiveStackLocation)
            {
                UpperStackPosition += 8;
                stackLocation = UpperStackPosition;
            }
            else
            {
                DownStackPosition -= 8;
                stackLocation = DownStackPosition;
            }

            var variable = new Variable(stackLocation, type, isPositiveStackLocation);

            Variables.Add(key, variable);

            return variable;
        }

        public sealed class Variable
        {
            public Variable(int stackLocation, TypeSyntax type)
            {
                InScopeStackLocation = stackLocation;
                Type = type;
            }

            public Variable(int stackLocation, TypeSyntax type, bool stackLocationIsPositive)
            {
                InScopeStackLocation = stackLocation;
                Type = type;
                StackLocationIsPositive = stackLocationIsPositive;
            }

            public bool StackLocationIsPositive { get; set; }
            public int InScopeStackLocation { get; set; }
            public TypeSyntax Type { get; set; }
        }
    }
}
