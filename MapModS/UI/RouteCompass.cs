using System.Collections.Generic;
using System.Linq;
using MapModS.Data;
using MapModS.Settings;
using Modding.Utils;
using UnityEngine;

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
        };

        public static void CreateRouteCompass()
        {
            if (compass != null && CompassC != null) CompassC.Destroy();

            if (Knight == null || GUIController.Instance == null) return;

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

            if (CompassC != null
                && TransitionText.selectedRoute.Any()
                && RandomizerMod.RandomizerData.Data.IsTransition(TransitionText.selectedRoute.First())
                && StringUtils.CurrentNormalScene() == RandomizerMod.RandomizerData.Data.GetTransitionDef(TransitionText.selectedRoute.First()).SceneName)
            {
                string transition = TransitionText.selectedRoute.First();
                string gate;

                if (specialDoors.ContainsKey(transition))
                {
                    gate = specialDoors[transition];
                }
                else
                {
                    gate = RandomizerMod.RandomizerData.Data.GetTransitionDef(transition).DoorName;
                }

                GameObject gateObject = UnityExtensions.FindGameObject(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), gate);

                if (gateObject != null)
                {
                    CompassC.trackedObjects = new() { gateObject };
                    compass.SetActive(true);
                    return;
                }

                GameObject gateObject2 = UnityExtensions.FindGameObject(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), "_Transition Gates/" + gate);

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
