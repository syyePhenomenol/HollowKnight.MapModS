using GlobalEnums;
using System;
using System.Linq;
using UnityEngine;

namespace MapChanger
{
    public static class Utils
    {
        /// <summary>
        /// For hooking to events not in MapChanger.Events.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void AddHookModule<T>() where T : HookModule
        {
            if (Events.HookModules.Any(hookModule => hookModule.GetType() == typeof(T)))
            {
                MapChangerMod.Instance.LogWarn($"HookModule of type {typeof(T).Name} has already been added!");
                return;
            }
            Events.HookModules.Add((T)Activator.CreateInstance(typeof(T)));
        }

        /// <summary>
        /// Generic method for creating a new GameObject with the MonoBehaviour, and returning the MonoBehaviour.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns>The new MonoBehaviour instance</returns>
        public static T MakeMonoBehaviour<T>(GameObject parent, string name) where T : MonoBehaviour
        {
            GameObject newObject = new(name);
            newObject.transform.SetParent(parent.transform);
            return newObject.AddComponent<T>();
        }


        public static string ToCleanPreviewText(string text)
        {
            return text.Replace("Pay ", "")
                .Replace("Once you own ", "")
                .Replace(", I'll gladly sell it to you.", "")
                .Replace("Requires ", "")
                .Replace("<br>", "");
        }

        public static string ToCleanName(this string name)
        {
            return name.Replace("-", " ").Replace("_", " ");
        }

        //public static MapZone ToMapZone(string mapArea)
        //{
        //    return mapArea switch
        //    {
        //        "Ancient Basin" => MapZone.ABYSS,
        //        "City of Tears" => MapZone.CITY,
        //        "Crystal Peak" => MapZone.MINES,
        //        "Deepnest" => MapZone.DEEPNEST,
        //        "Dirtmouth" => MapZone.TOWN,
        //        "Fog Canyon" => MapZone.FOG_CANYON,
        //        "Forgotten Crossroads" => MapZone.CROSSROADS,
        //        "Fungal Wastes" => MapZone.WASTES,
        //        "Greenpath" => MapZone.GREEN_PATH,
        //        "Howling Cliffs" => MapZone.CLIFFS,
        //        "Kingdom's Edge" => MapZone.OUTSKIRTS,
        //        "Queen's Gardens" => MapZone.ROYAL_GARDENS,
        //        "Resting Grounds" => MapZone.RESTING_GROUNDS,
        //        "Royal Waterways" => MapZone.WATERWAYS,
        //        "White Palace" => MapZone.WHITE_PALACE,
        //        _ => MapZone.NONE
        //    };
        //}

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

        //public static string GetActualSceneName(string objName)
        //{
        //    // Some room objects have non-standard scene names, so we truncate the name
        //    // in these situations

        //    if (objName == "Ruins1_31_top_2") return "Ruins1_31b";
        //    if (objName == "Waterways_04_part_b") return "Waterways_02";

        //    for (int i = 0; i < 2; i++)
        //    {
        //        if (RandomizerMod.RandomizerData.Data.IsRoom(objName) || objName.IsSpecialRoom())
        //        {
        //            return objName;
        //        }

        //        objName = DropSuffix(objName);
        //    }

        //    return null;
        //}

        //public static bool IsFSMMapState(string name)
        //{
        //    return name switch
        //    {
        //        "Abyss"
        //        or "Ancient Basin"
        //        or "City"
        //        or "Cliffs"
        //        or "Crossroads"
        //        or "Deepnest"
        //        or "Fog Canyon"
        //        or "Fungal Wastes"
        //        or "Fungus"
        //        or "Greenpath"
        //        or "Hive"
        //        or "Mines"
        //        or "Outskirts"
        //        or "Resting Grounds"
        //        or "Royal Gardens"
        //        or "Waterways"
        //        or "WHITE_PALACE"
        //        or "GODS_GLORY" => true,
        //        _ => false,
        //    };
        //}

        //public static double DistanceToMiddle(Transform transform)
        //{
        //    return Math.Pow(transform.position.x, 2) + Math.Pow(transform.position.y, 2);
        //}

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
