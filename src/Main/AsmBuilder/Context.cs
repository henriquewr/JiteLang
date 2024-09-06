using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiteLang.Main.AsmBuilder
{
    internal class Context<T>
    {
        public Context(T ret) 
        {
            Return = ret;
        }

        public T Return { get; set; }
    }
}
