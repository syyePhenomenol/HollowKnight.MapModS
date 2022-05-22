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
            return TransitionData.TransitionModeActive() && GUI.quickMapOpen;
        }

        public static void Build()
        {
            if (layout == null)
            {
                layout = new(true, "Transition Quick Map");
                layout.VisibilityCondition = Condition;

                uncheckedText = new(layout, "Unchecked")
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    TextAlignment = HorizontalAlignment.Right,
                    Font = MagicUI.Core.UI.TrajanNormal,
                    FontSize = 14,
                    Padding = new(10f, 20f, 20f, 10f)
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
            UpdateUnchecked();
        }

        public static void UpdateUnchecked()
        {
            uncheckedText.Text = TransitionData.GetUncheckedVisited(Utils.CurrentScene());
        }
    }
}
