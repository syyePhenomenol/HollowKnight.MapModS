using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapChanger;
using MapChanger.UI;
using RandoMapMod.Modes;
using RandoMapMod.Pins;
using RandoMapMod.Rooms;
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
        private static TextObject mode;
        private static TextObject shiftPan;

        private static TextObject mapKey;
        private static TextObject pinSelection;
        private static TextObject showReticle;
        private static TextObject lockSelection;

        private static TextObject benchwarpSelection;

        private static TextObject benchwarpSearch;
        private static TextObject roomSelection;
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

            mode = UIExtensions.PanelText(Root, "Mode");
            panelContents.Children.Add(mode);

            shiftPan = UIExtensions.PanelText(Root, "Shift Pan");
            panelContents.Children.Add(shiftPan);
            shiftPan.Text = $"{L.Localize("Hold Shift")}: {L.Localize("Pan faster")}";

            mapKey = UIExtensions.PanelText(Root, "Map Key");
            panelContents.Children.Add(mapKey);

            pinSelection = UIExtensions.PanelText(Root, "Pin Selection");
            panelContents.Children.Add(pinSelection);

            benchwarpSelection = UIExtensions.PanelText(Root, "Benchwarp Selection");
            panelContents.Children.Add(benchwarpSelection);

            roomSelection = UIExtensions.PanelText(Root, "Room Selection");
            panelContents.Children.Add(roomSelection);

            showReticle = UIExtensions.PanelText(Root, "Show Reticle");
            panelContents.Children.Add(showReticle);

            lockSelection = UIExtensions.PanelText(Root, "Lock Selection");
            panelContents.Children.Add(lockSelection);

            benchwarpSearch = UIExtensions.PanelText(Root, "Benchwarp Search");
            panelContents.Children.Add(benchwarpSearch);

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
            UpdateMode();
            UpdateControl();
            UpdateMapKey();
            UpdatePinSelection();
            UpdateBenchwarpSelection();
            UpdateRoomSelection();
            UpdateShowReticle();
            UpdateLockSelection();
            UpdateBenchwarpSearch();
            UpdateRouteInGame();
            UpdateOffRoute();
            UpdateCompass();

            if (RandoMapMod.GS.ControlPanelOn)
            {
                modEnabled.Visibility = Visibility.Visible;
                mode.Visibility = Visibility.Visible;
                shiftPan.Visibility = Visibility.Visible;
                mapKey.Visibility = Visibility.Visible;
                pinSelection.Visibility = Visibility.Visible;
                showReticle.Visibility = Visibility.Visible;
                lockSelection.Visibility = Visibility.Visible;

                if (Conditions.TransitionRandoModeEnabled())
                {
                    benchwarpSelection.Visibility = Visibility.Collapsed;
                    roomSelection.Visibility = Visibility.Visible;
                    benchwarpSearch.Visibility = Visibility.Visible;
                    routeInGame.Visibility = Visibility.Visible;
                    whenOffRoute.Visibility = Visibility.Visible;
                    compass.Visibility = Visibility.Visible;
                }
                else
                {
                    benchwarpSelection.Visibility = Visibility.Visible;
                    roomSelection.Visibility = Visibility.Collapsed;
                    benchwarpSearch.Visibility = Visibility.Collapsed;
                    routeInGame.Visibility = Visibility.Collapsed;
                    whenOffRoute.Visibility = Visibility.Collapsed;
                    compass.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                modEnabled.Visibility = Visibility.Collapsed;
                mode.Visibility = Visibility.Collapsed;
                shiftPan.Visibility = Visibility.Collapsed;
                mapKey.Visibility = Visibility.Collapsed;
                pinSelection.Visibility = Visibility.Collapsed;
                benchwarpSelection.Visibility = Visibility.Collapsed;
                roomSelection.Visibility = Visibility.Collapsed;
                showReticle.Visibility = Visibility.Collapsed;
                lockSelection.Visibility = Visibility.Collapsed;
                benchwarpSearch.Visibility = Visibility.Collapsed;
                routeInGame.Visibility = Visibility.Collapsed;
                whenOffRoute.Visibility = Visibility.Collapsed;
                compass.Visibility = Visibility.Collapsed;
            }
        }

        private static void UpdateMode()
        {
            mode.Text = $"{L.Localize("Mode")} (Ctrl-T): {MapChanger.Settings.CurrentMode().ModeName}";

            if (MapChanger.Settings.CurrentMode() is FullMapMode)
            {
                mode.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_On);
            }
            else if (Conditions.TransitionRandoModeEnabled())
            {
                mode.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Special);
            }
            else
            {
                mode.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral);
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

        private static void UpdatePinSelection()
        {
            UIExtensions.SetToggleText
                (
                    pinSelection,
                    $"{L.Localize("Toggle pin selection")} (Ctrl-P): ",
                    RandoMapMod.GS.PinSelectionOn
                );
        }

        private static void UpdateBenchwarpSelection()
        {
            if (Interop.HasBenchwarp())
            {
                UIExtensions.SetToggleText
                    (
                        benchwarpSelection,
                        $"{L.Localize("Benchwarp selection")} (Ctrl-W): ",
                        RandoMapMod.GS.BenchwarpSelectionOn
                    );
            }
            else
            {
                benchwarpSelection.Text = "Benchwarp is not installed or outdated";
            }
        }

        private static void UpdateRoomSelection()
        {
            UIExtensions.SetToggleText
                (
                    roomSelection,
                    $"{L.Localize("Toogle room selection")} (Ctrl-R): ",
                    RandoMapMod.GS.RoomSelectionOn
                );
        }

        private static void UpdateShowReticle()
        {
            UIExtensions.SetToggleText
                (
                    showReticle,
                    $"{L.Localize("Show reticles")} (Ctrl-S): ",
                    RandoMapMod.GS.ShowReticle
                );
        }

        private static void UpdateLockSelection()
        {
            UIExtensions.SetToggleText
                (
                    lockSelection,
                    $"{L.Localize("Lock selections")} (Ctrl-L): ",
                    RmmPinSelector.Instance.LockSelection
                        || BenchwarpRoomSelector.Instance.LockSelection
                        || TransitionRoomSelector.Instance.LockSelection
                );
        }

        private static void UpdateBenchwarpSearch()
        {
            if (Interop.HasBenchwarp())
            {
                UIExtensions.SetToggleText
                    (
                        benchwarpSearch,
                        $"{L.Localize("Pathfinder benchwarp")} (Ctrl-B): ",
                        RandoMapMod.GS.AllowBenchWarpSearch
                    );
            }
            else
            {
                benchwarpSearch.Text = "Benchwarp is not installed or outdated";
            }
        }

        private static void UpdateRouteInGame()
        {
            string text = $"{L.Localize("Show route in-game")} (Ctrl-G): ";

            switch (RandoMapMod.GS.RouteTextInGame)
            {
                case RouteTextInGame.Hide:
                    routeInGame.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral);
                    text += L.Localize("Off");
                    break;
                case RouteTextInGame.Show:
                    routeInGame.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_On);
                    text += L.Localize("Full");
                    break;
                case RouteTextInGame.NextTransitionOnly:
                    routeInGame.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_On);
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
                    whenOffRoute.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral);
                    text += L.Localize("Keep route");
                    break;
                case OffRouteBehaviour.Cancel:
                    whenOffRoute.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral);
                    text += L.Localize("Cancel route");
                    break;
                case OffRouteBehaviour.Reevaluate:
                    whenOffRoute.ContentColor = RmmColors.GetColor(RmmColorSetting.UI_On);
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
