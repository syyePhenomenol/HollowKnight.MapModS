using System.Linq;
using MapModS.Data;
using MapModS.Settings;
using Modding.Utils;
using UnityEngine;
using PD = MapModS.Data.PathfinderData;
using SM = UnityEngine.SceneManagement.SceneManager;
using TP = MapModS.UI.TransitionPersistent;

namespace MapModS.UI
{
    internal class RouteCompass
    {
        private static GameObject compass;
        private static DirectionalCompass CompassC => compass.GetComponent<DirectionalCompass>();
        private static GameObject Knight => HeroController.instance?.gameObject;

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

            if (CompassC != null && TP.selectedRoute.Any())
            {
                string transition = TP.selectedRoute.First();
                string scene = PD.GetScene(TP.selectedRoute.First());
                string gate = "";

                if (Utils.CurrentScene() == scene)
                {
                    if (PD.doorObjectsByTransition.ContainsKey(transition))
                    {
                        gate = PD.doorObjectsByTransition[transition];
                    }
                    else if (TransitionData.IsInTransitionLookup(transition))
                    {
                        gate = TransitionData.GetTransitionDoor(transition);
                    }
                }
                else if ((transition.IsStagTransition() || transition.IsTramTransition())
                    && PD.doorObjectsByScene.ContainsKey(Utils.CurrentScene()))
                {
                    gate = PD.doorObjectsByScene[Utils.CurrentScene()];
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
            return (MapModS.LS.modEnabled
                && (MapModS.LS.mapMode == MapMode.TransitionRando
                    || MapModS.LS.mapMode == MapMode.TransitionRandoAlt)
                && MapModS.GS.routeCompassEnabled);
        }
    }
}
