using GlobalEnums;
using Newtonsoft.Json;

namespace MapChanger.Defs
{
    public class MapLocationDef : IMapPosition
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string SceneName { get; set; }
        [JsonProperty]
        public string MappedScene { get; set; }
        [JsonProperty]
        public float OffsetX { get; set; }
        [JsonProperty]
        public float OffsetY { get; set; }
        public MapZone MapZone => Finder.GetMapZone(SceneName);
    }
}