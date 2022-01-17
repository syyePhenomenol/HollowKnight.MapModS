using MapModS.CanvasUtil;
using MapModS.Data;
using MapModS.Map;
using MapModS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MapModS.UI
{
    internal class TransitionText
    {
        public static GameObject Canvas;

        public static bool LockToggleEnable;

        private static CanvasPanel _instructionPanel;
        private static CanvasPanel _routePanel;
        private static Camera camera => Canvas?.GetComponent<Camera>();

        public static TransitionHelper th;
        public static string centeredScene = "";

        public static void Show()
        {
            if (Canvas == null) return;

            Canvas.SetActive(true);
            LockToggleEnable = false;
            RebuildText();
        }

        public static void Hide()
        {
            if (Canvas == null) return;

            Canvas.SetActive(false);
            LockToggleEnable = false;
        }

        public static void BuildText(GameObject _canvas)
        {
            Canvas = _canvas;
            _instructionPanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(10f, 40f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _instructionPanel.AddText("Instruction", "Display instruction here " + centeredScene, new Vector2(430f, 0f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);

            _routePanel = new CanvasPanel
                (_canvas, GUIController.Instance.Images["ButtonsMenuBG"], new Vector2(10f, 40f), new Vector2(1346f, 0f), new Rect(0f, 0f, 0f, 0f));
            _routePanel.AddText("Route", "Display route here", new Vector2(430f, 40f), Vector2.zero, GUIController.Instance.TrajanNormal, 14);

            _instructionPanel.SetActive(false, false);
            _routePanel.SetActive(false, false);

            SetTexts();
        }

        public static void RebuildText()
        {
            _instructionPanel.Destroy();
            _routePanel.Destroy();

            BuildText(Canvas);
        }

        public static void SetTexts()
        {
            if (GameManager.instance.gameMap == null) return;

            _instructionPanel.SetActive(true, false);
            _routePanel.SetActive(true, false);

            _instructionPanel.SetActive(!LockToggleEnable && MapModS.LS.ModEnabled
                && RandomizerMod.RandomizerMod.RS.GenerationSettings.TransitionSettings.Mode != RandomizerMod.Settings.TransitionSettings.TransitionMode.None
                && MapModS.LS.mapMode == MapMode.TransitionRando, false);
            _routePanel.SetActive(!LockToggleEnable && MapModS.LS.ModEnabled
                && RandomizerMod.RandomizerMod.RS.GenerationSettings.TransitionSettings.Mode != RandomizerMod.Settings.TransitionSettings.TransitionMode.None
                && MapModS.LS.mapMode == MapMode.TransitionRando, false);

            //SetSpoilers();
            //SetStyle();
            //SetRandomized();
            //SetOthers(); 
            //SetSize();
            //SetRefresh();
        }

        // Called every frame
        public static void Update()
        {
            if (_instructionPanel == null
                || _routePanel == null
                || HeroController.instance == null
                || GameManager.instance.IsGamePaused())
            {
                return;
            }

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100.0f))
            {
                MapModS.Instance.Log("hit detected");
                centeredScene = hit.transform.name;
            }

            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 100.0f, Color.yellow);

            //Ray ray = camera.ViewportPointToRay(new Vector3(960f, 540f, 0));



            //if (Physics.Raycast(ray, out hit))
            //{
            //    MapModS.Instance.Log("hit detected");
            //    centeredScene = hit.transform.name;
            //}

            //RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)));

            //foreach (RaycastHit hit in hits)
            //{
            //    centeredScene = hit.transform.name;
            //    break;
            //    //if (hit.transform.parent.transform.parent.GetComponent<GameMap>() != null)
            //    //{
            //    //    centeredScene = hit.transform.name;
            //    //}
            //}

            _instructionPanel.GetText("Instruction").UpdateText("Display instruction here " + centeredScene);
            // SetTexts();

            //if (!_mapControlPanel.Active)
            //{
            //    _mapControlPanel.Destroy();
            //    BuildMenu(Canvas);
            //}
        }
    }
}
