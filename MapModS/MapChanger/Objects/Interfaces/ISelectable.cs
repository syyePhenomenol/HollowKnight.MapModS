using UnityEngine;

namespace MapChanger.Objects
{
    public interface ISelectable
    {
        bool CanSelect();
        Vector2 GetPosition();
        void Select();
        void Deselect();
    }
}
