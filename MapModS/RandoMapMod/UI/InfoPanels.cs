using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapModS.Data;
using MapModS.Map;
using System.Linq;
using System.Threading;
using L = RandomizerMod.Localization;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.UI
{
    internal class InfoPanels
    {
        private static LayoutRoot layout;

        private static StackLayout stackLayout;

        private static Panel lookupPanel;
        private static TextObject lookupPanelText;

        private static string selectedLocation = "None selected";

        private static Panel uncheckedPanel;
        private static TextObject uncheckedPanelText;

        public static string selectedScene = "None";

        public static bool Condition()
        {
            return GUI.worldMapOpen
                && MapModS.LS.ModEnabled
                && !GUI.lockToggleEnable;
        }

        public static void Build()
        {
            if (layout == null)
            {
                layout = new(true, "Lookup");
                layout.VisibilityCondition = Condition;

                stackLayout = new(layout, "Info Panels")
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Spacing = 10f,
                    Padding = new(10f, 170f, 160f, 10f)
                };

                lookupPanel = new(layout, GUIController.Instance.Images["panelRight"].ToSlicedSprite(100f, 50f, 200f, 50f), "Lookup Panel")
                {
                    Borders = new(30f, 30f, 30f, 30f),
                    MinWidth = 400f,
                    MinHeight = 100f,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    //Padding = new(10f, 170f, 160f, 10f)
                };

                ((Image)layout.GetElement("Lookup Panel Background")).Tint = Colors.GetColor(ColorSetting.UI_Borders);

                lookupPanelText = new(layout, "Lookup Panel Text")
                {
                    ContentColor = Colors.GetColor(ColorSetting.UI_Neutral),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    TextAlignment = HorizontalAlignment.Left,
                    Font = MagicUI.Core.UI.Perpetua,
                    FontSize = 20,
                    MaxWidth = 450f
                };

                lookupPanel.Child = lookupPanelText;

                stackLayout.Children.Add(lookupPanel);

                selectedLocation = "None selected";

                uncheckedPanel = new(layout, GUIController.Instance.Images["panelRight"].ToSlicedSprite(100f, 50f, 250f, 50f), "Unchecked Panel")
                {
                    Borders = new(30f, 30f, 30f, 30f),
                    MinWidth = 200f,
                    MinHeight = 100f,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    //Padding = new(10f, 170f, 160f, 10f)
                };

                ((Image)layout.GetElement("Unchecked Panel Background")).Tint = Colors.GetColor(ColorSetting.UI_Borders);

                uncheckedPanelText = new(layout, "Unchecked Panel Text")
                {
                    ContentColor = Colors.GetColor(ColorSetting.UI_Neutral),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = MagicUI.Core.UI.TrajanNormal,
                    FontSize = 14
                };

                uncheckedPanel.Child = uncheckedPanelText;

                stackLayout.Children.Add(uncheckedPanel);

                selectedScene = "None";
            }
        }

        public static void Destroy()
        {
            layout?.Destroy();
            layout = null;
        }

        public static void UpdateAll()
        {
            UpdateLookupPanel();
            UpdateUncheckedPanel();
        }

        public static void UpdateLookupPanel()
        {
            if (MapModS.GS.LookupOn)
            {
                string text = $"{Utils.ToCleanName(selectedLocation)}";

                //OldPin pd = MainData.GetUsedPinDef(selectedLocation);

                //if (pd != null)
                //{
                //    text += $"\n\n{L.Localize("Room")}: {pd.sceneName}";

                //    text += $"\n\n{L.Localize("Status")}:";

                //    text += pd.pinLocationState switch
                //    {
                //        PinState.UncheckedUnreachable => $" {L.Localize("Randomized, unchecked, unreachable")}",
                //        PinState.UncheckedReachable => $" {L.Localize("Randomized, unchecked, reachable")}",
                //        PinState.NonRandomizedUnchecked => $" {L.Localize("Not randomized, either unchecked or persistent")}",
                //        PinState.OutOfLogicReachable => $" {L.Localize("Randomized, unchecked, reachable through sequence break")}",
                //        PinState.Previewed => $" {L.Localize("Randomized, previewed")}",
                //        PinState.Cleared => $" {L.Localize("Cleared")}",
                //        PinState.ClearedPersistent => $" {L.Localize("Randomized, cleared, persistent")}",
                //        _ => ""
                //    };

                //    if (MainData.IsInLogicLookup(selectedLocation))
                //    {
                //        text += $"\n\n{L.Localize("Logic")}: {MainData.GetRawLogic(selectedLocation)}";
                //    }

                //    if (RM.RS.TrackerData.previewedLocations.Contains(pd.name))
                //    {
                //        text += $"\n\n{L.Localize("Previewed item(s)")}:";

                //        string[] previewText = pd.name.PreviewText();

                //        if (previewText == null) return;

                //        foreach (string textPiece in previewText)
                //        {
                //            text += $" {Utils.ToCleanPreviewText(textPiece)},";
                //        }

                //        text = text.Substring(0, text.Length - 1);
                //    }

                //    if (MapModS.LS.spoilerOn
                //        && pd.randoItems != null
                //        && pd.randoItems.Any()
                //        && (!RM.RS.TrackerData.previewedLocations.Contains(pd.name)
                //            || (RM.RS.TrackerData.previewedLocations.Contains(pd.name)
                //                && !pd.canPreviewItem)))
                //    {
                //        text += $"\n\n{L.Localize("Spoiler item(s)")}:";

                //        foreach (ItemDef item in pd.randoItems)
                //        {
                //            text += $" {Utils.ToCleanName(item.itemName)},";
                //        }

                //        text = text.Substring(0, text.Length - 1);
                //    }
                //}

                lookupPanelText.Text = text;
                lookupPanel.Visibility = Visibility.Visible;
            }
            else
            {
                lookupPanel.Visibility = Visibility.Collapsed;
            }
        }

        // Called every 0.1 seconds
        public static void UpdateSelectedPinCoroutine()
        {
            if (!GUI.worldMapOpen
                //|| WorldMap.goCustomPins == null
                //|| WorldMap.CustomPins == null
                || GameManager.instance.IsGamePaused()
                || !Condition()
                || !MapModS.GS.LookupOn)
            {
                return;
            }

            //if (WorldMap.CustomPins.GetPinClosestToMiddle(selectedLocation, out selectedLocation))
            //{
            //    UpdateSelectedPin();
            //}
        }

        //public static void UpdateSelectedPin()
        //{
        //    if (!GUI.worldMapOpen
        //        || WorldMap.goCustomPins == null
        //        || WorldMap.CustomPins == null) return;

        //    WorldMap.CustomPins.ResizePins(selectedLocation);
        //    UpdateAll();
        //}

        private static Thread colorUpdateThread;

        // Called every 0.1 seconds
        public static void UpdateSelectedScene()
        {
            if (layout == null
                || GUI.lockToggleEnable
                || GameManager.instance.IsGamePaused()
                || !TransitionData.TransitionModeActive())
            {
                return;
            }

            if (GUI.worldMapOpen)
            {
                if (colorUpdateThread != null && colorUpdateThread.IsAlive) return;

                colorUpdateThread = new(() =>
                {
                    if (MapRooms.GetRoomClosestToMiddle(selectedScene, out selectedScene))
                    {
                        MapRooms.SetSelectedRoomColor(selectedScene, true);
                        TransitionPersistent.UpdateAll();
                        TransitionWorldMap.UpdateAll();
                        UpdateAll();
                    }
                });

                colorUpdateThread.Start();
            }
            else if (GUI.quickMapOpen)
            {
                MapRooms.SetSelectedRoomColor("", true);
            }
        }

        public static void UpdateUncheckedPanel()
        {
            if (TransitionData.TransitionModeActive() && MapModS.GS.ShowUncheckedPanel)
            {
                uncheckedPanelText.Text = selectedScene + "\n\n";
                uncheckedPanelText.Text += TransitionData.GetUncheckedVisited(selectedScene);
                uncheckedPanel.Visibility = Visibility.Visible;
            }
            else
            {
                uncheckedPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
