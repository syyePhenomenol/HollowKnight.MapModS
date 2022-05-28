using MagicUI.Core;
using MagicUI.Elements;
using MapModS.Data;


namespace MapModS.UI
{
    internal class TransitionQuickMap
    {
        private static LayoutRoot layout;

        private static TextObject uncheckedText;

        private static bool Condition()
        {
            return TransitionData.TransitionModeActive()
                && !GUI.lockToggleEnable
                && GUI.quickMapOpen;
        }

        public static void Build()
        {
            if (layout == null)
            {
                layout = new(true, "Transition Quick Map");
                layout.VisibilityCondition = Condition;

                uncheckedText = UIExtensions.TextFromEdge(layout, "Unchecked", true);

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
            UpdateUnchecked();
        }

        public static void UpdateUnchecked()
        {
            uncheckedText.Text = TransitionData.GetUncheckedVisited(Utils.CurrentScene());
        }
    }
}
