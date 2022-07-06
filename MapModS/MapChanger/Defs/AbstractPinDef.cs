using GlobalEnums;
using MapChanger.Defs;
using UnityEngine;

namespace MapChanger.Defs
{
    public abstract class AbstractPinDef
    {
        public string Name { get => MapPosition.Name; }
        public string SceneName { get => MapPosition.SceneName; }
        public bool Active { get; private protected set; }

        public MapPositionDef MapPosition { get; private set; }

        public AbstractPinDef(MapPositionDef mpd)
        {
            MapPosition = mpd;
        }

        public abstract void Update();
        public abstract Sprite GetSprite();

        public void SetPosition(IMapPosition mpd)
        {
            MapPosition.OffsetX = mpd.OffsetX;
            MapPosition.OffsetY = mpd.OffsetY;
            if (mpd.MappedScene != null)
            {
                MapPosition.MappedScene = mpd.MappedScene;
            }
        }
    }
}
