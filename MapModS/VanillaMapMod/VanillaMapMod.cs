using System;
using MapChanger;
using MapChanger.Defs;
using MapChanger.MonoBehaviours;
using MapChanger.UI;
using Modding;
using UnityEngine;
using VanillaMapMod.Settings;

namespace VanillaMapMod
{
    public sealed class VanillaMapMod : MapMod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        private static readonly string[] dependencies = { "MapChangerMod", "CMICore" };

        private static readonly MapModeSetting[] modes = new MapModeSetting[]
        {
            new()
            {
                Mod = "VanillaMapMod",
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
            },
            new()
            {
                Mod = "VanillaMapMod",
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
            }
        };

        private static readonly MainButton[] mainButtons = { new PinSizeButton() };

        private static readonly ExtraButtonPanel[] extraButtonPanels = { };

        internal static VanillaMapMod Instance { get; private set; }

        public override string GetVersion() => "2.0.0";
        public override int LoadPriority() => 10;

        internal static LocalSettings LS = new();
        public void OnLoadLocal(LocalSettings ls) => LS = ls;
        public LocalSettings OnSaveLocal() => LS;

        internal static GlobalSettings GS = new();
        public void OnLoadGlobal(GlobalSettings gs) => GS = gs;
        public GlobalSettings OnSaveGlobal() => GS;

        public VanillaMapMod() : base(dependencies, modes, mainButtons, extraButtonPanels)
        {
            if (Instance is not null) throw new NotSupportedException($"Cannot construct multiple instances of {GetType().Name}.");
            Instance = this;
        }

        internal static VMMPinGroup Pins { get; private set; }
        internal static Selector PinSelector { get; private set; }

        protected override void CreateMapObjects(GameObject goMap)
        {
            //MapChanger.UI.MapUI.AddLayer(new MapToggleText());

            Pins = Utils.MakeMonoBehaviour<VMMPinGroup>(goMap, nameof(Pins));
            PinSelector = Utils.MakeMonoBehaviour<Selector>(null, nameof(PinSelector));

            foreach (MapLocationDef mld in Finder.GetAllVanillaLocations().Values)
            {
                VMMPin pin = Utils.MakeMonoBehaviour<VMMPin>(Pins.gameObject, $"{nameof(VMMPin)} {mld.Name}");
                pin.Initialize(mld);

                PinSelector.Objects.Add(pin.Location.Name, new() { pin });
                Pins.MapObjects.Add(pin);
            }

            Pins.StaggerZ();
            Pins.InitializeGroups();
        }
    }
}
