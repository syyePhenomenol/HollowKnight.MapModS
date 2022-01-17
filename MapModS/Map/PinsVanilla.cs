using HutongGames.PlayMaker.Actions;
using Modding;
using System;
using System.Linq;
using UnityEngine;
using Vasi;

namespace MapModS.Map
{
    public static class PinsVanilla
    {
        public static void Hook()
        {
            On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
            On.GameManager.SetGameMap += GameManager_SetGameMap;
            On.GrubPin.OnEnable += On_GrubPin_OnEnable;
            On.FlamePin.OnEnable += On_FlamePin_Enable;
            On.BrummFlamePin.OnEnable += On_BrummFlamePin_Enable;
            ModHooks.GetPlayerBoolHook += BoolGetOverride;
        }

        public static void Unhook()
        {
            On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
            On.GameManager.SetGameMap -= GameManager_SetGameMap;
            On.GrubPin.OnEnable -= On_GrubPin_OnEnable;
            On.FlamePin.OnEnable -= On_FlamePin_Enable;
            On.BrummFlamePin.OnEnable -= On_BrummFlamePin_Enable;
            ModHooks.GetPlayerBoolHook -= BoolGetOverride;
        }

        public static void ReplaceBoolX(PlayMakerFSM fsm, string stateName, int index)
        {
            string boolString = FsmUtil.GetAction<PlayerDataBoolTest>(fsm, stateName, index).boolName.ToString();
            FsmUtil.GetAction<PlayerDataBoolTest>(fsm, stateName, index).boolName = "MMSX" + boolString;
        }

        // Replace all PlayerData boolNames with our own so we can force disable all pins,
        // without changing the existing PlayerData settings
        private static void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            if (self.FsmName == "toll_bench_pin"
                || (self.gameObject.name == "Crossroads_02" && self.FsmName == "Dreamer Pin"))
            {
                self.enabled = false;
            }
            else if (self.FsmName == "Check Grub Map Owned")
            {
                ReplaceBoolX(self, "Check", 1);
            }
            else if (self.gameObject.name == "Pin_Backer Ghost" && self.FsmName == "FSM")
            {
                ReplaceBoolX(self, "Check", 1);
                ReplaceBoolX(self, "Check", 3);
            }
            else if ((self.gameObject.name == "pin_banker" && self.FsmName == "pin_activation")
                || (self.gameObject.name == "pin_charm_slug" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_colosseum" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_dream moth" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_grub_king" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_hunter" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_jiji" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_leg eater" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_mapper" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_nailsmith" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_relic_dealer" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_sly (1)" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_sly" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_spa" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_stag_station (7)" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_stag_station" && self.FsmName == "FSM")
                || (self.gameObject.name == "pin_tram" && self.FsmName == "FSM"))
            {
                ReplaceBoolX(self, "Check", 0);
                ReplaceBoolX(self, "Check", 2);
            }
            else if ((self.gameObject.name == "pin_bench" && self.FsmName == "FSM")
                // For AdditionalMaps
                || (self.gameObject.name == "pin_bench(Clone)(Clone)" && self.FsmName == "FSM")
                || (self.gameObject.name == "Pin_BlackEgg" && self.FsmName == "FSM"))
            {
                if (self.FsmStates.FirstOrDefault(t => t.Name == "Check") != null)
                {
                    ReplaceBoolX(self, "Check", 0);
                }
            }
            else if ((self.gameObject.name == "Pin_Beast" && self.FsmName == "Display")
                || (self.gameObject.name == "Pin_Teacher" && self.FsmName == "Display")
                || (self.gameObject.name == "Pin_Watcher" && self.FsmName == "Display"))
            {
                ReplaceBoolX(self, "Init", 1);
            }
            else if ((self.gameObject.name == "Pin_Beast" && self.FsmName == "FSM")
                || (self.gameObject.name == "Pin_Teacher" && self.FsmName == "FSM")
                || (self.gameObject.name == "Pin_Watcher" && self.FsmName == "FSM"))
            {
                if (self.FsmStates.FirstOrDefault(t => t.Name == "Deactivate") != null)
                {
                    ReplaceBoolX(self, "Check", 0);
                }
            }
        }

        private static void GameManager_SetGameMap(On.GameManager.orig_SetGameMap orig, GameManager self, GameObject go_gameMap)
        {
            orig(self, go_gameMap);

            GameObject mapKey = GameObject.Find("Map Key");

            if (mapKey != null)
            {
                mapKey.transform.parent = null;
                mapKey.SetActive(false);
            }

            GameObject mapMarkers = GameObject.Find("Map Markers");

            if (mapMarkers != null)
            {
                mapMarkers.transform.parent = null;
                mapMarkers.SetActive(false);
            }

            SetupPins(go_gameMap);
        }

        // This is recursive and is done once per save load
        private static void SetupPins(GameObject obj)
        {
            if (obj == null)
                return;

            foreach (Transform child in obj.transform)
            {
                if (child == null)
                    continue;

                try
                {
                    switch (child.name)
                    {
                        case "pin_dream_tree":
                            // Move Ancestral Mound root pin
                            if (child.transform.parent.name == "Crossroads_ShamanTemple")
                            {
                                MoveSprite(child.gameObject, new Vector3(0.15f, -0.3f));
                            }

                            // Move Hive root pin (vanilla bug)
                            if (child.transform.parent.name == "Hive_02")
                            {
                                MoveSprite(child.gameObject, new Vector3(0.4f, -0.32f));
                            }
                            break;

                        // These are super buggy in vanilla!
                        case "Map Markers":
                            child.gameObject.SetActive(false);
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    MapModS.Instance.LogError(e);
                }

                SetupPins(child.gameObject);
            }
        }

        private static void On_GrubPin_OnEnable(On.GrubPin.orig_OnEnable orig, GrubPin self)
        {
            orig(self);

            if (!MapModS.LS.ModEnabled) return;

            self.gameObject.SetActive(false);
        }

        private static void On_FlamePin_Enable(On.FlamePin.orig_OnEnable orig, FlamePin self)
        {
            orig(self);

            if (!MapModS.LS.ModEnabled) return;

            self.gameObject.SetActive(false);
        }

        private static void On_BrummFlamePin_Enable(On.BrummFlamePin.orig_OnEnable orig, BrummFlamePin self)
        {
            orig(self);

            if (!MapModS.LS.ModEnabled) return;

            self.gameObject.SetActive(false);
        }

        public static void ForceDisablePins(GameObject obj)
        {
            if (obj == null)
                return;

            foreach (Transform child in obj.transform)
            {
                if (child == null)
                    continue;

                if (child.gameObject.name == "Pin_Black_Egg")
                {
                    if (MapModS.LS.ModEnabled)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                else if (child.gameObject.name == "pin_blue_health")
                {
                    if (MapModS.LS.ModEnabled)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
                else if (child.gameObject.name == "pin_dream_tree")
                {
                    if (MapModS.LS.ModEnabled)
                    {
                        child.gameObject.SetActive(false);
                    }
                }

                ForceDisablePins(child.gameObject);
            }
        }

        private static void MoveSprite(GameObject go, Vector3 offset)
        {
            go.transform.localPosition = offset;
        }

        public static bool BoolGetOverride(string boolName, bool orig)
        {
            if (boolName == "hasMarker")
            {
                return false;
            }

            if (boolName.StartsWith("MMSX"))
            {
                if (MapModS.LS.ModEnabled)
                {
                    return false;
                }
                else
                {
                    return PlayerData.instance.GetBool(boolName.Remove(0, 4));
                }
            }

            return orig;
        }
    }
}