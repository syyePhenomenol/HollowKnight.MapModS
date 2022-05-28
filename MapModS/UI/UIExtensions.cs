using MagicUI.Core;
using MagicUI.Elements;
using UnityEngine;
using L = RandomizerMod.Localization;

namespace MapModS.UI
{
    internal class UIExtensions
    {
        public static TextObject TextFromEdge(LayoutRoot onLayout, string name, bool onRight)
        {
            if (onRight)
            {
                return new(onLayout, name)
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    TextAlignment = HorizontalAlignment.Right,
                    Font = MagicUI.Core.UI.TrajanNormal,
                    FontSize = 14,
                    Padding = new(0f, 20f, 20f, 0f)
                };
            }
            else
            {
                return new(onLayout, name)
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    TextAlignment = HorizontalAlignment.Left,
                    Font = MagicUI.Core.UI.TrajanNormal,
                    FontSize = 14,
                    Padding = new(20f, 20f, 0f, 0f)
                };
            }
        }

        public static TextObject PanelText(LayoutRoot onLayout, string name)
        {
            return new(onLayout, name)
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlignment = HorizontalAlignment.Left,
                Font = MagicUI.Core.UI.TrajanNormal,
                FontSize = 14,
                Padding = new(0f, 2f, 0f, 2f)
            };
        }

        public static void SetToggleText(TextObject textObj, string baseText, bool value)
        {
            string text = baseText;

            if (value)
            {
                textObj.ContentColor = Color.green;
                text += L.Localize("On");
            }
            else
            {
                textObj.ContentColor = Color.white;
                text += L.Localize("Off");
            }

            textObj.Text = text;
        }
    }
}
