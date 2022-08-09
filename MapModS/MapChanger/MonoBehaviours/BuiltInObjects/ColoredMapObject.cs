using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public abstract class ColoredMapObject : MapObject
    {
        public Vector4 OrigColor { get; protected private set; }
        public abstract Vector4 Color { get; set; }

        public override void OnMainUpdate(bool active)
        {
            UpdateColor();
        }

        public abstract void UpdateColor();

        public void ResetColor()
        {
            Color = OrigColor;
        }
    }
}
