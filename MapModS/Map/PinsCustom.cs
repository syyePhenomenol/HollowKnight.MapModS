using GlobalEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MapModS.Data;
using MapModS.Settings;

namespace MapModS.Map
{
    public class PinsCustom : MonoBehaviour
    {
        private readonly Dictionary<PoolGroup, GameObject> _Groups = new();

        private readonly List<Pin> _pins = new();

        public void MakePins(GameMap gameMap)
        {
            DestroyPins();

            foreach (PinDef pinData in DataLoader.GetPinArray())
            {
                try
                {
                    MakePin(pinData, gameMap);
                }
                catch (Exception e)
                {
                    MapModS.Instance.LogError(e);
                }
            }
        }

        public void UpdatePins(MapZone mapZone)
        {
            foreach (Pin pin in _pins)
            {
                pin.UpdatePin(mapZone);
            }
        }

        // Called every time the map is opened
        public void RefreshGroups()
        {
            foreach (PoolGroup group in _Groups.Keys)
            {
                _Groups[group].SetActive(true);
                //_Groups[group].SetActive(MapModS.LS.GetOnFromGroup(group));
            }
        }

        public void ResizePins()
        {
            foreach (Pin pin in _pins)
            {
                ResizePin(pin.gameObject);
            }
        }

        public static void ResizePin(GameObject go)
        {
            float scale = MapModS.GS.PinSizeSetting switch
            {
                GlobalSettings.PinSize.small => 0.31f,
                GlobalSettings.PinSize.medium => 0.37f,
                GlobalSettings.PinSize.large => 0.42f,
                _ => throw new NotImplementedException()
            };

            go.transform.localScale = 1.45f * scale * new Vector2(1.0f, 1.0f);
        }

        public void DestroyPins()
        {
            foreach (Pin pin in _pins)
            {
                Destroy(pin.gameObject);
            }

            _pins.Clear();
        }

        private void MakePin(PinDef pinData, GameMap gameMap)
        {
            if (_pins.Any(pin => pin.PinData.name == pinData.name))
            {
                MapModS.Instance.LogWarn($"Duplicate pin found for group: {pinData.name} - Skipped.");
                return;
            }

            //if (pinData.additionalMaps == 1 && MapModS.AdditionalMapsInstalled
            //|| pinData.additionalMaps == 2 && !MapModS.AdditionalMapsInstalled)
            //{
            //    return;
            //}

            // Create new pin GameObject
            GameObject goPin = new($"pin_mapmod_{pinData.name}")
            {
                layer = 30
            };

            // Attach pin data to the GameObject
            Pin pin = goPin.AddComponent<Pin>();
            pin.SetPinData(pinData);
            _pins.Add(pin);

            // Attach sprite renderer to the GameObject
            SpriteRenderer sr = goPin.AddComponent<SpriteRenderer>();

            if (pinData.isShop)
            {
                sr.sprite = SpriteManager.GetSprite("pinShop");
            }
            else
            {
                sr.sprite = SpriteManager.GetSpriteFromPool(pinData.vanillaPool);
            }
            
            sr.sortingLayerName = "HUD";
            sr.size = new Vector2(1f, 1f);

            // Set pin transform (by pool)
            AssignGroup(goPin, pinData);

            string roomName;

            // Position the pin - if pinScene exists we set a different base offset
            if (MapModS.AdditionalMapsInstalled && pinData.pinSceneAM != null)
            {
                roomName = pinData.pinSceneAM;
            }
            else
            {
                roomName = pinData.pinScene ?? pinData.sceneName;
            }

            Vector3 vec = GetRoomPos(roomName, gameMap);
            vec.Scale(new Vector3(1.46f, 1.46f, 1));

            if (MapModS.AdditionalMapsInstalled && pinData.pinSceneAM != null)
            {
                vec += new Vector3(pinData.offsetXAM, pinData.offsetYAM, 0.0f);
            }
            else
            {
                vec += new Vector3(pinData.offsetX, pinData.offsetY, pinData.offsetZ);
            }

            goPin.transform.localPosition = new Vector3(vec.x, vec.y, vec.z - 0.01f);
        }

        private void AssignGroup(GameObject newPin, PinDef pinData)
        {
            if (!_Groups.ContainsKey(pinData.spoilerPool))
            {
                _Groups[pinData.spoilerPool] = new GameObject("PinGroup " + pinData.spoilerPool);
                _Groups[pinData.spoilerPool].transform.SetParent(transform);
                _Groups[pinData.spoilerPool].SetActive(true);
            }

            newPin.transform.SetParent(_Groups[pinData.spoilerPool].transform);

            //if (!_Groups.ContainsKey(pinData.vanillaPool))
            //{
            //    _Groups[pinData.vanillaPool] = new GameObject("PinGroup " + pinData.vanillaPool);
            //    _Groups[pinData.vanillaPool].transform.SetParent(transform);

            //    // Set all true for now (doesn't really affect any behaviour)
            //    _Groups[pinData.vanillaPool].SetActive(true);
            //}

            //newPin.transform.SetParent(_Groups[pinData.vanillaPool].transform);
        }

        private Vector3 GetRoomPos(string roomName, GameMap gameMap)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (roomObj.gameObject.name == roomName)
                    {
                        Vector3 roomVec = roomObj.transform.localPosition;
                        roomVec.Scale(areaObj.transform.localScale);
                        return areaObj.transform.localPosition + roomVec;
                    }
                }
            }

            MapModS.Instance.LogWarn($"{roomName} is not a valid room name!");
            return new Vector3(0, 0, 0);
        }

        protected void Start()
        {
            gameObject.SetActive(false);
        }
    }
}