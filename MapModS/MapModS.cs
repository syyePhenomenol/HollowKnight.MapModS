using MapModS.Data;
using MapModS.Map;
using MapModS.Settings;
using MapModS.Shop;
using MapModS.Trackers;
using MapModS.UI;
using Modding;
using System;
using System.Collections;

namespace MapModS
{
    public class MapModS : Mod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        public static MapModS Instance;

        private readonly string _version = "2.1.0";

        public override string GetVersion() => _version;

        public override int LoadPriority() => 10;

        public static LocalSettings LS { get; set; } = new LocalSettings();

        public void OnLoadLocal(LocalSettings s) => LS = s;

        public LocalSettings OnSaveLocal() => LS;

        public static GlobalSettings GS { get; set; } = new GlobalSettings();

        public void OnLoadGlobal(GlobalSettings s) => GS = s;

        public GlobalSettings OnSaveGlobal() => GS;

        public static bool AdditionalMapsInstalled = false;

        public override void Initialize()
        {
            Log("Initializing...");

            Instance = this;

            if (ModHooks.GetMod("Randomizer 4") is not Mod)
            {
                Log("Randomizer 4 was not detected. MapModS disabled");
                return;
            }

            AdditionalMapsInstalled = ModHooks.GetMod("Additional Maps") is Mod;

            if (AdditionalMapsInstalled)
            {
                Instance.Log("Additional Maps detected");
            }

            try
            {
                SpriteManager.LoadEmbeddedPngs("MapModS.Resources.Pins");
            }
            catch (Exception e)
            {
                LogError($"Error loading sprites!\n{e}");
                throw;
            }

            try
            {
                DataLoader.Load();
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
            if (RandomizerMod.RandomizerMod.RS.GenerationSettings == null) return;

            Log("Activating mod");

            Hook();
        }

        private void GameManager_LoadGame(On.GameManager.orig_LoadGame orig, GameManager self, int saveSlot, Action<bool> callback)
        {
            orig(self, saveSlot, callback);

            if (RandomizerMod.RandomizerMod.RS.GenerationSettings == null) return;

            Log("Activating mod");

            Hook();
        }

        private IEnumerator QuitToMenu_Start(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        {
            Unhook();

            return orig(self);
        }

        private void Hook()
        {
            // Track when items are picked up/Geo Rocks are broken
            ItemTracker.Hook();
            GeoRockTracker.Hook();

            // Remove Map Markers from the Shop
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

            // Add a Pause Menu GUI, map text UI and transition helper text
            GUI.Hook();

            // Add keyboard shortcut control
            InputListener.InstantiateSingleton();
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
            InputListener.DestroySingleton();
        }
    }
}