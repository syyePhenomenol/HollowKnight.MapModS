using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using MapModS.Data;
using MapModS.Settings;
using Modding;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Linq;
using UnityEngine;
using Vasi;

namespace MapModS.Map
{
    public class FullMap
    {
        public static void Hook()
        {
            On.RoughMapRoom.OnEnable += RoughMapRoom_OnEnable;
            IL.GameMap.WorldMap += ModifyMapBools;
            IL.GameMap.SetupMap += ModifyMapBools;
            IL.RoughMapRoom.OnEnable += ModifyMapBools;
            On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
            ModHooks.GetPlayerBoolHook += BoolGetOverride;
        }

        public static void Unhook()
        {
            On.RoughMapRoom.OnEnable -= RoughMapRoom_OnEnable;
            IL.GameMap.WorldMap -= ModifyMapBools;
            IL.GameMap.SetupMap -= ModifyMapBools;
            IL.RoughMapRoom.OnEnable -= ModifyMapBools;
            On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
            ModHooks.GetPlayerBoolHook -= BoolGetOverride;
        }

        // We need to purge the map after turning changing mode/disabling the mod. Essentially the opposite of SetupMap()
        public static void PurgeMap()
        {
            GameObject go_gameMap = GameManager.instance.gameMap;

            foreach (Transform areaObj in go_gameMap.transform)
            {
                if (!Colors.mapColors.ContainsKey(areaObj.name)
                    || areaObj.name == "WHITE_PALACE"
                    || areaObj.name == "GODS_GLORY") continue;

                foreach (Transform roomObj in areaObj.transform)
                {
                    roomObj.gameObject.SetActive(MainData.IsMinimalMapRoom(roomObj.name));

                    if (roomObj.name.Contains("Area Name"))
                    {
                        roomObj.gameObject.SetActive(true);
                    }

                    foreach (Transform roomObj2 in roomObj.transform.Cast<Transform>()
                        .Where(r => r.name.Contains("Area Name")))
                    {
                        roomObj2.gameObject.SetActive(true);
                    }
                }
            }
        }

        private class SpriteCopy : MonoBehaviour
        {
            public Sprite roughMap;
        }

        // Normally the game has no reason to "un-quill" the map, so the following allows us to do this
        // We keep a copy of the "rough map" sprite, and force-set that sprite every time before OnEnable() does its check
        private static void RoughMapRoom_OnEnable(On.RoughMapRoom.orig_OnEnable orig, RoughMapRoom self)
        {
            SpriteCopy spriteCopy = self.GetComponent<SpriteCopy>();
            SpriteRenderer sr = self.GetComponent<SpriteRenderer>();

            if (spriteCopy == null)
            {
                spriteCopy = self.gameObject.AddComponent<SpriteCopy>();
                spriteCopy.roughMap = sr.sprite;
            }
            else
            {
                sr.sprite = spriteCopy.roughMap;
                self.fullSpriteDisplayed = false;
            }

            ExtraMapData emd = self.GetComponent<ExtraMapData>();

            if (emd != null)
            {
                if (MapModS.LS.modEnabled)
                {
                    sr.color = emd.origCustomColor;
                }
                else
                {
                    sr.color = emd.origColor;
                }
            }

            orig(self);
        }

        // Adds the prefix "MMS_" to each boolName occurrence directly in the original code
        private static void ModifyMapBools(ILContext il)
        {
            ILCursor cursor = new(il);

            while (cursor.TryGotoNext
            (
                i => i.MatchLdstr("hasQuill")
                || i.MatchLdstr("mapAllRooms")
                || i.MatchLdstr("mapAbyss")
                || i.MatchLdstr("mapCity")
                || i.MatchLdstr("mapCliffs")
                || i.MatchLdstr("mapCrossroads")
                || i.MatchLdstr("mapMines")
                || i.MatchLdstr("mapDeepnest")
                || i.MatchLdstr("mapFogCanyon")
                || i.MatchLdstr("mapFungalWastes")
                || i.MatchLdstr("mapGreenpath")
                || i.MatchLdstr("mapOutskirts")
                || i.MatchLdstr("mapRoyalGardens")
                || i.MatchLdstr("mapRestingGrounds")
                || i.MatchLdstr("mapWaterways")
            ))
            {
                string name = cursor.ToString().Split('\"')[1];
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "MMS_" + name);
            }
        }

        // This method adds the prefix "MMS_" to each boolName, so that we can control it in BoolGetOverride
        public static void ReplaceBool(PlayMakerFSM fsm, string stateName, int index)
        {
            string boolString = FsmUtil.GetAction<PlayerDataBoolTest>(fsm, stateName, index).boolName.ToString();
            FsmUtil.GetAction<PlayerDataBoolTest>(fsm, stateName, index).boolName = "MMS_" + boolString;
        }

