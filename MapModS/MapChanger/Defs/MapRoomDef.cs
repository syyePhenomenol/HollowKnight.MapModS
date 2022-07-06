using GlobalEnums;
using Newtonsoft.Json;

namespace MapChanger.Defs
{
    public record MapRoomDef : IMapPosition
    {
        [JsonProperty]
        public string SceneName { get; set; }
        [JsonProperty]
        public string MappedScene { get; set; }
        [JsonProperty]
        public float OffsetX { get; set; }
        [JsonProperty]
        public float OffsetY { get; set; }
        [JsonIgnore]
        public MapZone MapZone => Finder.GetMapZone(MappedScene);
    }
}
