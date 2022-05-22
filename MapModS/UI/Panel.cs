using MagicUI.Core;
using MagicUI.Elements;
using UnityEngine;

namespace MapModS.UIExtensions
{
    public class Panel : Layout
    {
        private Image background;

        public Vector4 Borders = Vector4.zero;

        public float MinWidth = 0f;
        public float MinHeight = 0f;

        public Panel(LayoutRoot onLayout, Image background, string name = "New Panel") : base(onLayout, name)
        {
            this.background = background;
            Children.Add(background);
        }

        protected override Vector2 MeasureOverride()
        {
            foreach (ArrangableElement element in Children)
            {
                element.Measure();
            }

            return background.EffectiveSize;
        }

        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            float xMax = 0f;
            float yMax = 0f;

            foreach (ArrangableElement element in Children)
            {
                Vector2 size = element.EffectiveSize;

                if (element == background)
                {
                    element.Arrange(new Rect(alignedTopLeftCorner, size));
                }
                else
                {
                    element.Arrange(new Rect(alignedTopLeftCorner + new Vector2(Borders.x, Borders.y), size));

                    if (size.x > xMax) xMax = size.x;

                    if (size.y > yMax) yMax = size.y;
                }
            }

            if (xMax > MinWidth)
            {
                background.Width = xMax + Borders.z;
            }
            else
            {
                background.Width = MinWidth + Borders.z;
            }

            if (yMax > MinHeight)
            {
                background.Height = yMax + Borders.w;
            }
            else
            {
                background.Height = MinHeight + Borders.w;
            }
        }

        protected override void DestroyOverride()
        {
            Children.Clear();
        }
    }
}
