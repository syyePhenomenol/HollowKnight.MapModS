using UnityEngine;

namespace MapChanger.Objects
{
    public abstract class SpriteMapObject : MapObject, ISpriteRenderer
    {
        public SpriteRenderer SR { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            SR = gameObject.AddComponent<SpriteRenderer>();
            SR.sortingLayerName = HUD;

            SetSprite();
            SetSpriteColor();
        }

        public abstract void SetSprite();

        public abstract void SetSpriteColor();
    }
}
