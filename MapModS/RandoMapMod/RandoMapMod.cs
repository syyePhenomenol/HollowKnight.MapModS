﻿using System;
using System.Collections.Generic;
using MapChanger;
using MapChanger.Defs;
using MapChanger.UI;
using Modding;
using RandoMapMod.Modes;
using RandoMapMod.Pins;
using RandoMapMod.Rooms;
using RandoMapMod.Settings;
using RandoMapMod.Transition;
using RandoMapMod.UI;
using UnityEngine;

namespace RandoMapMod
{
    public class RandoMapMod : Mod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        internal const string MOD = "RandoMapMod";

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
            new TransitionNormalMode(),
            new TransitionVisitedOnlyMode(),
            new TransitionAllRoomsMode()
        };

        private static readonly Title title = new RmmTitle();

        private static readonly MainButton[] mainButtons = new MainButton[]
        {
            new ModEnabledButton(),
            new ModeButton(),
            new PinSizeButton(),
            new PinStyleButton(),
            new RandomizedButton(),
            new VanillaButton(),
            new SpoilersButton(),
            new PoolsPanelButton(),
            new PersistentButton(),
            new GroupByButton()
        };

        private static readonly ExtraButtonPanel[] extraButtonPanels = new ExtraButtonPanel[]
        {
            new PoolsPanel()
        };

        private static readonly MapUILayer[] mapUILayers = new MapUILayer[]
        {
            new Hotkeys(),
            new ControlPanel(),
            new MapKey(),
            new SelectionPanels(),
            new RmmBottomRowText(),
            new InstructionText(),
            new RouteSummaryText(),
            new RouteText(),
            new QuickMapTransitions()
        };

        private static readonly List<HookModule> hookModules = new()
        {
            new RmmPinManager(),
            new TransitionTracker(),
            new RouteTracker(),
            new RouteCompass()
        };

        internal static RandoMapMod Instance;

        public RandoMapMod()
        {
            Instance = this;
        }

        public override string GetVersion() => "MC PRERELEASE 5";

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

            Interop.FindInteropMods();
            Finder.InjectLocations(JsonUtil.Deserialize<Dictionary<string, MapLocationDef>>("MapModS.RandoMapMod.Resources.locations.json"));

            Events.OnEnterGame += OnEnterGame;
            Events.OnQuitToMenu += OnQuitToMenu;

            LogDebug($"Initialization complete.");
        }

        private static void OnEnterGame()
        {
            if (RandomizerMod.RandomizerMod.RS.GenerationSettings is null) return;

            MapChanger.Settings.AddModes(modes);
            Events.OnSetGameMap += OnSetGameMap;

            RmmColors.Load();
            RmmRoomManager.Load();
            if (Interop.HasBenchwarp())
            {
                BenchwarpInterop.Load();
                hookModules.Add(new BenchwarpInterop());
            }
            TransitionData.Load();
            PathfinderData.Load();

            foreach (HookModule hookModule in hookModules)
            {
                hookModule.OnEnterGame();
            }
        }

        private static void OnSetGameMap(GameObject goMap)
        {
            try
            {
                // Make rooms and pins
                RmmRoomManager.Make(goMap);
                RmmPinManager.Make(goMap);

                LS.Initialize();

                // Construct pause menu
                title.Make();

                foreach (MainButton button in mainButtons)
                {
                    button.Make();
                }

                foreach (ExtraButtonPanel ebp in extraButtonPanels)
                {
                    ebp.Make();
                }

                // Construct map UI
                foreach (MapUILayer uiLayer in mapUILayers)
                {
                    MapUILayerUpdater.Add(uiLayer);
                }
            }
            catch (Exception e)
            {
                Instance.LogError(e);
            }
        }

        private static void OnQuitToMenu()
        {
            Events.OnSetGameMap -= OnSetGameMap;

            foreach (HookModule hookModule in hookModules)
            {
                hookModule.OnQuitToMenu();
            }
        }
    }
}