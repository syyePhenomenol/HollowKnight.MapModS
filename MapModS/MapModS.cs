using MapModS.Data;
using MapModS.Map;
using MapModS.Settings;
using MapModS.Shop;
using MapModS.Trackers;
using MapModS.UI;
using Modding;
using System;
using System.Reflection;

namespace MapModS
{
    public class MapModS : Mod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        public static MapModS Instance;

        private readonly string _version = "Rando 4 PRERELEASE 6";

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

            AdditionalMapsInstalled = HasAdditionalMaps();

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

            // Add a Pause Menu GUI
            GUI.Hook();

            // Add keyboard shortcut control
            InputListener.InstantiateSingleton();

            Log("Initialization complete.");
        }

        public static bool HasAdditionalMaps()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == "AdditionalMaps")
                    {
                        Instance.Log("Additional Maps detected");
                        return true;
                    }
                }
            }

            return false;
        }
    }
}