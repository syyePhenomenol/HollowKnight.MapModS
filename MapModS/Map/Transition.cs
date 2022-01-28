using MapModS.Data;
using RandomizerCore;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace MapModS.Map
{
    internal static class Transition
    {
        public static HashSet<string> nonMapScenes = new()
        {
            "Room_temple",
            "Room_shop",
            "Room_Town_Stag_Station",
            "Room_mapper",
            "Room_Bretta",
            "Room_Ouiji",
            "Grimm_Divine",
            "Grimm_Main_Tent",
            "Crossroads_ShamanTemple",
            "Room_ruinhouse",
            "Room_Charm_Shop",
            "Room_Mender_House",
            "Fungus1_35",
            "Fungus1_36",
            "Room_Slug_Shrine",
            "Room_nailmaster_02",
            "Room_Fungus_Shaman",
            "Fungus3_archive",
            "Fungus3_archive_02",
            "Room_spider_small",
            "Deepnest_Spider_Town",
            "Deepnest_45_v02",
            "Deepnest_East_17",
            "Room_nailmaster_03",
            "Room_Wyrm",
            "Room_Colosseum_01",
            "Room_Colosseum_02",
            "Room_Colosseum_Spectate",
            "Abyss_15",
            "Abyss_Lighthouse_room",
            "Room_GG_Shortcut",
            "Room_nailsmith",
            "Ruins_House_01",
            "Ruins_House_02",
            "Ruins_House_03",
            "RestingGrounds_07",
            "Room_Mansion",
            "Mines_35",
            "Room_Queen",
            "Cliffs_03",
            "Room_nailmaster",
            "White_Palace_11"
        };

        public static GameObject CreateExtraMapRooms(GameMap gameMap)
        {
            GameObject go_extraMapRooms = new GameObject("MMS Custom Map Rooms");
            go_extraMapRooms.layer = 5;
            go_extraMapRooms.transform.SetParent(gameMap.transform);
            go_extraMapRooms.transform.localPosition = new Vector3(-13f, 16f, 0);
            go_extraMapRooms.SetActive(true);

            var areaNamePrefab = UnityEngine.Object.Instantiate(gameMap.areaCliffs.transform.GetChild(0).gameObject);
            
            var prefabTMP = areaNamePrefab.GetComponent<TextMeshPro>();
            prefabTMP.color = Color.white;
            prefabTMP.fontSize = 2.5f;
            prefabTMP.enableWordWrapping = false;
            prefabTMP.alignment = TextAlignmentOptions.TopLeft;

            areaNamePrefab.GetComponent<SetTextMeshProGameText>().sheetName = "MMS";
            areaNamePrefab.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            areaNamePrefab.GetComponent<RectTransform>().anchorMax = Vector2.zero;

            areaNamePrefab.SetActive(false);

            int mapPositionCounter = 0;

            foreach (string scene in nonMapScenes)
            {
                GameObject go_extraMapRoom = UnityEngine.Object.Instantiate(areaNamePrefab, go_extraMapRooms.transform);

                go_extraMapRoom.name = scene;
                go_extraMapRoom.GetComponent<SetTextMeshProGameText>().convName = scene;
                go_extraMapRoom.transform.localPosition = new Vector3((mapPositionCounter % 4) * 4f, -0.8f * (mapPositionCounter / 4), 0);

                var tmp = go_extraMapRoom.GetComponent<TextMeshPro>();

                ExtraMapData extraData = go_extraMapRoom.gameObject.AddComponent<ExtraMapData>();
                extraData.origColor = Color.white;
                extraData.sceneName = scene;

                go_extraMapRoom.SetActive(true);

                mapPositionCounter++;
            }

            return go_extraMapRooms;
        }

        enum RoomState
        {
            Default,
            Current,
            Adjacent,
            OutOfLogic
        }

        readonly static Dictionary<RoomState, Vector4> roomColor = new()
        {
            { RoomState.Default, new(255, 255, 255, 0.3f) }, // white
            { RoomState.Current, new(0, 255, 0, 0.4f) }, // green
            { RoomState.Adjacent, new(0, 255, 255, 0.4f) }, // cyan
            { RoomState.OutOfLogic, new(255, 0, 0, 0.3f) } // red
        };

        private static string GetAdjacentScene(RandomizerMod.RandomizerData.TransitionDef transitionDef)
        {
            foreach (TransitionPlacement tp in RandomizerMod.RandomizerMod.RS.Context.transitionPlacements)
            {
                if (tp.target == null) return null;

                if (tp.source.Name == transitionDef.Name)
                {
                    return RandomizerMod.RandomizerData.Data.GetTransitionDef(tp.target.Name).SceneName;
                }
            }

            if (transitionDef.VanillaTarget == null) return null;

            // If it's not in TransitionPlacements, it's the vanilla target
            return RandomizerMod.RandomizerData.Data.GetTransitionDef(transitionDef.VanillaTarget).SceneName;
        }

        private static void SetActiveSRColor(Transform transform, bool active, SpriteRenderer sr, Vector4 color)
        {
            if (sr == null)
            {
                transform.gameObject.SetActive(false);
                return;
            }

            transform.gameObject.SetActive(active);
            sr.color = color;
        }

        private static void SetActiveTMPColor(Transform transform, bool active, TextMeshPro tmp, Vector4 color)
        {
            if (tmp == null)
            {
                transform.gameObject.SetActive(false);
                return;
            }

            transform.gameObject.SetActive(active);
            tmp.color = color;
        }

        public static HashSet<string> SetupMapTransitionMode(GameMap gameMap, bool isAlt)
        {
            HashSet<string> inLogicScenes = new();

            HashSet<string> outOfLogicScenes = new();

            HashSet<string> visitedAdjacentScenes = new();

            HashSet<string> uncheckedReachableScenes = new();

            HashSet<string> visitedScenes = new(PlayerData.instance.scenesVisited);
            visitedScenes.Add(StringUtils.CurrentNormalScene());

            RandomizerCore.Logic.ProgressionManager pm = RandomizerMod.RandomizerMod.RS.TrackerData.pm;

            // Get the scenes that are visited or connected in vanilla fashion to other visited scenes
            foreach (KeyValuePair<string, RandomizerCore.Logic.LogicTransition> transitionEntry in RandomizerMod.RandomizerMod.RS.TrackerData.lm.TransitionLookup)
            {
                RandomizerMod.RandomizerData.TransitionDef transitionDef = RandomizerMod.RandomizerData.Data.GetTransitionDef(transitionEntry.Key);

                if (pm.Has(transitionEntry.Value.term.Id))
                {
                    inLogicScenes.Add(transitionDef.SceneName);
                    continue;
                }

                if (PlayerData.instance.scenesVisited.Contains(transitionDef.SceneName))
                {
                    outOfLogicScenes.Add(transitionDef.SceneName);
                }
            }

            // Get the scenes adjacent to the current scene that are in logic
            foreach (KeyValuePair<string, RandomizerCore.Logic.LogicTransition> transitionEntry in RandomizerMod.RandomizerMod.RS.TrackerData.lm.TransitionLookup)
            {
                if (pm.Has(transitionEntry.Value.term.Id))
                {
                    RandomizerMod.RandomizerData.TransitionDef transitionDef = RandomizerMod.RandomizerData.Data.GetTransitionDef(transitionEntry.Key);

                    if (transitionDef.SceneName == StringUtils.CurrentNormalScene()
                        && !RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableTransitions.Contains(transitionDef.Name))
                    {
                        string adjacentScene = GetAdjacentScene(transitionDef);

                        if (adjacentScene == null || !inLogicScenes.Contains(adjacentScene)) continue;

                        visitedAdjacentScenes.Add(adjacentScene);
                    }
                }
            }

            // Get scenes where there are unchecked reachable transitions
            foreach (string sourceTransition in RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableTransitions)
            {
                uncheckedReachableScenes.Add(RandomizerMod.RandomizerData.Data.GetTransitionDef(sourceTransition).SceneName);
            }

            // Show rooms with custom colors
            foreach (Transform areaObj in gameMap.transform)
            {
                if (areaObj.name == "Grub Pins"
                        || areaObj.name == "Dream_Gate_Pin"
                        || areaObj.name == "Compass Icon"
                        || areaObj.name == "Shade Pos"
                        || areaObj.name == "Flame Pins"
                        || areaObj.name == "Dreamer Pins"
                        || areaObj.name == "Map Markers") continue;

                if (areaObj.name == "MMS Custom Map Rooms")
                {
                    areaObj.gameObject.SetActive(true);
                }

                foreach (Transform roomObj in areaObj.transform)
                {
                    ExtraMapData emd = roomObj.GetComponent<ExtraMapData>();

                    if (emd == null)
                    {
                        //MapModS.Instance.Log(roomObj.name);
                        roomObj.gameObject.SetActive(false);
                        continue;
                    }

                    bool active = false;
                    Vector4 color = roomColor[RoomState.Default];

                    if (isAlt)
                    {
                        if (outOfLogicScenes.Contains(emd.sceneName))
                        {
                            color = roomColor[RoomState.OutOfLogic];
                        }

                        if (visitedScenes.Contains(emd.sceneName))
                        {
                            color = roomColor[RoomState.Default];
                            active = true;
                        }
                    }
                    else
                    {
                        if (outOfLogicScenes.Contains(emd.sceneName))
                        {
                            color = roomColor[RoomState.OutOfLogic];
                            active = true;
                        }

                        if (inLogicScenes.Contains(emd.sceneName))
                        {
                            color = roomColor[RoomState.Default];
                            active = true;
                        }
                    }

                    if (emd.sceneName == StringUtils.CurrentNormalScene())
                    {
                        color = roomColor[RoomState.Current];
                    }

                    if (visitedAdjacentScenes.Contains(emd.sceneName))
                    {
                        color = roomColor[RoomState.Adjacent];
                    }
                    
                    if (uncheckedReachableScenes.Contains(emd.sceneName))
                    {
                        color.w = 1f;
                    }

                    if (areaObj.name == "MMS Custom Map Rooms")
                    {
                        TextMeshPro tmp = roomObj.gameObject.GetComponent<TextMeshPro>();
                        SetActiveTMPColor(roomObj, active, tmp, color);
                        emd.origTransitionColor = tmp.color;
                        continue;
                    }

                    var sr = roomObj.GetComponent<SpriteRenderer>();

                    // For AdditionalMaps room objects, the child has the SR
                    if (emd.sceneName.Contains("White_Palace"))
                    {
                        foreach (Transform roomObj2 in roomObj.transform)
                        {
                            if (!roomObj2.name.Contains("RWP")) continue;
                            sr = roomObj2.GetComponent<SpriteRenderer>();
                            break;
                        }
                    }

                    SetActiveSRColor(roomObj, active, sr, color);

                    if (!active) continue;

                    // Force disable sub area names
                    foreach (Transform roomObj2 in roomObj.transform)
                    {
                        if (roomObj2.name.Contains("Area Name"))
                        {
                            roomObj2.gameObject.SetActive(false);
                        }
                    }

                    emd.origTransitionColor = sr.color;
                }
            }

            gameMap.panMinX = -28f;
            gameMap.panMaxX = 26f;
            gameMap.panMinY = -25f;
            gameMap.panMaxY = 20f;

            if (isAlt)
            {
                return visitedScenes;
            }

            return new(inLogicScenes.Union(outOfLogicScenes));
        }

        private static string GetActualSceneName(string objName)
        {
            // Some room objects have non-standard scene names, so we truncate the name
            // in these situations

            if (objName == "Ruins1_31_top_2") return "Ruins1_31b";

            for (int i = 0; i < 2; i++)
            {
                if (RandomizerMod.RandomizerData.Data.IsRoom(objName))
                {
                    return objName;
                }

                objName = StringUtils.DropSuffix(objName);
            }

            return null;
        }

        public class ExtraMapData : MonoBehaviour
        {
            public Color origColor;
            public Color origTransitionColor;
            public string sceneName;
        }

        // Store original color, also store the sceneName for the room object for convenience
        public static void AddExtraComponentsToMap(GameMap gameMap)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    //MapModS.Instance.Log(roomObj.name);

                    string sceneName = GetActualSceneName(roomObj.name);

                    if (sceneName == null) continue;

                    ExtraMapData extraData = roomObj.GetComponent<ExtraMapData>();

                    var sr = roomObj.GetComponent<SpriteRenderer>();

                    if (sr == null) continue;

                    if (extraData == null)
                    {
                        extraData = roomObj.gameObject.AddComponent<ExtraMapData>();
                        extraData.origColor = sr.color;
                        extraData.sceneName = sceneName;
                    }
                }
            }
        }

        public static void ResetMapColors(GameMap gameMap)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    ExtraMapData extra = roomObj.GetComponent<ExtraMapData>();
                    var sr = roomObj.GetComponent<SpriteRenderer>();

                    if (sr == null || extra == null) continue;

                    if (roomObj.name.Contains("White_Palace"))
                    {
                        foreach (Transform roomObj2 in roomObj.transform)
                        {
                            if (!roomObj2.name.Contains("RWP")) continue;
                            sr = roomObj2.GetComponent<SpriteRenderer>();
                            break;
                        }
                    }

                    sr.color = extra.origColor;
                }
            }
        }
    }
}