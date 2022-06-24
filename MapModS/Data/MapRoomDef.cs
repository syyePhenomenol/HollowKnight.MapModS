using GlobalEnums;

namespace MapModS.Data
{
    public enum RoomState
    {
        Normal,
        Current,
        Adjacent,
        Out_of_logic,
        Selected
    }

    public class MapRoomDef
    {
        public MapZone mapZone;
        public float offsetX;
        public float offsetY;
        public string mappedScene;
        public bool includeWithAdditionalMaps;
    }
}
