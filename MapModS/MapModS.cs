using MapModS.Data;
using MapModS.Map;
using MapModS.Settings;
using MapModS.Shop;
using MapModS.Trackers;
using MapModS.UI;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MapModS
{
    public class MapModS : Mod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        public static MapModS Instance;

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
                MainData.Load();
            }
            catch (Exception e)
            {
                LogError($"Error loading data!\n{e}");
                throw;
            }

            ModHooks.NewGameHook += ModHooks_NewGameHook;
            On.GameManager.LoadGame += GameManager_LoadGame;
            On.QuitToMenu.Start += QuitToMenu_Start;

            Log("Initialization complete.");
        }

        private void ModHooks_NewGameHook()
        {
            Hook();
        }

        private void GameManager_LoadGame(On.GameManager.orig_LoadGame orig, GameManager self, int saveSlot, Action<bool> callback)
        {
            orig(self, saveSlot, callback);

            Hook();
        }

        private IEnumerator QuitToMenu_Start(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        {
            Unhook();

            return orig(self);
        }

        private void Hook()
        {
            if (RandomizerMod.RandomizerMod.RS.GenerationSettings == null) return;

            Log("Activating mod");

            // Load default/custom assets
            SpriteManager.LoadPinSprites();
            Colors.LoadCustomColors();

            if (Dependencies.HasBenchwarp())
            {
                BenchwarpInterop.Load();
            }

            // Track when items are picked up/Geo Rocks are broken
            ItemTracker.Hook();
            GeoRockTracker.Hook();

            // Remove Map Markers from the Shop (when mod is enabled)
            ShopChanger.Hook();

            // Modify overall Map behaviour
            WorldMap.Hook();

            // Modify overall Quick Map behaviour
            QuickMap.Hook();

            // Allow the full Map to be toggled
            FullMap.Hook();

            // Disable Vanilla Pins when mod is enabled
            PinsVanilla.Hook();

            // Immediately update Map on scene change
            Quill.Hook();

            // Add all the UI elements (world map, quick map, pause menu)
            GUI.Hook();
        }

        private void Unhook()
        {
            ItemTracker.Unhook();
            GeoRockTracker.Unhook();
            ShopChanger.Unhook();
            WorldMap.Unhook();
            QuickMap.Unhook();
            FullMap.Unhook();
            PinsVanilla.Unhook();
            Quill.Unhook();
            GUI.Unhook();
        }
    }
}