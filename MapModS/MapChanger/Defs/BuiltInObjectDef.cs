using Newtonsoft.Json;

namespace MapChanger.Defs
{
    internal abstract record BuiltInObjectDef
    {
        [JsonProperty]
        public ColorSetting ColorSetting { get; init; }
    }
}