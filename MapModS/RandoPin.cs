using MapChanger;
using MapChanger.Defs;
using MapChanger.Objects;
using MapModS.Pins;
using System.Collections;
using UnityEngine;

namespace MapModS
{
    public class RandoPin: BorderedMapObject, IPeriodicUpdater, ISelectable
    {
        private const float SELECTED_SIZE_SCALE = 1.8f;
        public float UpdateWaitSeconds { get; } = 1f;

        public RandomizerModPinDef RMPinDef { get; set; }
        public override IMapPosition MapPosition { get => RMPinDef.MapPosition; }

        public void Initialize(RandomizerModPinDef rmPinDef, float offsetZ)
        {
            base.Initialize();

            RMPinDef = rmPinDef;
            transform.SetPositionZ(offsetZ);
            BorderPlacement = MapChanger.Objects.BorderPlacement.InFront;
        }

        public bool CanSelect()
        {
            return gameObject.activeSelf;
        }

        public Vector2 GetPosition()
        {
            return gameObject.transform.position;
        }

        public void Select()
        {

        }

        public void Deselect()
        {

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
            SR.sprite = RMPinDef.GetSprite();
        }

        public override void SetSpriteColor()
        {
            SR.color = RMPinDef.GetSpriteColor();
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
            transform.localScale = RMPinDef.GetScale();
        }
    }
}
