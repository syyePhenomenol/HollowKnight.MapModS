using MagicUI.Core;

namespace MapChanger.UI
{
    public abstract class UILayer
    {
        public LayoutRoot Root { get; private set; }

        internal void Build()
        {
            if (Root == null)
            {
                Root = new(true, $"{GetType().Name} Root");
                Root.VisibilityCondition = Condition;

                BuildLayout();
            }
        }

        internal void Destroy()
        {
            Root?.Destroy();
            Root = null;
        }

        public abstract bool Condition();

        public abstract void BuildLayout();

        public virtual void Set()
        {

        }
    }
}
