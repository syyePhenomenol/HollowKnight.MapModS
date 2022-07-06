using GlobalEnums;
using MapChanger;
using MapChanger.Objects;
using MapModS.Data;
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
    public class MapModS : Mod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        internal static MapModS Instance;

        public override string GetVersion() => "2.6.0";

        public override int LoadPriority() => 10;

        public static LocalSettings LS = new();
        public void OnLoadLocal(LocalSettings ls) => LS = ls;
        public LocalSettings OnSaveLocal() => LS;

        public static GlobalSettings GS = new();
        public void OnLoadGlobal(GlobalSettings gs) => GS = gs;
        public GlobalSettings OnSaveGlobal() => GS;

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
                //MainData.Load();
                //MapData.LoadGlobalMapDefs();
                //RandoPinData.LoadGlobalPinDefs();
            }
            catch (Exception e)
            {
                LogError($"Error loading data!\n{e}");
                throw;
            }

            Utils.AddHookModule<GUI>();

            Events.OnEnterGame += OnEnterGame;
            Events.OnSetGameMap += OnSetGameMap;

            Log("Initialization complete.");
        }

        private static void OnEnterGame()
        {
            if (RandomizerMod.RandomizerMod.RS.GenerationSettings == null) return;

            Instance.Log("Activating mod");

            LocationTracker.GetPreviouslyObtainedItems();
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
            randoPinGroup = Utils.MakeMonoBehaviour<MapObjectGroup>(goMap, "Rando Pin Group");

            IEnumerable<RandomizerModPinDef> pinDefs = RandoPinData.PinDefs.Values.OrderBy(pinDef => pinDef.MapPosition.OffsetX).ThenBy(pinDef => pinDef.MapPosition.OffsetY);
            for (int i = 0; i < pinDefs.Count(); i++)
            {
                RandoPin pin = Utils.MakeMonoBehaviour<RandoPin>(randoPinGroup.gameObject, pinDefs.ElementAt(i).Name);
                pin.Initialize(pinDefs.ElementAt(i), OFFSETZ_BASE + (float)i / pinDefs.Count() * OFFSETZ_RANGE);
                randoPinGroup.MapObjects.Add(pin);
            }
        }
    }
}