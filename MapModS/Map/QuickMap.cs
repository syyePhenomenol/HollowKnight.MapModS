using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using MapModS.Data;
using MapModS.UI;
using Modding;
using System.Linq;
using TMPro;
using UnityEngine;
using Vasi;
using static MapModS.Map.MapRooms;

namespace MapModS.Map
{
    public static class QuickMap
    {
        public static void Hook()
        {
            On.MapNextAreaDisplay.OnEnable += MapNextAreaDisplay_OnEnable;

            On.GameMap.PositionCompass += GameMap_PositionCompass;
            On.GameManager.GetCurrentMapZone += GameManager_GetCurrentMapZone;
            On.GameMap.GetDoorMapZone += GameMap_GetDoorMapZone;

            On.GameMap.QuickMapAncientBasin += GameMap_QuickMapAncientBasin;
            On.GameMap.QuickMapCity += GameMap_QuickMapCity;
            On.GameMap.QuickMapCliffs += GameMap_QuickMapCliffs;
            On.GameMap.QuickMapCrossroads += GameMap_QuickMapCrossroads;
            On.GameMap.QuickMapCrystalPeak += GameMap_QuickMapCrystalPeak;
            On.GameMap.QuickMapDeepnest += GameMap_QuickMapDeepnest;
            On.GameMap.QuickMapDirtmouth += GameMap_QuickMapDirtmouth;
            On.GameMap.QuickMapFogCanyon += GameMap_QuickMapFogCanyon;
            On.GameMap.QuickMapFungalWastes += GameMap_QuickMapFungalWastes;
            On.GameMap.QuickMapGreenpath += GameMap_QuickMapGreenpath;
            On.GameMap.QuickMapKingdomsEdge += GameMap_QuickMapKingdomsEdge;
            On.GameMap.QuickMapQueensGardens += GameMap_QuickMapQueensGardens;
            On.GameMap.QuickMapRestingGrounds += GameMap_QuickMapRestingGrounds;
            On.GameMap.QuickMapWaterways += GameMap_QuickMapWaterways;

            On.GameManager.SetGameMap += GameManager_SetGameMap;
        }

        public static void Unhook()
        {
            On.MapNextAreaDisplay.OnEnable -= MapNextAreaDisplay_OnEnable;

            On.GameMap.PositionCompass -= GameMap_PositionCompass;
            On.GameManager.GetCurrentMapZone -= GameManager_GetCurrentMapZone;
            On.GameMap.GetDoorMapZone -= GameMap_GetDoorMapZone;

            On.GameMap.QuickMapAncientBasin -= GameMap_QuickMapAncientBasin;
            On.GameMap.QuickMapCity -= GameMap_QuickMapCity;
            On.GameMap.QuickMapCliffs -= GameMap_QuickMapCliffs;
            On.GameMap.QuickMapCrossroads -= GameMap_QuickMapCrossroads;
            On.GameMap.QuickMapCrystalPeak -= GameMap_QuickMapCrystalPeak;
            On.GameMap.QuickMapDeepnest -= GameMap_QuickMapDeepnest;
            On.GameMap.QuickMapDirtmouth -= GameMap_QuickMapDirtmouth;
            On.GameMap.QuickMapFogCanyon -= GameMap_QuickMapFogCanyon;
            On.GameMap.QuickMapFungalWastes -= GameMap_QuickMapFungalWastes;
            On.GameMap.QuickMapGreenpath -= GameMap_QuickMapGreenpath;
            On.GameMap.QuickMapKingdomsEdge -= GameMap_QuickMapKingdomsEdge;
            On.GameMap.QuickMapQueensGardens -= GameMap_QuickMapQueensGardens;
            On.GameMap.QuickMapRestingGrounds -= GameMap_QuickMapRestingGrounds;
            On.GameMap.QuickMapWaterways -= GameMap_QuickMapWaterways;

            On.GameManager.SetGameMap -= GameManager_SetGameMap;
        }

