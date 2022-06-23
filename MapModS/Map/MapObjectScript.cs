using MapModS.Data;
using MapModS.UI;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

namespace MapModS.Map
{
    // Controls the color of map objects. Does not control if active/inactive
    public class MapObjectScript : MonoBehaviour
    {
        public enum MapObjectType
        {
            None,
            RoomSprite,
            RoomText,
            AreaText,
            NextArea
        }

        public Vector4 origColor;
        public Vector4 origModdedColor = Vector4.negativeInfinity;
        public ColorSetting transitionColor;
        public bool highlight = false;
        public string sceneName;

        private Transform srTransform;
        private SpriteRenderer SR => srTransform.GetComponent<SpriteRenderer>();

        private Transform tmpTransform;
        private TextMeshPro TMP => tmpTransform.GetComponent<TextMeshPro>();
        public MapObjectType type = MapObjectType.None;

        public void Start()
        {
            sceneName = Utils.GetActualSceneName(transform.name);

            if (transform.parent.name == "MMS Custom Map Rooms")
            {
                tmpTransform = transform;

                if (TMP != null)
                {
                    type = MapObjectType.RoomText;
                    origColor = TMP.color;
                }
            }
            else if (sceneName != null
                //&& sceneName != "Fungus3_48"
                && transform.GetComponent<GrubPin>() == null)
            {
                srTransform = GetRendererTransform();

                if (SR != null)
                {
                    type = MapObjectType.RoomSprite;
                    origColor = SR.color;
                    SetOrigModdedColor();
                }
            }
            else if (transform.name.Contains("Area Name"))
            {
                tmpTransform = transform;

                if (TMP != null)
                {
                    type = MapObjectType.AreaText;
                    origColor = TMP.color;
                    SetOrigModdedColor();
                }
            }
            else if (transform.name.Contains("Next Area"))
            {
                foreach (Transform child in transform)
                {
                    if (child.name.Contains("Area Name"))
                    {
                        tmpTransform = child;
                    }
                    else if (child.name.Contains("Arrow"))
                    {
                        srTransform = child;
                    }
                }

                if (tmpTransform != null
                    && TMP != null
                    && srTransform != null
                    && SR != null)
                {
                    type = MapObjectType.NextArea;
                    origColor = SR.color;
                    SetOrigModdedColor();
                }
            }

            MapModS.Instance.Log("Name: " + transform.name);
            MapModS.Instance.Log("- Parent: " + transform.parent.name);
            MapModS.Instance.Log("- Type: " + type);
            MapModS.Instance.Log("- Scene Name: " + sceneName);

            if (type == MapObjectType.None)
            {
                Destroy(this);
            }
        }

        public void StartColorUpdate()
        {
            StopAllCoroutines();
            ApplyRoomColor();
            StartCoroutine(UpdateRoomColor());
        }

        public void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator UpdateRoomColor()
        {
            while (true)
            {
                if (GUI.worldMapOpen)
                {
                    ApplyRoomColor();
                }

                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
        
        public void ApplyStaticColor()
        {
            if (MapModS.LS.modEnabled)
            {
                if (!origModdedColor.Equals(Vector4.negativeInfinity))
                {
                    ApplyColor(origModdedColor);
                    return;
                }
            }

            ApplyColor(origColor);
        }

        private void ApplyRoomColor()
        {
            if (MapModS.LS.modEnabled)
            {
                if (TransitionData.TransitionModeActive())
                {
                    if (GUI.worldMapOpen && InfoPanels.selectedScene == sceneName)
                    {
                        ApplyColor(Colors.GetColor(ColorSetting.Room_Selected));
                    }
                    else
                    {
                        ApplyColor(Colors.GetColor(transitionColor));
                    }
                }
                else
                {
                    if (GUI.worldMapOpen && UI.Benchwarp.selectedBenchScene == sceneName)
                    {
                        ApplyColor(Colors.GetColor(ColorSetting.Room_Benchwarp_Selected));
                    }
                    else
                    {
                        if (!origModdedColor.Equals(Vector4.negativeInfinity))
                        {
                            ApplyColor(origModdedColor);
                        }
                        else
                        {
                            ApplyColor(origColor);
                        }
                    }
                }
            }
            else
            {
                ApplyColor(origColor);
            }
        }

        private Transform GetRendererTransform()
        {
            if (transform.name.Contains("White_Palace"))
            {
                return transform.Cast<Transform>()
                    .Where(r => r.name.Contains("RWP")).FirstOrDefault();
            }
            else if (transform.name.Contains("GG_Atrium"))
            {
                return transform.Cast<Transform>()
                    .Where(r => r.name.Contains("RGH")).FirstOrDefault();
            }
            else
            {
                return transform;
            }
        }

        private void SetOrigModdedColor()
        {
            if (transform.parent.name == "Ancient Basin"
                && (transform.name == "Area Name (2)" || transform.name == "Abyss_22"))
            {
                origModdedColor = Colors.GetColor(ColorSetting.Map_Ancient_Basin);
            }
            else if (type == MapObjectType.RoomSprite)
            {
                (string, Vector4) key = (transform.parent.name, origColor);

                if (Colors.subAreaMapColorKeys.ContainsKey(transform.name) && !Colors.subAreaMapColors.ContainsKey(key))
                {
                    Colors.subAreaMapColors.Add(key, Colors.subAreaMapColorKeys[transform.name]);
                }

                if (Colors.subAreaMapColors.ContainsKey(key))
                {
                    origModdedColor = Colors.GetColor(Colors.subAreaMapColors[key]);
                }
            }

            if (type == MapObjectType.NextArea)
            {
                origModdedColor = Colors.GetColorFromMapZone(tmpTransform.GetComponent<SetTextMeshProGameText>().convName);
            }

            if (type == MapObjectType.AreaText && !origModdedColor.Equals(Vector4.negativeInfinity))
            {
                origModdedColor.w = 1f;
            }
        }

        private void ApplyColor(Vector4 color)
        {
            if (type == MapObjectType.RoomSprite)
            {
                if (highlight)
                {
                    color.w = 1f;
                }
                SR.color = color;
            }
            else if (type == MapObjectType.RoomText)
            {
                if (highlight)
                {
                    color.w = 1f;
                }
                TMP.color = color;
            }
            else if (type == MapObjectType.AreaText)
            {
                color.w = 1f;
                TMP.color = color;
            }
            else if (type == MapObjectType.NextArea)
            {
                color.w = 1f;
                SR.color = color;
                TMP.color = color;
            }
        }
    }
}
