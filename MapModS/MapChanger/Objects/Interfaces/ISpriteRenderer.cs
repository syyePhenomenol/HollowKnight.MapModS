using UnityEngine;

namespace MapChanger.Objects
{
    public interface ISpriteRenderer
    {
        SpriteRenderer SR { get; set; }

        void SetSprite();
        void SetSpriteColor();
    }
}
