using MapModS.Map;
using UnityEngine;
using System.Collections;

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
			if (GameManager.instance == null) return;

			if (!GameManager.instance.IsGameplayScene() && !GameManager.instance.IsGamePaused()) return;

			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				if (Input.GetKeyDown(KeyCode.M))
                {
                    PauseMenu.EnableClicked("");
                }

                if (MapModS.LS.ModEnabled)
				{
					if (Input.GetKeyDown(KeyCode.Alpha1))
					{
						PauseMenu.SpoilersClicked("");
					}

					if (Input.GetKeyDown(KeyCode.Alpha2))
					{
						PauseMenu.StyleClicked("");
					}

					if (Input.GetKeyDown(KeyCode.Alpha3))
					{
						PauseMenu.RandomizedClicked("");
					}

					if (Input.GetKeyDown(KeyCode.Alpha4))
					{
						PauseMenu.OthersClicked("");
					}
				}
			}
            //Used for various debugging tasks
            //if (Input.GetKeyDown(KeyCode.O)) {
            //	//MapModS.ReloadGameMapPins();
            //	MapModS.GetAllActiveObjects();
            //}
		}
	}
}