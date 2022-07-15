using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using UnityEngine;

namespace MapChanger
{
    public static class Finder
    {
        private static Dictionary<string, Vector2> mappedScenePositions;
        private static Dictionary<string, MappedSceneDef> mappedScenes;
        private static Dictionary<string, MapLocationDef> locations;
        private static HashSet<string> minimalMapScenes;
        private static readonly Dictionary<string, MapLocationDef> injectedLocations = new();

        internal static void Load()
        {
            mappedScenes = JsonUtil.Deserialize<Dictionary<string, MappedSceneDef>>("MapModS.MapChanger.Resources.mappedScenes.json");
            locations = JsonUtil.Deserialize<Dictionary<string, MapLocationDef>>("MapModS.MapChanger.Resources.locations.json");
            minimalMapScenes = JsonUtil.Deserialize<HashSet<string>>("MapModS.MapChanger.Resources.minimalMap.json");

            //ScaleOffsets(locations, "locations.json");

            if (Dependencies.HasAdditionalMaps())
            {
                Dictionary<string, MappedSceneDef> mappedSceneLookupAM = JsonUtil.Deserialize<Dictionary<string, MappedSceneDef>>("MapModS.MapChanger.Resources.AdditionalMaps.mappedScenes.json");
                foreach ((string scene, MappedSceneDef msd) in mappedSceneLookupAM.Select(kvp => (kvp.Key, kvp.Value)))
                {
                    mappedScenes[scene] = msd;
                }

                Dictionary<string, MapLocationDef> locationLookupAM = JsonUtil.Deserialize<Dictionary<string, MapLocationDef>>("MapModS.MapChanger.Resources.AdditionalMaps.locations.json");
                foreach ((string name, MapLocationDef mpd) in locationLookupAM.Select(kvp => (kvp.Key, kvp.Value)))
                {
                    locations[name] = mpd;
                }
                //ScaleOffsets(locationLookupAM, "locationsAM.json");
            }

            foreach ((string name, MapLocationDef mpd) in locations.Select(kvp => (kvp.Key, kvp.Value)))
            {
                CompleteLocationDef(name, mpd);
            }
        }

        //public static void GetAllMapObjectData(GameObject goMap)
        //{
        //    MapChangerMod.Instance.LogDebug("GetAllMapObjectData");

        //    Dictionary<string, BuiltInObjectDefOld> defaultMapObjectDefs = new();

        //    try
        //    {
        //        defaultMapObjectDefs = JsonUtil.Deserialize<Dictionary<string, BuiltInObjectDefOld>>("MapModS.MapChanger.Resources.defaultMapObjects.json");
        //    }
        //    catch (Exception e)
        //    {
        //        MapChangerMod.Instance.LogError(e);
        //    }

        //    Dictionary<string, BuiltInObjectDefOld> defaultMapObjects = new();

        //    MapChangerMod.Instance.LogDebug("Entering for loop");

        //    try
        //    {
        //        for (int index0 = 0; index0 < goMap.transform.childCount; index0++)
        //        {
        //            Transform t0 = goMap.transform.GetChild(index0);
        //            TryCreateMapObjectData(t0.name, new int[] { index0 }, t0);

        //            for (int index1 = 0; index1 < t0.childCount; index1++)
        //            {
        //                Transform t1 = t0.GetChild(index1);
        //                TryCreateMapObjectData(t0.name + "/" + t1.name, new int[] { index0, index1 }, t1);

        //                for (int index2 = 0; index2 < t1.childCount; index2++)
        //                {
        //                    Transform t2 = t1.GetChild(index2);
        //                    TryCreateMapObjectData(t0.name + "/" + t1.name + "/" + t2.name, new int[] { index0, index1, index2 }, t2);

        //                    for (int index3 = 0; index3 < t2.childCount; index3++)
        //                    {
        //                        Transform t3 = t2.GetChild(index3);
        //                        TryCreateMapObjectData(t0.name + "/" + t1.name + "/" + t2.name + "/" + t3.name, new int[] { index0, index1, index2, index3 }, t3);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MapChangerMod.Instance.LogError(e);
        //    }

        //    try
        //    {
        //        JsonUtil.Serialize(defaultMapObjects, "defaultMapObjects.json");
        //    }
        //    catch (Exception e)
        //    {
        //        MapChangerMod.Instance.LogError(e);
        //    }

        //    MapChangerMod.Instance.LogDebug("~GetAllMapObjectData");

        //    void TryCreateMapObjectData(string name, int[] indices, Transform transform)
        //    {
        //        if (transform.GetComponent<SpriteRenderer>() is null && transform.GetComponent<TextMeshPro>() is null) return;

        //        if (defaultMapObjectDefs.TryGetValue(name, out BuiltInObjectDefOld dmod))
        //        {
        //            dmod.Indices = indices;
        //        }
        //        else
        //        {
        //            dmod = new BuiltInObjectDefOld() { Indices = indices };
        //        }

        //        if (defaultMapObjects.ContainsKey(name))
        //        {
        //            defaultMapObjects[name + " Dupe"] = dmod;
        //        }
        //        else
        //        {
        //            defaultMapObjects[name] = dmod;
        //        }
        //    }

