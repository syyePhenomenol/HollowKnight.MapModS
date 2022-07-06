using GlobalEnums;
using MapChanger.Defs;
using System.Collections.Generic;
using System.Linq;

namespace MapChanger
{
    public static class Finder
    {
        private static Dictionary<string, MappedSceneDef> mappedScenes;
        private static Dictionary<string, MapPositionDef> locations;
        private static Dictionary<string, MapPositionDef> injectedLocations = new();

        internal static void Load()
        {
            mappedScenes = JsonUtil.Deserialize<Dictionary<string, MappedSceneDef>>("MapModS.MapChanger.Resources.mappedScenes.json");
            locations = JsonUtil.Deserialize<Dictionary<string, MapPositionDef>>("MapModS.MapChanger.Resources.locations.json");

            if (Dependencies.HasAdditionalMaps())
            {
                Dictionary<string, MappedSceneDef> mappedSceneLookupAM = JsonUtil.Deserialize <Dictionary<string, MappedSceneDef>> ("MapModS.MapChanger.Resources.AdditionalMaps.mappedScenes.json");
                foreach ((string scene, MappedSceneDef msd) in mappedSceneLookupAM.Select(kvp => (kvp.Key, kvp.Value)))
                {
                    mappedScenes[scene] = msd;
                }

                Dictionary<string, MapPositionDef> locationLookupAM = JsonUtil.Deserialize<Dictionary<string, MapPositionDef>>("MapModS.MapChanger.Resources.AdditionalMaps.locations.json");
                foreach ((string name, MapPositionDef mpd) in locationLookupAM.Select(kvp => (kvp.Key, kvp.Value)))
                {
                    locations[name] = mpd;
                }
            }
        }

        public static void InjectLocation(string name, MapPositionDef mpd)
        {
            injectedLocations[name] = mpd;
        }

        public static MapPositionDef GetLocation(string name)
        {
            if (name is null) return default;
            if (injectedLocations.TryGetValue(name, out MapPositionDef mpd))
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

        public static Dictionary<string, MapPositionDef> GetAllLocations()
        {
            Dictionary<string, MapPositionDef> newLocations = new(locations);
            foreach ((string name, MapPositionDef mpd) in injectedLocations.Select(kvp => (kvp.Key, kvp.Value)))
            {
                newLocations[name] = mpd;
            }

            return newLocations;
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
                return DropSuffix(scene);
            }
            if (scene.EndsWith("_boss_defeated"))
            {
                return DropSuffix(DropSuffix(scene));
            }
            return scene;

            static string DropSuffix(string scene)
            {
                if (scene == "") return "";
                string[] sceneSplit = scene.Split('_');
                string truncatedScene = sceneSplit[0];
                for (int i = 1; i < sceneSplit.Length - 1; i++)
                {
                    truncatedScene += "_" + sceneSplit[i];
                }
                return truncatedScene;
            }
        }

        public static MapZone GetCurrentMapZone()
        {
            return GetMapZone(CurrentScene());
        }
    }
}
