using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapChanger;
using MapChanger.UI;
using RandoMapMod.Modes;
using RandoMapMod.Settings;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    internal class ControlPanel : WorldMapStack
    {
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

        protected override HorizontalAlignment StackHorizontalAlignment => HorizontalAlignment.Left;
        protected override VerticalAlignment StackVerticalAlignment => VerticalAlignment.Bottom;

        protected override void BuildStack()
        {
            panel = new(Root, SpriteManager.Instance.GetTexture("GUI.PanelLeft").ToSlicedSprite(200f, 50f, 100f, 50f), "Panel")
            {
                MinWidth = 0f,
                MinHeight = 0f,
                Borders = new(10f, 20f, 30f, 20f),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            Stack.Children.Add(panel);

            ((Image)Root.GetElement("Panel Background")).Tint = RmmColors.GetColor(RmmColorSetting.UI_Borders);

            panelContents = new(Root, "Panel Contents")
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Vertical
            };

            panel.Child = panelContents;

            control = UIExtensions.PanelText(Root, "Collapsed");
            panelContents.Children.Add(control);

            modEnabled = UIExtensions.PanelText(Root, "Mod Enabled");
            panelContents.Children.Add(modEnabled);
            modEnabled.Text = $"Ctrl-M: {L.Localize("Disable mod")}";

            shiftPan = UIExtensions.PanelText(Root, "Shift Pan");
            panelContents.Children.Add(shiftPan);
            shiftPan.Text = $"{L.Localize("Hold Shift")}: {L.Localize("Pan faster")}";

            mapKey = UIExtensions.PanelText(Root, "Map Key");
            panelContents.Children.Add(mapKey);

            lookup = UIExtensions.PanelText(Root, "Lookup");
            panelContents.Children.Add(lookup);

            benchwarpWorldMap = UIExtensions.PanelText(Root, "Benchwarp World Map");
            panelContents.Children.Add(benchwarpWorldMap);

            benchwarpSearch = UIExtensions.PanelText(Root, "Benchwarp Search");
            panelContents.Children.Add(benchwarpSearch);

            uncheckedVisited = UIExtensions.PanelText(Root, "Unchecked");
            panelContents.Children.Add(uncheckedVisited);

            routeInGame = UIExtensions.PanelText(Root, "Route In Game");
            panelContents.Children.Add(routeInGame);

            whenOffRoute = UIExtensions.PanelText(Root, "Off-route");
            panelContents.Children.Add(whenOffRoute);

            compass = UIExtensions.PanelText(Root, "Compass");
            panelContents.Children.Add(compass);
        }

        protected override bool Condition()
        {
            return base.Condition() && Conditions.RandoMapModEnabled();
        }

        public override void Update()
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

            if (RandoMapMod.GS.ControlPanelOn)
            {
                modEnabled.Visibility = Visibility.Visible;
                shiftPan.Visibility = Visibility.Visible;
                mapKey.Visibility = Visibility.Visible;
                lookup.Visibility = Visibility.Visible;

                if (MapChanger.Settings.CurrentMode().GetType().IsSubclassOf(typeof(TransitionMode)))
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

        private static void UpdateControl()
        {
            if (RandoMapMod.GS.ControlPanelOn)
            {
                control.Text = $"Ctrl-H: {L.Localize("Hide hotkeys")}";
            }
            else
            {
                control.Text = $"Ctrl-H: {L.Localize("More hotkeys")}";
            }
        }

        private static void UpdateMapKey()
        {
            UIExtensions.SetToggleText
                (
                    mapKey,
                    $"{L.Localize("Toggle map key")} (Ctrl-K): ",
                    RandoMapMod.GS.MapKeyOn
                );
        }

        private static void UpdateLookup()
        {
            UIExtensions.SetToggleText
                (
                    lookup,
                    $"{L.Localize("Toggle lookup")} (Ctrl-L): ",
                    RandoMapMod.GS.LookupOn
                );
        }

        private static void UpdateBenchwarpWorldMap()
        {
            if (Interop.HasBenchwarp())
            {
                UIExtensions.SetToggleText
                    (
                        benchwarpWorldMap,
                        $"{L.Localize("Benchwarp selection")} (Ctrl-W): ",
                        RandoMapMod.GS.BenchwarpWorldMap
                    );
            }
            else
            {
                benchwarpWorldMap.Text = "Benchwarp is not installed or outdated";
            }
        }

        private static void UpdateBenchwarpSearch()
        {
            if (Interop.HasBenchwarp())
            {
                UIExtensions.SetToggleText
                    (
                        benchwarpSearch,
                        $"{L.Localize("Include benchwarp")} (Ctrl-B): ",
                        RandoMapMod.GS.AllowBenchWarpSearch
                    );
            }
            else
            {
                benchwarpSearch.Text = "Benchwarp is not installed or outdated";
            }
        }

        private static void UpdateUnchecked()
        {
            UIExtensions.SetToggleText
                (
                    uncheckedVisited,
                    $"{L.Localize("Show unchecked/visited")} (Ctrl-U): ",
                    RandoMapMod.GS.ShowUncheckedPanel
                );
        }

        private static void UpdateRouteInGame()
        {
            string text = $"{L.Localize("Show route in-game")} (Ctrl-R): ";

            switch (RandoMapMod.GS.RouteTextInGame)
            {
                case RouteTextInGame.Hide:
                    routeInGame.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                    text += L.Localize("Off");
                    break;
                case RouteTextInGame.Show:
                    routeInGame.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                    text += L.Localize("Full");
                    break;
                case RouteTextInGame.NextTransitionOnly:
                    routeInGame.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                    text += L.Localize("Next transition only");
                    break;
            }

            routeInGame.Text = text;
        }

        private static void UpdateOffRoute()
        {
            string text = $"{L.Localize("When off-route")} (Ctrl-E): ";

            switch (RandoMapMod.GS.WhenOffRoute)
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

        private static void UpdateCompass()
        {
            UIExtensions.SetToggleText
                (
                    compass,
                    $"{L.Localize("Show route compass")} (Ctrl-C): ",
                    RandoMapMod.GS.ShowRouteCompass
                );
        }
    }
}
