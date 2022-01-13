using GlobalEnums;
using MapModS.Data;
using MapModS.Settings;
using System;
using UnityEngine;

namespace MapModS.Map
{
    internal class Pin : MonoBehaviour
    {
        public PinDef PinData { get; private set; } = null;
        public SpriteRenderer SR => gameObject.GetComponent<SpriteRenderer>();

        private readonly Color _inactiveColor = Color.gray;
        private readonly Color _previewedColor = Color.yellow;
        private Color _origColor;

        public void SetPinData(PinDef pd)
        {
            PinData = pd;
            _origColor = SR.color;
        }

        public void UpdatePin(MapZone mapZone)
        {
            try
            {
                ShowBasedOnMap(mapZone);
                HideIfFound();
            }
            catch (Exception e)
            {
                MapModS.Instance.LogError(message: $"Failed to update pin! ID: {PinData.name}\n{e}");
            }
        }

        // Hides or shows the pin depending on the state of the map (NONE is World Map)
        private void ShowBasedOnMap(MapZone mapZone)
        {
            if (mapZone == MapZone.NONE)
            {
                if (MapModS.LS.mapMode != MapMode.PinsOverMap)
                {
                    gameObject.SetActive(true); return;
                }

                if (MapModS.AdditionalMapsInstalled)
                {
                    if (PinData.pinSceneAM != null)
                    {
                        if (SettingsUtil.GetMapSetting(PinData.mapZoneAM))
                        {
                            gameObject.SetActive(true); return;
                        }

                        gameObject.SetActive(false); return;
                    }
                    else if (SettingsUtil.GetMapSetting(PinData.mapZone))
                    {
                        gameObject.SetActive(true); return;
                    }
                }

                if (SettingsUtil.GetMapSetting(PinData.mapZone))
                {
                    gameObject.SetActive(true); return;
                }
            }

            if (PinData.pinSceneAM != null && mapZone == PinData.mapZoneAM)
            {
                gameObject.SetActive(true); return;
            }

            if (mapZone == PinData.mapZone)
            {
                gameObject.SetActive(true); return;
            }

            gameObject.SetActive(false);
        }

        private void HideIfFound()
        {
            //if (PinData.spoilerItem != null)
            //{
            //    if (RandomizerMod.RandomizerMod.RS.TrackerData.ob
            //    {
            //        gameObject.SetActive(false);
            //        return;
            //    }

            //    return;
            //}

            if (RandomizerMod.RandomizerMod.RS.TrackerData.clearedLocations.Contains(PinData.name))
            {
                gameObject.SetActive(false);
                return;
            }

            if (PinData.vanillaPool != PinData.spoilerPool) return;

            // For non-randomized items

            if (PinData.pdBool != null)
            {
                if (PlayerData.instance.GetBool(PinData.pdBool))
                {
                    gameObject.SetActive(false); return;
                }
            }

            if (PinData.pdInt != null)
            {
                if (PlayerData.instance.GetInt(PinData.pdInt) >= PinData.pdIntValue)
                {
                    gameObject.SetActive(false); return;
                }
            }

            if (PinData.vanillaPool == PoolGroup.WhisperingRoots)
            {
                if (PlayerData.instance.scenesEncounteredDreamPlantC.Contains(PinData.sceneName))
                {
                    gameObject.SetActive(false); return;
                }
            }

            if (PinData.vanillaPool == PoolGroup.Grubs)
            {
                if (PlayerData.instance.scenesGrubRescued.Contains(PinData.sceneName))
                {
                    gameObject.SetActive(false); return;
                }
            }

            if (PinData.vanillaPool == PoolGroup.GrimmkinFlames)
            {
                if (PlayerData.instance.scenesFlameCollected.Contains(PinData.sceneName))
                {
                    gameObject.SetActive(false); return;
                }
            }

            if (MapModS.LS.ObtainedVanillaItems.ContainsKey(PinData.objectName + PinData.sceneName))
            {
                gameObject.SetActive(false);
            }
        }

        public void SetSizeAndColor()
        {
            float scale = MapModS.GS.pinSize switch
            {
                PinSize.Small => 0.31f,
                PinSize.Medium => 0.37f,
                PinSize.Large => 0.42f,
                _ => throw new NotImplementedException()
            };

            transform.localScale = 1.45f * scale * new Vector2(1.0f, 1.0f);

            if (RandomizerMod.RandomizerMod.RS.TrackerData.previewedLocations.Contains(PinData.name))
            {
                SR.color = _previewedColor; return;
            }

            if (RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableLocations.Contains(PinData.name))
            {
                SR.color = _origColor;
            }
            else
            {
                // Non-randomized items also fall here
                transform.localScale = 0.7f * transform.localScale;
                SR.color = _inactiveColor;
            }
        }
    }
}