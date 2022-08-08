using MapChanger.Defs;
using MapChanger.Map;
using Modding;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MapChanger
{
    public class MapChangerMod : Mod
    {
        internal static MapChangerMod Instance;
        public override string GetVersion() => "0.0.1";

        //public override List<(string, string)> GetPreloadNames()
        //{
        //    var dict = new List<(string, string)>();
        //    int max = 499;
        //    //max = 3;
        //    for (int i = 0; i < max; i++)
        //    {
        //        switch (i)
        //        {
        //            case 0:
        //            case 1:
        //            case 2:
        //                continue;
        //        }
        //        string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
        //        dict.Add((Path.GetFileNameWithoutExtension(scenePath), "_SceneManager"));
        //    }
        //    return dict;
        //}

        //public MapChangerMod()
        //{
        //    On.SceneManager.Start += OnSceneManagerStart;
        //    UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        //}

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;

            Dependencies.GetDependencies();

            foreach (KeyValuePair<string, Assembly> pair in Dependencies.strictDependencies)
            {
                if (pair.Value == null)
                {
                    Log($"{pair.Key} is not installed. MapChangerMod disabled");
                    return;
                }
            }
            
            Finder.Load();
            Tracker.Load();
            BuiltInObjects.Load();
            VariableOverrides.Load();

            Events.Initialize();

            //JsonUtil.Serialize(tileMaps, "tileMaps.json");
        }

        //private void OnSceneManagerStart(On.SceneManager.orig_Start orig, SceneManager self)
        //{
        //    orig(self);
        //}

        //public record TileMapDef
        //{
        //    public string SceneName;
        //    public int Width;
        //    public int Height;
        //}

        //public static readonly Dictionary<string, TileMapDef> tileMaps = new();

        //private void OnSceneLoaded(Scene loadedScene, LoadSceneMode lsm)
        //{
        //    if (!Finder.IsMappedScene(loadedScene.name)) return;

        //    GameManager.instance.RefreshTilemapInfo(loadedScene.name);

        //    LogDebug($"{loadedScene.name} {GameManager.instance.tilemap.width} {GameManager.instance.tilemap.height}");

        //    tileMaps[loadedScene.name] = new()
        //    {
        //        SceneName = loadedScene.name,
        //        Width = GameManager.instance.tilemap.width,
        //        Height = GameManager.instance.tilemap.height
        //    };
        //}
    }
}
