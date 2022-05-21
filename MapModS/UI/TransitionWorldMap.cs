using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapModS.Data;
using MapModS.Settings;
using System.Collections.Generic;
using System.Linq;
using L = RandomizerMod.Localization;
using TP = MapModS.UI.TransitionPersistent;

namespace MapModS.UI
{
    internal class TransitionWorldMap
    {
        private static LayoutRoot layout;

        private static bool Condition()
        {
            return GUI.worldMapOpen && TransitionData.TransitionModeActive();
        }

        public static void Build()
        {
            if (layout == null)
            {
                layout = new(true, "Transition World Map");
                layout.VisibilityCondition = Condition;

                TextObject instruction = new(layout, "Instructions")
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Font = MagicUI.Core.UI.TrajanNormal,
                    FontSize = 14,
                    Padding = new(20f, 20f, 10f, 10f)
                };

                TextObject control = new(layout, "Control")
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    TextAlignment = HorizontalAlignment.Right,
                    Font = MagicUI.Core.UI.TrajanNormal,
                    FontSize = 14,
                    Padding = new(10f, 20f, 20f, 10f)
                };

                Image panelBackground = new(layout, GUIController.Instance.Images["UncheckedBG"].ToSprite(), "Background")
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Padding = new(1380f, 170f, 10f, 10f)
                };

                TextObject panelText = new(layout, "Panel Text")
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Font = MagicUI.Core.UI.TrajanNormal,
                    FontSize = 14,
                    MaxWidth = 485f,
                    Padding = new(1400f, 190f, 10f, 10f)
                };

                UpdateAll();
            }
        }

        public static void Destroy()
        {
            layout.Destroy();
            layout = null;
        }

        public static void UpdateAll()
        {
            UpdateInstructions((TextObject)layout.GetElement("Instructions"));
            UpdateControl((TextObject)layout.GetElement("Control"));

            Image background = (Image)layout.GetElement("Background");
            TextObject panelText = (TextObject)layout.GetElement("Panel Text");

            UpdatePanelText(panelText);

            if (MapModS.GS.uncheckedPanelActive)
            {
                background.Visibility = Visibility.Visible;
                panelText.Visibility = Visibility.Visible;
            }
            else
            {
                background.Visibility = Visibility.Hidden;
                panelText.Visibility = Visibility.Hidden;
            }
        }

        public static void UpdateInstructions(TextObject textObj)
        {
            string text = $"{L.Localize("Selected room")}: {TP.selectedScene}.";

            List<InControl.BindingSource> bindings = new(InputHandler.Instance.inputActions.menuSubmit.Bindings);

            if (TP.selectedScene == Utils.CurrentScene())
            {
                text += $" {L.Localize("You are here")}.";
            }
            else
            {
                text += $" {L.Localize("Press")} ";

                text += $"[{bindings.First().Name}]";

                if (bindings.Count > 1 && bindings[1].BindingSourceType == InControl.BindingSourceType.DeviceBindingSource)
                {
                    text += $" {L.Localize("or")} ";

                    text += $"({bindings[1].Name})";
                }

                text += $" {L.Localize("to find new route or switch starting / final transitions")}.";
            }

            textObj.Text = text;
        }

        public static void UpdateControl(TextObject textObj)
        {
            string text = $"{L.Localize("Current route")}: ";

            if (TP.lastStartScene != ""
                && TP.lastFinalScene != ""
                && TP.lastStartTransition != ""
                && TP.lastFinalTransition != ""
                && TP.selectedRoute.Any())
            {
                text += $"{TP.lastStartTransition} ->...-> {TP.lastFinalTransition}      ";
                text += $"{L.Localize("Transitions")}: {TP.selectedRoute.Count()}";
            }
            else
            {
                text += L.Localize("None");
            }

            if (Dependencies.HasDependency("Benchwarp"))
            {
                text += $"\n{L.Localize("Include benchwarp")} (Ctrl-B): ";

                if (MapModS.GS.allowBenchWarpSearch)
                {
                    text += L.Localize("On");
                }
                else
                {
                    text += L.Localize("Off");
                }
            }

            text += $"\n{L.Localize("Show unchecked/visited")} (Ctrl-U): ";

            if (MapModS.GS.uncheckedPanelActive)
            {
                text += L.Localize("On");
            }
            else
            {
                text += L.Localize("Off");
            }

            text += $"\n{L.Localize("Show route in-game")} (Ctrl-R): ";

            switch (MapModS.GS.routeTextInGame)
            {
                case RouteTextInGame.Hide:
                    text += L.Localize("Off");
                    break;
                case RouteTextInGame.Show:
                    text += L.Localize("Full");
                    break;
                case RouteTextInGame.ShowNextTransitionOnly:
                    text += L.Localize("Next Transition Only");
                    break;
            }

            text += $"\n{L.Localize("Show route compass")} (Ctrl-C): ";

            if (MapModS.GS.routeCompassEnabled)
            {
                text += L.Localize("On");
            }
            else
            {
                text += L.Localize("Off");
            }

            textObj.Text = text;
        }

        public static void UpdatePanelText(TextObject textObj)
        {
            textObj.Text = TransitionData.GetUncheckedVisited(TP.selectedScene);
        }
    }
}
