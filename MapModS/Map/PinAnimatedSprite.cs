using MapModS.Data;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using MapModS.Settings;

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
        int spriteIndex = 0;

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
            if (pinDef.pinLocationState == PinLocationState.NonRandomizedUnchecked)
            {
                SR.sprite = SpriteManager.GetSpriteFromPool(pinDef.locationPoolGroup, PinBorderColor.Normal);
                return;
            }

            if (pinDef.randoItems == null || spriteIndex + 1 > pinDef.randoItems.Count()) return;

            PoolGroup pool;
            PinBorderColor pinBorderColor = PinBorderColor.Normal;

            if (pinDef.pinLocationState == PinLocationState.OutOfLogicReachable)
            {
                pinBorderColor = PinBorderColor.OutOfLogic;
            }

            if (pinDef.pinLocationState == PinLocationState.Previewed)
            {
                pinBorderColor = PinBorderColor.Previewed;
            }

            if (pinDef.pinLocationState == PinLocationState.Previewed)
            {
                pool = pinDef.randoItems.ElementAt(spriteIndex).poolGroup;
            }
            else if (MapModS.LS.SpoilerOn
                || pinDef.pinLocationState == PinLocationState.ClearedPersistent)
            {
                pool = pinDef.randoItems.ElementAt(spriteIndex).poolGroup;

                if (pinDef.randoItems.ElementAt(spriteIndex).persistent)
                {
                    pinBorderColor = PinBorderColor.Persistent;
                }
            }
            else
            {
                pool = pinDef.locationPoolGroup;
            }

            SR.sprite = SpriteManager.GetSpriteFromPool(pool, pinBorderColor);
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

        public void SetSizeAndColor()
        {
            if (pinDef.pinLocationState == PinLocationState.UncheckedReachable
                || pinDef.pinLocationState == PinLocationState.OutOfLogicReachable
                || pinDef.pinLocationState == PinLocationState.Previewed)
            {
                transform.localScale = 1.45f * GetPinScale() * new Vector2(1.0f, 1.0f);
                SR.color = _origColor;
            }
            else if (pinDef.pinLocationState == PinLocationState.ClearedPersistent)
            {
                transform.localScale = 1.015f * GetPinScale() * new Vector2(1.0f, 1.0f);
                SR.color = _origColor;
            }
            else
            {
                transform.localScale = 1.015f * GetPinScale() * new Vector2(1.0f, 1.0f);
                SR.color = _inactiveColor;
            }
        }

        public void SetSizeAndColorSelected()
        {
            transform.localScale = 1.8f * GetPinScale() * new Vector2(1.0f, 1.0f);
            SR.color = _origColor;
        }
    }
}
