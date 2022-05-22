using MagicUI.Core;
using MagicUI.Elements;
using System;
using UnityEngine;

namespace MapModS.UIExtensions
{
    public class Panel : Container
    {
        public readonly Image Background;

        private float minWidth;

        public float MinWidth
        {
            get => minWidth;
            set
            {
                if (minWidth != value)
                {
                    minWidth = value;
                    InvalidateMeasure();
                }
            }
        }

        private float minHeight;

        public float MinHeight
        {
            get => minHeight;
            set
            {
                if (minHeight != value)
                {
                    minHeight = value;
                    InvalidateMeasure();
                }
            }
        }

        private Vector4 borders;

        public Vector4 Borders
        {
            get => borders;
            set
            {
                if (borders != value)
                {
                    borders = value;
                    InvalidateMeasure();
                }
            }
        }

        public Panel(LayoutRoot onLayout, Sprite bgSprite, string name = "New Panel") : base(onLayout, name)
        {
            Background = new(onLayout, bgSprite, name + " Background");

            minWidth = bgSprite.rect.width;
            minHeight = bgSprite.rect.width;
            borders = Vector4.zero;
        }

        protected override Vector2 MeasureOverride()
        {
            Child?.Measure();

            if (Child != null && Background != null)
            {
                Background.Width = Math.Max(MinWidth, Child.EffectiveSize.x + borders.x + borders.z);

                Background.Height = Math.Max(MinHeight, Child.EffectiveSize.y + borders.y + borders.w);
            }
            
            Background?.Measure();

            return Background.EffectiveSize;
        }

        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            Child?.Arrange(new Rect(alignedTopLeftCorner + new Vector2(borders.x, borders.y), Child.EffectiveSize));

            Background?.Arrange(new Rect(alignedTopLeftCorner, Background.EffectiveSize));

            return;
        }

        protected override void DestroyOverride()
        {
            Child?.Destroy();
            Background?.Destroy();
        }
    }
}
