using System.Collections.Generic;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public abstract class BorderedPin : Pin, IBorder
    {
        private static readonly Dictionary<BorderPlacement, float> borderOffset = new()
        {
            { BorderPlacement.Behind, 0.00001f },
            { BorderPlacement.InFront, -0.00001f }
        };

        public SpriteRenderer BorderSR { get; set; }
        public BorderPlacement BorderPlacement { get; set; }

        public override void Initialize()
        {
            GameObject goBorder = new($"{transform.name} Border");
            goBorder.layer = UI_LAYER;
            goBorder.transform.SetParent(transform);

            BorderSR = goBorder.AddComponent<SpriteRenderer>();
            BorderSR.sortingLayerName = HUD;

            SetBorderPosition();
            SetBorderSprite();
            SetBorderColor();

            base.Initialize();
        }

        public virtual void SetBorderPosition()
        {
            BorderSR.transform.localPosition = new Vector3(0f, 0f, borderOffset[BorderPlacement]);
        }

        public abstract void SetBorderSprite();

        public abstract void SetBorderColor();
    }
}
