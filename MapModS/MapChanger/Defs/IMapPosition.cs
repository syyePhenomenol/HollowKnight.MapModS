using Newtonsoft.Json;

namespace MapChanger.Defs
{
    public interface IMapPosition
    {
        [JsonProperty]
        string MappedScene { get; set; }
        [JsonProperty]
        float OffsetX { get; set; }
        [JsonProperty]
        float OffsetY { get; set; }
    }
}