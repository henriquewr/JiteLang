using JiteLang.Main.Visitor.Type.Scope;
using System.Collections.Generic;

namespace JiteLang.Main.Shared
{
    internal interface IScope<TVariable, TMethod, ParentType> 
        where ParentType : IScope<TVariable, TMethod, ParentType>
    {
        public ParentType? Parent { get; set; }
        public Dictionary<string, TVariable> Variables { get; set; }
        public Dictionary<string, TMethod> Methods { get; set; }
        public TVariable? GetVariable(string key);
        public TMethod? GetMethod(string key);

        public abstract static ParentType CreateGlobal();
    }
}