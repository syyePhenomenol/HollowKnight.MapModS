using Newtonsoft.Json;

namespace MapChanger.Defs
{
    internal enum MiscObjectType
    { 
        AreaName,
        NextArea
    }

    internal record MiscObjectDef : BuiltInObjectDef
    {
        [JsonProperty]
        internal MiscObjectType Type { get; init; }
    }
}
