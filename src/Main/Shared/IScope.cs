using System.Collections.Generic;

namespace JiteLang.Main.Shared
{
    internal interface IScope<TVariableKey, TVariable, TMethodKey, TMethod, ParentType> 
        where TVariableKey : notnull 
        where TMethodKey : notnull
        where ParentType : IScope<TVariableKey, TVariable, TMethodKey, TMethod, ParentType>
    {
        public ParentType? Parent { get; set; }
        public Dictionary<TVariableKey, TVariable> Variables { get; set; }
        public Dictionary<TMethodKey, TMethod> Methods { get; set; }
        public TVariable GetVariable(TVariableKey key);
        public TMethod GetMethod(TMethodKey key);
    }
}
