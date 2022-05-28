using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapModS.Data;
using MapModS.Map;
using MapModS.Settings;
using UnityEngine;
using L = RandomizerMod.Localization;

namespace MapModS.UI
{
    internal class ControlPanel
    {
        private static LayoutRoot layout;

        private static Panel panel;
        private static StackLayout panelContents;
        private static TextObject control;

        private static TextObject modEnabled;
        private static TextObject shiftPan;

        private static TextObject mapKey;
        private static TextObject lookup;

        private static TextObject benchwarp;
        private static TextObject uncheckedVisited;
        private static TextObject routeInGame;
        private static TextObject compass;

        public static bool Condition()
        {
            return MapModS.LS.ModEnabled
                && GUI.worldMapOpen
                && !GUI.lockToggleEnable;
        }

        public static void Build()
        {
            if (layout == null)
            {
                layout = new(true, "Control Panel");
                layout.VisibilityCondition = Condition;

                panel = new(layout, GUIController.Instance.Images["panelLeft"].ToSlicedSprite(200f, 50f, 100f, 50f), "Panel")
                {
                    MinWidth = 0f,
                    MinHeight = 0f,
                    Borders = new(10f, 20f, 30f, 20f),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Padding = new(160f, 0f, 0f, 150f)
                };

                panelContents = new(layout, "Panel Contents")
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Orientation = Orientation.Vertical
                };

                panel.Child = panelContents;

                control = UIExtensions.PanelText(layout, "Collapsed");
                panelContents.Children.Add(control);

                modEnabled = UIExtensions.PanelText(layout, "Mod Enabled");
                panelContents.Children.Add(modEnabled);
                modEnabled.Text = "Ctrl-M: Disable mod";

                shiftPan = UIExtensions.PanelText(layout, "Shift Pan");
                panelContents.Children.Add(shiftPan);
                shiftPan.Text = "Hold Shift: Pan faster";

                mapKey = UIExtensions.PanelText(layout, "Map Key");
                panelContents.Children.Add(mapKey);

                lookup = UIExtensions.PanelText(layout, "Lookup");
                panelContents.Children.Add(lookup);

                benchwarp = UIExtensions.PanelText(layout, "Benchwarp");
                panelContents.Children.Add(benchwarp);

                uncheckedVisited = UIExtensions.PanelText(layout, "Unchecked");
                panelContents.Children.Add(uncheckedVisited);

                routeInGame = UIExtensions.PanelText(layout, "Route In Game");
                panelContents.Children.Add(routeInGame);

                compass = UIExtensions.PanelText(layout, "Compass");
                panelContents.Children.Add(compass);

                layout.ListenForHotkey(KeyCode.H, () =>
                {
                    MapModS.GS.ToggleControlPanel();
                    UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.ModEnabled);

                layout.ListenForHotkey(KeyCode.K, () =>
                {
                    MapModS.LS.ToggleMapKey();
                    UpdateAll();
                    MapKey.UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.ModEnabled);

                layout.ListenForHotkey(KeyCode.L, () =>
                {
                    MapModS.LS.ToggleLookup();

                    if (MapModS.LS.lookupOn)
                    {
                        LookupText.UpdateSelectedPin();
                    }
                    else
                    {
                        WorldMap.CustomPins.ResizePins("None selected");
                    }

                    UpdateAll();
                    LookupText.UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.ModEnabled);

                layout.ListenForHotkey(KeyCode.B, () =>
                {
                    MapModS.GS.ToggleAllowBenchWarp();
                    TransitionPersistent.rejectedTransitionPairs = new();
                    UpdateAll();
                    TransitionPersistent.UpdateAll();
                    TransitionWorldMap.UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.ModEnabled);

                layout.ListenForHotkey(KeyCode.U, () =>
                {
                    MapModS.GS.ToggleUncheckedPanel();
                    UpdateAll();
                    TransitionWorldMap.UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.ModEnabled);

                layout.ListenForHotkey(KeyCode.R, () =>
                {
                    MapModS.GS.ToggleRouteTextInGame();
                    UpdateAll();
                    TransitionPersistent.UpdateAll();
                    TransitionWorldMap.UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.ModEnabled);

                layout.ListenForHotkey(KeyCode.C, () =>
                {
                    MapModS.GS.ToggleRouteCompassEnabled();
                    UpdateAll();
                    TransitionWorldMap.UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.ModEnabled);

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
            UpdateControl();
            UpdateMapKey();
            UpdateLookup();
            UpdateBenchwarp();
            UpdateUnchecked();
            UpdateRouteInGame();
            UpdateCompass();

            if (MapModS.GS.controlPanelOn)
            {
                modEnabled.Visibility = Visibility.Visible;
                shiftPan.Visibility = Visibility.Visible;
                mapKey.Visibility = Visibility.Visible;
                lookup.Visibility = Visibility.Visible;

                if (TransitionData.TransitionModeActive())
                {
                    benchwarp.Visibility = Visibility.Visible;
                    uncheckedVisited.Visibility = Visibility.Visible;
                    routeInGame.Visibility = Visibility.Visible;
                    compass.Visibility = Visibility.Visible;
                }
                else
                {
                    benchwarp.Visibility = Visibility.Collapsed;
                    uncheckedVisited.Visibility = Visibility.Collapsed;
                    routeInGame.Visibility = Visibility.Collapsed;
                    compass.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                modEnabled.Visibility = Visibility.Collapsed;
                shiftPan.Visibility = Visibility.Collapsed;
                mapKey.Visibility = Visibility.Collapsed;
                lookup.Visibility = Visibility.Collapsed;
                benchwarp.Visibility = Visibility.Collapsed;
                uncheckedVisited.Visibility = Visibility.Collapsed;
                routeInGame.Visibility = Visibility.Collapsed;
                compass.Visibility = Visibility.Collapsed;
            }
        }

        public static void UpdateControl()
        {
            if (MapModS.GS.controlPanelOn)
            {
                control.Text = "Ctrl-H: Hide hotkeys";
            }
            else
            {
                control.Text = "Ctrl-H: More hotkeys";
            }
        }

        public static void UpdateMapKey()
        {
            UIExtensions.SetToggleText
                (
                    mapKey,
                    $"{L.Localize("Toggle map key")} (Ctrl-K): ",
                    MapModS.LS.mapKeyOn
                );
        }

        public static void UpdateLookup()
        {
            UIExtensions.SetToggleText
                (
                    lookup,
                    $"{L.Localize("Toggle lookup")} (Ctrl-L): ",
                    MapModS.LS.lookupOn
                );
        }

        public static void UpdateBenchwarp()
        {
            if (Dependencies.HasDependency("Benchwarp"))
            {
                UIExtensions.SetToggleText
                    (
                        benchwarp,
                        $"{L.Localize("Include benchwarp")} (Ctrl-B): ",
                        MapModS.GS.allowBenchWarpSearch
                    );
            }
            else
            {
                benchwarp.Text = "Benchwarp is not installed";
            }
        }

        public static void UpdateUnchecked()
        {
            UIExtensions.SetToggleText
                (
                    uncheckedVisited,
                    $"{L.Localize("Show unchecked/visited")} (Ctrl-U): ",
                    MapModS.GS.uncheckedPanelActive
                );
        }

        public static void UpdateRouteInGame()
        {
            string text = $"{L.Localize("Show route in-game")} (Ctrl-R): ";

            switch (MapModS.GS.routeTextInGame)
            {
                case RouteTextInGame.Hide:
                    routeInGame.ContentColor = Color.white;
                    text += L.Localize("Off");
                    break;
                case RouteTextInGame.Show:
                    routeInGame.ContentColor = Color.green;
                    text += L.Localize("Full");
                    break;
                case RouteTextInGame.ShowNextTransitionOnly:
                    routeInGame.ContentColor = Color.green;
                    text += L.Localize("Next transition only");
                    break;
            }

            routeInGame.Text = text;
        }

        public static void UpdateCompass()
        {
            UIExtensions.SetToggleText
                (
                    compass,
                    $"{L.Localize("Show route compass")} (Ctrl-C): ",
                    MapModS.GS.routeCompassEnabled
                );
        }
    }
}
