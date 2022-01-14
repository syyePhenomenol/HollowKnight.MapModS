using MapModS.CanvasUtil;
using MapModS.Map;
using MapModS.Settings;
using System.Linq;
using UnityEngine;

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
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(10f, 1040f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _mapDisplayPanel.AddText("Spoilers", "", new Vector2(430f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);
            _mapDisplayPanel.AddText("Randomized", "", new Vector2(630f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);
            _mapDisplayPanel.AddText("Others", "", new Vector2(870f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);
            _mapDisplayPanel.AddText("Style", "", new Vector2(1070f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);
            _mapDisplayPanel.AddText("Size", "", new Vector2(1290f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);

            _refreshDisplayPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(10f, 1040f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _refreshDisplayPanel.AddText("Refresh", "", new Vector2(750f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);

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
            if (GameManager.instance.gameMap == null) return;

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
            _mapDisplayPanel.GetText("Spoilers").UpdateText
                (
                    MapModS.LS.SpoilerOn ? "Spoilers (ctrl-1): on" : "Spoilers (ctrl-1): off"
                );
            _mapDisplayPanel.GetText("Spoilers").SetTextColor
                (
                    MapModS.LS.SpoilerOn ? Color.green : Color.white
                );
        }

        private static void SetRandomized()
        {
            if (WorldMap.CustomPins.RandomizedGroups.Any(MapModS.LS.GetOnFromGroup)
                && !WorldMap.CustomPins.RandomizedGroups.All(MapModS.LS.GetOnFromGroup))
            {
                _mapDisplayPanel.GetText("Randomized").SetTextColor(Color.yellow);
                _mapDisplayPanel.GetText("Randomized").UpdateText("Randomized (ctrl-2): custom");
            }
            else if (MapModS.LS.RandomizedOn)
            {
                _mapDisplayPanel.GetText("Randomized").SetTextColor(Color.green);
                _mapDisplayPanel.GetText("Randomized").UpdateText("Randomized (ctrl-2): on");
            }
            else
            {
                _mapDisplayPanel.GetText("Randomized").SetTextColor(Color.white);
                _mapDisplayPanel.GetText("Randomized").UpdateText("Randomized (ctrl-2): off");
            }
        }

        private static void SetOthers()
        {
            if (WorldMap.CustomPins.OthersGroups.Any(MapModS.LS.GetOnFromGroup)
                && !WorldMap.CustomPins.OthersGroups.All(MapModS.LS.GetOnFromGroup))
            {
                _mapDisplayPanel.GetText("Others").SetTextColor(Color.yellow);
                _mapDisplayPanel.GetText("Others").UpdateText("Others (ctrl-3): custom");
            }
            else if (MapModS.LS.OthersOn)
            {
                _mapDisplayPanel.GetText("Others").SetTextColor(Color.green);
                _mapDisplayPanel.GetText("Others").UpdateText("Others (ctrl-3): on");
            }
            else
            {
                _mapDisplayPanel.GetText("Others").SetTextColor(Color.white);
                _mapDisplayPanel.GetText("Others").UpdateText("Others (ctrl-3): off");
            }
        }

        private static void SetStyle()
        {
            switch (MapModS.LS.pinStyle)
            {
                case PinStyle.Normal:
                    _mapDisplayPanel.GetText("Style").UpdateText("Style (ctrl-4): normal");
                    break;

                case PinStyle.Q_Marks_1:
                    _mapDisplayPanel.GetText("Style").UpdateText("Style (ctrl-4): q marks 1");
                    break;

                case PinStyle.Q_Marks_2:
                    _mapDisplayPanel.GetText("Style").UpdateText("Style (ctrl-4): q marks 2");
                    break;

                case PinStyle.Q_Marks_3:
                    _mapDisplayPanel.GetText("Style").UpdateText("Style (ctrl-4): q marks 3");
                    break;
            }
        }

        private static void SetSize()
        {
            switch (MapModS.GS.pinSize)
            {
                case PinSize.Small:
                    _mapDisplayPanel.GetText("Size").UpdateText("Size (ctrl-5): small");
                    break;

                case PinSize.Medium:
                    _mapDisplayPanel.GetText("Size").UpdateText("Size (ctrl-5): medium");
                    break;

                case PinSize.Large:
                    _mapDisplayPanel.GetText("Size").UpdateText("Size (ctrl-5): large");
                    break;
            }
        }

        private static void SetRefresh()
        {
            if (MapModS.LS.ModEnabled)
            {
                _refreshDisplayPanel.GetText("Refresh").UpdateText("MapMod S enabled. Close map to refresh");
            }
            else
            {
                _refreshDisplayPanel.GetText("Refresh").UpdateText("MapMod S disabled. Close map to refresh");
            }
        }
    }
}