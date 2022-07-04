using GlobalEnums;
using Newtonsoft.Json;

namespace MapModS
{
    public interface IMapPosition
    {
        [JsonProperty]
        string MappedScene { get; set; }
        [JsonProperty]
        MapZone MapZone { get; set; }
        [JsonProperty]
        float OffsetX { get; set; }
        [JsonProperty]
        float OffsetY { get; set; }
        [JsonProperty]
        float OffsetZ { get; set; }
    }
}