using JiteLang.Main.Bound;
using JiteLang.Main.LangParser.Types;
using JiteLang.Main.Shared;
using System;
using System.Collections.Generic;

namespace JiteLang.Main.AsmBuilder.Scope
{
    internal class CodeScope : IScope<string, CodeVariable, string, CodeMethod, CodeScope>
    {
        public CodeScope(CodeScope parent,
            Dictionary<string, CodeVariable> variables,
            Dictionary<string, CodeMethod> methods,
            int bytesAllocated)
        {
            Parent = parent;
            Variables = variables;
            Methods = methods;
            BytesAllocated = bytesAllocated;
        }
        public CodeScope(CodeScope parent, int bytesAllocated) : this(parent,
            new(),
            new(),
            bytesAllocated)
        {
        }
        public CodeScope(CodeScope parent) : this(parent,
            new(),
            new(),
            0)
        {
        }

        public Dictionary<string, CodeVariable> Variables { get; set; }
        public Dictionary<string, CodeMethod> Methods { get; set; }
        public CodeScope? Parent { get; set; }

        public static CodeScope CreateGlobal()
        {
            return new CodeScope(null!);
        }

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

        public CodeMethod GetMethod(string key)
        {
            var currentContext = this;

            while (currentContext != null)
            {
                if (currentContext.Methods.TryGetValue(key, out var method))
                {
                    return method;
                }

                currentContext = currentContext.Parent;
            }

            throw new KeyNotFoundException($"Method '{key}' not found in the current or parent scopes.");
        }

        public CodeVariable GetVariable(string key)
        {
            return GetVariable(key, out _);
        }

        public CodeVariable GetVariable(string key, out int stackOffsetToVariable)
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

            void EatScopeWithoutStackFrame(ref CodeScope? scope)
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

        public CodeVariable AddVariable(string key, TypeSymbol type, bool isPositiveStackLocation)
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

            var variable = new CodeVariable(stackLocation, type, isPositiveStackLocation);

            Variables.Add(key, variable);

            return variable;
        }

        public CodeMethod AddMethod(string key, TypeSymbol type, Dictionary<string, CodeMethodParameter> @params)
        {
            var method = new CodeMethod(type, @params);

            Methods.Add(key, method);

            return method;
        }
    }
}
