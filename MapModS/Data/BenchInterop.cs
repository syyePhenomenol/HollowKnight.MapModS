using System.Collections.Generic;
using System.Linq;
using B = Benchwarp.Bench;
using RD = RandomizerMod.RandomizerData.Data;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Data
{
    internal class BenchInterop
    {
        public static IEnumerable<string> GetVisitedBenchScenes()
        {
            return Benchwarp.Benchwarp.LS.visitedBenchScenes.Select(s => s.SceneName);
        }

        public static Dictionary<string, List<BenchDef>> GetVisitedBenches()
        {
            Dictionary<string, List<BenchDef>> benches = B.Benches.Where(b => Benchwarp.Benchwarp.LS.visitedBenchScenes.Any(s => b.sceneName == s.SceneName))
                .Select(b => new BenchDef(b.sceneName, b.areaName + " " + b.name))
                .GroupBy(b => b.mappedSceneName)
                .ToDictionary(b => b.First().mappedSceneName, b => b.ToList());

            BenchDef start = new(RD.GetStartDef(RM.RS.GenerationSettings.StartLocationSettings.StartLocation).SceneName, "Start");

            if (benches.ContainsKey(start.mappedSceneName))
            {
                benches[start.mappedSceneName].Insert(0, start);
            }
            else
            {
                benches[start.mappedSceneName] = new List<BenchDef>() { start };
            }

            return benches;
        }

        public static void DoBenchwarp(string scene)
        {
            B bench = B.Benches.FirstOrDefault(b => b.sceneName == scene);

            if (bench != null)
            {
                bench.SetBench();
            }
            else
            {
                Benchwarp.Events.SetToStart();
            }

            Benchwarp.ChangeScene.WarpToRespawn();
        }
    }

    public class BenchDef
    {
        public BenchDef(string sceneName, string benchName)
        {
            this.sceneName = sceneName;
            this.benchName = benchName;

            if (MainData.IsNonMappedScene(sceneName))
            {
                MapRoomDef mrd = MainData.GetNonMappedRoomDef(sceneName);

                if (mrd.includeWithAdditionalMaps || !Dependencies.HasDependency("AdditionalMaps"))
                {
                    mappedSceneName = mrd.mappedScene;
                    return;
                }
            }

            mappedSceneName = sceneName;
        }

        public readonly string sceneName;
        public readonly string mappedSceneName;
        public readonly string benchName;
    }
}
