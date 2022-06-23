using GlobalEnums;
using MapModS.Data;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Map
{
    public class ExtraMapData : MonoBehaviour
    {
        public Vector4 origColor;
        public Vector4 origCustomColor = Vector4.negativeInfinity;
        public Vector4 origTransitionColor;
        public string sceneName;
        public bool highlight;
    }

    internal class MapRooms
    {
        // Store metadata
        public static void AddExtraComponentsToMap(GameMap gameMap)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                if (!Colors.mapColors.ContainsKey(areaObj.name)) continue;

                foreach (Transform roomObj in areaObj.transform)
                {
                    TryAddExtraMapData(roomObj);

                    foreach (Transform roomObj2 in roomObj.transform)
                    {
                        TryAddExtraMapData(roomObj2);
                    }
                }
            }
        }

        private static void TryAddExtraMapData(Transform obj)
        {
            string sceneName = Utils.GetActualSceneName(obj.name);

            if (obj.GetComponent<ExtraMapData>() != null
                || obj.GetComponent<GrubPin>() != null
                || obj.name == "Fungus3_48"
                || (obj.parent.name != "WHITE_PALACE"
                    && obj.parent.name != "GODS_GLORY"
                    && MainData.IsNonMappedScene(obj.name))) return;

            ExtraMapData emd;

            if (sceneName != null)
            {
                emd = obj.gameObject.AddComponent<ExtraMapData>();
                emd.sceneName = sceneName;
            }
            else if (obj.name.Contains("Area Name"))
            {
                emd = obj.gameObject.AddComponent<ExtraMapData>();
                emd.sceneName = obj.parent.name + " Area Name";
            }
            else
            {
                return;
            }

            if (obj.GetComponent<SpriteRenderer>() != null)
            {
                emd.origColor = obj.GetComponent<SpriteRenderer>().color;
            }
            else if (obj.GetComponent<TextMeshPro>() != null)
            {
                emd.origColor = obj.GetComponent<TextMeshPro>().color;
            }
            else
            {
                Object.Destroy(emd);
                return;
            }

            if (obj.parent.name == "Ancient Basin"
                && (obj.name == "Area Name (2)" || obj.name == "Abyss_22"))
            {
                emd.origCustomColor = Colors.GetColor(ColorSetting.Map_Ancient_Basin);
            }
            else
            {
                (string, Vector4) key = (obj.parent.name, emd.origColor);

                if (Colors.subAreaMapColorKeys.ContainsKey(obj.name) && !Colors.subAreaMapColors.ContainsKey(key))
                {
                    Colors.subAreaMapColors.Add(key, Colors.subAreaMapColorKeys[obj.name]);
                }

                if (Colors.subAreaMapColors.ContainsKey(key))
                {
                    emd.origCustomColor = Colors.GetColor(Colors.subAreaMapColors[key]);
                }
            }

            if (obj.name.Contains("Area Name") && !emd.origCustomColor.Equals(Vector4.negativeInfinity))
            {
                emd.origCustomColor.w = 1f;
            }
        }

        public static GameObject CreateExtraMapRooms(GameMap gameMap)
        {
            GameObject go_extraMapRooms = new("MMS Custom Map Rooms");
            go_extraMapRooms.layer = 5;
            go_extraMapRooms.transform.SetParent(gameMap.transform);
            go_extraMapRooms.transform.localPosition = Vector3.zero;
            go_extraMapRooms.SetActive(false);

            var areaNamePrefab = Object.Instantiate(gameMap.areaCliffs.transform.GetChild(0).gameObject);

            Object.Destroy(areaNamePrefab.GetComponent<DisplayOnWorldMapOnly>());

            var prefabTMP = areaNamePrefab.GetComponent<TextMeshPro>();
            prefabTMP.color = Color.white;
            prefabTMP.fontSize = 2.5f;
            prefabTMP.enableWordWrapping = false;
            prefabTMP.margin = new Vector4(0f, 0f, 1f, 1f);
            prefabTMP.alignment = TextAlignmentOptions.Center;

            areaNamePrefab.GetComponent<SetTextMeshProGameText>().sheetName = "MMS";

            areaNamePrefab.SetActive(false);

            foreach (string scene in MainData.GetNonMappedScenes().Where(s => RandomizerMod.RandomizerData.Data.IsRoom(s) || s.IsSpecialRoom()))
            {
                MapRoomDef mrd = MainData.GetNonMappedRoomDef(scene);

                GameObject go_extraMapRoom = Object.Instantiate(areaNamePrefab, go_extraMapRooms.transform);
                go_extraMapRoom.name = scene;
                go_extraMapRoom.GetComponent<SetTextMeshProGameText>().convName = scene;
                
                if (scene == "GG_Workshop" && Dependencies.HasAdditionalMaps())
                {
                    go_extraMapRoom.transform.localPosition = new Vector3(9.9f, 14.6f, 0f);
                }
                else
                {
                    go_extraMapRoom.transform.localPosition = new Vector3(mrd.offsetX, mrd.offsetY, 0f);
                }

                ExtraMapData extraData = go_extraMapRoom.GetComponent<ExtraMapData>();
                if (extraData == null)
                {
                    extraData = go_extraMapRoom.AddComponent<ExtraMapData>();
                }
                extraData.sceneName = scene;

                go_extraMapRoom.SetActive(false);
            }

            Object.Destroy(areaNamePrefab);

            return go_extraMapRooms;
        }

