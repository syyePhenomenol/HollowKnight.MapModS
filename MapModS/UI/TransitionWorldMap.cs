using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapModS.Data;
using System.Collections.Generic;
using System.Linq;
using L = RandomizerMod.Localization;
using TP = MapModS.UI.TransitionPersistent;

namespace MapModS.UI
{
    internal class TransitionWorldMap
    {
        private static LayoutRoot layout;

        private static TextObject instruction;
        private static TextObject routeSummary;

        private static Panel panel;
        private static TextObject panelText;

        private static bool Condition()
        {
            return TransitionData.TransitionModeActive()
                && !GUI.lockToggleEnable
                && GUI.worldMapOpen;
        }

        public static void Build()
        {
            if (layout == null)
            {
                layout = new(true, "Transition World Map");
                layout.VisibilityCondition = Condition;

                instruction = UIExtensions.TextFromEdge(layout, "Instructions", false);

                routeSummary = UIExtensions.TextFromEdge(layout, "Route Summary", true);

                panel = new(layout, GUIController.Instance.Images["panelRight"].ToSlicedSprite(100f, 50f, 0f, 50f), "Panel")
                {
                    Borders = new(30f, 30f, 30f, 30f),
                    MinWidth = 200f,
                    MinHeight = 100f,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Padding = new(10f, 170f, 160f, 10f)
                };

                panelText = new(layout, "Panel Text")
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Font = MagicUI.Core.UI.TrajanNormal,
                    FontSize = 14
                };

                panel.Child = panelText;

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
            UpdateInstructions();
            UpdateRouteSummary();
            UpdatePanel();
        }

        public static void UpdateInstructions()
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

            instruction.Text = text;
        }

        public static void UpdateRouteSummary()
        {
            string text = $"{L.Localize("Current route")}: ";

            if (TP.lastStartTransition != ""
                && TP.lastFinalTransition != ""
                && TP.transitionsCount > 0
                && TP.selectedRoute.Any())
            {
                if (TP.lastFinalTransition.IsSpecialTransition())
                {
                    if (TP.transitionsCount == 1)
                    {
                        text += $"{TP.lastStartTransition.ToCleanName()}";
                    }
                    else
                    {
                        text += $"{TP.lastStartTransition.ToCleanName()} ->...-> {TP.lastFinalTransition.ToCleanName()}";
                    }
                }
                else
                {
                    text += $"{TP.lastStartTransition.ToCleanName()} ->...-> {TP.lastFinalTransition.GetAdjacentTransition().ToCleanName()}";
                }
                
                text += $"\n\n{L.Localize("Transitions")}: {TP.transitionsCount}";
            }
            else
            {
                text += L.Localize("None");
            }

            routeSummary.Text = text;
        }

        public static void UpdatePanel()
        {
            panelText.Text = TransitionData.GetUncheckedVisited(TP.selectedScene);

            if (MapModS.GS.uncheckedPanelActive)
            {
                panel.Visibility = Visibility.Visible;
            }
            else
            {
                panel.Visibility = Visibility.Hidden;
            }
        }
    }
}
