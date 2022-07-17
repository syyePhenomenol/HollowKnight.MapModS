using System;
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
            }

            //foreach ((string name, MapLocationDef mpd) in locations.Select(kvp => (kvp.Key, kvp.Value)))
            //{
            //    CompleteLocationDef(name, mpd);
            //}
        }

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
                position = mappedScenePositions[scene].Snap();

                return true;
            }
            return false;
        }

        public static void InjectLocations(Dictionary<string, MapLocationDef> locations)
        {
            foreach ((string name, MapLocationDef mpd) in locations.Select(kvp => (kvp.Key, kvp.Value)))
            {
                //CompleteLocationDef(name, mpd);
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

        public static Dictionary<string, MapLocationDef> GetAllVanillaLocations()
        {
            return locations;
        }

        public static Dictionary<string, MapLocationDef> GetAllLocations()
        {
            Dictionary<string, MapLocationDef> newLocations = new(locations);
            foreach ((string name, MapLocationDef mld) in injectedLocations.Select(kvp => (kvp.Key, kvp.Value)))
            {
                newLocations[name] = mld;
            }

            return newLocations;
        }

        internal static bool IsMinimalMapScene(string scene)
        {
            return minimalMapScenes.Contains(scene);
        }

        //private static void CompleteLocationDef(string name, MapLocationDef mpd)
        //{
        //    if (mpd.Name is null)
        //    {
        //        mpd.Name = name;
        //    }
        //    if (mpd.SceneName is null && Dependencies.HasItemChanger())
        //    {
        //        mpd.SceneName = IC.ICInterop.GetScene(name);
        //    }
        //    if (mpd.MappedScene is null)
        //    {
        //        mpd.MappedScene = GetMappedScene(mpd.SceneName);
        //    }
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
            return GameManager.GetBaseSceneName(GameManager.instance.sceneName);
            //string scene = GameManager.instance.sceneName;
            //if (scene is null) return default;
            //if (scene.EndsWith("_boss") || scene.EndsWith("_preload"))
            //{
            //    return scene.DropSuffix();
            //}
            //if (scene.EndsWith("_boss_defeated"))
            //{
            //    return scene.DropSuffix().DropSuffix();
            //}
            //return scene;
        }

        public static MapZone GetCurrentMapZone()
        {
            return GetMapZone(CurrentScene());
        }
    }
}
