using MapModS.Data;
using MapModS.Map;
using UnityEngine;

namespace MapModS.UI
{
    // This class handles hotkey behaviour
    internal class InputListener : MonoBehaviour
    {
        private static GameObject _instance_GO = null;

        public static void InstantiateSingleton()
        {
            if (_instance_GO == null)
            {
                _instance_GO = GameObject.Find("RandoMapInputListener");
                if (_instance_GO == null)
                {
                    MapModS.Instance.Log("Adding Input Listener.");
                    _instance_GO = new GameObject("RandoMapInputListener");
                    _instance_GO.AddComponent<InputListener>();
                    DontDestroyOnLoad(_instance_GO);
                }
            }
        }

        public static void DestroySingleton()
        {
            if (_instance_GO != null)
            {
                Destroy(_instance_GO);
            }
        }

        protected void Update()
        {
            if (GameManager.instance == null || GameManager.instance.gameMap == null) return;

            if (!GameManager.instance.IsGameplayScene() && !GameManager.instance.IsGamePaused()) return;

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    PauseMenu.EnableClicked("Enable");
                }

                if (MapModS.LS.ModEnabled)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        PauseMenu.SpoilersClicked("Spoilers");
                    }

                    if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        PauseMenu.RandomizedClicked("Randomized");
                    }

                    if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        PauseMenu.OthersClicked("Others");
                    }

                    if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        PauseMenu.StyleClicked("Style");
                    }

                    if (Input.GetKeyDown(KeyCode.Alpha5))
                    {
                        PauseMenu.SizeClicked("Size");
                    }

                    // For debugging pins
                    //if (Input.GetKeyDown(KeyCode.R))
                    //{
                    //    DataLoader.LoadNewPinDef();
                    //    WorldMap.CustomPins.ReadjustPinPostiions();
                    //}
                }
            }
        }
    }
}