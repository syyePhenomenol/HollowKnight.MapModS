using UnityEngine;

namespace MapModS.Map
{
    internal abstract class MapObject
    {
        public string Name { get; private protected init; }
        public Vector4 OrigColor { get; private protected init; }
    }
}
