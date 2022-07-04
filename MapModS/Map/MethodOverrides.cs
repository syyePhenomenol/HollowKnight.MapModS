using GlobalEnums;
using System.Collections.Generic;
using UnityEngine;
using Vasi;

namespace MapModS.Map
{
    internal class MethodOverrides : HookModule
    {
        internal override void Hook()
        {
            On.GameMap.SetupMap += SetupMapOverride;
            On.GameMap.SetupMapMarkers += SetupMarkersOverride;
            On.GameMap.DisableMarkers += DisableMarkersOverride;

            On.GameMap.PositionCompass += SetDoorMapZoneEarly;
            On.GameMap.GetDoorMapZone += DoorMapZoneOverride;
            On.GameManager.GetCurrentMapZone += CurrentMapZoneOverride;
        }

        internal override void Unhook()
        {
            On.GameMap.SetupMap -= SetupMapOverride;
            On.GameMap.SetupMapMarkers -= SetupMarkersOverride;
            On.GameMap.DisableMarkers -= DisableMarkersOverride;

            On.GameMap.PositionCompass -= SetDoorMapZoneEarly;
            On.GameMap.GetDoorMapZone -= DoorMapZoneOverride;
            On.GameManager.GetCurrentMapZone -= CurrentMapZoneOverride;
        }

        private static void SetupMarkersOverride(On.GameMap.orig_SetupMapMarkers orig, GameMap self)
        {
            if (MapModS.LS.ModEnabled)
            {
                self.gameObject.Child("Map Markers").SetActive(false);
                return;
            }

            // Basically do the same stuff as the vanilla implementation, but avoid calling DisableMarkers()
            self.gameObject.Child("Map Markers").SetActive(true);
            for (int i = 0; i < PlayerData.instance.GetVariable<List<Vector3>>("placedMarkers_b").Count; i++)
            {
                self.mapMarkersBlue[i].SetActive(true);
                self.mapMarkersBlue[i].transform.localPosition = PlayerData.instance.GetVariable<List<Vector3>>("placedMarkers_b")[i];
            }
            for (int j = 0; j < PlayerData.instance.GetVariable<List<Vector3>>("placedMarkers_r").Count; j++)
            {
                self.mapMarkersRed[j].SetActive(true);
                self.mapMarkersRed[j].transform.localPosition = PlayerData.instance.GetVariable<List<Vector3>>("placedMarkers_r")[j];
            }
            for (int k = 0; k < PlayerData.instance.GetVariable<List<Vector3>>("placedMarkers_y").Count; k++)
            {
                self.mapMarkersYellow[k].SetActive(true);
                self.mapMarkersYellow[k].transform.localPosition = PlayerData.instance.GetVariable<List<Vector3>>("placedMarkers_y")[k];
            }
            for (int l = 0; l < PlayerData.instance.GetVariable<List<Vector3>>("placedMarkers_w").Count; l++)
            {
                self.mapMarkersWhite[l].SetActive(true);
                self.mapMarkersWhite[l].transform.localPosition = PlayerData.instance.GetVariable<List<Vector3>>("placedMarkers_w")[l];
            }
        }

        private static void DisableMarkersOverride(On.GameMap.orig_DisableMarkers orig, GameMap self)
        {
            self.gameObject.Child("Map Markers").SetActive(false);
        }

        private static void SetupMapOverride(On.GameMap.orig_SetupMap orig, GameMap self, bool pinsOnly)
        {
            if (MapModS.LS.ModEnabled) return;
            orig(self, pinsOnly);
        }

        // Fixes some null referencing shenanigans
        private static void SetDoorMapZoneEarly(On.GameMap.orig_PositionCompass orig, GameMap self, bool posShade)
        {
            self.doorMapZone = self.GetDoorMapZone();
            orig(self, posShade);
        }

        // The following fixes loading the Quick Map for some of the special areas (like Ancestral Mound)
        private static string DoorMapZoneOverride(On.GameMap.orig_GetDoorMapZone orig, GameMap self)
        {
            if (!MapModS.LS.ModEnabled) return orig(self);

            MapZone mapZone = Utils.CurrentMapZone();
            if (mapZone != MapZone.NONE)
            {
                return mapZone.ToString();
            }
            return orig(self);
        }

        private static string CurrentMapZoneOverride(On.GameManager.orig_GetCurrentMapZone orig, GameManager self)
        {
            if (!MapModS.LS.ModEnabled) return orig(self);

            MapZone mapZone = Utils.CurrentMapZone();
            if (mapZone != MapZone.NONE)
            {
                return mapZone.ToString();
            }
            return orig(self);
        }
    }
}
