using System;
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
    internal class BenchwarpInterop
    {
        internal const string BENCH_EXTRA_SUFFIX = "_Extra";
        internal const string BENCH_WARP_START = "Warp-Start";

        // Forward and reverse lookup
        private static Dictionary<BenchKey, string> benchNames = new();

        private static Dictionary<string, BenchKey> benchKeys = new();

        internal static string selectedBenchScene = "";
        internal static int benchPointer = 0;

        internal static void Load()
        {
            benchNames = new();
            benchKeys = new();

            if (Interop.HasBenchRando() && BenchRandoInterop.IsBenchRandoEnabled())
            {
                benchNames = BenchRandoInterop.GetBenchTransitions();
            }
            else
            {
                Dictionary<string, string> benchwarp = MapChanger.JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.RandoMapMod.Resources.benchwarp.json");

                foreach (KeyValuePair<string, string> kvp in benchwarp)
                {
                    Bench bench = Bench.Benches.FirstOrDefault(b => b.sceneName == kvp.Key);

                    if (bench is null) continue;

                    benchNames.Add(new(bench.sceneName, bench.respawnMarker), kvp.Value);
                }
            }

            BenchKey startKey = new(ItemChanger.Internal.Ref.Settings.Start.SceneName, "ITEMCHANGER_RESPAWN_MARKER");

            benchNames.Add(startKey, BENCH_WARP_START);

            benchKeys = benchNames.ToDictionary(t => t.Value, t => t.Key);
        }

        internal static IEnumerable<string> GetAllBenchNames()
        {
            return benchNames.Values;
        }

        internal static IEnumerable<string> GetAllBenchMappedScenes()
        {
            return benchKeys.Values.Select(benchKey => Finder.GetMappedScene(benchKey.SceneName));
        }

        internal static bool IsVisitedBench(string benchName)
        {
            return benchName is BENCH_WARP_START or $"{BENCH_WARP_START}{BENCH_EXTRA_SUFFIX}"
                || ((benchKeys.TryGetValue(benchName, out BenchKey benchKey)
                    || benchKeys.TryGetValue(benchName.Substring(0, benchName.Length - BENCH_EXTRA_SUFFIX.Length), out benchKey))
                && Benchwarp.Benchwarp.LS.visitedBenchScenes.Contains(benchKey));
        }

        internal static IEnumerable<string> GetVisitedBenchTransitions()
        {
            return Benchwarp.Benchwarp.LS.visitedBenchScenes
                .Where(b => benchNames.ContainsKey(b))
                .Select(b => benchNames[b])
                .Concat(new List<string>() { BENCH_WARP_START });
        }

        internal static (string, string) GetBenchKey(string transition)
        {
            if (benchKeys.TryGetValue(transition, out BenchKey benchKey))
            {
                return (benchKey.SceneName, benchKey.RespawnMarkerName);
            }

            return ("", "");
        }

        internal static IEnumerator DoBenchwarp(string benchName)
        {
            if (benchKeys.TryGetValue(benchName, out BenchKey benchKey))
            {
                yield return DoBenchwarpInternal(benchKey);
            }
            else if (benchKeys.TryGetValue(benchName.Substring(0, benchName.Length - BENCH_EXTRA_SUFFIX.Length), out benchKey))
            {
                yield return DoBenchwarpInternal(benchKey);
            }
        }

        private static IEnumerator DoBenchwarpInternal(BenchKey benchKey)
        {
            SetInventoryButton(true);
            yield return new WaitWhile(() => GameManager.instance.inventoryFSM.ActiveStateName != "Closed");
            yield return new WaitForSeconds(0.15f);
            UIManager.instance.TogglePauseGame();
            yield return new WaitWhile(() => !GameManager.instance.IsGamePaused());
            yield return new WaitForSecondsRealtime(0.1f);

            if (GameManager.instance.IsGamePaused())
            {
                Bench bench = Bench.Benches.FirstOrDefault(b => b.sceneName == benchKey.SceneName && b.respawnMarker == benchKey.RespawnMarkerName);

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

        internal static void AddBenchScenes(Dictionary<string, string> adjacentScenes)
        {
            foreach (KeyValuePair<BenchKey, string> kvp in benchNames)
            {
                adjacentScenes[kvp.Value] = kvp.Key.SceneName;
            }
        }

        internal static void AddBenchTerms(Dictionary<string, string> adjacentTerms)
        {
            foreach (KeyValuePair<BenchKey, string> kvp in benchNames)
            {
                adjacentTerms[kvp.Value] = kvp.Value;
            }
        }

        internal static bool IsBenchName(string benchName)
        {
            return benchKeys.ContainsKey(benchName) || benchKeys.ContainsKey(benchName.Substring(0, benchName.Length - BENCH_EXTRA_SUFFIX.Length));
        }

        private static void SetInventoryButton(bool value)
        {
            InputHandler.Instance.inputActions.openInventory.CommitWithState(value, ReflectionHelper.GetField<OneAxisInputControl, ulong>(InputHandler.Instance.inputActions.openInventory, "pendingTick") + 1, 0);
        }
    }
}
