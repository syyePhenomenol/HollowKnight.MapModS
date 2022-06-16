using MagicUI.Core;
using MagicUI.Elements;
using MapModS.Data;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using static MapModS.Map.Transition;
using L = RandomizerMod.Localization;
using TP = MapModS.UI.TransitionPersistent;

namespace MapModS.UI
{
    internal class Benchwarp
    {
        private static LayoutRoot layout;

        // Only for normal modes (not transition mode)
        private static TextObject benchwarpText;
        private static Dictionary<string, List<BenchDef>> benches = new();
        private static string selectedBenchScene = "";
        private static int benchPointer = 0;

        private static bool Condition()
        {
            return MapModS.LS.modEnabled
                && !TransitionData.TransitionModeActive()
                && !GUI.lockToggleEnable
                && GUI.worldMapOpen
                && MapModS.GS.benchwarpWorldMap;
        }

        public static void Build()
        {
            if (layout == null)
            {
                layout = new(true, "Benchwarp Layout");
                layout.VisibilityCondition = Condition;

                benchwarpText = UIExtensions.TextFromEdge(layout, "Benchwarp Text", false);

                UpdateAll();
            }
        }

        public static void Destroy()
        {
            layout?.Destroy();
            layout = null;

            ResetBenchSelection();
        }

        public static void ResetBenchSelection()
        {
            selectedBenchScene = "";
            benchPointer = 0;
            SetSelectedRoomColor(selectedBenchScene, false);
            attackHoldTimer.Reset();
        }

        public static void UpdateAll()
        {
            if (Dependencies.HasDependency("Benchwarp"))
            {
                GetBenchScenes();
                UpdateBenchwarpText();
            }
        }

        // Just grab one bench per actual scene for now
        public static void GetBenchScenes()
        {
            benches = BenchInterop.GetVisitedBenches();
        }

        public static void UpdateBenchwarpText()
        {
            string text = "";

            if (Dependencies.HasDependency("Benchwarp") && selectedBenchScene != "")
            {
                List<InControl.BindingSource> bindings = new(InputHandler.Instance.inputActions.attack.Bindings);

                text += $"{L.Localize("Hold")} ";

                text += Utils.GetBindingsText(bindings);

                text += $" {L.Localize("to benchwarp to")} {GetSelectedBench().benchName}.";

                if (benches.ContainsKey(selectedBenchScene) && benches[selectedBenchScene].Count > 1)
                {
                    text += $"\n{L.Localize("Tap")} ";

                    text += Utils.GetBindingsText(bindings);

                    text += $" {L.Localize("to toggle to another bench here")}.";
                }
            }

            benchwarpText.Text = text;
        }

        private static Thread benchUpdateThread;

        // Called every 0.1 seconds
        public static void UpdateSelectedBenchCoroutine()
        {
            if (layout == null
                || !MapModS.LS.modEnabled
                || TransitionData.TransitionModeActive()
                || !GUI.worldMapOpen
                || GUI.lockToggleEnable
                || !Dependencies.HasDependency("Benchwarp")
                || !MapModS.GS.benchwarpWorldMap
                || GameManager.instance.IsGamePaused())
            {
                return;
            }

            if (benchUpdateThread != null && benchUpdateThread.IsAlive) return;

            benchUpdateThread = new(() =>
            {
                if (GetBenchClosestToMiddle())
                {
                    benchPointer = 0;
                    SetSelectedRoomColor(selectedBenchScene, false);
                    UpdateBenchwarpText();
                }
            });

            benchUpdateThread.Start();
        }

        public static bool GetBenchClosestToMiddle()
        {
            string previousScene = selectedBenchScene;
            selectedBenchScene = "";

            double minDistance = double.PositiveInfinity;

            GameObject go_GameMap = GameManager.instance.gameMap;

            if (go_GameMap == null) return false;

            foreach (Transform areaObj in go_GameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (!roomObj.gameObject.activeSelf || !benches.ContainsKey(roomObj.name)) continue;

                    ExtraMapData extra = roomObj.GetComponent<ExtraMapData>();

                    if (extra == null) continue;

                    double distance = Utils.DistanceToMiddle(roomObj);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        selectedBenchScene = extra.sceneName;
                    }
                }
            }

            return selectedBenchScene != previousScene;
        }

        public static Stopwatch attackHoldTimer = new();

        // Called every frame
        public static void Update()
        {
            if (!MapModS.LS.modEnabled
                || !GUI.worldMapOpen
                || GUI.lockToggleEnable
                || !Dependencies.HasDependency("Benchwarp")
                || !MapModS.GS.benchwarpWorldMap
                || GameManager.instance.IsGamePaused()
                || InputHandler.Instance == null)
            {
                return;
            }

            // Hold attack to benchwarp in transition world map
            if (TransitionData.TransitionModeActive())
            {
                if (!TP.selectedRoute.Any() || !TP.selectedRoute.First().IsBenchwarpTransition()) return;

                if (InputHandler.Instance.inputActions.attack.WasPressed)
                {
                    attackHoldTimer.Restart();
                }

                if (InputHandler.Instance.inputActions.attack.WasReleased)
                {
                    attackHoldTimer.Reset();
                }

                if (attackHoldTimer.ElapsedMilliseconds >= 500)
                {
                    attackHoldTimer.Reset();
                    GameManager.instance.StartCoroutine(CloseInventoryBenchwarp(PathfinderData.GetBenchwarpScene(TP.selectedRoute.First())));
                }
            }
            else
            {
                if (selectedBenchScene == "") return;

                if (InputHandler.Instance.inputActions.attack.WasPressed)
                {
                    attackHoldTimer.Restart();
                }

                if (InputHandler.Instance.inputActions.attack.WasReleased)
                {
                    if (attackHoldTimer.ElapsedMilliseconds < 500)
                    {
                        ToggleBench();
                        UpdateBenchwarpText();
                    }

                    attackHoldTimer.Reset();
                }

                if (attackHoldTimer.ElapsedMilliseconds >= 500)
                {
                    attackHoldTimer.Reset();
                    GameManager.instance.StartCoroutine(CloseInventoryBenchwarp(GetSelectedBench().sceneName));
                }
            }
        }

        private static void ToggleBench()
        {
            if (!benches.ContainsKey(selectedBenchScene)
                || benchPointer > benches[selectedBenchScene].Count - 1)
            {
                MapModS.Instance.LogWarn("Invalid bench toggle");
                return;
            }

            benchPointer = (benchPointer + 1) % benches[selectedBenchScene].Count;
        }

        private static BenchDef GetSelectedBench()
        {
            if (!benches.ContainsKey(selectedBenchScene)
                || benchPointer > benches[selectedBenchScene].Count - 1)
            {
                MapModS.Instance.LogWarn("Invalid bench selection");
                return benches.First().Value.First();
            }

            return benches[selectedBenchScene][benchPointer];
        }

        private static IEnumerator CloseInventoryBenchwarp(string scene)
        {
            GameManager.instance.inventoryFSM.SendEvent("HERO DAMAGED");
            yield return new WaitWhile(() => GameManager.instance.inventoryFSM.ActiveStateName != "Closed");
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            UIManager.instance.TogglePauseGame();
            yield return new WaitWhile(() => !GameManager.instance.IsGamePaused());
            yield return new WaitForSecondsRealtime(0.1f);
            if (GameManager.instance.IsGamePaused())
            {
                BenchInterop.DoBenchwarp(scene);
            }
        }
    }
}
