using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MapModS.UI
{
    // All the following was modified from the GUI implementation of BenchwarpMod by homothety
    public class GUIController : MonoBehaviour
    {
        public Dictionary<string, Texture2D> Images = new();

        public static GUIController Instance;

        public static void Setup()
        {
            GameObject GUIObj = new("MapModS GUI");
            Instance = GUIObj.AddComponent<GUIController>();
            DontDestroyOnLoad(GUIObj);
            Instance.LoadResources();
        }

        public static void Unload()
        {
            if (Instance != null)
            {
                Instance.StopAllCoroutines();
                Destroy(Instance.gameObject);
            }
        }

        public void StartScripts()
        {
            StartCoroutine("UpdateSelectedScene");

            StartCoroutine("UpdateSelectedPin");
        }

        public void Update()
        {
            try
            {
                TransitionPersistent.Update();
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
                TransitionPersistent.UpdateSelectedScene();
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