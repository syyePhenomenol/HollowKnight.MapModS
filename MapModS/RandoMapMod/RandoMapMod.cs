using System;
using System.Collections.Generic;
using MapChanger;
using MapChanger.Defs;
using MapChanger.UI;
using Modding;
using RandoMapMod.Modes;
using RandoMapMod.Pins;
using RandoMapMod.Settings;
//using RandoMapMod.Transition;
using UnityEngine;

namespace RandoMapMod
{
    public class RandoMapMod : MapMod, ILocalSettings<LocalSettings>, IGlobalSettings<GlobalSettings>
    {
        protected override string[] Dependencies => new string[]
        {
            "MapChangerMod",
            "Randomizer 4",
            "CMICore",
        };

        public override MapMode[] Modes => new MapMode[]
        {
            new FullMapMode(),
            new AllPinsMode(),
            new PinsOverMapMode(),
            //new TransitionNormalMode(),
            //new TransitionVisitedOnlyMode(),
            //new TransitionAllRoomsMode()
        };


        protected override MainButton[] MainButtons => new MainButton[]
        {

        };

        protected override ExtraButtonPanel[] ExtraButtonPanels => new ExtraButtonPanel[]
        {

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

        protected override void LoadGlobalData()
        {
            try
            {
                Interop.FindInteropMods();
                Finder.InjectLocations(JsonUtil.Deserialize<Dictionary<string, MapLocationDef>>("MapModS.RandoMapMod.Resources.locations.json"));
            }
            catch (Exception e)
            {
                LogError(e);
            }
            
            //TODO: pathfinder, transition, etc.
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

        protected override bool ActivateCondition()
        {
            return RandomizerMod.RandomizerMod.RS.GenerationSettings is not null;
        }

        protected override void CreateMapObjects(GameObject goMap)
        {
            //TransitionData.SetTransitionLookup();
            //PathfinderData.Load();
            //Pathfinder.Initialize();

            RmmPinMaster.MakePins(goMap);

            LS.Initialize();
        }
    }
}