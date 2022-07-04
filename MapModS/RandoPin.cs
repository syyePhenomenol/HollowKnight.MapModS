using MapModS.Pins;
using System.Collections;
using UnityEngine;

namespace MapModS
{
    public class RandoPin: BorderedMapObject, IPeriodicUpdater, ISelectable
    {
        private const float SELECTED_SIZE_SCALE = 1.8f;
        public float UpdateWaitSeconds { get; } = 1f;

        public RandomizerModPinDef RMDef { get; set; }
        public override IMapPosition MapPosition { get => RMDef; }

        public override void Awake()
        {
            base.Awake();
            BorderPlacement = BorderPlacement.InFront;
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

            RMDef.Update();

            if (RMDef.Active)
            {
                gameObject.SetActive(true);
                base.Set();
                StartCoroutine(PeriodicUpdate());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public override void SetSprite()
        {
            SR.sprite = RMDef.GetMainSprite();
        }

        public override void SetSpriteColor()
        {
            SR.color = RMDef.GetMainColor();
        }

        public override void SetBorderSprite()
        {
            BorderSR.sprite = RMDef.GetBorderSprite();
        }

        public override void SetBorderColor()
        {
            BorderSR.color = RMDef.GetBorderColor();
        }

        public override void SetScale()
        {
            transform.localScale = RMDef.GetScale();
        }
    }
}
