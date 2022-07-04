using GlobalEnums;
using MapModS.Map;
using MapModS.Settings;
using UnityEngine;

namespace MapModS.Pins
{
    public enum BorderPlacement
    {
        Behind,
        InFront
    }

    public abstract class AbstractPinDef : IMapPosition
    {
        public string Name { get; private protected init; }
        public string Scene { get; private protected init; }
        public bool Active { get; private protected set; }
        public string MappedScene { get; set; }
        public MapZone MapZone { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float OffsetZ { get; set; }
        public BorderPlacement BorderPlacement { get; init; }
        public abstract Sprite GetMainSprite();
        public abstract Vector4 GetMainColor();
        public abstract Sprite GetBorderSprite();
        public abstract Vector4 GetBorderColor();
        public abstract Vector3 GetScale();

        public abstract void Update();

        public void SetPosition(IMapPosition mpd)
        {
            OffsetX = mpd.OffsetX;
            OffsetY = mpd.OffsetY;
            if (mpd.MappedScene != null)
            {
                MappedScene = mpd.MappedScene;
            }
            if (mpd.MapZone != MapZone.NONE)
            {
                MapZone = mpd.MapZone;
            }
        }

        private protected void SetMapData()
        {
            if (MapData.RoomLookup.TryGetValue(Scene, out MapPositionDef mpd))
            {
                MappedScene = mpd.MappedScene;
                MapZone = mpd.MapZone;
            }
            else
            {
                MapModS.Instance.LogWarn($"No valid MapZone for {Name}!");
                MappedScene = "Unknown";
            }
        }

    }
}
