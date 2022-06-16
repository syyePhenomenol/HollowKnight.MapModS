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

                panel = new(layout, GUIController.Instance.Images["panelRight"].ToSlicedSprite(100f, 50f, 250f, 50f), "Panel")
                {
                    Borders = new(30f, 30f, 30f, 30f),
                    MinWidth = 200f,
                    MinHeight = 100f,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Padding = new(10f, 170f, 160f, 10f)
                };

                ((Image)layout.GetElement("Panel Background")).Tint = Colors.GetColor(ColorSetting.UI_Borders);

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
            string text = "";

            if (!MapModS.GS.uncheckedPanelActive)
            {
                text += $"{L.Localize("Selected room")}: {TP.selectedScene}.";
            }

            List<InControl.BindingSource> bindings = new(InputHandler.Instance.inputActions.menuSubmit.Bindings);

            if (TP.selectedScene == Utils.CurrentScene())
            {
                text += $" {L.Localize("You are here")}.";
            }

            text += $" {L.Localize("Press")} ";

            text += Utils.GetBindingsText(bindings);

            if (TP.selectedRoute.Any()
                && TP.selectedScene == TP.lastFinalScene
                && TP.selectedRoute.Count() == TP.transitionsCount)
            {
                text += $" {L.Localize("to change starting / final transitions of current route")}.";
            }
            else
            {
                text += $" {L.Localize("to find a new route")}.";
            }


            if (TP.selectedRoute.Any() && TP.selectedRoute.First().IsBenchwarpTransition() && Dependencies.HasDependency("Benchwarp"))
            {
                bindings = new(InputHandler.Instance.inputActions.attack.Bindings);

                text += $" {L.Localize("Hold")} ";

                text += Utils.GetBindingsText(bindings);

                text += $" {L.Localize("to benchwarp")}.";
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
            panelText.Text = TP.selectedScene + "\n\n";

            panelText.Text += TransitionData.GetUncheckedVisited(TP.selectedScene);

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
