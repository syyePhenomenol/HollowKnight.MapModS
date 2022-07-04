using UnityEngine;

namespace MapModS
{
    public interface ISelectable
    {
        bool CanSelect();
        Vector2 GetPosition();
        void Select();
        void Deselect();
    }
}
