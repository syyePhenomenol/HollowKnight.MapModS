using GlobalEnums;
using Newtonsoft.Json;

namespace MapChanger.Defs
{
    internal record RoomSpriteDef : BuiltInObjectDef
    {
        [JsonProperty]
        internal string SceneName { get; init; }
        [JsonIgnore]
        public MapZone MapZone => Finder.GetMapZone(SceneName);
    }
}
