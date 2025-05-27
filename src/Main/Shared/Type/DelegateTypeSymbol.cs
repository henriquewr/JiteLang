using JiteLang.Main.Shared.Type.Members.Method;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace JiteLang.Main.Shared.Type
{
    internal class DelegateTypeSymbol : TypeSymbol
    {
        public DelegateTypeSymbol(string fullText, string text, TypeSymbol returnType, IImmutableList<ParameterSymbol> parameters) : base(fullText, text, true)
        {
            ReturnType = returnType;
            Parameters = parameters;
        }

        public TypeSymbol ReturnType { get; set; }
        public IImmutableList<ParameterSymbol> Parameters { get; set; }
        public override int Size => 8;

        public static DelegateTypeSymbol Generate(string methodName, TypeSymbol returnType, params IEnumerable<TypeSymbol> @params)
        {
            var paramsSymbol = @params.Select(x => new ParameterSymbol(x)).ToImmutableList();
            var methodTypeName = paramsSymbol.Aggregate(methodName + returnType.FullText, (acc, item) => acc + item.Type.FullText);

            var methodType = new DelegateTypeSymbol(methodTypeName, methodName, returnType, paramsSymbol);
            return methodType;
        }
    }
}