        // Don't show next map objects in Quick Map during Transition mode
        // Also set custom colors for these objects properly
        private static void MapNextAreaDisplay_OnEnable(On.MapNextAreaDisplay.orig_OnEnable orig, MapNextAreaDisplay self)
        {
            orig(self);

            Transform areaName = null;
            TextMeshPro tmp = null;
            Transform mapArrow = null;
            SpriteRenderer sr = null;

            foreach (Transform transform in self.transform)
            {
                if (transform.name.Contains("Area Name"))
                {
                    areaName = transform;
                    tmp = areaName.GetComponent<TextMeshPro>();
                }
                else if (transform.name.Contains("Arrow"))
                {
                    mapArrow = transform;
                    sr = mapArrow.GetComponent<SpriteRenderer>();
                }
            }

            if (tmp == null)
            {
                ReflectionHelper.CallMethod(self, "DeactivateChildren");
                return;
            }

            ExtraMapData emd = self.GetComponent<ExtraMapData>();

            if (emd == null)
            {
                emd = self.gameObject.AddComponent<ExtraMapData>();
                emd.sceneName = self.name + " Next Area";

                emd.origColor = tmp.color;

                if (sr != null)
                {
                    emd.origColor = sr.color;
                }

                emd.origCustomColor = Colors.GetColorFromMapZone(areaName.GetComponent<SetTextMeshProGameText>().convName);
            }

            if (TransitionData.TransitionModeActive())
            {
                ReflectionHelper.CallMethod(self, "DeactivateChildren");
                return;
            }

            if (MapModS.LS.modEnabled && !emd.origCustomColor.Equals(Vector4.negativeInfinity))
            {
                tmp.color = emd.origCustomColor;
                    
                if (sr != null)
                {
                    sr.color = emd.origCustomColor;
                }
            }
            else
            {
                tmp.color = emd.origColor;

                if (sr != null)
                {
                    sr.color = emd.origColor;
                }
            }
        }

        // Fixes some null referencing shenanigans
        private static void GameMap_PositionCompass(On.GameMap.orig_PositionCompass orig, GameMap self, bool posShade)
        {
            self.doorMapZone = self.GetDoorMapZone();

            orig(self, posShade);
        }

        // The following fixes loading the Quick Map for some of the special areas (like Ancestral Mound)
        private static string GameMap_GetDoorMapZone(On.GameMap.orig_GetDoorMapZone orig, GameMap self)
        {
            if (!MapModS.LS.modEnabled) return orig(self);

            MapZone mapZone = MainData.GetFixedMapZone();

            if (mapZone != MapZone.NONE)
            {
                return mapZone.ToString();
            }

            return orig(self);
        }

        private static string GameManager_GetCurrentMapZone(On.GameManager.orig_GetCurrentMapZone orig, GameManager self)
        {
            if (!MapModS.LS.modEnabled) return orig(self);

            MapZone mapZone = MainData.GetFixedMapZone();

            if (mapZone != MapZone.NONE)
            {
                return mapZone.ToString();
            }

            return orig(self);
        }

        // These are called every time we open the respective Quick Map
        private static void GameMap_QuickMapAncientBasin(On.GameMap.orig_QuickMapAncientBasin orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.ABYSS);
        }

