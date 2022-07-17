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
        private static Dictionary<string, MapLocationDef> roomTextDefs;
        private static Dictionary<string, MapLocationDef> roomTextDefsAM;
        private static Dictionary<string, MiscObjectDef> miscObjectDefs;

        public static MapObjectGroup LifebloodRootPins { get; private set; }
        public static MapObjectGroup RoomSprites { get; private set; }
        public static MapObjectGroup RoomTexts { get; private set; }
        public static MapObjectGroup AreaNames { get; private set; }
        public static MapObjectGroup NextAreas { get; private set; }

        public static Selector RoomSelector { get; private set; }

        internal static void Load()
        {
            roomSpriteDefs = JsonUtil.Deserialize<Dictionary<string, RoomSpriteDef>>("MapModS.MapChanger.Resources.roomSprites.json");
            roomTextDefs = JsonUtil.Deserialize<Dictionary<string, MapLocationDef>>("MapModS.MapChanger.Resources.roomTexts.json");
            miscObjectDefs = JsonUtil.Deserialize<Dictionary<string, MiscObjectDef>>("MapModS.MapChanger.Resources.miscObjects.json");

            if (Dependencies.HasAdditionalMaps())
            {
                roomTextDefsAM = JsonUtil.Deserialize<Dictionary<string, MapLocationDef>>("MapModS.MapChanger.Resources.AdditionalMaps.roomTexts.json");
                foreach ((string scene, MapLocationDef roomTextDef) in roomTextDefsAM.Select(kvp => (kvp.Key, kvp.Value)))
                {
                    if (!roomTextDefs.ContainsKey(scene)) continue;
                    if (roomTextDef is null)
                    {
                        MapChangerMod.Instance.LogDebug($"Null: {scene}");
                        roomTextDefs.Remove(scene);
                    }
                    else
                    {
                        roomTextDefs[scene] = roomTextDef;
                    }
                }
            }

            foreach ((string scene, MapLocationDef roomTextDef) in roomTextDefs.Select(kvp => (kvp.Key, kvp.Value)))
            {
                roomTextDef.Name = scene;
                roomTextDef.SceneName = scene;
                roomTextDef.MappedScene = Finder.GetMappedScene(scene);
            }

            Events.AfterSetGameMap += Add;
        }

        private static void Add(GameObject goMap)
        {
            MapChangerMod.Instance.LogDebug("~AttachMapModifiers");

            LifebloodRootPins = Utils.MakeMonoBehaviour<MapObjectGroup>(goMap, nameof(LifebloodRootPins));
            RoomSprites = Utils.MakeMonoBehaviour<MapObjectGroup>(goMap, nameof(RoomSprites));
            RoomTexts = Utils.MakeMonoBehaviour<MapObjectGroup>(goMap, nameof(RoomTexts));
            AreaNames = Utils.MakeMonoBehaviour<MapObjectGroup>(goMap, nameof(AreaNames));
            NextAreas = Utils.MakeMonoBehaviour<MapObjectGroup>(goMap, nameof(NextAreas));

            //RoomSelector = Utils.MakeMonoBehaviour<Selector>(null, nameof(RoomSelector));

            foreach (Transform t0 in goMap.transform)
            {
                if (t0.name.Contains("WHITE_PALACE") || t0.name.Contains("GODS_GLORY")) continue;

                foreach (Transform t1 in t0.transform)
                {
                    foreach (Transform t2 in t1.transform)
                    {
                        if (t2.name == "pin_blue_health")
                        {
                            LifebloodRootPins.MapObjects.Add(t2.gameObject.AddComponent<LifebloodPin>());
                        }
                        else if (t2.name == "pin_dream_tree")
                        {
                            LifebloodRootPins.MapObjects.Add(t2.gameObject.AddComponent<WhisperingRootPin>());
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

                RoomSprite roomSprite = child.gameObject.AddComponent<RoomSprite>();
                roomSprite.Initialize(rsd);
                RoomSprites.MapObjects.Add(roomSprite);

                //if (RoomSelector.Objects.ContainsKey(roomSprite.SceneName))
                //{
                //    RoomSelector.Objects[roomSprite.SceneName].Add(roomSprite);
                //}
                //else
                //{
                //    RoomSelector.Objects[roomSprite.SceneName] = new() { roomSprite }; 
                //}

                MapChangerMod.Instance.LogDebug($"Attaching: {pathName}");
            }

            Finder.SetMappedScenePositions(RoomSprites.MapObjects);

            var textPrefab = UnityEngine.Object.Instantiate(goMap.transform.Find("Cliffs").Find("Area Name (1)").gameObject);
            var tmpPrefab = textPrefab.GetComponent<TextMeshPro>();
            tmpPrefab.color = Color.white;
            tmpPrefab.fontSize = 1.8f;
            tmpPrefab.enableWordWrapping = false;
            tmpPrefab.margin = new Vector4(1f, 1f, 1f, 1f);
            tmpPrefab.alignment = TextAlignmentOptions.Center;

            UnityEngine.Object.Destroy(textPrefab.GetComponent<DisplayOnWorldMapOnly>());
            UnityEngine.Object.Destroy(textPrefab.GetComponent<SetTextMeshProGameText>());
            UnityEngine.Object.Destroy(textPrefab.GetComponent<ChangeFontByLanguage>());

            foreach ((string scene, MapLocationDef mld) in roomTextDefs.Select(kvp => (kvp.Key, kvp.Value)))
            {
                GameObject goRoomText = UnityEngine.Object.Instantiate(textPrefab, RoomTexts.transform);
                RoomText roomText = goRoomText.AddComponent<RoomText>();
                roomText.Initialize(mld);
                RoomTexts.MapObjects.Add(roomText);
                //RoomSelector.Objects[roomText.SceneName] = new() { roomText };
                MapChangerMod.Instance.LogDebug($"Creating RoomText: {scene}");
            }

            UnityEngine.Object.Destroy(textPrefab);

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
                    MapChangerMod.Instance.LogDebug($"Transform not found: {pathName}");
                    continue;
                }

                if (mod.Type == MiscObjectType.NextArea)
                {
                    NextArea nextArea = child.gameObject.AddComponent<NextArea>();
                    nextArea.Initialize(mod);
                    NextAreas.MapObjects.Add(nextArea);
                }
                else if (mod.Type == MiscObjectType.AreaName)
                {
                    AreaName areaName = child.gameObject.AddComponent<AreaName>();
                    areaName.Initialize(mod);
                    AreaNames.MapObjects.Add(areaName);
                }
                MapChangerMod.Instance.LogDebug($"Attaching: {pathName}");
            }

            MapChangerMod.Instance.LogDebug("~AttachMapModifiers");
        }


        public static void ImportDefs()
        {
            //try
            //{
            //    Dictionary<string, MapLocationDef> mlds = JsonUtil.DeserializeFromExternalFile<Dictionary<string, MapLocationDef>>("locations.json");

            //    foreach (TestPin tp in TempPins.MapObjects)
            //    {
            //        tp.SetPosition(mlds[tp.Location.Name]);
            //    }
            //}
            //catch (Exception e)
            //{
            //    MapChangerMod.Instance.LogError(e);
            //}

            //try
            //{
            //    Dictionary<string, MapLocationDef> mlds = JsonUtil.DeserializeFromExternalFile<Dictionary<string, MapLocationDef>>("locationsAM.json");

            //    foreach (VanillaPin tp in TempPins.MapObjects)
            //    {
            //        if (mlds.ContainsKey(tp.Location.Name))
            //        {
            //            tp.SetPosition(mlds[tp.Location.Name]);
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    MapChangerMod.Instance.LogError(e);
            //}

            //JsonUtil.Serialize(roomTextDefs, "roomTexts.json");
            //JsonUtil.Serialize(roomTextDefsAM, "roomTextsAM.json");
        }

        public static void ExportDefs()
        {
            try
            {
                JsonUtil.Serialize(Finder.GetAllLocations(), "locations.json");
            }
            catch (Exception e)
            {
                MapChangerMod.Instance.LogError(e);
            }
        }
    }
}
