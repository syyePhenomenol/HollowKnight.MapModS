using System;
using GlobalEnums;
using TMPro;
using UnityEngine;

namespace MapChanger.MonoBehaviours
{
    public class QuickMapTitle : ColoredMapObject
    {
        public static QuickMapTitle Instance { get; private set; }

        private TextMeshPro tmp;
        public override Vector4 Color
        {
            get => tmp.color;
            set
            {
                tmp.color = value;
            }
        }

        public override void Initialize()
        {
            tmp = GetComponent<TextMeshPro>();
            OrigColor = tmp.color;

            gameObject.SetActive(true);

            Instance = this;

            Events.BeforeOpenQuickMap += OnOpenQuickMap;
        }

        private void OnOpenQuickMap(GameMap gameMap, MapZone mapZone)
        {
            UpdateColor(mapZone);
        }

        public void UpdateColor(MapZone mapZone)
        {
            if (!Settings.MapModEnabled)
            {
                ResetColor();
                return;
            }

            try { Settings.CurrentMode().OnQuickMapTitleUpdateColor?.Invoke(this, mapZone); }
            catch (Exception e) { MapChangerMod.Instance.LogError(e); }
        }

        public void OnDestroy()
        {
            Events.BeforeOpenQuickMap -= OnOpenQuickMap;
        }
    }
}
