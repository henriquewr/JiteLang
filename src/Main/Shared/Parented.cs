
namespace JiteLang.Main.Shared
{
    internal abstract class Parented<TParent> where TParent : Parented<TParent>
    {
        protected Parented(TParent parent)
        {
            Parent = parent;
        }

        public TParent? Parent { get; set; }

        public virtual T? GetFirstOrDefaultOfType<T>()
        {
            var currentParent = Parent;

            while (currentParent != null)
            {
                if (currentParent is T firstOfType)
                {
                    return firstOfType;
                }

                currentParent = currentParent.Parent;
            }

            return default;
        }
    }
}