using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    public abstract class ExtraButton : ButtonWrapper
    {
        public readonly string Mod;

        public ExtraButton(string name, string mod) : base($"{mod} {name}")
        {
            Mod = mod;
        }

        internal override void Make(Layout parent)
        {
            Button = new(PauseMenu.Instance.Root, Name)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Borderless = true,
                MinHeight = 28f,
                MinWidth = 85f,
                Content = Name,
                Font = MagicUI.Core.UI.TrajanNormal,
                FontSize = 11,
                Margin = 0f
            };

            Button.Click += OnClick;
            parent.Children.Add(Button);
        }
    }
}
