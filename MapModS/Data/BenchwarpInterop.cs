using Benchwarp;
using InControl;
using Modding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RD = RandomizerMod.RandomizerData.Data;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Data
{
    internal class BenchwarpInterop
    {
        // Forward and reverse lookup
        internal static Dictionary<(string, string), string> benchTransitions = new();

        internal static Dictionary<string, (string, string)> benchKeys = new();

        internal static Dictionary<string, List<WorldMapBenchDef>> benches;
        internal static string selectedBenchScene = "";
        internal static int benchPointer = 0;

        public static void Load()
        {
            benchTransitions = new();
            benchKeys = new();

            if (Dependencies.HasBenchRando() && IsBenchRandoEnabled())
            {
                benchTransitions = BenchRandoInterop.GetBenchTransitions();
            }
            else
            {
                Dictionary<string, string> benchwarp = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.benchwarp.json");

                foreach(KeyValuePair<string, string> kvp in benchwarp)
                {
                    Bench bench = Bench.Benches.FirstOrDefault(b => b.sceneName == kvp.Key);

                    if (bench == null) continue;

                    benchTransitions.Add((bench.sceneName, bench.respawnMarker), kvp.Value);
                }
            }

            (string, string) startKey = new(ItemChanger.Internal.Ref.Settings.Start.SceneName, "ITEMCHANGER_RESPAWN_MARKER");

            benchTransitions.Add(startKey, "Warp-Start");

            benchKeys = benchTransitions.ToDictionary(t => t.Value, t => t.Key);
        }

        public static void UpdateVisitedBenches()
        {
            benches = Benchwarp.Benchwarp.LS.visitedBenchScenes
                .Select(b => new WorldMapBenchDef(b))
                .GroupBy(b => b.mappedSceneName)
                .ToDictionary(b => b.First().mappedSceneName, b => b.ToList());

            BenchKey startKey = new(ItemChanger.Internal.Ref.Settings.Start.SceneName, "ITEMCHANGER_RESPAWN_MARKER");
            WorldMapBenchDef startDef = new(startKey);

            if (benches.ContainsKey(startDef.mappedSceneName))
            {
                benches[startDef.mappedSceneName].Insert(0, startDef);
            }
            else
            {
                benches[startDef.mappedSceneName] = new List<WorldMapBenchDef>() { startDef };
            }
        }

        public static IEnumerable<string> GetVisitedBenchTransitions()
        {
            return Benchwarp.Benchwarp.LS.visitedBenchScenes
                .Where(b => benchTransitions.ContainsKey((b.SceneName, b.RespawnMarkerName)))
                .Select(b => benchTransitions[(b.SceneName, b.RespawnMarkerName)])
                .Concat(new List<string>() { "Warp-Start" });
        }

        internal static IEnumerator DoBenchwarp(string transition)
        {
            (string, string) benchKey = benchKeys[transition];

            yield return DoBenchwarpInternal(benchKey.Item1, benchKey.Item2);
        }

        internal static IEnumerator DoBenchwarp(string mappedScene, int benchPointer)
        {
            WorldMapBenchDef bench = benches[mappedScene][benchPointer];

            yield return DoBenchwarpInternal(bench.sceneName, bench.respawnMarker);
        }

        private static IEnumerator DoBenchwarpInternal(string scene, string respawnMarker)
        {
            InputHandler.Instance.inputActions.openInventory.CommitWithState(true, ReflectionHelper.GetField<OneAxisInputControl, ulong>(InputHandler.Instance.inputActions.openInventory, "pendingTick") + 1, 0);
            yield return new WaitWhile(() => GameManager.instance.inventoryFSM.ActiveStateName != "Closed");
            yield return new WaitForSeconds(0.15f);
            UIManager.instance.TogglePauseGame();
            yield return new WaitWhile(() => !GameManager.instance.IsGamePaused());
            yield return new WaitForSecondsRealtime(0.1f);

            if (GameManager.instance.IsGamePaused())
            {
                Bench bench = Bench.Benches.FirstOrDefault(b => b.sceneName == scene && b.respawnMarker == respawnMarker);

                if (bench != null)
                {
                    bench.SetBench();
                }
                else
                {
                    Events.SetToStart();
                }

                ChangeScene.WarpToRespawn();
            }
        }

        internal static bool IsBenchRandoEnabled()
        {
            return RM.RS.Context.LM.TermLookup.Keys.Any(t => t.StartsWith("Bench-"));
        }
    }


    public class WorldMapBenchDef
    {
        public WorldMapBenchDef(BenchKey benchKey)
        {
            sceneName = benchKey.SceneName;
            respawnMarker = benchKey.RespawnMarkerName;

            if (BenchwarpInterop.benchTransitions.ContainsKey((sceneName, respawnMarker)))
            {
                benchName = BenchwarpInterop.benchTransitions[(sceneName, respawnMarker)].ToCleanName();
            }
            else
            {
                benchName = sceneName + " " + respawnMarker;
            }

            if (Dependencies.HasAdditionalMaps() && sceneName == "GG_Workshop")
            {
                mappedSceneName = "GG_Atrium";
                return;
            }

            if (MainData.IsNonMappedScene(sceneName))
            {
                MapRoomDef mrd = MainData.GetNonMappedRoomDef(sceneName);

                if (!Dependencies.HasAdditionalMaps() || mrd.includeWithAdditionalMaps)
                {
                    mappedSceneName = mrd.mappedScene;
                    return;
                }
            }

            mappedSceneName = sceneName;
        }

        public readonly string sceneName;
        public readonly string respawnMarker;
        public readonly string benchName;
        public readonly string mappedSceneName;
    }
}
