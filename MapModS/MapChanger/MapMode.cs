using System;
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
        public virtual bool EnableCustomColors => false;
        public virtual bool EnableExtraRoomNames => false;

        public virtual Func<RoomSprite, bool> RoomSpriteActiveOverride => null;

        public virtual Action<RoomSprite> OnRoomUpdateColor => SetDefaultRoomColor;
        
        private void SetDefaultRoomColor(RoomSprite roomSprite)
        {
            roomSprite.Color = roomSprite.OrigColor;
        }

        //public virtual Func<RoomSprite, bool> OnRoomSpriteSet => null;
        //public virtual Func<RoomText, bool> OnRoomTextSet => null;
    }
}
