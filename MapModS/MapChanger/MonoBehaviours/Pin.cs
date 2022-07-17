using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public abstract class Pin : MapObject, ISpriteRenderer
    {
        public Transform GameMap => transform.parent.transform.parent;
        public SpriteRenderer Sr { get; set; }
        public abstract IMapPosition MapPosition { get; }

        public Vector2 BaseOffset { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            SetPosition();
            SetScale();

            Sr = gameObject.AddComponent<SpriteRenderer>();
            Sr.sortingLayerName = HUD;
            SetSprite();
            SetSpriteColor();
        }

        public abstract void SetSprite();

        public abstract void SetSpriteColor();

        public virtual void SetPosition()
        {
            if (Finder.TryGetMappedScenePosition(MapPosition.MappedScene, out Vector2 position))
            {
                BaseOffset = position;

                Vector2 snappedPosition = new Vector2(position.x + MapPosition.OffsetX, position.y + MapPosition.OffsetY).Snap();

                transform.localPosition = new(snappedPosition.x, snappedPosition.y, transform.localPosition.z);
            }
            else
            {
                MapChangerMod.Instance.LogWarn($"{MapPosition.MappedScene} not found on the map!");
            }
        }

        public abstract void SetScale();
    }
}
