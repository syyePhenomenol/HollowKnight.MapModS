using UnityEngine;

namespace MapModS
{
    public abstract class MapObject : MonoBehaviour
    {
        private protected const int UI_LAYER = 5;
        private protected const string HUD = "HUD";

        public abstract IMapPosition MapPosition { get; }
        public Transform GameMap => transform.parent.transform.parent;

        public virtual void Awake()
        {
            gameObject.layer = UI_LAYER;
        }

        public virtual void Set()
        {
            SetScale();
        }

        /// <summary>
        /// Probably only call this once.
        /// </summary>
        public virtual void SetPosition()
        {
            if (TryGetRoomPos(MapPosition.MappedScene, out Vector2 vec))
            {
                vec += new Vector2(MapPosition.OffsetX, MapPosition.OffsetY);
                transform.localPosition = new Vector3(vec.x, vec.y, MapPosition.OffsetZ);
            }
            else
            {
                MapModS.Instance.LogWarn($"{MapPosition.MappedScene} not found on the map!");
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
                        pos = areaObj.transform.localPosition + roomVec;
                        return true;
                    }
                }
            }

            pos = Vector2.zero;
            return false;
        }

        public abstract void SetScale();
    }
}
