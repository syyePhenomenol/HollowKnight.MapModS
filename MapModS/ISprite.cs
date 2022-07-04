using UnityEngine;

namespace MapModS
{
    public interface ISprite
    {
        SpriteRenderer SR { get; set; }

        void SetSprite();
        void SetSpriteColor();
    }
}
