using JiteLang.Main.Bound;
using JiteLang.Main.Shared;
using System.Collections.Generic;

namespace JiteLang.Main.Visitor.Type.Scope
{
    internal class TypeScope : IScope<string, TypeVariable, string, TypeMethod, TypeScope>
    {
        public TypeScope(TypeScope parent, Dictionary<string, TypeVariable> variables, Dictionary<string, TypeMethod> methods)
        {
            Parent = parent;
            Variables = variables;
            Methods = methods;
        }

        public TypeScope(TypeScope parent) : this(parent, new(), new())
        {

        }

        public Dictionary<string, TypeVariable> Variables { get; set; }
        public Dictionary<string, TypeMethod> Methods { get; set; }
        public TypeScope? Parent { get; set; }

        public TypeVariable GetVariable(string key)
        {
            var currentContext = this;

            while (currentContext != null)
            {
                if (currentContext.Variables.TryGetValue(key, out var variable))
                {
                    return variable;
                }

                currentContext = currentContext.Parent;
            }

            throw new KeyNotFoundException($"Variable '{key}' not found in the current or parent scopes.");
        }

        public TypeMethod GetMethod(string key)
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

        public TypeVariable AddVariable(string name, TypeSymbol type)
        {
            var variable = new TypeVariable(type);

            Variables.Add(name, variable);

            return variable;
        }

        public TypeMethod AddMethod(string name, TypeSymbol returnType)
        {
            var method = new TypeMethod(returnType);

            Methods.Add(name, method);

            return method;
        }

        public TypeMethod AddMethod(string name, TypeSymbol returnType, Dictionary<string, TypeMethodParameter> @params)
        {
            var method = new TypeMethod(returnType, @params);

            Methods.Add(name, method);

            return method;
        }

        public static TypeScope CreateGlobal()
        {
            return new TypeScope(null!);
        }
    }
}
