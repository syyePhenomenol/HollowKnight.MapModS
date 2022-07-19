using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    public abstract class ExtraButton : ButtonWrapper
    {
        private ExtraButtonPanel ebp;

        public ExtraButton(string name, ExtraButtonPanel ebp) : base(name)
        {
            this.ebp = ebp;
        }

        internal override void Make()
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
            ebp.ExtraButtonsGrid.Children.Add(Button);
        }
    }
}
