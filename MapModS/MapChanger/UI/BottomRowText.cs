using System.Collections.Generic;
using MagicUI.Core;
using MagicUI.Elements;

namespace MapChanger.UI
{
    public abstract class BottomRowText : UILayer
    {
        protected List<TextObject> MapTexts { get; private set; }

        public override bool Condition()
        {
            return Settings.MapModEnabled && (States.WorldMapOpen || States.QuickMapOpen);
        }

        public override void BuildLayout()
        {
            MapTexts = new();

            GridLayout grid = new(Root, $"{GetType().Name} + Grid")
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new(20f)
            };

            BuildTexts();

            foreach (TextObject text in MapTexts)
            {
                grid.ColumnDefinitions.Add(new GridDimension(200f, GridUnit.AbsoluteMin));
                grid.Children.Add(text);
            }
        }

        public override void Set()
        { 
            foreach (TextObject text in MapTexts)
            {
                SetText(text);
            }
        }

        public abstract void BuildTexts();

        public abstract void SetText(TextObject textObject);
    }
}
