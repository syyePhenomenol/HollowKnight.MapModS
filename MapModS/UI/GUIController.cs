using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace MapModS.UI
{
    // All the following was modified from the GUI implementation of BenchwarpMod by homothetyhk
    public class GUIController : MonoBehaviour
    {
        public Dictionary<string, Texture2D> Images = new();

        private static GUIController _instance;

        private GameObject _pauseCanvas;
        private GameObject _mapCanvas;
        private GameObject _transitionCanvas;
        private GameObject _lookupCanvas;

        public static GUIController Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<GUIController>();

                if (_instance != null) return _instance;

                MapModS.Instance.LogWarn("Couldn't find GUIController");

                GameObject GUIObj = new();
                _instance = GUIObj.AddComponent<GUIController>();
                DontDestroyOnLoad(GUIObj);

                return _instance;
            }
        }

        public Font TrajanBold { get; private set; }
        public Font TrajanNormal { get; private set; }
        public Font Perpetua { get; private set; }
        private Font Arial { get; set; }

        public static void Setup()
        {
            GameObject GUIObj = new("MapModS GUI");
            _instance = GUIObj.AddComponent<GUIController>();
            DontDestroyOnLoad(GUIObj);
        }

        public static void Unload()
        {
            if (_instance != null)
            {
                _instance.StopAllCoroutines();

                Destroy(_instance._pauseCanvas);
                Destroy(_instance._mapCanvas);
                Destroy(_instance._transitionCanvas);
                Destroy(_instance._lookupCanvas);
                Destroy(_instance.gameObject);
            }
        }

        public void BuildMenus()
        {
            LoadResources();

            _pauseCanvas = new GameObject();
            _pauseCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler pauseScaler = _pauseCanvas.AddComponent<CanvasScaler>();
            pauseScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            pauseScaler.referenceResolution = new Vector2(1920f, 1080f);
            _pauseCanvas.AddComponent<GraphicRaycaster>();

            PauseMenu.BuildMenu(_pauseCanvas);

            DontDestroyOnLoad(_pauseCanvas);

            _mapCanvas = new GameObject();
            _mapCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler mapScaler = _mapCanvas.AddComponent<CanvasScaler>();
            mapScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            mapScaler.referenceResolution = new Vector2(1920f, 1080f);

            MapText.BuildText(_mapCanvas);

            DontDestroyOnLoad(_mapCanvas);

            _mapCanvas.SetActive(false);

            _transitionCanvas = new GameObject();
            _transitionCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler transitionScaler = _transitionCanvas.AddComponent<CanvasScaler>();
            transitionScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            transitionScaler.referenceResolution = new Vector2(1920f, 1080f);

            TransitionText.BuildText(_transitionCanvas);

            DontDestroyOnLoad(_transitionCanvas);

            _transitionCanvas.SetActive(true);

            TransitionText.Initialize();
            StartCoroutine("UpdateSelectedScene");

            _lookupCanvas = new GameObject();
            _lookupCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler lookupScaler = _lookupCanvas.AddComponent<CanvasScaler>();
            lookupScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            lookupScaler.referenceResolution = new Vector2(1920f, 1080f);

            LookupText.BuildText(_lookupCanvas);

            DontDestroyOnLoad(_lookupCanvas);

            _lookupCanvas.SetActive(false);

            LookupText.Initialize();
            StartCoroutine("UpdateSelectedPin");
        }

        public void Update()
        {
            try
            {
                PauseMenu.Update();
                TransitionText.Update();
                LookupText.Update();
            }
            catch (Exception e)
            {
                MapModS.Instance.LogError(e);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Member is actually used")]
        IEnumerator UpdateSelectedScene()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                TransitionText.UpdateSelectedScene();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Member is actually used")]
        IEnumerator UpdateSelectedPin()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                LookupText.UpdateSelectedPinCoroutine();
            }
        }

        private void LoadResources()
        {
            TrajanBold = Modding.CanvasUtil.TrajanBold;
            TrajanNormal = Modding.CanvasUtil.TrajanNormal;
            Perpetua = Modding.CanvasUtil.GetFont("Perpetua");

            try
            {
                Arial = Font.CreateDynamicFontFromOSFont
                (
                    Font.GetOSInstalledFontNames().First(x => x.ToLower().Contains("arial")),
                    13
                );
            }
            catch
            {
                MapModS.Instance.LogWarn("Unable to find Arial! Using Perpetua.");
                Arial = Modding.CanvasUtil.GetFont("Perpetua");
            }

            if (TrajanBold == null || TrajanNormal == null || Arial == null)
            {
                MapModS.Instance.LogError("Could not find game fonts");
            }

            Assembly asm = Assembly.GetExecutingAssembly();

            foreach (string res in asm.GetManifestResourceNames())
            {
                if (!res.StartsWith("MapModS.Resources.GUI.")) continue;

                try
                {
                    using Stream imageStream = asm.GetManifestResourceStream(res);
                    byte[] buffer = new byte[imageStream.Length];
                    imageStream.Read(buffer, 0, buffer.Length);

                    Texture2D tex = new(1, 1);
                    tex.LoadImage(buffer.ToArray());

                    string[] split = res.Split('.');
                    string internalName = split[split.Length - 2];

                    Images.Add(internalName, tex);
                }
                catch (Exception e)
                {
                    MapModS.Instance.LogError("Failed to load image: " + res + "\n" + e);
                }
            }
        }
    }
}