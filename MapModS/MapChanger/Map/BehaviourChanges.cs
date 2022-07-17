using System;
using System.Collections.Generic;
using GlobalEnums;
using HutongGames.PlayMaker;
using UnityEngine;
using Vasi;
using UnityEngine.SceneManagement;

namespace MapChanger.Map
{
    /// <summary>
    /// Many changes to the map behaviour with respect to removing buggy base game behaviour,
    /// getting toggles to work, and QoL improvements.
    /// </summary>
    internal class BehaviourChanges : HookModule
    {
        private class MapKeyUpCheck : FsmStateAction
        {
            public override void OnEnter()
            {
                if (!Settings.MapModEnabled || !Settings.CurrentMode().DisableVanillaPins)
                {
                    MapKey?.gameObject.LocateMyFSM("Control")?.SendEvent("MAP KEY UP");
                }
                Finish();
            }
        }

        private class ActivateGoCheck : FsmStateAction
        {
            private readonly GameObject gameObject;

            internal ActivateGoCheck(GameObject gameObject)
            {
                this.gameObject = gameObject;
            }

            public override void OnEnter()
            {
                gameObject?.SetActive(!Settings.MapModEnabled || !Settings.CurrentMode().DisableVanillaPins);
                Finish();
            }
        }

        private class RoughMapCopy : MonoBehaviour
        {
            internal Sprite RoughMap { get; private set; }

            internal void AddSprite(Sprite sprite)
            {
                RoughMap = sprite;
            }
        }

        private const string MAP_MARKERS = "Map Markers";
        private const string PLACED_MARKERS_B = "placedMarkers_b";
        private const string PLACED_MARKERS_R = "placedMarkers_r";
        private const string PLACED_MARKERS_Y = "placedMarkers_y";
        private const string PLACED_MARKERS_W = "placedMarkers_w";

        internal static Transform MapKey => GameCameras.instance.hudCamera.transform
            .FindChildInHierarchy("Inventory")?.FindChildInHierarchy("Map Key");
        internal static Transform MarkerAction => GameCameras.instance.hudCamera.transform
            .FindChildInHierarchy("Inventory")?.FindChildInHierarchy("Map")?
            .FindChildInHierarchy("World Map")?.FindChildInHierarchy("Map Marker Action");

        internal override void Hook()
        {
            On.PlayMakerFSM.Start += ModifyFsms;
            On.RoughMapRoom.OnEnable += StoreRoughMapCopy;

            On.MapNextAreaDisplay.OnEnable += NextAreaOverride;
            On.GameMap.SetupMapMarkers += SetupMarkersOverride;
            On.GameMap.DisableMarkers += DisableMarkersOverride;

            On.GameMap.PositionCompass += SetDoorMapZoneEarly;
            On.GameMap.GetDoorMapZone += DoorMapZoneOverride;
            On.GameManager.GetCurrentMapZone += CurrentMapZoneOverride;

            On.GameMap.Update += ZoomFasterOnKeyboard;
            On.GameManager.UpdateGameMap += DisableUpdatedMapPrompt;

            //Events.AfterSetGameMap += MoveIconsForward;
            Events.AfterOpenWorldMap += IncreasePanningRange;
        }

        internal override void Unhook()
        {
            On.PlayMakerFSM.Start -= ModifyFsms;
            On.RoughMapRoom.OnEnable -= StoreRoughMapCopy;

            On.MapNextAreaDisplay.OnEnable -= NextAreaOverride;
            On.GameMap.SetupMapMarkers -= SetupMarkersOverride;
            On.GameMap.DisableMarkers -= DisableMarkersOverride;

            On.GameMap.PositionCompass -= SetDoorMapZoneEarly;
            On.GameMap.GetDoorMapZone -= DoorMapZoneOverride;
            On.GameManager.GetCurrentMapZone -= CurrentMapZoneOverride;

            On.GameMap.Update -= ZoomFasterOnKeyboard;
            On.GameManager.UpdateGameMap -= DisableUpdatedMapPrompt;

            //Events.AfterSetGameMap -= MoveIconsForward;
            Events.AfterOpenWorldMap -= IncreasePanningRange;
        }

        private void ModifyFsms(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self)
        {
            try
            {
                if (self.gameObject.name == "World Map" && self.FsmName == "UI Control"
                    && self.TryGetState("Zoomed In", out FsmState state))
                {
                    state.RemoveAction(1);
                    state.InsertAction(1, new MapKeyUpCheck());
                    state.RemoveAction(6);
                    state.InsertAction(6, new ActivateGoCheck(MarkerAction?.gameObject));
                }
            }
            catch (Exception e)
            {
                MapChangerMod.Instance.LogError(e);
            }

            orig(self);
        }

        private static void StoreRoughMapCopy(On.RoughMapRoom.orig_OnEnable orig, RoughMapRoom self)
        {
            if (self.GetComponent<SpriteRenderer>() is SpriteRenderer sr)
            {
                RoughMapCopy rmc = self.GetComponent<RoughMapCopy>();
                if (rmc is null)
                {
                    rmc = self.gameObject.AddComponent<RoughMapCopy>();
                    rmc.AddSprite(sr.sprite);
                }
                else
                {
                    sr.sprite = rmc.RoughMap;
                    self.fullSpriteDisplayed = false;
                }
            }

            orig(self);
        }

