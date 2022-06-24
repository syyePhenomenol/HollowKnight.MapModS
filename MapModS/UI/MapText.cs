using MagicUI.Core;
using MagicUI.Elements;
using MapModS.Data;
using MapModS.Map;
using MapModS.Settings;
using System;
using System.Collections.Generic;
using UnityEngine;
using L = RandomizerMod.Localization;

namespace MapModS.UI
{
    internal class MapText
    {
        private static LayoutRoot layout;

        private static TextObject refresh;

        private static readonly Dictionary<string, Tuple<Padding, Action<TextObject>>> _textObjects = new()
        {
            { "Spoilers", new(new(10f, 10f, 1000f, 20f), UpdateSpoilers) },
            { "Randomized", new(new(10f, 10f, 500f, 20f), UpdateRandomized) },
            { "Others", new(new(10f, 10f, 10f, 20f), UpdateOthers) },
            { "Style", new(new(500f, 10f, 10f, 20f), UpdateStyle) },
            { "Size", new(new(1000f, 10f , 10f, 20f), UpdateSize) },
        };

        public static bool Condition()
        {
            return GUI.worldMapOpen || GUI.quickMapOpen;
        }

        public static void Build()
        {
            if (layout == null)
            {
                layout = new(true, "Map Text");
                layout.VisibilityCondition = Condition;

                foreach (string textName in _textObjects.Keys)
                {
                    TextObject textObj = new(layout, textName)
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Font = MagicUI.Core.UI.TrajanNormal,
                        FontSize = 16,
                        Padding = _textObjects[textName].Item1
                    };
                }

                refresh = new(layout, "Refresh")
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Font = MagicUI.Core.UI.TrajanNormal,
                    FontSize = 16,
                    Padding = new(10f, 10f, 10f, 20f)
                };

                UpdateAll();
            }
        }

        public static void Destroy()
        {
            layout?.Destroy();
            layout = null;
        }

        public static void UpdateAll()
        {
            foreach (string textName in _textObjects.Keys)
            {
                TextObject textObj = (TextObject)layout.GetElement(textName);

                _textObjects[textName].Item2.Invoke(textObj);

                if (MapModS.LS.modEnabled)
                {
                    textObj.Visibility = Visibility.Visible;
                }
                else
                {
                    textObj.Visibility = Visibility.Hidden;
                }
            }

            refresh.Visibility = Visibility.Hidden;
        }

        public static void SetToRefresh()
        {
            foreach (string text in _textObjects.Keys)
            {
                TextObject textObj = (TextObject)layout.GetElement(text);

                textObj.Visibility = Visibility.Hidden;
            }

            refresh.Visibility = Visibility.Visible;

            if (MapModS.LS.modEnabled)
            {
                refresh.Text = L.Localize("MapModS enabled. Close map to refresh");
            }
            else
            {
                refresh.Text = L.Localize("MapModS disabled. Close map to refresh");
            }
        }

        private static void UpdateSpoilers(TextObject textObj)
        {
            string text = $"{L.Localize("Spoilers")} (ctrl-1): ";

            if (MapModS.LS.spoilerOn)
            {
                textObj.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                text += L.Localize("on");
            }
            else
            {
                textObj.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                text += L.Localize("off");
            }

            textObj.Text = text;
        }

        private static void UpdateRandomized(TextObject textObj)
        {
            string text = $"{L.Localize("Randomized")} (ctrl-2): ";

            if (MapModS.LS.randomizedOn)
            {
                textObj.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                text += L.Localize("on");
            }
            else
            {
                textObj.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                text += L.Localize("off");
            }

            if (WorldMap.CustomPins.IsRandomizedCustom())
            {
                textObj.ContentColor = Colors.GetColor(ColorSetting.UI_Custom);
            }

            textObj.Text = text;
        }

        private static void UpdateOthers(TextObject textObj)
        {
            string text = $"{L.Localize("Others")} (ctrl-3): ";

            if (MapModS.LS.othersOn)
            {
                textObj.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                text += L.Localize("on");
            }
            else
            {
                textObj.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                text += L.Localize("off");
            }

            if (WorldMap.CustomPins.IsOthersCustom())
            {
                textObj.ContentColor = Colors.GetColor(ColorSetting.UI_Custom);
            }

            textObj.Text = text;
        }

        private static void UpdateStyle(TextObject textObj)
        {
            string text = $"{L.Localize("Style")} (ctrl-4): ";

            switch (MapModS.GS.pinStyle)
            {
                case PinStyle.Normal:
                    text += L.Localize("normal");
                    break;

                case PinStyle.Q_Marks_1:
                    text += $"{L.Localize("q marks")} 1";
                    break;

                case PinStyle.Q_Marks_2:
                    text += $"{L.Localize("q marks")} 2";
                    break;

                case PinStyle.Q_Marks_3:
                    text += $"{L.Localize("q marks")} 3";
                    break;
            }

            textObj.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
            textObj.Text = text;
        }

        private static void UpdateSize(TextObject textObj)
        {
            string text = $"{L.Localize("Size")} (ctrl-5): ";

            switch (MapModS.GS.pinSize)
            {
                case PinSize.Small:
                    text += L.Localize("small");
                    break;

                case PinSize.Medium:
                    text += L.Localize("medium");
                    break;

                case PinSize.Large:
                    text += L.Localize("large");
                    break;
            }

            textObj.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
            textObj.Text = text;
        }
    }
}