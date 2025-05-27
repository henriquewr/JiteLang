using JiteLang.Main.Shared;
using JiteLang.Main.Shared.Type;
using System.Collections.Generic;

namespace JiteLang.Main.Visitor.Type.Scope
{
    internal class TypeScope : IScope<TypeVariable, TypeMethod, TypeScope>
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

        public TypeVariable? GetVariable(string key)
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

            return null;
        }

        public TypeMethod? GetMethod(string key)
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

            return null;
        }

        public TypeIdentifier? GetIdentifier(string key)
        {
            var currentContext = this;

            while (currentContext != null)
            {
                if (currentContext.Variables.TryGetValue(key, out var variable))
                {
                    return variable;
                }

                if (currentContext.Methods.TryGetValue(key, out var method))
                {
                    return method;
                }

                currentContext = currentContext.Parent;
            }

            return null;
        }

        public TypeLocal AddVariable(string name, TypeSymbol type)
        {
            var variable = new TypeLocal(type, name);

            Variables.Add(name, variable);

            return variable;
        }

        public TypeMethod AddMethod(string name, DelegateTypeSymbol methodType)
        {
            var method = new TypeMethod(methodType, name);

            Methods.Add(name, method);

            return method;
        }

        public TypeMethod AddMethod(string name, DelegateTypeSymbol methodType, Dictionary<string, TypeMethodParameter> @params)
        {
            var method = new TypeMethod(methodType, name, @params);

            Methods.Add(name, method);

            return method;
        }

        public static TypeScope CreateGlobal()
        {
            return new TypeScope(null!);
        }
    }
}