            //void TryCreateMapObjectData(string name, Transform transform)
            //{
            //    if (transform.gameObject.GetComponent<SpriteRenderer>() is SpriteRenderer sr)
            //    {
            //        CreateMapObjectData(name, sr.color, false);
            //    }
            //    if (transform.gameObject.GetComponent<TextMeshPro>() is TextMeshPro tmp)
            //    {
            //        CreateMapObjectData(name, tmp.color, true);
            //    }
            //}

            //void CreateMapObjectData(string name, Vector4 color, bool isText)
            //{
            //    MapObjectData mapObjectData = new();
            //    mapObjectData.isText = isText;

            //    if (!colorIndices.TryGetValue(color, out int index))
            //    {
            //        index = colorIndices.Count();
            //        colorIndices[color] = index;
            //    }
            //    mapObjectData.colorIndex = index;

            //    mapObjectData.Color = new float[] { color.x, color.y, color.z, color.w };

            //    defaultMapObjects[name] = mapObjectData;
            //}
        //}
        
        internal static void SetMappedScenePositions(List<MapObject> roomSprites)
        {
            mappedScenePositions = roomSprites.Where(rs => IsScene(rs.transform.name))
                .ToDictionary(rs => rs.transform.name, rs => (Vector2)(rs.transform.localPosition + rs.transform.parent.transform.localPosition));
        }

        internal static bool TryGetMappedScenePosition(string scene, out Vector2 position)
        {
            position = Vector2.zero;

            if (scene is null) return false;

            if (mappedScenePositions.ContainsKey(scene))
            {
                position = mappedScenePositions[scene];
                return true;
            }
            return false;
        }

        public static void InjectLocations(Dictionary<string, MapLocationDef> locations)
        {
            foreach ((string name, MapLocationDef mpd) in locations.Select(kvp => (kvp.Key, kvp.Value)))
            {
                CompleteLocationDef(name, mpd);
                injectedLocations[name] = mpd;
            }
        }

        public static MapLocationDef GetLocation(string name)
        {
            if (name is null) return default;
            if (injectedLocations.TryGetValue(name, out MapLocationDef mpd))
            {
                return mpd;
            }
            if (locations.TryGetValue(name, out mpd))
            {
                return mpd;
            }
            //TODO: Best guess here
            return default;
        }

        public static Dictionary<string, MapLocationDef> GetAllLocations()
        {
            Dictionary<string, MapLocationDef> newLocations = new(locations);
            foreach ((string name, MapLocationDef mpd) in injectedLocations.Select(kvp => (kvp.Key, kvp.Value)))
            {
                newLocations[name] = mpd;
            }

            return newLocations;
        }

        internal static bool IsMinimalMapScene(string scene)
        {
            return minimalMapScenes.Contains(scene);
        }

        private static void CompleteLocationDef(string name, MapLocationDef mpd)
        {
            if (mpd.Name is null)
            {
                mpd.Name = name;
            }
            if (mpd.SceneName is null && Dependencies.HasItemChanger())
            {
                mpd.SceneName = IC.ICInterop.GetScene(name);
            }
            if (mpd.MappedScene is null)
            {
                mpd.MappedScene = GetMappedScene(mpd.SceneName);
            }
        }

        //public static void ScaleOffsets(Dictionary<string, MapLocationDef> locations, string name)
        //{
        //    foreach (MapLocationDef mld in locations.Values)
        //    {
        //        mld.OffsetX = (float)Math.Round(mld.OffsetX / 1.46f, 1);
        //        mld.OffsetY = (float)Math.Round(mld.OffsetY / 1.46f, 1);
        //    }
        //    JsonUtil.Serialize(locations, name);
        //}

        public static string GetMappedScene(string scene)
        {
            if (scene is null) return default;
            if (mappedScenes.TryGetValue(scene, out MappedSceneDef msd))
            {
                return msd.MappedScene;
            }
            return default;
        }

        public static MapZone GetMapZone(string scene)
        {
            if (scene is null) return default;
            if (mappedScenes.TryGetValue(scene, out MappedSceneDef msd))
            {
                return msd.MapZone;
            }
            return default;
        }

        public static bool IsScene(string scene)
        {
            if (scene is null) return false;
            return (mappedScenes.ContainsKey(scene));
        }

        public static bool IsMappedScene(string scene)
        {
            if (scene is null) return false;
            if (mappedScenes.TryGetValue(scene, out MappedSceneDef msd))
            {
                return scene == msd.MappedScene;
            }
            return false;
        }

        public static string CurrentScene()
        {
            string scene = GameManager.instance.sceneName;
            if (scene is null) return default;
            if (scene.EndsWith("_boss") || scene.EndsWith("_preload"))
            {
                return scene.DropSuffix();
            }
            if (scene.EndsWith("_boss_defeated"))
            {
                return scene.DropSuffix().DropSuffix();
            }
            return scene;
        }

        public static MapZone GetCurrentMapZone()
        {
            return GetMapZone(CurrentScene());
        }
    }
}
