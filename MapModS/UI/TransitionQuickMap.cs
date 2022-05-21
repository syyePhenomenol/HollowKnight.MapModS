using MagicUI.Core;
using MagicUI.Elements;
using MapModS.Data;


namespace MapModS.UI
{
    internal class TransitionQuickMap
    {
        private static LayoutRoot layout;
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

                TextObject uncheckedText = new(layout, "Unchecked")
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    TextAlignment = HorizontalAlignment.Left,
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
            UpdateUnchecked((TextObject)layout.GetElement("Unchecked"));
        }

        public static void UpdateUnchecked(TextObject textObj)
        {
            textObj.Text = TransitionData.GetUncheckedVisited(Utils.CurrentScene());
        }
    }
}
