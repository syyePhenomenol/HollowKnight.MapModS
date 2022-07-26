using GlobalEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SM = UnityEngine.SceneManagement.SceneManager;

namespace MapChanger
{
    public static class Utils
    {
        ///// <summary>
        ///// For hooking to events not in MapChanger.Events.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        //internal static void AddHookModule<T>() where T : IMainHooks
        //{
        //    if (Events.HookModules.Any(hookModule => hookModule.GetType() == typeof(T)))
        //    {
        //        MapChangerMod.Instance.LogWarn($"HookModule of type {typeof(T).Name} has already been added!");
        //        return;
        //    }
        //    Events.HookModules.Add((T)Activator.CreateInstance(typeof(T)));
        //}

        /// <summary>
        /// Generic method for creating a new GameObject with the MonoBehaviour, and returning the MonoBehaviour.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns>The new MonoBehaviour instance</returns>
        public static T MakeMonoBehaviour<T>(GameObject parent, string name) where T : MonoBehaviour
        {
            GameObject go = new(name);
            if (parent is not null)
            {
                go.transform.SetParent(parent.transform);
            }
            go.transform.localScale = Vector3.one;
            go.SetActive(false);
            return go.AddComponent<T>();
        }

        //public static string ToCleanPreviewText(string text)
        //{
        //    return text.Replace("Pay ", "")
        //        .Replace("Once you own ", "")
        //        .Replace(", I'll gladly sell it to you.", "")
        //        .Replace("Requires ", "");
        //}

        public static string ToCleanName(this string name)
        {
            return name.Replace("-", " ").Replace("_", " ");
        }

        public static bool HasMapSetting(MapZone mapZone)
        {
            return mapZone switch
            {
                MapZone.ABYSS => PlayerData.instance.GetBool("mapAbyss"),
                MapZone.CITY => PlayerData.instance.GetBool("mapCity"),
                MapZone.CLIFFS => PlayerData.instance.GetBool("mapCliffs"),
                MapZone.CROSSROADS => PlayerData.instance.GetBool("mapCrossroads"),
                MapZone.MINES => PlayerData.instance.GetBool("mapMines"),
                MapZone.DEEPNEST => PlayerData.instance.GetBool("mapDeepnest"),
                MapZone.TOWN => PlayerData.instance.GetBool("mapDirtmouth"),
                MapZone.FOG_CANYON => PlayerData.instance.GetBool("mapFogCanyon"),
                MapZone.WASTES => PlayerData.instance.GetBool("mapFungalWastes"),
                MapZone.GREEN_PATH => PlayerData.instance.GetBool("mapGreenpath"),
                MapZone.OUTSKIRTS => PlayerData.instance.GetBool("mapOutskirts"),
                MapZone.ROYAL_GARDENS => PlayerData.instance.GetBool("mapRoyalGardens"),
                MapZone.RESTING_GROUNDS => PlayerData.instance.GetBool("mapRestingGrounds"),
                MapZone.WATERWAYS => PlayerData.instance.GetBool("mapWaterways"),
                MapZone.WHITE_PALACE => PlayerData.instance.GetBool("AdditionalMapsGotWpMap"),
                MapZone.GODS_GLORY => PlayerData.instance.GetBool("AdditionalMapsGotGhMap"),
                _ => false,
            };
        }

        public static string DropSuffix(this string scene)
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

        private static readonly List<GameObject> rootObjects = new(500);
        /// <summary>
        /// Copied from ItemChanger by homothety.
        /// Finds a GameObject in the current scene by its full path.
        /// </summary>
        /// <param name="path">The full path to the GameObject, with forward slash ('/') separators.</param>
        /// <returns></returns>
        public static GameObject FindGameObjectInCurrentScene(string path)
        {
            SM.GetActiveScene().GetRootGameObjects(rootObjects); // clears list

            int index = path.IndexOf('/');
            GameObject result = null;
            if (index >= 0)
            {
                string rootName = path.Substring(0, index);
                GameObject root = rootObjects.FirstOrDefault(g => g.name == rootName);
                if (root != null) result = root.transform.Find(path.Substring(index + 1)).gameObject;
            }
            else
            {
                result = rootObjects.FirstOrDefault(g => g.name == path);
            }

            rootObjects.Clear();
            return result;
        }

        public static Transform FindChildInHierarchy(this Transform parent, string pathName)
        {
            string[] splitNames = pathName.Split('/');
            Transform transform = parent;

            foreach (string splitName in splitNames)
            {
                if (splitName == "") return transform;
                Transform child = transform.Find(splitName);
                if (child is null) return null;
                transform = child;
            }
            return transform;
        }

        public static float Snap(this float offset)
        {
            // Snap to nearest 0.1
            return (float)Math.Round(offset * 10f, MidpointRounding.AwayFromZero) / 10f;
        }

        public static Vector2 Snap(this Vector2 vec)
        {
            return new(vec.x.Snap(), vec.y.Snap());
        }

        public static string CurrentScene()
        {
            return GameManager.GetBaseSceneName(GameManager.instance.sceneName);
        }

        //public static string GetBindingsText(List<InControl.BindingSource> bindings)
        //{
        //    string text = "";

        //    text += $"[{bindings.First().Name}]";

        //    if (bindings.Count > 1 && bindings[1].BindingSourceType == InControl.BindingSourceType.DeviceBindingSource)
        //    {
        //        text += $" {L.Localize("or")} ";

        //        text += $"({bindings[1].Name})";
        //    }

        //    return text;
        //}


    }
}
