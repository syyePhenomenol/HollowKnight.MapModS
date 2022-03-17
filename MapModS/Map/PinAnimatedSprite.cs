using MapModS.Data;
using MapModS.Settings;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using PBC = MapModS.Map.PinBorderColor;
using PLS = MapModS.Data.PinLocationState;

namespace MapModS.Map
{
    public enum PinBorderColor
    {
        Normal,
        Previewed,
        OutOfLogic,
        Persistent
    }

    public class PinAnimatedSprite : MonoBehaviour
    {
        public PinDef pinDef { get; private set; } = null;
        
        SpriteRenderer SR => gameObject.GetComponent<SpriteRenderer>();

        private int spriteIndex = 0;

        private readonly Color _inactiveColor = Color.gray;
        
        private Color _origColor;

        public void SetPinData(PinDef pd)
        {
            pinDef = pd;
            _origColor = SR.color;
        }

        void OnEnable()
        {
            if (gameObject.activeSelf
                && pinDef != null
                && pinDef.randoItems != null
                && pinDef.randoItems.Count() > 1)
            {
                StartCoroutine("CycleSprite");
            }
        }

        void OnDisable()
        {
            if (!gameObject.activeSelf)
            {
                StopAllCoroutines();
            }
        }

        IEnumerator CycleSprite()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1);
                spriteIndex = (spriteIndex + 1) % pinDef.randoItems.Count();
                SetSprite();
            }
        }

        public void ResetSpriteIndex()
        {
            spriteIndex = 0;
        }

        public void SetSprite()
        {
            if (!gameObject.activeSelf) return;

            // Non-randomized
            if (pinDef.pinLocationState == PLS.NonRandomizedUnchecked)
            {
                SR.sprite = SpriteManager.GetSpriteFromPool(pinDef.locationPoolGroup, PinBorderColor.Normal);
                
                return;
            }

            if (pinDef.randoItems == null || spriteIndex + 1 > pinDef.randoItems.Count()) return;

            // Set pool to display
            string pool = pinDef.locationPoolGroup;

            if (pinDef.pinLocationState == PLS.Previewed
                || pinDef.pinLocationState == PLS.ClearedPersistent
                || MapModS.LS.SpoilerOn)
            {
                pool = pinDef.randoItems.ElementAt(spriteIndex).poolGroup;
            }

            // Set border color of pin
            PBC pinBorderColor = PBC.Normal;

            switch (pinDef.pinLocationState)
            {
                case PLS.OutOfLogicReachable:

                    pinBorderColor = PBC.OutOfLogic;

                    break;

                case PLS.Previewed:

                    pinBorderColor = PBC.Previewed;

                    break;

                case PLS.ClearedPersistent:

                    if (pinDef.randoItems.ElementAt(spriteIndex).persistent)
                    {
                        pinBorderColor = PBC.Persistent;
                    }

                    break;

                default:

                    break;
            }

            SR.sprite = SpriteManager.GetSpriteFromPool(pool, pinBorderColor);
        }

        public void SetSizeAndColor()
        {
            // Size
            transform.localScale = pinDef.pinLocationState switch
            {
                PLS.UncheckedReachable
                or PLS.OutOfLogicReachable
                or PLS.Previewed
                => GetPinScale() * new Vector2(1.45f, 1.45f),

                _ => GetPinScale() * new Vector2(1.015f, 1.015f),
            };

            // Color
            SR.color = pinDef.pinLocationState switch
            {
                PLS.UncheckedReachable
                or PLS.OutOfLogicReachable
                or PLS.Previewed
                or PLS.ClearedPersistent
                => _origColor,

                _ => _inactiveColor,
            };
        }

        public void SetSizeAndColorSelected()
        {
            transform.localScale = GetPinScale() * new Vector2(1.8f, 1.8f);
            SR.color = _origColor;
        }

        private float GetPinScale()
        {
            return MapModS.GS.pinSize switch
            {
                PinSize.Small => 0.31f,
                PinSize.Medium => 0.37f,
                PinSize.Large => 0.42f,
                _ => throw new NotImplementedException()
            };
        }
    }
}
