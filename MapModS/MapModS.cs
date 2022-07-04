using GlobalEnums;
using MapModS.Data;
using MapModS.Map;
using MapModS.Pins;
using MapModS.Settings;
using MapModS.UI;
using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MapModS
{
    public class MapModS : Mod, ILocalSettings<RandoLocalSettings>, IGlobalSettings<RandoGlobalSettings>
    {
        internal static MapModS Instance;

        public override string GetVersion() => "2.6.0";

        public override int LoadPriority() => 10;

        public static RandoLocalSettings LS = new();
        public void OnLoadLocal(RandoLocalSettings ls) => LS = ls;
        public RandoLocalSettings OnSaveLocal() => LS;

        public static RandoGlobalSettings GS = new();
        public void OnLoadGlobal(RandoGlobalSettings gs) => GS = gs;
        public RandoGlobalSettings OnSaveGlobal() => GS;

        public static bool WorldMapOpen { get; internal set; }
        public static bool QuickMapOpen { get; internal set; }
        public static MapZone CurrentMapZone { get; internal set; }

        public override void Initialize()
        {
            Log("Initializing...");

            Instance = this;

            Dependencies.GetDependencies();

            foreach (KeyValuePair<string, Assembly> pair in Dependencies.strictDependencies)
            {
                if (pair.Value == null)
                {
                    Log($"{pair.Key} is not installed. MapModS disabled");
                    return;
                }
            }

            foreach (KeyValuePair<string, Assembly> pair in Dependencies.optionalDependencies)
            {
                if (pair.Value == null)
                {
                    Log($"{pair.Key} is not installed. Some features are disabled.");
                }
            }

            try
            {
                MainData.Load();
                MapData.LoadGlobalMapDefs();
                RandoPinData.LoadGlobalPinDefs();
                VariableOverrides.LoadOverrideDefs();
                ItemTracker.LoadTrackingDefs();
            }
            catch (Exception e)
            {
                LogError($"Error loading data!\n{e}");
                throw;
            }

            AddHookModule<ItemTracker>();
            AddHookModule<GUI>();

            Events.OnEnterGame += OnEnterGame;
            Events.OnSetGameMap += OnSetGameMap;

            Events.Initialize();

            Log("Initialization complete.");
        }

        private static void OnEnterGame()
        {
            if (RandomizerMod.RandomizerMod.RS.GenerationSettings == null) return;

            Instance.Log("Activating mod");

            ItemTracker.GetPreviouslyObtainedItems();
            RandoPinData.SetPinDefs();
            //ItemTracker.VerifyTrackingDefs();
            LS.Initialize();
        }

        private const float OFFSETZ_BASE = -0.6f;
        private const float OFFSETZ_RANGE = 0.1f;

        public static MapObjectGroup randoPinGroup;

        private static void OnSetGameMap(GameObject goMap)
        {
            //MakeMonoBehaviour<RandoPinGroup>(goMap, RandoPinGroup.Name);
            randoPinGroup = MakeMonoBehaviour<MapObjectGroup>(goMap, "Rando Pin Group");

            IEnumerable<RandomizerModPinDef> pinDefs = RandoPinData.PinDefs.Values.OrderBy(pinDef => pinDef.OffsetX).ThenBy(pinDef => pinDef.OffsetY);
            for (int i = 0; i < pinDefs.Count(); i++)
            {
                pinDefs.ElementAt(i).OffsetZ = OFFSETZ_BASE + (float)i / pinDefs.Count() * OFFSETZ_RANGE;
                RandoPin pin = MakeMonoBehaviour<RandoPin>(randoPinGroup.gameObject, pinDefs.ElementAt(i).Name);
                pin.RMDef = pinDefs.ElementAt(i);
                pin.SetPosition();
                randoPinGroup.MapObjects.Add(pin);
            }
        }

        /// <summary>
        /// Adds a custom HookModule, for hooking to other events not in Events
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void AddHookModule<T>() where T : HookModule
        {
            if (Events.HookModules.Any(hookModule => hookModule.GetType() == typeof(T)))
            {
                Instance.LogWarn($"HookModule of type {typeof(T).Name} has already been added!");
                return;
            }
            Events.HookModules.Add((T)Activator.CreateInstance(typeof(T)));
        }

        /// <summary>
        /// Generic method for creating a new GameObject with the MonoBehaviour,
        /// and returning the MonoBehaviour
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
    }
}