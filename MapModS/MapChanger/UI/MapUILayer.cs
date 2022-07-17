using MagicUI.Core;

namespace MapChanger.UI
{
    public abstract class MapUILayer
    {
        public LayoutRoot Layout { get; protected set; }

        public abstract bool Condition();

        public abstract void Build();

        public abstract void Destroy();

        public abstract void Set();
    }
}
