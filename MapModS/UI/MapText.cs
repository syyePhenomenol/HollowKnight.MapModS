using MapModS.CanvasUtil;
using MapModS.Map;
using MapModS.Settings;
using System.Linq;
using UnityEngine;
using RandomizerMod;

namespace MapModS.UI
{
    internal class MapText
    {
        public static GameObject Canvas;

        public static bool LockToggleEnable;

        private static CanvasPanel _mapDisplayPanel;
        private static CanvasPanel _refreshDisplayPanel;

        public static void Show()
        {
            if (Canvas == null) return;

            Canvas.SetActive(true);
            LockToggleEnable = false;
            RebuildText();
        }

        public static void Hide()
        {
            if (Canvas == null) return;

            Canvas.SetActive(false);
            LockToggleEnable = false;
        }

        public static void BuildText(GameObject _canvas)
        {
            Canvas = _canvas;
            _mapDisplayPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(0f, 1030f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _mapDisplayPanel.AddText("Spoilers", "", new Vector2(-540f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperCenter);
            _mapDisplayPanel.AddText("Randomized", "", new Vector2(-270f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperCenter);
            _mapDisplayPanel.AddText("Others", "", new Vector2(0f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperCenter);
            _mapDisplayPanel.AddText("Style", "", new Vector2(270f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperCenter);
            _mapDisplayPanel.AddText("Size", "", new Vector2(540f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperCenter);

            _refreshDisplayPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(0f, 1030f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _refreshDisplayPanel.AddText("Refresh", "", new Vector2(0f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14, FontStyle.Normal, TextAnchor.UpperCenter);

            _mapDisplayPanel.SetActive(false, false);
            _refreshDisplayPanel.SetActive(false, false);

            SetTexts();
        }

        public static void RebuildText()
        {
            _mapDisplayPanel.Destroy();
            _refreshDisplayPanel.Destroy();

            BuildText(Canvas);
        }

        public static void SetTexts()
        {
            if (GameManager.instance.gameMap == null || WorldMap.CustomPins == null) return;

            _mapDisplayPanel.SetActive(!LockToggleEnable && MapModS.LS.ModEnabled, false);
            _refreshDisplayPanel.SetActive(LockToggleEnable, false);

            SetSpoilers();
            SetStyle();
            SetRandomized();
            SetOthers();
            SetSize();
            SetRefresh();
        }

        private static void SetSpoilers()
        {
            string spoilersText = $"{Localization.Localize("Spoilers")} (ctrl-1): ";

            if (MapModS.LS.SpoilerOn)
            {
                _mapDisplayPanel.GetText("Spoilers").SetTextColor(Color.green);
                spoilersText += Localization.Localize("on");
            }
            else
            {
                _mapDisplayPanel.GetText("Spoilers").SetTextColor(Color.white);
                spoilersText += Localization.Localize("off");
            }

            _mapDisplayPanel.GetText("Spoilers").UpdateText(spoilersText);
        }

        private static void SetRandomized()
        {
            if (WorldMap.CustomPins == null) return;

            string randomizedText = $"{Localization.Localize("Randomized")} (ctrl-2): ";

            if (MapModS.LS.randomizedOn)
            {
                _mapDisplayPanel.GetText("Randomized").SetTextColor(Color.green);
                randomizedText += Localization.Localize("on");
            }
            else
            {
                _mapDisplayPanel.GetText("Randomized").SetTextColor(Color.white);
                randomizedText += Localization.Localize("off");
            }

            if (WorldMap.CustomPins.IsRandomizedCustom())
            {
                _mapDisplayPanel.GetText("Randomized").SetTextColor(Color.yellow);
                randomizedText += $" ({Localization.Localize("custom")})";
            }

            _mapDisplayPanel.GetText("Randomized").UpdateText(randomizedText);
        }

        private static void SetOthers()
        {
            if (WorldMap.CustomPins == null) return;

            string othersText = $"{Localization.Localize("Others")} (ctrl-3): ";

            if (MapModS.LS.othersOn)
            {
                _mapDisplayPanel.GetText("Others").SetTextColor(Color.green);
                othersText += Localization.Localize("on");
            }
            else
            {
                _mapDisplayPanel.GetText("Others").SetTextColor(Color.white);
                othersText += Localization.Localize("off");
            }

            if (WorldMap.CustomPins.IsOthersCustom())
            {
                _mapDisplayPanel.GetText("Others").SetTextColor(Color.yellow);
                othersText += $" ({Localization.Localize("custom")})";
            }

            _mapDisplayPanel.GetText("Others").UpdateText(othersText);
        }

        private static void SetStyle()
        {
            string styleText = $"{Localization.Localize("Style")} (ctrl-4): ";

            switch (MapModS.GS.pinStyle)
            {
                case PinStyle.Normal:
                    styleText += Localization.Localize("normal");
                    break;

                case PinStyle.Q_Marks_1:
                    styleText += $"{Localization.Localize("q marks")} 1";
                    break;

                case PinStyle.Q_Marks_2:
                    styleText += $"{Localization.Localize("q marks")} 2";
                    break;

                case PinStyle.Q_Marks_3:
                    styleText += $"{Localization.Localize("q marks")} 3";
                    break;
            }

            _mapDisplayPanel.GetText("Style").UpdateText(styleText);
        }

        private static void SetSize()
        {
            string sizeText = $"{Localization.Localize("Size")} (ctrl-5): ";

            switch (MapModS.GS.pinSize)
            {
                case PinSize.Small:
                    sizeText += Localization.Localize("small");
                    break;

                case PinSize.Medium:
                    sizeText += Localization.Localize("medium");
                    break;

                case PinSize.Large:
                    sizeText += Localization.Localize("large");
                    break;
            }

            _mapDisplayPanel.GetText("Size").UpdateText(sizeText);
        }

        private static void SetRefresh()
        {
            if (MapModS.LS.ModEnabled)
            {
                _refreshDisplayPanel.GetText("Refresh").UpdateText(Localization.Localize("MapModS enabled. Close map to refresh"));
            }
            else
            {
                _refreshDisplayPanel.GetText("Refresh").UpdateText(Localization.Localize("MapModS disabled. Close map to refresh"));
            }
        }
    }
}