        public static void ReplaceNumberOfBools(PlayMakerFSM fsm, string stateName, int number)
        {
            for (int i = 0; i < number; i++)
            {
                ReplaceBool(fsm, stateName, i);
            }
        }

        private static void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            // Replace all instances of "hasMap" with "MMS_hasMap"
            if (self.gameObject.name == "Knight" && self.FsmName == "Map Control")
            {
                ReplaceBool(self, "Button Down Check", 1);
                FsmUtil.GetAction<GetPlayerDataBool>(self, "Has Map?", 3).boolName = "MMS_hasMap";
            }
            else if (self.gameObject.name == "Inventory" && self.FsmName == "Inventory Control")
            {
                ReplaceBool(self, "Next Map", 0);
                ReplaceBool(self, "Next Map 2", 0);
                ReplaceBool(self, "Next Map 3", 0);
            }

            // Patch the zoomed out map UI when we reveal the full map
            else if (self.gameObject.name == "World Map" && self.FsmName == "UI Control")
            {
                foreach (FsmState state in self.FsmStates)
                {
                    if (Utils.IsFSMMapState(state.Name))
                    {
                        ReplaceBool(self, state.Name, 0);
                        continue;
                    }

                    switch (state.Name)
                    {
                        case "Mi Right":
                        case "GP Up":
                        case "GP Right":
                        case "RG Up":
                        case "RG Right":
                        case "FG Left":
                        case "C Right":
                        case "Hive Down":
                        case "Wat Right":
                        case "Ab Right":
                            ReplaceNumberOfBools(self, state.Name, 1);
                            break;

                        case "QG Up":
                        case "Out Down":
                        case "Wat Down":
                            ReplaceNumberOfBools(self, state.Name, 2);
                            break;

                        case "FG Up":
                        case "C Up":
                        case "Out Up":
                        case "FW Up":
                        case "FW Down":
                            ReplaceNumberOfBools(self, state.Name, 3);
                            break;

                        case "QG Down":
                        case "C Down":
                        case "FW Left":
                        case "Wat Up":
                            ReplaceNumberOfBools(self, state.Name, 4);
                            break;

                        case "T Left":
                        case "T Right":
                        case "CR Right":
                        case "RG Left":
                        case "FG Down":
                        case "FW Right":
                        case "Wat Left":
                        case "Ab Left":
                            ReplaceNumberOfBools(self, state.Name, 5);
                            break;

                        case "CR Left":
                        case "RG Down":
                        case "FG Right":
                        case "C Left":
                        case "D Up":
                        case "Ab Up":
                            ReplaceNumberOfBools(self, state.Name, 6);
                            break;

                        case "CR Down":
                            ReplaceNumberOfBools(self, state.Name, 7);
                            break;

                        case "Mi Down":
                        case "GP Down":
                        case "QG Right":
                        case "Out Left":
                        case "D Right":
                        case "Pos Check":
                            ReplaceNumberOfBools(self, state.Name, 8);
                            break;

                        case "Cl Down":
                            ReplaceNumberOfBools(self, state.Name, 9);
                            break;

                        case "T Down":
                            ReplaceNumberOfBools(self, state.Name, 10);
                            break;

                        case "Activate":
                            FsmString[] boolStrings = { "MMS_visitedHive", "MMS_mapOutskirts" };
                            FsmUtil.GetAction<PlayerDataBoolAllTrue>(self, state.Name, 8).stringVariables = boolStrings;
                            break;
                    }
                }
            }

            // These are the map zone objects when zoomed out
            else if (Utils.IsFSMMapState(self.gameObject.name) && self.FsmName == "deactivate")
            {
                ReplaceBool(self, "Check", 0);
            }

            else if (self.FsmName == "Bench Control")
            {
                FsmUtil.GetAction<GetPlayerDataBool>(self, "Open Map", 0).boolName = "MMS_hasMap";
            }
        }

        public static bool BoolGetOverride(string boolName, bool orig)
        {
            // Always have a map when the mod is enabled
            if (boolName == "MMS_hasMap" && MapModS.LS.modEnabled)
            {
                return true;
            }

            if (boolName.StartsWith("MMS_"))
            {
                if (MapModS.LS.modEnabled &&
                    (MapModS.LS.mapMode == MapMode.FullMap
                        || MapModS.LS.mapMode == MapMode.TransitionRando
                        || MapModS.LS.mapMode == MapMode.TransitionRandoAlt))
                {
                    return true;
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