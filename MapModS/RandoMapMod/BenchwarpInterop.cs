using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Benchwarp;
using InControl;
using MapChanger;
using Modding;
using UnityEngine;

namespace RandoMapMod
{
    //TODO: Turn into HookModule
    internal class BenchwarpInterop
    {
        // Forward and reverse lookup
        internal static Dictionary<(string, string), string> benchTransitions = new();

        internal static Dictionary<string, (string, string)> benchKeys = new();

        internal static Dictionary<string, List<WorldMapBenchDef>> Benches { get; private set; }
        internal static string selectedBenchScene = "";
        internal static int benchPointer = 0;

        public static void Load()
        {
            if (Interop.HasBenchRando() && BenchRandoInterop.IsBenchRandoEnabled())
            {
                benchTransitions = BenchRandoInterop.GetBenchTransitions();
            }
            else
            {
                Dictionary<string, string> benchwarp = MapChanger.JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.benchwarp.json");

                foreach (KeyValuePair<string, string> kvp in benchwarp)
                {
                    Bench bench = Bench.Benches.FirstOrDefault(b => b.sceneName == kvp.Key);

                    if (bench is null) continue;

                    benchTransitions.Add((bench.sceneName, bench.respawnMarker), kvp.Value);
                }
            }

            (string, string) startKey = new(ItemChanger.Internal.Ref.Settings.Start.SceneName, "ITEMCHANGER_RESPAWN_MARKER");

            benchTransitions.Add(startKey, "Warp-Start");

            benchKeys = benchTransitions.ToDictionary(t => t.Value, t => t.Key);
        }

        public static void OnEnterGame()
        {
            RandomizerMod.IC.TrackerUpdate.OnFinishedUpdate += UpdateVisitedBenches;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;

            UpdateVisitedBenches();
        }

        public static void OnQuitToMenu()
        {
            benchTransitions = new();
            benchKeys = new();

            RandomizerMod.IC.TrackerUpdate.OnFinishedUpdate -= UpdateVisitedBenches;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private static void OnSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
        {
            UpdateVisitedBenches();
        }

        public static void UpdateVisitedBenches()
        {
            Benches = Benchwarp.Benchwarp.LS.visitedBenchScenes
                .Select(b => new WorldMapBenchDef(b))
                .GroupBy(b => b.mappedSceneName)
                .ToDictionary(b => b.First().mappedSceneName, b => b.ToList());

            BenchKey startKey = new(ItemChanger.Internal.Ref.Settings.Start.SceneName, "ITEMCHANGER_RESPAWN_MARKER");
            WorldMapBenchDef startDef = new(startKey);

            if (Benches.ContainsKey(startDef.mappedSceneName))
            {
                Benches[startDef.mappedSceneName].Insert(0, startDef);
            }
            else
            {
                Benches[startDef.mappedSceneName] = new List<WorldMapBenchDef>() { startDef };
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
            WorldMapBenchDef bench = Benches[mappedScene][benchPointer];

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
                    Benchwarp.Events.SetToStart();
                }

                ChangeScene.WarpToRespawn();
            }
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

            mappedSceneName = Finder.GetMappedScene(sceneName);
        }

        public readonly string sceneName;
        public readonly string respawnMarker;
        public readonly string benchName;
        public readonly string mappedSceneName;
    }
}