        private void NextAreaOverride(On.MapNextAreaDisplay.orig_OnEnable orig, MapNextAreaDisplay self)
        {
            // Do nothing
        }

        private static void SetupMarkersOverride(On.GameMap.orig_SetupMapMarkers orig, GameMap self)
        {
            if (Settings.MapModEnabled)
            {
                self.gameObject.Child(MAP_MARKERS).SetActive(false);
                return;
            }

            // Basically do the same stuff as the vanilla implementation, but avoid calling DisableMarkers()
            self.gameObject.Child(MAP_MARKERS).SetActive(true);
            for (int i = 0; i < PlayerData.instance.GetVariable<List<Vector3>>(PLACED_MARKERS_B).Count; i++)
            {
                self.mapMarkersBlue[i].SetActive(true);
                self.mapMarkersBlue[i].transform.localPosition = PlayerData.instance.GetVariable<List<Vector3>>(PLACED_MARKERS_B)[i];
            }
            for (int j = 0; j < PlayerData.instance.GetVariable<List<Vector3>>(PLACED_MARKERS_R).Count; j++)
            {
                self.mapMarkersRed[j].SetActive(true);
                self.mapMarkersRed[j].transform.localPosition = PlayerData.instance.GetVariable<List<Vector3>>(PLACED_MARKERS_R)[j];
            }
            for (int k = 0; k < PlayerData.instance.GetVariable<List<Vector3>>(PLACED_MARKERS_Y).Count; k++)
            {
                self.mapMarkersYellow[k].SetActive(true);
                self.mapMarkersYellow[k].transform.localPosition = PlayerData.instance.GetVariable<List<Vector3>>(PLACED_MARKERS_Y)[k];
            }
            for (int l = 0; l < PlayerData.instance.GetVariable<List<Vector3>>(PLACED_MARKERS_W).Count; l++)
            {
                self.mapMarkersWhite[l].SetActive(true);
                self.mapMarkersWhite[l].transform.localPosition = PlayerData.instance.GetVariable<List<Vector3>>(PLACED_MARKERS_W)[l];
            }
        }

        private static void DisableMarkersOverride(On.GameMap.orig_DisableMarkers orig, GameMap self)
        {
            self.gameObject.Child(MAP_MARKERS).SetActive(false);
        }

        // Fixes some null referencing
        private static void SetDoorMapZoneEarly(On.GameMap.orig_PositionCompass orig, GameMap self, bool posShade)
        {
            self.doorMapZone = self.GetDoorMapZone();
            orig(self, posShade);
        }

        // The following fixes loading the Quick Map for some of the special areas (like Ancestral Mound)
        private static string DoorMapZoneOverride(On.GameMap.orig_GetDoorMapZone orig, GameMap self)
        {
            if (!Settings.MapModEnabled) return orig(self);

            MapZone mapZone = Finder.GetCurrentMapZone();
            if (mapZone != MapZone.NONE)
            {
                return mapZone.ToString();
            }
            return orig(self);
        }

        private static string CurrentMapZoneOverride(On.GameManager.orig_GetCurrentMapZone orig, GameManager self)
        {
            if (!Settings.MapModEnabled) return orig(self);

            MapZone mapZone = Finder.GetCurrentMapZone();
            if (mapZone != MapZone.NONE)
            {
                return mapZone.ToString();
            }
            return orig(self);
        }

        private static void ZoomFasterOnKeyboard(On.GameMap.orig_Update orig, GameMap self)
        {
            if (Settings.MapModEnabled
                && self.canPan
                && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                if (InputHandler.Instance.inputActions.down.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x, self.transform.position.y + self.panSpeed * Time.deltaTime, self.transform.position.z);
                }
                if (InputHandler.Instance.inputActions.up.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x, self.transform.position.y - self.panSpeed * Time.deltaTime, self.transform.position.z);
                }
                if (InputHandler.Instance.inputActions.left.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x + self.panSpeed * Time.deltaTime, self.transform.position.y, self.transform.position.z);
                }
                if (InputHandler.Instance.inputActions.right.IsPressed)
                {
                    self.transform.position = new Vector3(self.transform.position.x - self.panSpeed * Time.deltaTime, self.transform.position.y, self.transform.position.z);
                }
            }

            orig(self);
        }

        private static bool DisableUpdatedMapPrompt(On.GameManager.orig_UpdateGameMap orig, GameManager self)
        {
            if (Settings.MapModEnabled)
            {
                return false;
            }

            return orig(self);
        }

        //TODO: these don't work
        //private static void MoveIconsForward(GameObject goMap)
        //{
        //    Transform dgPin = goMap.transform.Find("Dream_Gate_Pin").transform;
        //    dgPin.localPosition = new(dgPin.localPosition.x, dgPin.localPosition.y, -1.5f);
        //    Transform compass = goMap.transform.Find("Compass Icon").transform;
        //    compass.localPosition = new(dgPin.localPosition.x, dgPin.localPosition.y, -1.4f);
        //}

        private static void IncreasePanningRange(GameMap gameMap)
        {
            gameMap.panMinX = -29f;
            gameMap.panMaxX = 26f;
            gameMap.panMinY = -25f;
            gameMap.panMaxY = 20f;
        }

        //TODO: quill stuff - store mapped scenes in separate hashset?
    }
}
