﻿using System;
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
                Dictionary<string, MappedSceneDef> mappedSceneLookupAM = JsonUtil.Deserialize<Dictionary<string, MappedSceneDef>>("MapModS.MapChanger.Resources.mappedScenesAM.json");
                foreach ((string scene, MappedSceneDef msd) in mappedSceneLookupAM.Select(kvp => (kvp.Key, kvp.Value)))
                {
                    mappedScenes[scene] = msd;
                }
            }
        }

        public static void InjectLocations(Dictionary<string, MapLocationDef> locations)
        {
            foreach ((string name, MapLocationDef mpd) in locations.Select(kvp => (kvp.Key, kvp.Value)))
            {
                injectedLocations[name] = mpd;
            }
        }

        public static bool TryGetLocation(string name, out MapLocationDef mld)
        {
            mld = null;
            if (name is null) return false;
            if (injectedLocations.TryGetValue(name, out mld))
            {
                return true;
            }
            if (locations.TryGetValue(name, out mld))
            {
                return true;
            }
            return false;
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
            return mappedScenes.ContainsKey(scene);
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

        public static MapZone GetCurrentMapZone()
        {
            return GetMapZone(Utils.CurrentScene());
        }
    }
}
