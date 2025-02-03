using System.Collections.Generic;

namespace JiteLang.Main.PredefinedExternMethods.PredefinedExternMethods
{
    internal class PredefinedExternMethodResolver
    {
        public PredefinedExternMethodResolver()
        {
        }

        public static readonly Dictionary<string, IPredefinedExternMethod> ExternMethods = new()
        {
            { Method_Print.C_Name , new Method_Print() }
        };

        public static IPredefinedExternMethod? Resolve(string methodName)
        {
            ExternMethods.TryGetValue(methodName, out var result);
            return result;
        }
    }
}