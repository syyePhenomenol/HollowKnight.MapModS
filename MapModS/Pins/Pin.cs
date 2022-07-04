using System;
using UnityEngine;

namespace MapModS.Pins
{
    public class Pin : MonoBehaviour
    {
        private protected const int UI_LAYER = 5;
        private protected const string HUD = "HUD";

        private Transform GameMap => transform.parent.transform.parent;
        private SpriteRenderer sr;

        public AbstractPinDef PinDef { get; private set; }

        public virtual void Initialize(AbstractPinDef pinDef, float offsetZ)
        {
            PinDef = pinDef;
            gameObject.name = pinDef.Name;
            gameObject.layer = UI_LAYER;

            sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = HUD;

            ApplyOffset(offsetZ);
        }

        private void ApplyOffset(float offsetZ)
        {
            string roomName = PinDef.MappedScene ?? PinDef.Scene;

            if (TryGetRoomPos(roomName, out Vector2 vec))
            {
                vec += new Vector2(PinDef.OffsetX, PinDef.OffsetY);
                transform.localPosition = new Vector3(vec.x, vec.y, offsetZ);
            }
            else
            {
                MapModS.Instance.LogWarn($"{roomName} not found on the map!");
            }
        }

        private bool TryGetRoomPos(string roomName, out Vector2 pos)
        {
            foreach (Transform areaObj in GameMap)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (roomObj.gameObject.name == roomName)
                    {
                        Vector3 roomVec = roomObj.transform.localPosition;
                        //roomVec.Scale(areaObj.transform.localScale);
                        pos = areaObj.transform.localPosition + roomVec;
                        return true;
                    }
                }
            }

            pos = Vector2.zero;
            return false;
        }

        public virtual void Set()
        {
            PinDef.Update();

            if (PinDef.Active)
            {
                gameObject.SetActive(true);
                SetSprite();
                SetScale();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private protected virtual void SetSprite()
        {
            sr.sprite = PinDef.GetMainSprite();
            sr.color = PinDef.GetMainColor();
        }

        private protected virtual void SetScale()
        {
            transform.localScale = PinDef.GetScale();
        }
    }
}
