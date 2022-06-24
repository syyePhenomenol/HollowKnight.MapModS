using BenchRando;
using System.Collections.Generic;
using System.Linq;

namespace MapModS.Data
{
    internal class BenchRandoInterop
    {
        internal static Dictionary<(string, string), string> GetBenchTransitions()
        {
            return BRData.BenchLookup.ToDictionary(kvp => (kvp.Value.SceneName, kvp.Value.GetRespawnMarkerName()), kvp => kvp.Key);
        }
    }
}
