using BenchRando.IC;
using Benchwarp;
using ItemChanger;
using System.Collections.Generic;
using System.Linq;
using static BenchRando.BRData;

namespace RandoMapMod
{
    internal class BenchRandoInterop
    {
        internal static Dictionary<BenchKey, string> GetBenchTransitions()
        {
            return BenchLookup.ToDictionary(kvp => new BenchKey(kvp.Value.SceneName, kvp.Value.GetRespawnMarkerName()), kvp => kvp.Key);
        }

        internal static bool IsBenchRandoEnabled()
        {
            BRLocalSettingsModule bsm = ItemChangerMod.Modules.Get<BRLocalSettingsModule>();
            return bsm != null && bsm.LS.Settings.IsEnabled();
        }

        internal static bool HasBenchPlacements()
        {
            BRLocalSettingsModule bsm = ItemChangerMod.Modules.Get<BRLocalSettingsModule>();
            return bsm != null && bsm.LS.Settings.RandomizedItems is not BenchRando.Rando.ItemRandoMode.None;
        }
    }
}
