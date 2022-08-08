using Newtonsoft.Json;

namespace MapChanger.Defs
{
    public record TileMapDef
    {
        [JsonProperty]
        public string SceneName { get; init; }
        [JsonProperty]
        public int Width { get; init; }
        [JsonProperty]
        public int Height { get; init; }
    }
}
