using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    public class ExtraButtonPanel
    {
        public readonly string Name;

        private readonly ExtraButton[] extraButtons;

        public DynamicUniformGrid ExtraButtonsGrid { get; private set; }

        public ExtraButtonPanel(string name, ExtraButton[] extraButtons)
        {
            Name = name;
            this.extraButtons = extraButtons;
        }

        internal void Make()
        {
            ExtraButtonsGrid = new(PauseMenu.Instance.Root, Name)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Orientation = Orientation.Vertical,
                Padding = new(415f, 865f, 10f, 10f),
                HorizontalSpacing = 0f,
                VerticalSpacing = 5f
            };

            ExtraButtonsGrid.ChildrenBeforeRollover = 10;

            foreach (ExtraButton extraButton in extraButtons)
            {
                extraButton.Make();
            }

            PauseMenu.ExtraButtonPanels.Add(this);
        }

        public void Hide()
        {
            ExtraButtonsGrid.Visibility = Visibility.Hidden;
        }

        public void Set()
        {
            foreach (ExtraButton extraButton in extraButtons)
            {
                extraButton.Set();
            }
        }
    }
}
