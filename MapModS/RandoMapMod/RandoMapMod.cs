using System;
using System.Collections.Generic;
using MapChanger;
using MapChanger.Defs;
using MapChanger.UI;
using Modding;
using RandoMapMod.Modes;
using RandoMapMod.Pins;
using RandoMapMod.Settings;
using RandoMapMod.UI;
//using RandoMapMod.Transition;
using UnityEngine;

namespace RandoMapMod
{
    public class RandoMapMod : Mod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        private static readonly string[] dependencies = new string[]
        {
            "MapChangerMod",
            "Randomizer 4",
            "CMICore",
        };

        private static readonly MapMode[] modes = new MapMode[]
        {
            new FullMapMode(),
            new AllPinsMode(),
            new PinsOverMapMode(),
            //new TransitionNormalMode(),
            //new TransitionVisitedOnlyMode(),
            //new TransitionAllRoomsMode()
        };


        private static readonly MainButton[] mainButtons = new MainButton[]
        {
            new PinSizeButton(),
            new RandomizedButton(),
            new VanillaButton(),
            new PoolsPanelButton(),
        };

        private static readonly ExtraButtonPanel[] extraButtonPanels = new ExtraButtonPanel[]
        {
            new PoolsPanel()
        };

        internal static RandoMapMod Instance;

        public RandoMapMod()
        {
            Instance = this;
        }

        public override string GetVersion() => "2.7.0";

        public override int LoadPriority() => 10;

        public static LocalSettings LS = new();
        public void OnLoadLocal(LocalSettings ls) => LS = ls;
        public LocalSettings OnSaveLocal() => LS;

        public static GlobalSettings GS = new();
        public void OnLoadGlobal(GlobalSettings gs) => GS = gs;
        public GlobalSettings OnSaveGlobal() => GS;

        public override void Initialize()
        {
            LogDebug($"Initializing");

            foreach (string dependency in dependencies)
            {
                if (ModHooks.GetMod(dependency) is not Mod)
                {
                    MapChangerMod.Instance.LogWarn($"Dependency not found for {GetType().Name}: {dependency}");
                    return;
                }
            }

            try
            {
                Interop.FindInteropMods();
                Finder.InjectLocations(JsonUtil.Deserialize<Dictionary<string, MapLocationDef>>("MapModS.RandoMapMod.Resources.locations.json"));

                Events.AfterEnterGame += OnEnterGame;
                Events.BeforeQuitToMenu += OnQuitToMenu;
            }
            catch (Exception e)
            {
                LogError(e);
            }

            LogDebug($"Initialization complete.");
            
            //TODO: pathfinder, transition, etc.
        }

        private static void OnEnterGame()
        {
            if (RandomizerMod.RandomizerMod.RS.GenerationSettings is null) return;

            MapChanger.Settings.AddModes(modes);

            Events.AfterSetGameMap += SetMapObjects;
        }

        private static void SetMapObjects(GameObject goMap)
        {
            //TransitionData.SetTransitionLookup();
            //PathfinderData.Load();
            //Pathfinder.Initialize();

            RmmPinMaster.MakePins(goMap);

            LS.Initialize();

            foreach (MainButton button in mainButtons)
            {
                button.Make(PauseMenu.MainButtonsGrid);
            }

            foreach (ExtraButtonPanel ebp in extraButtonPanels)
            {
                ebp.Make();
            }
        }

        private static void OnQuitToMenu()
        {
            Events.AfterSetGameMap -= SetMapObjects;
        }

        //private static bool SetTransitionRoomColors(RoomSprite roomSprite)
        //{
        //    roomSprite.gameObject.SetActive(true);

        //    if (roomSprite.Selected)
        //    {
        //        roomSprite.Sr.color = Colors.GetColor(ColorSetting.Room_Selected);
        //    }
        //    else
        //    {
        //        roomSprite.Sr.color = Colors.GetColor(ColorSetting.Room_Normal);
        //    }
        //    return true;
        //}
    }
}