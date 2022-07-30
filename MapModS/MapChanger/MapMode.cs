using System;
using GlobalEnums;
using MapChanger.MonoBehaviours;

namespace MapChanger
{
    public enum OverrideType
    {
        Neutral,
        ForceOff,
        ForceOn
    }

    public class MapMode
    {
        public (string, string) ModeKey => (Mod, ModeName);

        public virtual string Mod => "MapChangerMod";
        public virtual string ModeName => "Disabled";
        public virtual bool ForceHasMap => false;
        public virtual bool ForceHasQuill => false;
        public virtual OverrideType VanillaPins => OverrideType.Neutral;
        public virtual OverrideType MapMarkers => OverrideType.Neutral;
        /// <summary>
        /// Determines if the map immediately gets filled in when visiting a new scene with Quill.
        /// </summary>
        public virtual bool ImmediateMapUpdate => false;
        /// <summary>
        /// Forces all map areas/rooms to be visible and filled in.
        /// </summary>
        public virtual bool FullMap => false;
        public virtual bool DisableAreaNames => false;
        public virtual bool DisableNextArea => false;

        public virtual Func<RoomSprite, bool> RoomSpriteActiveOverride => null;
        public virtual Action<RoomSprite> OnRoomUpdateColor => (roomSprite) => { roomSprite.ResetColor(); };
        public virtual Action<AreaName> OnAreaNameUpdateColor => (areaName) => { areaName.ResetColor(); };
        public virtual Action<NextArea> OnNextAreaUpdateColor => (nextArea) => { nextArea.ResetColor(); };
        public virtual Action<QuickMapTitle, MapZone> OnQuickMapTitleUpdateColor => (qmt, mapZone) => { qmt.ResetColor(); };
    }
}
