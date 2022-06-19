using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapModS.Data;
using MapModS.Map;
using System.Linq;
using L = RandomizerMod.Localization;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.UI
{
    internal class LookupText
    {
        private static LayoutRoot layout;

        private static Panel panel;
        private static TextObject panelText;

        private static string selectedLocation = "None selected";

        public static bool Condition()
        {
            return GUI.worldMapOpen
                && MapModS.LS.modEnabled
                && !GUI.lockToggleEnable;
        }

        public static void Build()
        {
            if (layout == null)
            {
                layout = new(true, "Lookup");
                layout.VisibilityCondition = Condition;

                panel = new(layout, GUIController.Instance.Images["panelRight"].ToSlicedSprite(100f, 50f, 200f, 50f), "Panel")
                {
                    Borders = new(30f, 30f, 30f, 30f),
                    MinWidth = 400f,
                    MinHeight = 100f,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Padding = new(10f, 170f, 160f, 10f)
                };

                ((Image)layout.GetElement("Panel Background")).Tint = Colors.GetColor(ColorSetting.UI_Borders);

                panelText = new(layout, "Panel Text")
                {
                    ContentColor = Colors.GetColor(ColorSetting.UI_Neutral),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    TextAlignment = HorizontalAlignment.Left,
                    Font = MagicUI.Core.UI.Perpetua,
                    FontSize = 20,
                    MaxWidth = 450f
                };

                panel.Child = panelText;

                selectedLocation = "None selected";

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
            UpdatePanel();
        }

        public static void UpdatePanel()
        {
            string text = $"{Utils.ToCleanName(selectedLocation)}";

            PinDef pd = MainData.GetUsedPinDef(selectedLocation);

            if (pd != null)
            {
                text += $"\n\n{L.Localize("Room")}: {pd.sceneName}";

                text += $"\n\n{L.Localize("Status")}:";

                text += pd.pinLocationState switch
                {
                    PinLocationState.UncheckedUnreachable => $" {L.Localize("Randomized, unchecked, unreachable")}",
                    PinLocationState.UncheckedReachable => $" {L.Localize("Randomized, unchecked, reachable")}",
                    PinLocationState.NonRandomizedUnchecked => $" {L.Localize("Not randomized, either unchecked or persistent")}",
                    PinLocationState.OutOfLogicReachable => $" {L.Localize("Randomized, unchecked, reachable through sequence break")}",
                    PinLocationState.Previewed => $" {L.Localize("Randomized, previewed")}",
                    PinLocationState.Cleared => $" {L.Localize("Cleared")}",
                    PinLocationState.ClearedPersistent => $" {L.Localize("Randomized, cleared, persistent")}",
                    _ => ""
                };

                if (MainData.IsInLogicLookup(selectedLocation))
                {
                    text += $"\n\n{L.Localize("Logic")}: {MainData.GetRawLogic(selectedLocation)}";
                }

                if (RM.RS.TrackerData.previewedLocations.Contains(pd.name))
                {
                    text += $"\n\n{L.Localize("Previewed item(s)")}:";

                    string[] previewText = MainData.GetPreviewText(pd.name);

                    if (previewText == null) return;

                    foreach (string textPiece in previewText)
                    {
                        text += $" {Utils.ToCleanPreviewText(textPiece)},";
                    }

                    text = text.Substring(0, text.Length - 1);
                }

                if (MapModS.LS.spoilerOn
                    && pd.randoItems != null
                    && pd.randoItems.Any()
                    && (!RM.RS.TrackerData.previewedLocations.Contains(pd.name)
                        || (RM.RS.TrackerData.previewedLocations.Contains(pd.name)
                            && !pd.canPreviewItem)))
                {
                    text += $"\n\n{L.Localize("Spoiler item(s)")}:";

                    foreach (ItemDef item in pd.randoItems)
                    {
                        text += $" {Utils.ToCleanName(item.itemName)},";
                    }

                    text = text.Substring(0, text.Length - 1);
                }
            }

            panelText.Text = text;

            if (MapModS.GS.lookupOn)
            {
                panel.Visibility = Visibility.Visible;
            }
            else
            {
                panel.Visibility = Visibility.Hidden;
            }
        }

        public static void UpdateSelectedPinCoroutine()
        {
            if (WorldMap.goCustomPins == null
                || WorldMap.CustomPins == null
                || GameManager.instance.IsGamePaused()
                || !Condition()
                || !MapModS.GS.lookupOn)
            {
                return;
            }

            if (WorldMap.CustomPins.GetPinClosestToMiddle(selectedLocation, out selectedLocation))
            {
                UpdateSelectedPin();
            }
        }

        public static void UpdateSelectedPin()
        {
            if (!GUI.worldMapOpen
                || WorldMap.goCustomPins == null
                || WorldMap.CustomPins == null) return;

            WorldMap.CustomPins.ResizePins(selectedLocation);
            UpdateAll();
        }
    }
}
