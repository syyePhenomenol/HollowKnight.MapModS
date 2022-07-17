using MapChanger;
using MapChanger.MonoBehaviours;
using MapModS.Pins;
using MapModS.Settings;
using Modding;
using System;
using System.Collections.Generic;
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
                RandoPinData.InjectRandoLocations();
            }
            catch (Exception e)
            {
                LogError($"Error loading data!\n{e}");
                throw;
            }

            //Utils.AddHookModule<GUI>();

            Events.AfterEnterGame += OnEnterGame;
            Events.AfterSetGameMap += OnSetGameMap;

            Log("Initialization complete.");
        }

        private static void OnEnterGame()
        {
            if (RandomizerMod.RandomizerMod.RS.GenerationSettings == null) return;

            //SpriteManager.LoadPinSprites();

            MapChanger.Settings.AddModes(new MapModeSetting[]
            {
                new()
                {
                    ModeKey = "RMM-Full Map",
                    ModeName = "Full Map",
                    ForceHasMap = true,
                    ForceHasQuill = true,
                    ImmediateMapUpdate = true,
                    ForceFullMap = true,
                    DisableAreaNames = false,
                    DisableNextArea = false,
                    DisableVanillaPins = false,
                    EnableCustomColors = true,
                    EnableExtraRoomNames = false
                },
                new()
                {
                    ModeKey = "RMM-Transition",
                    ModeName = "Transition",
                    ForceHasMap = true,
                    ForceHasQuill = true,
                    ImmediateMapUpdate = true,
                    ForceFullMap = true,
                    DisableAreaNames = true,
                    DisableNextArea = true,
                    DisableVanillaPins = true,
                    EnableCustomColors = true,
                    EnableExtraRoomNames = true,
                    OnRoomSpriteSet = SetTransitionRoomColors,
                }
            });
            //MapChanger.Settings.ToggleMode();

            Instance.Log("Activating mod");
            //RandoPinData.SetPinDefs();
            //LS.Initialize();
        }

        public static MapObjectGroup randoPinGroup;

        private static bool SetTransitionRoomColors(RoomSprite roomSprite)
        {
            roomSprite.gameObject.SetActive(true);

            if (roomSprite.Selected)
            {
                roomSprite.Sr.color = Colors.GetColor(ColorSetting.Room_Selected);
            }
            else
            {
                roomSprite.Sr.color = Colors.GetColor(ColorSetting.Room_Normal);
            }
            return true;
        }

        private static void OnSetGameMap(GameObject goMap)
        {
            //randoPinGroup = Utils.MakeMonoBehaviour<MapObjectGroup>(goMap, "Rando Pin Group");

            //IEnumerable<RandomizerModPinDef> pinDefs = RandoPinData.PinDefs.Values.OrderBy(pinDef => pinDef.MapPosition.OffsetX).ThenBy(pinDef => pinDef.MapPosition.OffsetY);
            //for (int i = 0; i < pinDefs.Count(); i++)
            //{
            //    RandoPin pin = Utils.MakeMonoBehaviour<RandoPin>(randoPinGroup.gameObject, pinDefs.ElementAt(i).Name);
            //    pin.Initialize(pinDefs.ElementAt(i), OFFSETZ_BASE + (float)i / pinDefs.Count() * OFFSETZ_RANGE);
            //    randoPinGroup.MapObjects.Add(pin);
            //}
        }
    }
}