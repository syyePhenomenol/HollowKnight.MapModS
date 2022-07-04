using System.Collections;
using UnityEngine;

namespace MapModS.Pins
{
    public class RandoPin : BorderedPin, IPeriodicUpdater, ISelectable
    {
        private const float SELECTED_SIZE_SCALE = 1.8f;
        public float UpdateWaitSeconds { get; } = 1f;

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

            base.Set();

            if (PinDef.Active)
            {
                StartCoroutine(PeriodicUpdate());
            }
        }
    }
}
