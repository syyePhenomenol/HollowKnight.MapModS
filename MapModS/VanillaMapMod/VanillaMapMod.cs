using MapChanger;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using Modding;
using VanillaMapMod.Settings;

namespace VanillaMapMod
{
    public sealed class VanillaMapMod : MapMod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        private const string versionString = "2.0.0";

        private static readonly string[] dependencies = {"MapChangerMod", "CMICore"};

        private static readonly MapModeSetting[] modes =
        {
            new()
            {
                ModeKey = "VMMFullMap",
                ModeName = "Full Map",
                ForceHasMap = true,
                ForceHasQuill = true,
                ImmediateMapUpdate = true,
                ForceFullMap = true,
                DisableAreaNames = false,
                DisableNextArea = false,
                DisableVanillaPins = true,
                EnableCustomColors = false,
                EnableExtraRoomNames = false,
            },
            new()
            {
                ModeKey = "VMMNormal",
                ModeName = "Normal",
                ForceHasMap = false,
                ForceHasQuill = false,
                ImmediateMapUpdate = true,
                ForceFullMap = false,
                DisableAreaNames = false,
                DisableNextArea = false,
                DisableVanillaPins = true,
                EnableCustomColors = false,
                EnableExtraRoomNames = false,
            }
        };

        internal static VanillaMapMod Instance { get; private set; }

        public override string GetVersion() => versionString;

        public override int LoadPriority() => 10;

        internal static LocalSettings LS = new();
        public void OnLoadLocal(LocalSettings ls) => LS = ls;
        public LocalSettings OnSaveLocal() => LS;

        internal static GlobalSettings GS = new();
        public void OnLoadGlobal(GlobalSettings gs) => GS = gs;
        public GlobalSettings OnSaveGlobal() => GS;

        public VanillaMapMod() : base(dependencies, modes)
        {
            Instance = this;
        }

        internal static VmmPinGroup Pins { get; private set; }

        internal static Selector PinSelector { get; private set; }

        protected override void CreateMapObjects(UnityEngine.GameObject goMap)
        {
            Pins = Utils.MakeMonoBehaviour<VmmPinGroup>(goMap, nameof(Pins));

            PinSelector = Utils.MakeMonoBehaviour<Selector>(null, nameof(PinSelector));

            foreach (MapLocationDef mld in Finder.GetAllVanillaLocations().Values)
            {
                VmmPin pin = Utils.MakeMonoBehaviour<VmmPin>(Pins.gameObject, $"{nameof(VmmPin)} {mld.Name}");
                pin.Initialize(mld);

                PinSelector.Objects.Add(pin.Location.Name, new() { pin });

                Pins.MapObjects.Add(pin);
            }

            Pins.StaggerZ();
            Pins.InitializeGroups();
        }
    }
}
