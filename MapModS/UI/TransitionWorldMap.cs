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
        }

        public static void UpdateInstructions()
        {
            string text = "";

            if (!MapModS.GS.uncheckedPanelActive)
            {
                text += $"{L.Localize("Selected room")}: {InfoPanels.selectedScene}.";
            }

            List<InControl.BindingSource> bindings = new(InputHandler.Instance.inputActions.menuSubmit.Bindings);

            if (InfoPanels.selectedScene == Utils.CurrentScene())
            {
                text += $" {L.Localize("You are here")}.";
            }

            text += $" {L.Localize("Press")} ";

            text += Utils.GetBindingsText(bindings);

            if (TP.selectedRoute.Any()
                && InfoPanels.selectedScene == TP.lastFinalScene
                && TP.selectedRoute.Count() == TP.transitionsCount)
            {
                text += $" {L.Localize("to change starting / final transitions of current route")}.";
            }
            else
            {
                text += $" {L.Localize("to find a new route")}.";
            }


            if (TP.selectedRoute.Any() && TP.selectedRoute.First().IsBenchwarpTransition() && Dependencies.HasBenchwarp())
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
                    text += $"{TP.lastStartTransition.ToCleanName()} ->...-> {TP.lastFinalTransition.GetAdjacentTerm().ToCleanName()}";
                }
                
                text += $"\n\n{L.Localize("Transitions")}: {TP.transitionsCount}";
            }
            else
            {
                text += L.Localize("None");
            }

            routeSummary.Text = text;
        }


    }
}
