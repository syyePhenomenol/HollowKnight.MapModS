using System.Collections.Generic;
using System.Linq;
using MapModS.Data;
using MapModS.Settings;
using Modding.Utils;
using UnityEngine;
using PD = MapModS.Data.PathfinderData;
using SM = UnityEngine.SceneManagement.SceneManager;

namespace MapModS.UI
{
    internal class RouteCompass
    {
        private static GameObject compass;
        private static DirectionalCompass CompassC => compass.GetComponent<DirectionalCompass>();
        private static GameObject Knight => HeroController.instance?.gameObject;

        private static readonly Dictionary<string, string> specialDoors = new()
        {
            { "Town[door_station]", "_Props/Stag_station/open/door_station" },
            { "Town[door_sly]", "_Props/Sly_shop/open/door_sly" },
            { "Town[door_mapper]", "_Props/Mappers_house/open/door_mapper" },
            { "Town[door_jiji]", "Jiji Door/door_jiji" },
            { "Town[door_bretta]", "bretta_house/open/door_bretta" },
            { "Crossroads_04[door_Mender_House]", "_Transition Gates/Mender Door/door_Mender_House" },
            { "Ruins2_04[door_Ruin_Elevator]", "Bathhouse Door/door_Ruin_Elevator" },
            { "Fungus1_07[top1]", "Missing Prefab/Missing Prefab (Dummy)/top1" },
            { "Fungus1_07[left1]", "Missing Prefab/Missing Prefab (Dummy)/left1" },
            { "Fungus1_11[bot1]", "Missing Prefab/Missing Prefab (Dummy)/bot1" },
            { "Fungus1_11[top1]", "Missing Prefab/Missing Prefab (Dummy)/top1" },
            { "Fungus1_11[left1]", "Missing Prefab/Missing Prefab (Dummy)/left1" },

            { "Left_Elevator_Up", "elev_main/Lift Call Lever" },
            { "Left_Elevator_Down", "_Scenery/elev_main/Lift Call Lever" },
            { "Right_Elevator_Up", "elev_main/Lift Call Lever" },
            { "Right_Elevator_Down", "elev_main/Lift Call Lever" },

            { "Abyss_05[warp]", "Dusk Knight/Shield" },
            { "White_Palace_11[warp]", "doorWarp" },
            { "White_Palace_03_hub[warp]", "doorWarp" },
            { "White_Palace_20[warp]", "End Scene" }
        };

        private static readonly Dictionary<string, string> doorsByScene = new()
        {
            { "Room_Town_Stag_Station", "Station Bell/Bell" },
            { "Crossroads_47", "_Scenery/Station Bell/Bell" },
            { "Fungus1_16_alt", "Station Bell/Bell" },
            { "Fungus2_02", "Station Bell/Bell" },
            { "Deepnest_09", "Station Bell/Bell" },
            { "Abyss_22", "Station Bell/Bell" },
            { "Ruins1_29", "Station Bell/Bell" },
            { "Ruins2_08", "Station Bell/Bell" },
            { "RestingGrounds_09", "Station Bell Lever/Bell/Bell" },
            { "Fungus3_40", "Station Bell/Bell" },
            { "Crossroads_46", "Tram Call Box" },
            { "Crossroads_46b", "Tram Call Box" },
            { "Abyss_03_b", "Tram Call Box" },
            { "Abyss_03" , "Tram Call Box" },
            { "Abyss_03_c", "Tram Call Box" }
        };

        public static void CreateRouteCompass()
        {
            if (compass != null && CompassC != null) CompassC.Destroy();

            if (Knight == null
                || GUIController.Instance == null
                || GameManager.instance.IsNonGameplayScene()) return;

            Texture2D tex = GUIController.Instance.Images["arrow"];

            Sprite arrow = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            compass = DirectionalCompass.Create
            (
                "Route Compass", // name
                Knight, // parent entity
                arrow, // sprite
                new Vector4(255f, 255f, 255f, 210f) / 255f, // color
                1.5f, // radius
                2.0f, // scale
                IsCompassEnabled, // bool condition
                false, // lerp
                0.5f // lerp duration
            );

            compass.SetActive(false);
        }

        public static void UpdateCompass()
        {
            if (compass == null) return;

            if (CompassC != null && TransitionPersistent.selectedRoute.Any())
            {
                string transition = TransitionPersistent.selectedRoute.First();
                string scene = PD.GetScene(TransitionPersistent.selectedRoute.First());
                string gate = "";

                if (Utils.CurrentScene() == scene)
                {
                    if (specialDoors.ContainsKey(transition))
                    {
                        gate = specialDoors[transition];
                    }
                    else if (TransitionData.IsInTransitionLookup(transition))
                    {
                        gate = TransitionData.GetTransitionDoor(transition);
                    }
                }
                else if ((transition.IsStagTransition() || transition.IsTramTransition())
                    && doorsByScene.ContainsKey(Utils.CurrentScene()))
                {
                    gate = doorsByScene[Utils.CurrentScene()];
                }

                if (gate == "")
                {
                    compass.SetActive(false);
                    return;
                }

                GameObject gateObject = UnityExtensions.FindGameObject(SM.GetActiveScene(), gate);

                if (gateObject != null)
                {
                    CompassC.trackedObjects = new() { gateObject };
                    compass.SetActive(true);
                    return;
                }

                GameObject gateObject2 = UnityExtensions.FindGameObject(SM.GetActiveScene(), "_Transition Gates/" + gate);

                if (gateObject2 != null)
                {
                    CompassC.trackedObjects = new() { gateObject2 };
                    compass.SetActive(true);
                }
            }
            else
            {
                compass.SetActive(false);
            }
        }

        public static bool IsCompassEnabled()
        {
            return (MapModS.LS.ModEnabled
                && (MapModS.LS.mapMode == MapMode.TransitionRando
                    || MapModS.LS.mapMode == MapMode.TransitionRandoAlt)
                && MapModS.GS.routeCompassEnabled);
        }
    }
}
