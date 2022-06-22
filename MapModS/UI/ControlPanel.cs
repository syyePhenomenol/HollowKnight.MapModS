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

        private static TextObject benchwarpWorldMap;

        private static TextObject benchwarpSearch;
        private static TextObject uncheckedVisited;
        private static TextObject routeInGame;
        private static TextObject whenOffRoute;
        private static TextObject compass;

        public static bool Condition()
        {
            return MapModS.LS.modEnabled
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

                ((Image)layout.GetElement("Panel Background")).Tint = Colors.GetColor(ColorSetting.UI_Borders);

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
                modEnabled.Text = $"Ctrl-M: {L.Localize("Disable mod")}";

                shiftPan = UIExtensions.PanelText(layout, "Shift Pan");
                panelContents.Children.Add(shiftPan);
                shiftPan.Text = $"{L.Localize("Hold Shift")}: {L.Localize("Pan faster")}";

                mapKey = UIExtensions.PanelText(layout, "Map Key");
                panelContents.Children.Add(mapKey);

                lookup = UIExtensions.PanelText(layout, "Lookup");
                panelContents.Children.Add(lookup);

                benchwarpWorldMap = UIExtensions.PanelText(layout, "Benchwarp World Map");
                panelContents.Children.Add(benchwarpWorldMap);

                benchwarpSearch = UIExtensions.PanelText(layout, "Benchwarp Search");
                panelContents.Children.Add(benchwarpSearch);

                uncheckedVisited = UIExtensions.PanelText(layout, "Unchecked");
                panelContents.Children.Add(uncheckedVisited);

                routeInGame = UIExtensions.PanelText(layout, "Route In Game");
                panelContents.Children.Add(routeInGame);

                whenOffRoute = UIExtensions.PanelText(layout, "Off-route");
                panelContents.Children.Add(whenOffRoute);

                compass = UIExtensions.PanelText(layout, "Compass");
                panelContents.Children.Add(compass);

                layout.ListenForHotkey(KeyCode.H, () =>
                {
                    MapModS.GS.ToggleControlPanel();
                    UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.modEnabled);

                layout.ListenForHotkey(KeyCode.K, () =>
                {
                    MapModS.GS.ToggleMapKey();
                    UpdateAll();
                    MapKey.UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.modEnabled);

                layout.ListenForHotkey(KeyCode.L, () =>
                {
                    MapModS.GS.ToggleLookup();

                    if (MapModS.GS.lookupOn)
                    {
                        LookupText.UpdateSelectedPin();
                    }
                    else
                    {
                        WorldMap.CustomPins.ResizePins("None selected");
                    }

                    UpdateAll();
                    LookupText.UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.modEnabled);
                
                if (Dependencies.HasBenchwarp())
                {
                    layout.ListenForHotkey(KeyCode.W, () =>
                    {
                        MapModS.GS.ToggleBenchwarpWorldMap();
                        UpdateAll();
                        Benchwarp.UpdateAll();
                    }, ModifierKeys.Ctrl, () => MapModS.LS.modEnabled);

                    layout.ListenForHotkey(KeyCode.B, () =>
                    {
                        MapModS.GS.ToggleAllowBenchWarp();
                        TransitionPersistent.ResetRoute();
                        RouteCompass.UpdateCompass();
                        UpdateAll();
                        TransitionPersistent.UpdateAll();
                        TransitionWorldMap.UpdateAll();
                    }, ModifierKeys.Ctrl, () => MapModS.LS.modEnabled);
                }

                layout.ListenForHotkey(KeyCode.U, () =>
                {
                    MapModS.GS.ToggleUncheckedPanel();
                    UpdateAll();
                    TransitionWorldMap.UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.modEnabled);

                layout.ListenForHotkey(KeyCode.R, () =>
                {
                    MapModS.GS.ToggleRouteTextInGame();
                    UpdateAll();
                    TransitionPersistent.UpdateAll();
                    TransitionWorldMap.UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.modEnabled);

                layout.ListenForHotkey(KeyCode.E, () =>
                {
                    MapModS.GS.ToggleWhenOffRoute();
                    UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.modEnabled);

                layout.ListenForHotkey(KeyCode.C, () =>
                {
                    MapModS.GS.ToggleRouteCompassEnabled();
                    UpdateAll();
                    TransitionWorldMap.UpdateAll();
                }, ModifierKeys.Ctrl, () => MapModS.LS.modEnabled);

#if DEBUG
                layout.ListenForHotkey(KeyCode.Alpha6, () =>
                {
                    MainData.LoadDebugResources();
                    WorldMap.CustomPins.ReadjustPinPostiions();
                    MapRooms.ReadjustRoomPostiions();
                }, ModifierKeys.Ctrl);
#endif
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
            UpdateBenchwarpWorldMap();
            UpdateBenchwarpSearch();
            UpdateUnchecked();
            UpdateRouteInGame();
            UpdateOffRoute();
            UpdateCompass();

            if (MapModS.GS.controlPanelOn)
            {
                modEnabled.Visibility = Visibility.Visible;
                shiftPan.Visibility = Visibility.Visible;
                mapKey.Visibility = Visibility.Visible;
                lookup.Visibility = Visibility.Visible;

                if (MapModS.LS.mapMode == MapMode.TransitionRando
                    || MapModS.LS.mapMode == MapMode.TransitionRandoAlt)
                {
                    benchwarpWorldMap.Visibility = Visibility.Collapsed;
                    benchwarpSearch.Visibility = Visibility.Visible;
                    uncheckedVisited.Visibility = Visibility.Visible;
                    routeInGame.Visibility = Visibility.Visible;
                    whenOffRoute.Visibility = Visibility.Visible;
                    compass.Visibility = Visibility.Visible;
                }
                else
                {
                    benchwarpWorldMap.Visibility = Visibility.Visible;
                    benchwarpSearch.Visibility = Visibility.Collapsed;
                    uncheckedVisited.Visibility = Visibility.Collapsed;
                    routeInGame.Visibility = Visibility.Collapsed;
                    whenOffRoute.Visibility = Visibility.Collapsed;
                    compass.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                modEnabled.Visibility = Visibility.Collapsed;
                shiftPan.Visibility = Visibility.Collapsed;
                mapKey.Visibility = Visibility.Collapsed;
                lookup.Visibility = Visibility.Collapsed;
                benchwarpWorldMap.Visibility = Visibility.Collapsed;
                benchwarpSearch.Visibility = Visibility.Collapsed;
                uncheckedVisited.Visibility = Visibility.Collapsed;
                routeInGame.Visibility = Visibility.Collapsed;
                whenOffRoute.Visibility = Visibility.Collapsed;
                compass.Visibility = Visibility.Collapsed;
            }
        }

        public static void UpdateControl()
        {
            if (MapModS.GS.controlPanelOn)
            {
                control.Text = $"Ctrl-H: {L.Localize("Hide hotkeys")}";
            }
            else
            {
                control.Text = $"Ctrl-H: {L.Localize("More hotkeys")}";
            }
        }

        public static void UpdateMapKey()
        {
            UIExtensions.SetToggleText
                (
                    mapKey,
                    $"{L.Localize("Toggle map key")} (Ctrl-K): ",
                    MapModS.GS.mapKeyOn
                );
        }

        public static void UpdateLookup()
        {
            UIExtensions.SetToggleText
                (
                    lookup,
                    $"{L.Localize("Toggle lookup")} (Ctrl-L): ",
                    MapModS.GS.lookupOn
                );
        }

        public static void UpdateBenchwarpWorldMap()
        {
            if (Dependencies.HasBenchwarp())
            {
                UIExtensions.SetToggleText
                    (
                        benchwarpWorldMap,
                        $"{L.Localize("Benchwarp selection")} (Ctrl-W): ",
                        MapModS.GS.benchwarpWorldMap
                    );
            }
            else
            {
                benchwarpWorldMap.Text = "Benchwarp is not installed or outdated";
            }
        }

        public static void UpdateBenchwarpSearch()
        {
            if (Dependencies.HasBenchwarp())
            {
                UIExtensions.SetToggleText
                    (
                        benchwarpSearch,
                        $"{L.Localize("Include benchwarp")} (Ctrl-B): ",
                        MapModS.GS.allowBenchWarpSearch
                    );
            }
            else
            {
                benchwarpSearch.Text = "Benchwarp is not installed or outdated";
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
                    routeInGame.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                    text += L.Localize("Off");
                    break;
                case RouteTextInGame.Show:
                    routeInGame.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                    text += L.Localize("Full");
                    break;
                case RouteTextInGame.ShowNextTransitionOnly:
                    routeInGame.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                    text += L.Localize("Next transition only");
                    break;
            }

            routeInGame.Text = text;
        }

        public static void UpdateOffRoute()
        {
            string text = $"{L.Localize("When off-route")} (Ctrl-E): ";

            switch (MapModS.GS.whenOffRoute)
            {
                case OffRouteBehaviour.Keep:
                    whenOffRoute.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                    text += L.Localize("Keep route");
                    break;
                case OffRouteBehaviour.Cancel:
                    whenOffRoute.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                    text += L.Localize("Cancel route");
                    break;
                case OffRouteBehaviour.Reevaluate:
                    whenOffRoute.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                    text += L.Localize("Reevaluate route");
                    break;
            }

            whenOffRoute.Text = text;
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
