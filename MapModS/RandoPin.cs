using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using MapModS.Pins;
using System.Collections;
using UnityEngine;

namespace MapModS
{
    public class RandoPin: BorderedPin, IPeriodicUpdater, ISelectable
    {
        private const float SELECTED_SIZE_SCALE = 1.8f;
        public float UpdateWaitSeconds { get; } = 1f;

        public RandomizerModPinDef RMPinDef { get; set; }
        public override IMapPosition MapPosition { get => RMPinDef.MapPosition; }

        public bool Selected { get; private set; }

        public void Initialize(RandomizerModPinDef rmPinDef, float offsetZ)
        {
            RMPinDef = rmPinDef;
            RMPinDef.Update();
            transform.SetPositionZ(offsetZ);
            BorderPlacement = BorderPlacement.InFront;

            base.Initialize();
        }

        public bool CanUsePosition()
        {
            return gameObject.activeSelf;
        }

        public (string, Vector2) GetKeyAndPosition()
        {
            return (RMPinDef.Name, transform.position);
        }

        public void Select()
        {
            Selected = true;
            Set();
        }

        public void Deselect()
        {
            Selected = false;
            Set();
        }

        public IEnumerator PeriodicUpdate()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(UpdateWaitSeconds);
                SetSprite();
            }
        }

        public override void Set()
        {
            StopAllCoroutines();

            RMPinDef.Update();

            if (RMPinDef.Active)
            {
                gameObject.SetActive(true);
                SetSprite();
                SetSpriteColor();
                SetBorderColor();
                SetScale();
                StartCoroutine(PeriodicUpdate());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public override void SetSprite()
        {
            Sr.sprite = RMPinDef.GetSprite();
        }

        public override void SetSpriteColor()
        {
            Sr.color = RMPinDef.GetSpriteColor();
        }

        public override void SetBorderSprite()
        {
            BorderSR.sprite = RMPinDef.GetBorderSprite();
        }

        public override void SetBorderColor()
        {
            BorderSR.color = RMPinDef.GetBorderColor();
        }

        public override void SetScale()
        {
            transform.localScale = new(RMPinDef.GetScale(), RMPinDef.GetScale(), 1f);
        }
    }
}
