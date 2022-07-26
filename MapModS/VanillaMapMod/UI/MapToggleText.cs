using MagicUI.Core;
using MagicUI.Elements;
using MapChanger.UI;

namespace VanillaMapMod
{
    internal sealed class MapToggleText : BottomRowText
    {
        private static readonly string[] Texts = 
        {
            "RASFASAFE",
            "sefo89w8o3",
            "23498yhn2983vhrn"
        };

        public override void BuildTexts()
        {
            for (int i = 0; i < Texts.Length; i++)
            {
                TextObject textObject = new TextObject(Root, Texts[i])
                {
                    Text = Texts[i],
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = MagicUI.Core.UI.TrajanNormal,
                    FontSize = 16,
                }.WithProp(GridLayout.Column, i);

                MapTexts.Add(textObject);
            }
        }

        public override void SetText(TextObject textObject)
        {

        }
    }
}
