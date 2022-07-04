using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MapModS.Map
{
    internal enum OverrideType
    {
        Map,
        Pins
    }

    internal record FsmBoolOverrideDef
    {
        [JsonProperty]
        internal Dictionary<string, FsmActionBoolOverride[]> BoolsIndex { get; init; }
        [JsonProperty]
        internal Dictionary<string, FsmActionBoolRangeOverride> BoolsRange { get; init; }
    }

    internal record FsmActionBoolOverride
    {
        [JsonProperty]
        internal int Index { get; init; }

        [JsonProperty]
        internal OverrideType Type { get; init; }
    }

    internal record FsmActionBoolRangeOverride
    {
        [JsonProperty]
        internal int Range { get; init; }

        [JsonProperty]
        internal OverrideType Type { get; init; }
    }
}
