using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchwarp;
using BenchRando;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Data
{
    internal class BenchRandoInterop
    {
        internal static Dictionary<(string, string), string> GetBenchTransitions()
        {
            Dictionary<(string, string), string> benchTransitions = new();

            foreach (KeyValuePair<string, BenchDef> kvp in BRData.BenchLookup)
            {
                Bench bench = Bench.Benches.FirstOrDefault(b => b.sceneName == kvp.Key);

                if (bench == null) continue;

                benchTransitions.Add((kvp.Value.SceneName, kvp.Value.GetRespawnMarkerName()), kvp.Key);
            }

            return benchTransitions;
        }

        internal static bool IsBenchRandoEnabled()
        {
            return RM.RS.Context.LM.TermLookup.Keys.Any(t => t.StartsWith("Bench-"));
        }
    }
}
