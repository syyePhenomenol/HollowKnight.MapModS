using GlobalEnums;
using MapModS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Map
{
    internal class Transition
    {
        public static GameObject CreateExtraMapRooms(GameMap gameMap)
        {
            GameObject go_extraMapRooms = new("MMS Custom Map Rooms");
            go_extraMapRooms.layer = 5;
            go_extraMapRooms.transform.SetParent(gameMap.transform);
            go_extraMapRooms.transform.localPosition = Vector3.zero;
            go_extraMapRooms.SetActive(false);

            var areaNamePrefab = UnityEngine.Object.Instantiate(gameMap.areaCliffs.transform.GetChild(0).gameObject);

            UnityEngine.Object.Destroy(areaNamePrefab.GetComponent<DisplayOnWorldMapOnly>());

            var prefabTMP = areaNamePrefab.GetComponent<TextMeshPro>();
            prefabTMP.color = Color.white;
            prefabTMP.fontSize = 2.5f;
            prefabTMP.enableWordWrapping = false;
            prefabTMP.margin = new Vector4(0f, 0f, 1f, 1f);
            prefabTMP.alignment = TextAlignmentOptions.Center;

            areaNamePrefab.GetComponent<SetTextMeshProGameText>().sheetName = "MMS";

            areaNamePrefab.SetActive(false);

            foreach (string scene in MainData.GetNonMappedScenes())
            {
                MapRoomDef mrd = MainData.GetNonMappedRoomDef(scene);

                GameObject go_extraMapRoom = UnityEngine.Object.Instantiate(areaNamePrefab, go_extraMapRooms.transform);

                go_extraMapRoom.name = scene;
                go_extraMapRoom.GetComponent<SetTextMeshProGameText>().convName = scene;
                go_extraMapRoom.transform.localPosition = new Vector3(mrd.offsetX, mrd.offsetY, 0f);

                ExtraMapData extraData = go_extraMapRoom.gameObject.AddComponent<ExtraMapData>();
                extraData.origColor = Color.white;
                extraData.sceneName = scene;

                go_extraMapRoom.SetActive(false);
            }

            UnityEngine.Object.Destroy(areaNamePrefab);

            return go_extraMapRooms;
        }

        //public readonly static Dictionary<RoomState, Vector4> roomColor = new()
        //{
        //    { RoomState.Normal, new(1f, 1f, 1f, 0.3f) }, // white
        //    { RoomState.Current, new(0, 1f, 0, 0.4f) }, // green
        //    { RoomState.Adjacent, new(0, 1f, 1f, 0.4f) }, // cyan
        //    { RoomState.Out_of_logic, new(1f, 0, 0, 0.3f) }, // red
        //    { RoomState.Selected, new(1f, 1f, 0, 0.7f) }, // yellow
        //    { RoomState.Debug, new(0, 0, 1f, 0.5f) } // blue
        //};

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

        public static HashSet<string> SetupMapTransitionMode(GameMap gameMap, MapZone mapZone)
        {
            bool isAlt = MapModS.LS.mapMode == Settings.MapMode.TransitionRandoAlt;

            HashSet<string> inLogicScenes = new();

            HashSet<string> outOfLogicScenes = new();

            HashSet<string> visitedAdjacentScenes = new();

            HashSet<string> uncheckedReachableScenes = new();

            HashSet<string> visitedScenes = new(PlayerData.instance.scenesVisited);
            visitedScenes.Add(Utils.CurrentScene());

            RandomizerCore.Logic.ProgressionManager pm = RM.RS.TrackerData.pm;

            // Get in-logic, out-of-logic, and adjacent visited scenes
            foreach (KeyValuePair<string, RandomizerCore.Logic.LogicTransition> t in RM.RS.TrackerData.lm.TransitionLookup)
            {
                string scene = TransitionData.GetTransitionScene(t.Key);

                if (pm.Has(t.Value.term.Id))
                {
                    inLogicScenes.Add(scene);

                    if (scene == Utils.CurrentScene())
                    {
                        // visitedTransitions doesn't include vanilla transitions
                        if ((RM.RS.TrackerData.visitedTransitions.ContainsKey(t.Key) || !TransitionData.IsRandomizedTransition(t.Key))
                            && TransitionData.GetAdjacentTransition(t.Key) != null)
                        {
                            visitedAdjacentScenes.Add(TransitionData.GetAdjacentScene(t.Key));
                        }
                    }
                }
                else if (PlayerData.instance.scenesVisited.Contains(scene))
                {
                    outOfLogicScenes.Add(scene);
                }
            }

            // Get scenes where there are unchecked reachable transitions
            foreach (string transition in RM.RS.TrackerData.uncheckedReachableTransitions)
            {
                uncheckedReachableScenes.Add(TransitionData.GetTransitionScene(transition));
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
                        || areaObj.name == "Map Markers"
                        || areaObj.name == "MMS Custom Pin Group") continue;

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
                    Vector4 color = Colors.GetColor(ColorSetting.Room_Normal);

                    if (isAlt)
                    {
                        if (visitedScenes.Contains(emd.sceneName))
                        {
                            color = Colors.GetColor(ColorSetting.Room_Normal);
                            active = true;
                        }

                        if (outOfLogicScenes.Contains(emd.sceneName)
                            && !inLogicScenes.Contains(emd.sceneName))
                        {
                            color = Colors.GetColor(ColorSetting.Room_Out_of_logic);
                        }
                    }
                    else
                    {
                        if (outOfLogicScenes.Contains(emd.sceneName))
                        {
                            color = Colors.GetColor(ColorSetting.Room_Out_of_logic);
                            active = true;
                        }

                        if (inLogicScenes.Contains(emd.sceneName))
                        {
                            color = Colors.GetColor(ColorSetting.Room_Normal);
                            active = true;
                        }
                    }

                    if (visitedAdjacentScenes.Contains(emd.sceneName))
                    {
                        color = Colors.GetColor(ColorSetting.Room_Adjacent);
                    }

#if DEBUG
                    if (!active)
                    {
                        color = roomColor[RoomState.Debug];
                        active = true;
                    }
#endif

                    if (emd.sceneName == Utils.CurrentScene())
                    {
                        color = Colors.GetColor(ColorSetting.Room_Current);
                    }

                    if (uncheckedReachableScenes.Contains(emd.sceneName))
                    {
                        emd.highlight = true;
                        color.w = 1f;
                    }
                    else
                    {
                        emd.highlight = false;
                    }

                    if (areaObj.name == "MMS Custom Map Rooms")
                    {
                        if (mapZone == MapZone.NONE || MainData.GetNonMappedRoomDef(roomObj.name).mapZone == mapZone)
                        {
                            TextMeshPro tmp = roomObj.gameObject.GetComponent<TextMeshPro>();
                            SetActiveTMPColor(roomObj, active, tmp, color);
                            emd.origTransitionColor = tmp.color;
                        }
                        else
                        {
                            roomObj.gameObject.SetActive(false);
                        }

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

            if (isAlt)
            {
                return visitedScenes;
            }

            return new(inLogicScenes.Union(outOfLogicScenes));
        }

        public class ExtraMapData : MonoBehaviour
        {
            public Color origColor;
            public Color origTransitionColor;
            public string sceneName;
            public bool highlight;
        }

        // Store original color, also store the sceneName for the room object for convenience
        public static void AddExtraComponentsToMap(GameMap gameMap)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    //MapModS.Instance.Log(roomObj.name);

                    string sceneName = Utils.GetActualSceneName(roomObj.name);

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

        public static void SetSelectedRoomColor(string selectedScene, bool transitionMode)
        {
            GameObject go_GameMap = GameManager.instance.gameMap;

            if (go_GameMap == null) return;

            foreach (Transform areaObj in go_GameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (!roomObj.gameObject.activeSelf) continue;

                    ExtraMapData extra = roomObj.GetComponent<ExtraMapData>();

                    if (extra == null) continue;

                    if (areaObj.name == "MMS Custom Map Rooms")
                    {
                        TextMeshPro tmp = roomObj.gameObject.GetComponent<TextMeshPro>();

                        if (extra.sceneName == selectedScene)
                        {
                            Vector4 color = Colors.GetColor(ColorSetting.Room_Selected);

                            if (extra.highlight)
                            {
                                color.w = 1f;
                            }

                            tmp.color = color;
                        }
                        else
                        {
                            tmp.color = extra.origTransitionColor;
                        }

                        continue;
                    }

                    SpriteRenderer sr = roomObj.GetComponent<SpriteRenderer>();

                    // For AdditionalMaps room objects, the child has the SR
                    if (extra.sceneName.Contains("White_Palace"))
                    {
                        foreach (Transform roomObj2 in roomObj.transform)
                        {
                            if (!roomObj2.name.Contains("RWP")) continue;
                            sr = roomObj2.GetComponent<SpriteRenderer>();
                            break;
                        }
                    }

                    if (sr == null) continue;


                    if (transitionMode)
                    {
                        if (extra.sceneName == selectedScene)
                        {
                            Vector4 color = Colors.GetColor(ColorSetting.Room_Selected);

                            if (extra.highlight)
                            {
                                color.w = 1f;
                            }

                            sr.color = color;
                        }
                        else
                        {
                            sr.color = extra.origTransitionColor;
                        }
                    }
                    else
                    {
                        if (extra.sceneName == selectedScene)
                        {
                            Vector4 color = Colors.GetColor(ColorSetting.Room_Benchwarp_Selected);

                            sr.color = color;
                        }
                        else
                        {
                            sr.color = extra.origColor;
                        }
                    }
                }
            }
        }

        public static void ResetMapColors(GameObject goGameMap)
        {
            GameMap gameMap = goGameMap.GetComponent<GameMap>();

            if (gameMap == null) return;

            foreach (Transform areaObj in gameMap.transform)
            {
                if (areaObj.name == "Grub Pins"
                        || areaObj.name == "Dream_Gate_Pin"
                        || areaObj.name == "Compass Icon"
                        || areaObj.name == "Shade Pos"
                        || areaObj.name == "Flame Pins"
                        || areaObj.name == "Dreamer Pins"
                        || areaObj.name == "Map Markers"
                        || areaObj.name == "MMS Custom Pin Group") continue;

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