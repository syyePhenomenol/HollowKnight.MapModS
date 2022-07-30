using System;
using System.Collections.Generic;
using System.Linq;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using TMPro;
using UnityEngine;

namespace MapChanger.Map
{
    public static class BuiltInObjects
    {

        private static Dictionary<string, RoomSpriteDef> roomSpriteDefs;
        //private static Dictionary<string, MapLocationDef> roomTextDefs;
        //private static Dictionary<string, MapLocationDef> roomTextDefsAM;
        private static Dictionary<string, MiscObjectDef> miscObjectDefs;
        public static Dictionary<string, RoomSprite> MappedRooms { get; private set; }

        internal static void Load()
        {
            roomSpriteDefs = JsonUtil.Deserialize<Dictionary<string, RoomSpriteDef>>("MapModS.MapChanger.Resources.roomSprites.json");
            //roomTextDefs = JsonUtil.Deserialize<Dictionary<string, MapLocationDef>>("MapModS.MapChanger.Resources.roomTexts.json");
            miscObjectDefs = JsonUtil.Deserialize<Dictionary<string, MiscObjectDef>>("MapModS.MapChanger.Resources.miscObjects.json");

            //if (Dependencies.HasAdditionalMaps())
            //{
            //    roomTextDefsAM = JsonUtil.Deserialize<Dictionary<string, MapLocationDef>>("MapModS.MapChanger.Resources.AdditionalMaps.roomTexts.json");
            //    foreach ((string scene, MapLocationDef roomTextDef) in roomTextDefsAM.Select(kvp => (kvp.Key, kvp.Value)))
            //    {
            //        if (!roomTextDefs.ContainsKey(scene)) continue;
            //        if (roomTextDef is null)
            //        {
            //            MapChangerMod.Instance.LogDebug($"Null: {scene}");
            //            roomTextDefs.Remove(scene);
            //        }
            //        else
            //        {
            //            roomTextDefs[scene] = roomTextDef;
            //        }
            //    }
            //}

            //foreach ((string scene, MapLocationDef roomTextDef) in roomTextDefs.Select(kvp => (kvp.Key, kvp.Value)))
            //{
            //    roomTextDef.Name = scene;
            //    roomTextDef.SceneName = scene;
            //    roomTextDef.MappedScene = Finder.GetMappedScene(scene);
            //}

            Events.AfterSetGameMap += Make;
        }

        private static void Make(GameObject goMap)
        {
            MapChangerMod.Instance.LogDebug("AttachMapModifiers");

            MappedRooms = new();

            foreach (Transform t0 in goMap.transform)
            {
                if (t0.name.Contains("WHITE_PALACE") || t0.name.Contains("GODS_GLORY")) continue;

                foreach (Transform t1 in t0.transform)
                {
                    foreach (Transform t2 in t1.transform)
                    {
                        if (t2.name == "pin_blue_health")
                        {
                            LifebloodPin lifebloodPin = t2.gameObject.AddComponent<LifebloodPin>();
                            lifebloodPin.Initialize();
                        }
                        else if (t2.name == "pin_dream_tree")
                        {
                            WhisperingRootPin whisperingRootPin = t2.gameObject.AddComponent<WhisperingRootPin>();
                            whisperingRootPin.Initialize();
                        }
                    }
                }
            }

            foreach ((string pathName, RoomSpriteDef rsd) in roomSpriteDefs.Select(kvp => (kvp.Key, kvp.Value)))
            {
                Transform child = goMap.transform.FindChildInHierarchy(pathName);
                if (child is null)
                {
                    MapChangerMod.Instance.LogDebug($"Transform not found: {pathName}");
                    continue;
                }

                child.gameObject.SetActive(false);
                RoomSprite roomSprite = child.gameObject.AddComponent<RoomSprite>();
                roomSprite.Initialize(rsd);

                MappedRooms[roomSprite.name] = roomSprite;

                MapChangerMod.Instance.LogDebug($"Attaching: {pathName}");
            }

            //var textPrefab = UnityEngine.Object.Instantiate(goMap.transform.Find("Cliffs").Find("Area Name (1)").gameObject);
            //var tmpPrefab = textPrefab.GetComponent<TextMeshPro>();
            //tmpPrefab.color = Color.white;
            //tmpPrefab.fontSize = 1.8f;
            //tmpPrefab.enableWordWrapping = false;
            //tmpPrefab.margin = new Vector4(1f, 1f, 1f, 1f);
            //tmpPrefab.alignment = TextAlignmentOptions.Center;

            //UnityEngine.Object.Destroy(textPrefab.GetComponent<DisplayOnWorldMapOnly>());
            //UnityEngine.Object.Destroy(textPrefab.GetComponent<SetTextMeshProGameText>());
            //UnityEngine.Object.Destroy(textPrefab.GetComponent<ChangeFontByLanguage>());

            //foreach ((string scene, MapLocationDef mld) in roomTextDefs.Select(kvp => (kvp.Key, kvp.Value)))
            //{
            //    GameObject goRoomText = UnityEngine.Object.Instantiate(textPrefab, RoomTexts.transform);
            //    RoomText roomText = goRoomText.AddComponent<RoomText>();
            //    roomText.Initialize(mld);
            //    roomText.Parent = RoomTexts;
            //    //RoomSelector.Objects[roomText.SceneName] = new() { roomText };
            //    MapChangerMod.Instance.LogDebug($"Creating RoomText: {scene}");

            //    MapRoomLookup[roomText.SceneName] = roomText;
            //}

            //UnityEngine.Object.Destroy(textPrefab);

            // Disable extra map arrow
            goMap.transform.FindChildInHierarchy("Deepnest/Fungus2_25/Next Area (3)")?.gameObject.SetActive(false);

            foreach ((string pathName, MiscObjectDef mod) in miscObjectDefs.Select(kvp => (kvp.Key, kvp.Value)))
            {
                Transform child;
                if (pathName.Contains("*"))
                {
                    child = goMap.transform.FindChildInHierarchy(pathName.Split('*')[0]).GetChild(1);
                }
                else
                {
                    child = goMap.transform.FindChildInHierarchy(pathName);
                }

                if (child is null)
                {
                    MapChangerMod.Instance.LogWarn($"Transform not found: {pathName}");
                    continue;
                }

                child.gameObject.SetActive(false);

                if (mod.Type == MiscObjectType.NextArea)
                {
                    NextArea nextArea = child.gameObject.AddComponent<NextArea>();
                    nextArea.Initialize(mod);
                }
                else if (mod.Type == MiscObjectType.AreaName)
                {
                    AreaName areaName = child.gameObject.AddComponent<AreaName>();
                    areaName.Initialize(mod);
                }

                MapChangerMod.Instance.LogDebug($"Attaching: {pathName}");
            }

            GameObject goQmt = GameCameras.instance.hudCamera.transform.FindChildInHierarchy("Quick Map/Area Name").gameObject;
            goQmt.SetActive(false);
            QuickMapTitle qmt = goQmt.AddComponent<QuickMapTitle>();
            qmt.Initialize();

            MapChangerMod.Instance.LogDebug("~AttachMapModifiers");
        }

        internal static bool TryGetMapRoomPosition(string scene, out float x, out float y)
        {
            (x, y) = (0f, 0f);

            if (scene is null) return false;

            if (MappedRooms.TryGetValue(scene, out RoomSprite roomSprite))
            {
                Vector2 position = roomSprite.transform.parent.localPosition + roomSprite.transform.localPosition;
                (x, y) = (position.x, position.y);
                return true;
            }

            MapChangerMod.Instance.LogDebug($"Map room not recognized: {scene}");
            return false;
        }
    }
}
