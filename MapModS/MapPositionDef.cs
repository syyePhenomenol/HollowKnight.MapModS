using GlobalEnums;
using Newtonsoft.Json;

namespace MapModS
{
    public class MapPositionDef : IMapPosition
    {
        public string MappedScene { get; set; }
        public MapZone MapZone { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float OffsetZ { get; set; }
    }
}