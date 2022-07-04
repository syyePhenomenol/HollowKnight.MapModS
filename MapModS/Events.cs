using MapModS.Data;
using MapModS.Map;
using System.Collections;
using System.Collections.Generic;
using System;
using GlobalEnums;
using HutongGames.PlayMaker;
using UnityEngine;
using Vasi;

namespace MapModS
{
    /// <summary>
    /// Contains some common/map-related events to hook to.
    /// </summary>
    public static class Events
    {
        public static event Action OnEnterGame;
        public static event Action OnQuitToMenu;
        public static event Action<GameObject> OnSetGameMap;
        public static event Action<GameMap> OnOpenWorldMap;
        public static event Action<GameMap, MapZone> OnOpenQuickMap;
        public static event Action<GameMap> OnCloseMap;

        private class QuickMapCustom : FsmStateAction
        {
            private readonly MapZone mapZone;
            private readonly GameMap gameMap;

            internal QuickMapCustom(GameMap gameMap, MapZone mapZone)
            {
                this.mapZone = mapZone;
                this.gameMap = gameMap;
            }

            public override void OnEnter()
            {
                OpenQuickMapEvent(gameMap, mapZone);
            }
        }

        internal static readonly List<HookModule> HookModules = new()
        {
            new MethodOverrides(),
            new VariableOverrides(),
            new QoL()
        };

        private static readonly MapZone[] customMapZones =
        {
            MapZone.WHITE_PALACE,
            MapZone.GODS_GLORY
        };

        internal static void Initialize()
        {
            On.GameManager.StartNewGame += AfterStartNewGame;
            On.GameManager.ContinueGame += AfterContinueGame;
            On.QuitToMenu.Start += QuitToMenuEvent;
            On.GameManager.SetGameMap += SetGameMapEvent;
            On.GameMap.WorldMap += OpenWorldMapEvent;
            On.GameMap.QuickMapAncientBasin += OnOpenQuickMapAncientBasin;
            On.GameMap.QuickMapCity += OnOpenQuickMapCity;
            On.GameMap.QuickMapCliffs += OnOpenQuickMapCliffs;
            On.GameMap.QuickMapCrossroads += OnOpenQuickMapCrossroads;
            On.GameMap.QuickMapCrystalPeak += OnOpenQuickMapCrystalPeak;
            On.GameMap.QuickMapDeepnest += OnOpenQuickMapDeepnest;
            On.GameMap.QuickMapDirtmouth += OnOpenQuickMapDirtmouth;
            On.GameMap.QuickMapFogCanyon += OnOpenQuickMapFogCanyon;
            On.GameMap.QuickMapFungalWastes += OnOpenQuickMapFungalWastes;
            On.GameMap.QuickMapGreenpath += OnOpenQuickMapGreenpath;
            On.GameMap.QuickMapKingdomsEdge += OnOpenQuickMapKingdomsEdge;
            On.GameMap.QuickMapQueensGardens += OnOpenQuickMapQueensGardens;
            On.GameMap.QuickMapRestingGrounds += OnOpenQuickMapRestingGrounds;
            On.GameMap.QuickMapWaterways += OnOpenQuickMapWaterways;
            On.GameMap.CloseQuickMap += CloseMapEvent;
        }

        private static void AfterStartNewGame(On.GameManager.orig_StartNewGame orig, GameManager self, bool permadeathMode, bool bossRushMode)
        {
            orig(self, permadeathMode, bossRushMode);
            EnterGameEvent();
        }

        private static void AfterContinueGame(On.GameManager.orig_ContinueGame orig, GameManager self)
        {
            orig(self);
            EnterGameEvent();
        }

        private static void EnterGameEvent()
        {
            // Load default/custom assets
            SpriteManager.LoadPinSprites();
            Colors.LoadCustomColors();

            foreach (HookModule hookModule in HookModules)
            {
                hookModule.Hook();
            }

            try { OnEnterGame?.Invoke(); }
            catch (Exception e) { MapModS.Instance.LogError(e); }
        }

