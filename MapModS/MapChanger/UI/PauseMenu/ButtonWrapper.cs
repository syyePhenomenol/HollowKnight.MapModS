using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    public abstract class ButtonWrapper
    {
        public readonly string Name;
        public Button Button { get; protected set; }

        public ButtonWrapper(string name)
        {
            Name = name;
        }

        internal abstract void Make();

        public virtual void OnClick(Button button)
        {
            PauseMenu.Instance.Update();
        }

        public abstract void Update();
    }
}
