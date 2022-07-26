using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using HutongGames.PlayMaker;
using MapChanger.Map;
using MapChanger.UI;
using Modding;
using UnityEngine;
using Vasi;

namespace MapChanger
{
    public static class Events
    {
        public static event Action BeforeEnterGame;
        public static event Action AfterEnterGame;
        public static event Action BeforeQuitToMenu;
        public static event Action<GameObject> AfterSetGameMap;
        public static event Action<GameMap> BeforeOpenWorldMap;
        public static event Action<GameMap> AfterOpenWorldMap;
        public static event Action<GameMap, MapZone> BeforeOpenQuickMap;
        public static event Action<GameMap, MapZone> AfterOpenQuickMap;
        public static event Action<GameMap> BeforeCloseMap;

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
                AfterOpenQuickMapEvent(gameMap, mapZone);
                Finish();
            }
        }

        internal static readonly List<IMainHooks> HookModules = new()
        {
            new Tracker(),
            new VariableOverrides(),
            new BehaviourChanges(),
            new Hotkeys(),
            new MapUI(),
            new PauseMenu(),
            new MapObjectUpdater()
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

            foreach (Mod mod in ModHooks.GetAllMods())
            {
                if (mod is MapMod mapMod)
                {
                    HookModules.Add(mapMod);
                }
            }
        }

        private static void AfterStartNewGame(On.GameManager.orig_StartNewGame orig, GameManager self, bool permadeathMode, bool bossRushMode)
        {
            BeforeEnterGameEvent();
            orig(self, permadeathMode, bossRushMode);
            AfterEnterGameEvent();
        }

        private static void AfterContinueGame(On.GameManager.orig_ContinueGame orig, GameManager self)
        {
            BeforeEnterGameEvent();
            orig(self);
            AfterEnterGameEvent();
        }

        private static void BeforeEnterGameEvent()
        {
            //Settings.Initialize();

            try { BeforeEnterGame?.Invoke(); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }
        }

        private static void AfterEnterGameEvent()
        {
            // Load default/custom assets
            Colors.LoadCustomColors();

            foreach (IMainHooks hookModule in HookModules)
            {
                hookModule.OnEnterGame();
            }

            try { AfterEnterGame?.Invoke(); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }
        }

        private static IEnumerator QuitToMenuEvent(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        {
            foreach (IMainHooks hookModule in HookModules)
            {
                hookModule.OnQuitToMenu();
            }

            try { BeforeQuitToMenu?.Invoke(); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }

            return orig(self);
        }

        private static void SetGameMapEvent(On.GameManager.orig_SetGameMap orig, GameManager self, GameObject goMap)
        {
            orig(self, goMap);
            HookQuickMapCustom();

            try { AfterSetGameMap?.Invoke(goMap); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }

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
            States.WorldMapOpen = true;
            States.CurrentMapZone = MapZone.NONE;

            try { BeforeOpenWorldMap?.Invoke(self); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }

            orig(self);

            try { AfterOpenWorldMap?.Invoke(self); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }
        }

        private static void BeforeOpenQuickMapEvent(GameMap gameMap, MapZone mapZone)
        {
            States.QuickMapOpen = true;
            States.CurrentMapZone = mapZone;

            try { BeforeOpenQuickMap?.Invoke(gameMap, mapZone); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }
        }

        private static void AfterOpenQuickMapEvent(GameMap gameMap, MapZone mapZone)
        {
            try { AfterOpenQuickMap?.Invoke(gameMap, mapZone); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }
        }

        private static void CloseMapEvent(On.GameMap.orig_CloseQuickMap orig, GameMap self)
        {
            States.WorldMapOpen = false;
            States.QuickMapOpen = false;
            States.CurrentMapZone = MapZone.NONE;

            try { BeforeCloseMap?.Invoke(self); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }

            orig(self);
        }

        private static void OnOpenQuickMapAncientBasin(On.GameMap.orig_QuickMapAncientBasin orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.ABYSS);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.ABYSS);
        }

        private static void OnOpenQuickMapCity(On.GameMap.orig_QuickMapCity orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.CITY);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.CITY);
        }

        private static void OnOpenQuickMapCliffs(On.GameMap.orig_QuickMapCliffs orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.CLIFFS);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.CLIFFS);
        }

        private static void OnOpenQuickMapCrossroads(On.GameMap.orig_QuickMapCrossroads orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.CROSSROADS);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.CROSSROADS);
        }

        private static void OnOpenQuickMapCrystalPeak(On.GameMap.orig_QuickMapCrystalPeak orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.MINES);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.MINES);
        }

        private static void OnOpenQuickMapDeepnest(On.GameMap.orig_QuickMapDeepnest orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.DEEPNEST);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.DEEPNEST);
        }

        private static void OnOpenQuickMapDirtmouth(On.GameMap.orig_QuickMapDirtmouth orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.TOWN);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.TOWN);
        }

        private static void OnOpenQuickMapFogCanyon(On.GameMap.orig_QuickMapFogCanyon orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.FOG_CANYON);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.FOG_CANYON);
        }

        private static void OnOpenQuickMapFungalWastes(On.GameMap.orig_QuickMapFungalWastes orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.WASTES);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.WASTES);
        }

        private static void OnOpenQuickMapGreenpath(On.GameMap.orig_QuickMapGreenpath orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.GREEN_PATH);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.GREEN_PATH);
        }

        private static void OnOpenQuickMapKingdomsEdge(On.GameMap.orig_QuickMapKingdomsEdge orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.OUTSKIRTS);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.OUTSKIRTS);
        }

        private static void OnOpenQuickMapQueensGardens(On.GameMap.orig_QuickMapQueensGardens orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.ROYAL_GARDENS);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.ROYAL_GARDENS);
        }

        private static void OnOpenQuickMapRestingGrounds(On.GameMap.orig_QuickMapRestingGrounds orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.RESTING_GROUNDS);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.RESTING_GROUNDS);
        }

        private static void OnOpenQuickMapWaterways(On.GameMap.orig_QuickMapWaterways orig, GameMap self)
        {
            BeforeOpenQuickMapEvent(self, MapZone.WATERWAYS);
            orig(self);
            AfterOpenQuickMapEvent(self, MapZone.WATERWAYS);
        }
    }
}
