using BenchRando.IC;
using ItemChanger;
using System.Collections.Generic;
using System.Linq;
using static BenchRando.BRData;

namespace MapModS.Data
{
    internal class BenchRandoInterop
    {
        internal static Dictionary<(string, string), string> GetBenchTransitions()
        {
            return BenchLookup.ToDictionary(kvp => (kvp.Value.SceneName, kvp.Value.GetRespawnMarkerName()), kvp => kvp.Key);
        }

        internal static bool IsBenchRandoEnabled()
        {
            BRLocalSettingsModule bsm = ItemChangerMod.Modules.Get<BRLocalSettingsModule>();
            return bsm != null && bsm.LS.Settings.IsEnabled();
        }
    }
}