        private static void GameMap_QuickMapCity(On.GameMap.orig_QuickMapCity orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.CITY);
            SetTitleColor();
        }

        private static void GameMap_QuickMapCliffs(On.GameMap.orig_QuickMapCliffs orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.CLIFFS);
            SetTitleColor();
        }

        private static void GameMap_QuickMapCrossroads(On.GameMap.orig_QuickMapCrossroads orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.CROSSROADS);
            SetTitleColor();
        }

        private static void GameMap_QuickMapCrystalPeak(On.GameMap.orig_QuickMapCrystalPeak orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.MINES);
            SetTitleColor();
        }

        private static void GameMap_QuickMapDeepnest(On.GameMap.orig_QuickMapDeepnest orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.DEEPNEST);
            SetTitleColor();
        }

        private static void GameMap_QuickMapDirtmouth(On.GameMap.orig_QuickMapDirtmouth orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.TOWN);
            SetTitleColor();
        }

        private static void GameMap_QuickMapFogCanyon(On.GameMap.orig_QuickMapFogCanyon orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.FOG_CANYON);
            SetTitleColor();
        }

        private static void GameMap_QuickMapFungalWastes(On.GameMap.orig_QuickMapFungalWastes orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.WASTES);
            SetTitleColor();
        }

        private static void GameMap_QuickMapGreenpath(On.GameMap.orig_QuickMapGreenpath orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.GREEN_PATH);
            SetTitleColor();
        }

        private static void GameMap_QuickMapKingdomsEdge(On.GameMap.orig_QuickMapKingdomsEdge orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.OUTSKIRTS);
            SetTitleColor();
        }

        private static void GameMap_QuickMapQueensGardens(On.GameMap.orig_QuickMapQueensGardens orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.ROYAL_GARDENS);
            SetTitleColor();
        }

        private static void GameMap_QuickMapRestingGrounds(On.GameMap.orig_QuickMapRestingGrounds orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.RESTING_GROUNDS);
            SetTitleColor();
        }

        private static void GameMap_QuickMapWaterways(On.GameMap.orig_QuickMapWaterways orig, GameMap self)
        {
            orig(self);

            WorldMap.UpdateMap(self, MapZone.WATERWAYS);
            SetTitleColor();
        }

        public static void SetTitleColor()
        {
            GameObject title = GameCameras.instance.hudCamera.gameObject?.Child("Quick Map")?.Child("Area Name");
            if (title == null) return;
            
            TextMeshPro tmp = title.GetComponent<TextMeshPro>();
            ExtraMapData emd = title.GetComponent<ExtraMapData>();

            if (emd == null)
            {
                emd = title.AddComponent<ExtraMapData>();
                emd.sceneName = "Quick Map Title";
                emd.origColor = tmp.color;
            }

            if (TransitionData.TransitionModeActive())
            {
                tmp.color = Colors.GetColor(ColorSetting.UI_Neutral);
            }
            else if (MapModS.LS.modEnabled)
            {
                tmp.color = Colors.GetColorFromMapZone(GameManager.instance.GetCurrentMapZone());
            }
            else
            {
                tmp.color = emd.origColor;
            }
        }

        private static void GameManager_SetGameMap(On.GameManager.orig_SetGameMap orig, GameManager self, GameObject go_gameMap)
        {
            orig(self, go_gameMap);

            GameObject quickMapGameObject = GameObject.Find("Quick Map");
            PlayMakerFSM quickMapFSM = quickMapGameObject.LocateMyFSM("Quick Map");

            // Replace all PlayerData boolNames with our own so we can show all Quick Maps,
            // without changing the existing PlayerData settings

            foreach (FsmState state in quickMapFSM.FsmStates)
            {
                if (Utils.IsFSMMapState(state.Name))
                {
                    string boolString = FsmUtil.GetAction<PlayerDataBoolTest>(state, 0).boolName.ToString();
                    FsmUtil.GetAction<PlayerDataBoolTest>(state, 0).boolName = "MMS_" + boolString;
                }
            }

            // Patch custom area quick map behaviour

            GameMap gameMap = go_gameMap.GetComponent<GameMap>();

            if (quickMapFSM.FsmStates.Any(state => state.Name == "WHITE_PALACE"))
            {
                MapModS.Instance.Log("AdditionalMaps WHITE_PALACE area detected");
                FsmUtil.AddAction(FsmUtil.GetState(quickMapFSM, "WHITE_PALACE"), new QuickMapCustomArea(MapZone.WHITE_PALACE, gameMap));
            }

            if (quickMapFSM.FsmStates.Any(state => state.Name == "GODS_GLORY"))
            {
                MapModS.Instance.Log("AdditionalMaps GODS_GLORY area detected");
                FsmUtil.AddAction(FsmUtil.GetState(quickMapFSM, "GODS_GLORY"), new QuickMapCustomArea(MapZone.GODS_GLORY, gameMap));
            }
        }
    }

    public class QuickMapCustomArea : FsmStateAction
    {
        private readonly MapZone _customMapZone;
        private readonly GameMap _GameMap;

        public QuickMapCustomArea(MapZone mapZone, GameMap gameMap)
        {
            _customMapZone = mapZone;
            _GameMap = gameMap;
        }

        public override void OnEnter()
        {
            if (!MapModS.LS.modEnabled)
            {
                Finish();
                return;
            }

            WorldMap.UpdateMap(_GameMap, _customMapZone);
            QuickMap.SetTitleColor();
            _GameMap.SetupMapMarkers();

            GUI.worldMapOpen = false;
            GUI.quickMapOpen = true;
            TransitionQuickMap.UpdateAll();

            Finish();
        }
    }
}