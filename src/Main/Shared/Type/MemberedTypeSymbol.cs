using JiteLang.Main.Bound.Context;
using JiteLang.Main.Shared.Type.Members;
using JiteLang.Main.Shared.Type.Members.Method;
using System.Collections.Generic;
using System.Linq;

namespace JiteLang.Main.Shared.Type
{
    internal abstract class MemberedTypeSymbol : TypeSymbol
    {
        public override int Size => Fields.Count * 8;

        protected MemberedTypeSymbol(string fullText, string text, bool isReferenceType, List<FieldSymbol> fields, List<MethodSymbol> methods) 
            : base(fullText, text, isReferenceType)
        {
            Fields = fields;
            Methods = methods;

            Constructors = AddEmptyCtorIfNeeds(null);
        }

        private List<CtorSymbol> AddEmptyCtorIfNeeds(List<CtorSymbol>? ctors)
        {
            ctors ??= new();

            if (!ctors.Any(x => x.Parameters.Count == 0))
            {
                ctors.Add(new(this));
            }

            return ctors;
        }

        public IEnumerable<TypedMemberSymbol> GetTypedMembers()
        {
            foreach (var field in Fields)
            {
                yield return field;
            }

            foreach (var method in Methods)
            {
                yield return method;
            }
        }

        public CtorSymbol? GetMatchingCtor(BindingContext bindingContext, IList<TypeSymbol> argsTypes)
        {
            var ctors = GetCtors(argsTypes.Count);

            foreach (var ctor in ctors)
            {
                bool validCtor = true;

                for (int i = 0; i < argsTypes.Count; i++)
                {
                    var argType = argsTypes[i];
                    var methodParamType = ctor.Parameters[i].Type;

                    if (!argType.Equals(methodParamType) && !bindingContext.ConversionTable.TryGetImplicitConversion(argType, methodParamType, out var conversion))
                    {
                        validCtor = false;
                        break;
                    }
                }

                if (validCtor)
                {
                    return ctor;
                }
            }

            return null;
        }

        public IEnumerable<CtorSymbol> GetCtors(int paramsCount)
        {
            var ctors = Constructors.Where(x => x.Parameters.Count == paramsCount);
            return ctors;
        }

        public List<CtorSymbol> Constructors { get; set; }
        public List<MethodSymbol> Methods { get; set; }
        public List<FieldSymbol> Fields { get; set; }
    }
}