#if DEBUG
        // For debugging rooms
        public static void ReadjustRoomPostiions()
        {
            if (MainData.newRooms == null) return;

            foreach (Transform room in WorldMap.goExtraRooms.transform)
            {
                if (MainData.newRooms.TryGetValue(room.name, out MapRoomDef mrd))
                {
                    Vector3 newPos = new(mrd.offsetX, mrd.offsetY, 0f);
                    room.localPosition = newPos;
                }
            }
        }
#endif

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
                string scene = TransitionData.GetScene(t.Key);

                if (pm.Has(t.Value.term.Id))
                {
                    inLogicScenes.Add(scene);
                }
                else if (PlayerData.instance.scenesVisited.Contains(scene))
                {
                    outOfLogicScenes.Add(scene);
                }
            }

            visitedAdjacentScenes = Pathfinder.GetAdjacentReachableScenes(Utils.CurrentScene());

            // Manuallly add Godhome/Tram scenes
            if (pm.lm.TermLookup.ContainsKey("Warp-Godhome_to_Junk_Pit") && pm.Get("Warp-Godhome_to_Junk_Pit") > 0)
            {
                inLogicScenes.Add("GG_Atrium");
            }

            if (pm.lm.TermLookup.ContainsKey("Warp-Junk_Pit_to_Godhome") && pm.Get("Warp-Junk_Pit_to_Godhome") > 0)
            {
                inLogicScenes.Add("GG_Waterways");
            }

            if (pm.lm.TermLookup.ContainsKey("GG_Workshop") && pm.Get("GG_Workshop") > 0)
            {
                inLogicScenes.Add("GG_Workshop");
            }

            if (pm.Get("Upper_Tram") > 0)
            {
                inLogicScenes.Add("Room_Tram_RG");
            }

            if (pm.Get("Lower_Tram") > 0)
            {
                inLogicScenes.Add("Room_Tram");
            }

            // Get scenes where there are unchecked reachable transitions
            foreach (string transition in RM.RS.TrackerData.uncheckedReachableTransitions)
            {
                uncheckedReachableScenes.Add(TransitionData.GetScene(transition));
            }

            // Show rooms with custom colors
            foreach (Transform areaObj in gameMap.transform)
            {
                if (areaObj.name == "MMS Custom Map Rooms")
                {
                    areaObj.gameObject.SetActive(true);
                }

                foreach (Transform roomObj in areaObj.transform)
                {
                    ExtraMapData emd = roomObj.GetComponent<ExtraMapData>();

                    if (emd == null || roomObj.name.Contains("Area Name"))
                    {
                        roomObj.gameObject.SetActive(false);
                        continue;
                    }

                    bool active = false;
                    Vector4 color = Colors.GetColor(ColorSetting.Room_Normal);

                    if (isAlt)
                    {
                        if (visitedScenes.Contains(emd.sceneName))
                        {
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
                        color = Colors.GetColor(ColorSetting.Room_Debug);
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
                        SetActiveColor(roomObj.gameObject, active && (mapZone == MapZone.NONE || MainData.GetNonMappedRoomDef(roomObj.name).mapZone == mapZone), color);

                        if (!active) continue;
                        emd.origTransitionColor = color;

                        continue;
                    }

                    Transform obj = GetActualRoomObj(roomObj);
                    if (obj == null) continue;

                    SetActiveColor(roomObj.gameObject, active, color);

                    if (!active) continue;
                    emd.origTransitionColor = color;

                    // Force disable sub area names
                    foreach (Transform roomObj2 in roomObj.transform.Cast<Transform>()
                        .Where(r => r.name.Contains("Area Name")))
                    {
                        roomObj2.gameObject.SetActive(false);
                    }
                }
            }

            if (isAlt)
            {
                return visitedScenes;
            }

            return new(inLogicScenes.Union(outOfLogicScenes));
        }

        private static void SetActiveColor(GameObject obj, bool active, Vector4 color)
        {
            if (obj.GetComponent<SpriteRenderer>() != null)
            {
                obj.SetActive(active);
                obj.GetComponent<SpriteRenderer>().color = color;
            }
            else if (obj.GetComponent<TextMeshPro>() != null)
            {
                obj.SetActive(active);
                obj.GetComponent<TextMeshPro>().color = color;
            }
            else
            {
                obj.gameObject.SetActive(false);
            }
        }

        public static void SetSelectedRoomColor(string selectedScene, bool transitionMode)
        {
            GameObject go_GameMap = GameManager.instance.gameMap;
            if (go_GameMap == null) return;

            foreach (Transform areaObj in go_GameMap.transform)
            {
                if (areaObj.name == "MMS Custom Map Rooms" && transitionMode)
                {
                    foreach (Transform roomObj in areaObj.transform)
                    {
                        TextMeshPro tmp = roomObj.GetComponent<TextMeshPro>();
                        ExtraMapData emd = roomObj.GetComponent<ExtraMapData>();
                        if (tmp == null || emd == null) continue;

                        if (emd.sceneName == selectedScene)
                        {
                            Vector4 color = Colors.GetColor(ColorSetting.Room_Selected);
                            if (emd.highlight)
                            {
                                color.w = 1f;
                            }
                            tmp.color = color;
                        }
                        else
                        {
                            tmp.color = emd.origTransitionColor;
                        }
                    }
                    continue;
                }

                if (!Colors.mapColors.ContainsKey(areaObj.name)) continue;

                Vector4 customMapColor = Colors.GetColor(Colors.mapColors[areaObj.name]);

                foreach (Transform roomObj in areaObj.transform)
                {
                    if (!roomObj.gameObject.activeSelf) continue;

                    ExtraMapData emd = roomObj.GetComponent<ExtraMapData>();
                    if (emd == null) continue;

                    Transform obj = GetActualRoomObj(roomObj);
                    if (obj == null) continue;

                    var sr = obj.GetComponent<SpriteRenderer>();
                    if (sr == null) continue;

                    if (transitionMode)
                    {
                        if (emd.sceneName == selectedScene)
                        {
                            Vector4 color = Colors.GetColor(ColorSetting.Room_Selected);
                            if (emd.highlight)
                            {
                                color.w = 1f;
                            }
                            sr.color = color;
                        }
                        else
                        {
                            sr.color = emd.origTransitionColor;
                        }
                    }
                    else
                    {
                        if (emd.sceneName == selectedScene)
                        {
                            Vector4 color = Colors.GetColor(ColorSetting.Room_Benchwarp_Selected);
                            sr.color = color;
                        }
                        else
                        {
                            Vector4 color = customMapColor;

                            if (!emd.origCustomColor.Equals(Vector4.negativeInfinity))
                            {
                                color = emd.origCustomColor;
                            }

                            if (!color.Equals(Vector4.negativeInfinity) && MapModS.LS.modEnabled)
                            {
                                sr.color = color;
                            }
                            else
                            {
                                sr.color = emd.origColor;
                            }
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
                if (!Colors.mapColors.ContainsKey(areaObj.name)) continue;

                Vector4 customMapColor = Colors.GetColor(Colors.mapColors[areaObj.name]);

                foreach (Transform roomObj in areaObj.transform)
                {
                    ExtraMapData emd = roomObj.GetComponent<ExtraMapData>();
                    if (emd == null) continue;

                    Transform obj = GetActualRoomObj(roomObj);
                    TrySetColor(obj.gameObject, emd, customMapColor);

                    foreach (Transform roomObj2 in roomObj.transform.Cast<Transform>().Where(t => t.name.Contains("Area Name")))
                    {
                        emd = roomObj.GetComponent<ExtraMapData>();
                        if (emd == null) continue;
                        TrySetColor(roomObj2.gameObject, emd, customMapColor);
                    }
                }
            }
        }

        private static void TrySetColor(GameObject obj, ExtraMapData emd, Vector4 customMapColor)
        {
            var sr = obj.GetComponent<SpriteRenderer>();
            var tmp = obj.GetComponent<TextMeshPro>();

            Vector4 color = customMapColor;

            if (!emd.origCustomColor.Equals(Vector4.negativeInfinity))
            {
                color = emd.origCustomColor;
            }

            if (sr != null)
            {
                if (!color.Equals(Vector4.negativeInfinity) && MapModS.LS.modEnabled)
                {
                    sr.color = color;
                }
                else
                {
                    sr.color = emd.origColor;
                }
            }
            else if (tmp != null)
            {
                if (!color.Equals(Vector4.negativeInfinity) && MapModS.LS.modEnabled)
                {
                    color.w = 1f;
                    tmp.color = color;
                }
                else
                {
                    tmp.color = emd.origColor;
                }
            }
        }

        private static Transform GetActualRoomObj(Transform t)
        {
            if (t.name.Contains("White_Palace"))
            {
                return t.Cast<Transform>()
                    .Where(r => r.name.Contains("RWP")).FirstOrDefault();
            }
            // Hotfix
            else if (t.name.Contains("GG_Atrium"))
            {
                return t.Cast<Transform>()
                    .Where(r => r.name.Contains("RGH")).FirstOrDefault();
            }
            else
            {
                return t;
            }
        }

        public static bool GetRoomClosestToMiddle(string previousScene, out string selectedScene)
        {
            selectedScene = null;
            double minDistance = double.PositiveInfinity;

            GameObject go_GameMap = GameManager.instance.gameMap;

            if (go_GameMap == null) return false;

            foreach (Transform areaObj in go_GameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (!roomObj.gameObject.activeSelf) continue;

                    ExtraMapData extra = roomObj.GetComponent<ExtraMapData>();

                    if (extra == null) continue;

                    double distance = Utils.DistanceToMiddle(roomObj);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        selectedScene = extra.sceneName;
                    }
                }
            }

            return selectedScene != previousScene;
        }
    }
}