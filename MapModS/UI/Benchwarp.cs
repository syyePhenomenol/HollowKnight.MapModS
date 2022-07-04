﻿using MagicUI.Core;
using MagicUI.Elements;
using MapModS.Data;
using MapModS.Map;
using MapModS.Pathfinding;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using BI = MapModS.Data.BenchwarpInterop;
using L = RandomizerMod.Localization;
using TP = MapModS.UI.TransitionPersistent;

namespace MapModS.UI
{
    internal class Benchwarp
    {
        private static LayoutRoot layout;

        // Only for normal modes (not transition mode)
        private static TextObject benchwarpText;
        public static string selectedBenchScene = "";
        private static int benchPointer = 0;

        private static bool Condition()
        {
            return MapModS.LS.ModEnabled
                && !TransitionData.TransitionModeActive()
                && !GUI.lockToggleEnable
                && GUI.worldMapOpen
                && MapModS.GS.BenchwarpWorldMap;
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
            attackHoldTimer.Reset();
        }

        public static void UpdateAll()
        {
            if (Dependencies.HasBenchwarp() && !TransitionData.TransitionModeActive())
            {
                if (!MapModS.GS.BenchwarpWorldMap)
                {
                    ResetBenchSelection();
                }

                BI.UpdateVisitedBenches();
                UpdateBenchwarpText();
                //MapRooms.SetSelectedRoomColor(selectedBenchScene, false);
            }
        }

        public static void UpdateBenchwarpText()
        {
            string text = "";

            if (Dependencies.HasBenchwarp() && selectedBenchScene != "")
            {
                List<InControl.BindingSource> bindings = new(InputHandler.Instance.inputActions.attack.Bindings);

                text += $"{L.Localize("Hold")} ";

                text += Utils.GetBindingsText(bindings);

                text += $" {L.Localize("to warp to")} {GetSelectedBench().benchName.Replace("Warp ", "").Replace("Bench ", "")}.";

                if (BI.benches.ContainsKey(selectedBenchScene) && BI.benches[selectedBenchScene].Count > 1)
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
                || !MapModS.LS.ModEnabled
                || TransitionData.TransitionModeActive()
                || GUI.lockToggleEnable
                || !Dependencies.HasBenchwarp()
                || GameManager.instance.IsGamePaused())
            {
                return;
            }

            if (GUI.worldMapOpen && MapModS.GS.BenchwarpWorldMap)
            {
                if (benchUpdateThread != null && benchUpdateThread.IsAlive) return;

                benchUpdateThread = new(() =>
                {
                    if (GetBenchClosestToMiddle(selectedBenchScene, out selectedBenchScene))
                    {
                        benchPointer = 0;
                        MapRooms.SetSelectedRoomColor(selectedBenchScene, false);
                        UpdateBenchwarpText();
                    }
                });

                benchUpdateThread.Start();
            }
            else if (GUI.worldMapOpen || GUI.quickMapOpen)
            {
                MapRooms.SetSelectedRoomColor("", false);
            }
        }

        public static bool GetBenchClosestToMiddle(string previousScene, out string selectedScene)
        {
            selectedScene = "";
            double minDistance = double.PositiveInfinity;

            GameObject go_GameMap = GameManager.instance.gameMap;

            if (go_GameMap == null) return false;

            foreach (Transform areaObj in go_GameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (!roomObj.gameObject.activeSelf || !BI.benches.ContainsKey(roomObj.name)) continue;

                    ExtraMapData emd = roomObj.GetComponent<ExtraMapData>();
                    if (emd == null) continue;

                    double distance = Utils.DistanceToMiddle(roomObj);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        selectedScene = emd.sceneName;
                    }
                }
            }

            return selectedScene != previousScene;
        }

        public static Stopwatch attackHoldTimer = new();

        // Called every frame
        public static void Update()
        {
            if (!MapModS.LS.ModEnabled
                || !GUI.worldMapOpen
                || GUI.lockToggleEnable
                || !Dependencies.HasBenchwarp()
                || GameManager.instance.IsGamePaused()
                || InputHandler.Instance == null)
            {
                return;
            }

            // Hold attack to benchwarp
            if (InputHandler.Instance.inputActions.attack.WasPressed)
            {
                attackHoldTimer.Restart();
            }

            if (InputHandler.Instance.inputActions.attack.WasReleased)
            {
                if (!TransitionData.TransitionModeActive()
                    && MapModS.GS.BenchwarpWorldMap
                    && attackHoldTimer.ElapsedMilliseconds < 500)
                {
                    ToggleBench();
                    UpdateBenchwarpText();
                }

                attackHoldTimer.Reset();
            }

            if (attackHoldTimer.ElapsedMilliseconds >= 500)
            {
                if (TransitionData.TransitionModeActive())
                {
                    if (TP.selectedRoute.Any() && TP.selectedRoute.First().IsBenchwarpTransition())
                    {
                        attackHoldTimer.Reset();
                        GameManager.instance.StartCoroutine(BI.DoBenchwarp(TP.selectedRoute.First()));
                        return;
                    }

                    attackHoldTimer.Reset();
                }
                else if (MapModS.GS.BenchwarpWorldMap)
                {
                    if (selectedBenchScene != "")
                    {
                        attackHoldTimer.Reset();
                        GameManager.instance.StartCoroutine(BI.DoBenchwarp(selectedBenchScene, benchPointer));
                        return;
                    }

                    attackHoldTimer.Reset();
                }
            }
        }

        private static void ToggleBench()
        {
            if (!BI.benches.ContainsKey(selectedBenchScene)
                || benchPointer > BI.benches[selectedBenchScene].Count - 1)
            {
                MapModS.Instance.LogWarn("Invalid bench toggle");
                return;
            }

            benchPointer = (benchPointer + 1) % BI.benches[selectedBenchScene].Count;
        }

        private static WorldMapBenchDef GetSelectedBench()
        {
            if (!BI.benches.ContainsKey(selectedBenchScene)
                || benchPointer > BI.benches[selectedBenchScene].Count - 1)
            {
                MapModS.Instance.LogWarn("Invalid bench selection");
                return BI.benches.First().Value.First();
            }

            return BI.benches[selectedBenchScene][benchPointer];
        }
    }
}
