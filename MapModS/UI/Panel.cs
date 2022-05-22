using MagicUI.Core;
using MagicUI.Elements;
using System;
using UnityEngine;

namespace MapModS.UIExtensions
{
    /// <summary>
    /// A container that overlays a single element on top of a background image. The background will always be scaled to surround
    /// the element. To have a Sprite scale without stretching its borders, use ToSliceSprite() to create the Sprite.
    /// </summary>
    public sealed class Panel : Container
    {
        private readonly Image backgroundObj;

        private float minWidth;

        /// <summary>
        /// The minimum width of the background
        /// </summary>
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

        /// <summary>
        /// The minimum height of the background
        /// </summary>
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

        /// <summary>
        /// How far around the enclosed element the background will stretch to (left, top, right, bottom)
        /// </summary>
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

        /// <summary>
        /// Creates a panel
        /// </summary>
        /// <param name="onLayout">The layout root to draw the panel on</param>
        /// <param name="background">The sprite of the background</param>
        /// <param name="name">The name of the panel</param>
        public Panel(LayoutRoot onLayout, Sprite background, string name = "New Panel") : base(onLayout, name)
        {
            backgroundObj = new(onLayout, background, name + " Background");

            SetLogicalChild(backgroundObj);

            minWidth = background.rect.width;
            minHeight = background.rect.width;
            borders = Vector4.zero;
        }

        protected override Vector2 MeasureOverride()
        {
            Child?.Measure();

            if (Child != null)
            {
                backgroundObj.Width = Math.Max(MinWidth, Child.EffectiveSize.x + borders.x + borders.z);

                backgroundObj.Height = Math.Max(MinHeight, Child.EffectiveSize.y + borders.y + borders.w);
            }
            
            backgroundObj.Measure();

            return backgroundObj.EffectiveSize;
        }

        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            Child?.Arrange(new Rect(alignedTopLeftCorner + new Vector2(borders.x, borders.y), new Vector2(backgroundObj.EffectiveSize.x - borders.x, backgroundObj.EffectiveSize.y - borders.y)));

            backgroundObj.Arrange(new Rect(alignedTopLeftCorner, backgroundObj.EffectiveSize));

            return;
        }

        protected override void DestroyOverride()
        {
            Child?.Destroy();
            backgroundObj.Destroy();
        }
    }
}
