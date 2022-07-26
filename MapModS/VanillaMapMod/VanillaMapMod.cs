using System;
using MapChanger;
using MapChanger.UI;
using Modding;
using UnityEngine;
using VanillaMapMod.Settings;

namespace VanillaMapMod
{
    public sealed class VanillaMapMod : MapMod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        protected override string[] Dependencies => new string[] 
        { 
            "MapChangerMod", 
            "CMICore"
        };

        public override MapMode[] Modes => new MapMode[]
        {
            new NormalMode(),
            new FullMapMode()
        };

        protected override MainButton[] MainButtons => new MainButton[] 
        { 
            new PinSizeButton(),
            new ModPinsButton(),
            new VanillaPinsButton(),
            new PoolsPanelButton(),
        };

        protected override ExtraButtonPanel[] ExtraButtonPanels => new ExtraButtonPanel[]
        { 
            new PoolsPanel() 
        };

        internal static VanillaMapMod Instance { get; private set; }

        public VanillaMapMod()
        {
            Instance = this;
        }

        public override string GetVersion() => "2.0.0";
        public override int LoadPriority() => 10;

        internal static LocalSettings LS = new();

        public void OnLoadLocal(LocalSettings ls) => LS = ls;

        public LocalSettings OnSaveLocal()
        {
            //VanillaMapMod.Instance.LogDebug("On Save Local");

            LS.ModEnabled = MapChanger.Settings.MapModEnabled;

            if (MapChanger.Settings.CurrentMode().Mod is "VanillaMapMod"
                && Enum.TryParse(MapChanger.Settings.CurrentMode().ModeName, out VMMMode mode))
            {
                LS.Mode = mode;
            }

            return LS;
        }

        internal static GlobalSettings GS = new();
        public void OnLoadGlobal(GlobalSettings gs) => GS = gs;
        public GlobalSettings OnSaveGlobal() => GS;

        public override void ImportLS()
        {
            MapChanger.Settings.SetModEnabled(LS.ModEnabled);

            MapChanger.Settings.SetMode("VanillaMapMod", LS.Mode.ToString().Replace('_', ' '));
        }

        protected override void CreateMapObjects(GameObject goMap)
        {
            try
            {
                VmmPinMaster.MakePins(goMap);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }
    }
}
