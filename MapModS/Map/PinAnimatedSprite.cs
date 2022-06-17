using MapModS.Data;
using MapModS.Settings;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
//using PBC = MapModS.Map.PinBorderColor;
using PLS = MapModS.Data.PinLocationState;

namespace MapModS.Map
{
    //public enum PinBorderColor
    //{
    //    Normal,
    //    Previewed,
    //    Out_of_logic,
    //    Persistent
    //}

    public class PinAnimatedSprite : MonoBehaviour
    {
        public PinDef PD { get; private set; } = null;
        
        SpriteRenderer SR => gameObject.GetComponent<SpriteRenderer>();

        SpriteRenderer BorderSR => transform.GetChild(0).GetComponent<SpriteRenderer>();

        private int spriteIndex = 0;

        private readonly Color _inactiveColor = Color.gray;
        
        private Color _origColor;

        public void SetPinData(PinDef pd)
        {
            PD = pd;
            _origColor = SR.color;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Member is actually used")]
        private void OnEnable()
        {
            if (gameObject.activeSelf
                && PD != null
                && PD.randoItems != null
                && PD.randoItems.Count() > 1)
            {
                StartCoroutine("CycleSprite");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Member is actually used")]
        private void OnDisable()
        {
            if (!gameObject.activeSelf)
            {
                StopAllCoroutines();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Member is actually used")]
        private IEnumerator CycleSprite()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1);
                spriteIndex = (spriteIndex + 1) % PD.randoItems.Count();
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
            if (PD.pinLocationState == PLS.NonRandomizedUnchecked)
            {
                SR.sprite = SpriteManager.GetSpriteFromPool(PD.locationPoolGroup, false);
                
                return;
            }

            if (PD.randoItems == null || spriteIndex + 1 > PD.randoItems.Count()) return;

            // Set pool to display
            string pool = PD.locationPoolGroup;
            bool normalOverride = false;

            if (PD.pinLocationState == PLS.Previewed
                || PD.pinLocationState == PLS.ClearedPersistent
                || MapModS.LS.spoilerOn)
            {
                pool = PD.randoItems.ElementAt(spriteIndex).poolGroup;
                normalOverride = true;
            }

            SR.sprite = SpriteManager.GetSpriteFromPool(pool, normalOverride);

            SetBorderColor();
        }

        public void SetSizeAndColor()
        {
            // Size
            transform.localScale = PD.pinLocationState switch
            {
                PLS.UncheckedReachable
                or PLS.OutOfLogicReachable
                or PLS.Previewed
                =>  new Vector3(1.45f * GetPinScale(), 1.45f * GetPinScale(), 1f),
                _ => new Vector3(1.015f * GetPinScale(), 1.015f * GetPinScale(), 1f)
            };

            // Color
            SR.color = PD.pinLocationState switch
            {
                PLS.UncheckedReachable
                or PLS.OutOfLogicReachable
                or PLS.Previewed
                or PLS.ClearedPersistent
                => _origColor,

                _ => _inactiveColor,
            };

            SetBorderColor();
        }

        public void SetSizeAndColorSelected()
        {
            transform.localScale = new Vector3(1.8f * GetPinScale(), 1.8f * GetPinScale(), 1f);
            SR.color = _origColor;
            SetBorderColor();
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
        
        private void SetBorderColor()
        {
            // Set border color of pin
            switch (PD.pinLocationState)
            {
                case PLS.UncheckedUnreachable:
                case PLS.NonRandomizedUnchecked:
                    BorderSR.color = GrayOut(Colors.GetColor(ColorSetting.Pin_Normal));
                    break;
                case PLS.UncheckedReachable:
                    BorderSR.color = Colors.GetColor(ColorSetting.Pin_Normal);
                    break;
                case PLS.OutOfLogicReachable:
                    BorderSR.color = Colors.GetColor(ColorSetting.Pin_Out_of_logic);
                    break;
                case PLS.Previewed:
                    BorderSR.color = Colors.GetColor(ColorSetting.Pin_Previewed);
                    break;
                case PLS.ClearedPersistent:
                    if (PD.randoItems.ElementAt(spriteIndex).persistent)
                    {
                        BorderSR.color = Colors.GetColor(ColorSetting.Pin_Persistent);
                    }
                    break;

                default:
                    break;
            }
        }

        private Vector4 GrayOut(Vector4 color)
        {
            Vector4 newColor = new();

            newColor.x = color.x / 2f;
            newColor.y = color.y / 2f;
            newColor.z = color.z / 2f;
            newColor.w = color.w;

            return newColor;
        }
    }
}
