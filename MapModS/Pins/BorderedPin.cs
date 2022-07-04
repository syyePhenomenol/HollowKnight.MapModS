using System.Collections.Generic;
using UnityEngine;

namespace MapModS.Pins
{
    public class BorderedPin : Pin
    {
        private static readonly Dictionary<BorderPlacement, float> borderOffset = new()
        {
            { BorderPlacement.Behind, 0.00001f },
            { BorderPlacement.InFront, -0.00001f }
        };

        private SpriteRenderer borderSR;

        public override void Initialize(AbstractPinDef pinDef, float offsetZ)
        {
            base.Initialize(pinDef, offsetZ);

            GameObject goBorder = new($"{PinDef.Name} Border");
            goBorder.layer = UI_LAYER;
            goBorder.transform.SetParent(transform);
            goBorder.transform.localPosition = new Vector3(0f, 0f, borderOffset[PinDef.BorderPlacement]);

            borderSR = goBorder.AddComponent<SpriteRenderer>();
            borderSR.sortingLayerName = HUD;
        }

        private protected override void SetSprite()
        {
            base.SetSprite();

            borderSR.sprite = PinDef.GetBorderSprite();
            borderSR.color = PinDef.GetBorderColor();
        }
    }
}