        private static IEnumerator QuitToMenuEvent(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        {
            foreach (HookModule hookModule in HookModules)
            {
                hookModule.Unhook();
            }

            try { OnQuitToMenu?.Invoke(); }
            catch (Exception e) { MapModS.Instance.LogError(e); }

            return orig(self);
        }

        private static void SetGameMapEvent(On.GameManager.orig_SetGameMap orig, GameManager self, GameObject goMap)
        {
            orig(self, goMap);
            HookQuickMapCustom();

            try { OnSetGameMap?.Invoke(goMap); }
            catch (Exception e) { MapModS.Instance.LogError(e); }

            static void HookQuickMapCustom()
            {
                GameObject goQuickMap = GameObject.Find("Quick Map");
                PlayMakerFSM fsmQuickMap = goQuickMap.LocateMyFSM("Quick Map");

                foreach (MapZone mapZone in customMapZones)
                {
                    if (fsmQuickMap.TryGetState(mapZone.ToString(), out FsmState state))
                    {
                        FsmUtil.AddAction(state, new QuickMapCustom(goQuickMap.GetComponent<GameMap>(), mapZone));
                    }
                }
            }
        }

        private static void OpenWorldMapEvent(On.GameMap.orig_WorldMap orig, GameMap self)
        {
            orig(self);
            MapModS.WorldMapOpen = true;
            MapModS.CurrentMapZone = MapZone.NONE;

            try { OnOpenWorldMap?.Invoke(self); }
            catch (Exception e) { MapModS.Instance.LogError(e); }
        }

        private static void OpenQuickMapEvent(GameMap gameMap, MapZone mapZone)
        {
            MapModS.QuickMapOpen = true;
            MapModS.CurrentMapZone = mapZone;

            try { OnOpenQuickMap?.Invoke(gameMap, mapZone); }
            catch (Exception e) { MapModS.Instance.LogError(e); }
        }

        private static void CloseMapEvent(On.GameMap.orig_CloseQuickMap orig, GameMap self)
        {
            MapModS.WorldMapOpen = false;
            MapModS.QuickMapOpen = false;
            MapModS.CurrentMapZone = MapZone.NONE;

            try { OnCloseMap?.Invoke(self); }
            catch (Exception e) { MapModS.Instance.LogError(e); }

            orig(self);
        }

        private static void OnOpenQuickMapAncientBasin(On.GameMap.orig_QuickMapAncientBasin orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.ABYSS);
        }

        private static void OnOpenQuickMapCity(On.GameMap.orig_QuickMapCity orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.CITY);
        }

        private static void OnOpenQuickMapCliffs(On.GameMap.orig_QuickMapCliffs orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.CLIFFS);
        }

        private static void OnOpenQuickMapCrossroads(On.GameMap.orig_QuickMapCrossroads orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.CROSSROADS);
        }

        private static void OnOpenQuickMapCrystalPeak(On.GameMap.orig_QuickMapCrystalPeak orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.MINES);
        }

        private static void OnOpenQuickMapDeepnest(On.GameMap.orig_QuickMapDeepnest orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.DEEPNEST);
        }

        private static void OnOpenQuickMapDirtmouth(On.GameMap.orig_QuickMapDirtmouth orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.TOWN);
        }

        private static void OnOpenQuickMapFogCanyon(On.GameMap.orig_QuickMapFogCanyon orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.FOG_CANYON);
        }

        private static void OnOpenQuickMapFungalWastes(On.GameMap.orig_QuickMapFungalWastes orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.WASTES);
        }

        private static void OnOpenQuickMapGreenpath(On.GameMap.orig_QuickMapGreenpath orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.GREEN_PATH);
        }

        private static void OnOpenQuickMapKingdomsEdge(On.GameMap.orig_QuickMapKingdomsEdge orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.OUTSKIRTS);
        }

        private static void OnOpenQuickMapQueensGardens(On.GameMap.orig_QuickMapQueensGardens orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.ROYAL_GARDENS);
        }

        private static void OnOpenQuickMapRestingGrounds(On.GameMap.orig_QuickMapRestingGrounds orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.RESTING_GROUNDS);
        }

        private static void OnOpenQuickMapWaterways(On.GameMap.orig_QuickMapWaterways orig, GameMap self)
        {
            orig(self);
            OpenQuickMapEvent(self, MapZone.WATERWAYS);
        }
    }
}
