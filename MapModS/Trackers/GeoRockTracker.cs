using Modding;
using Vasi;

namespace MapModS.Trackers
{
    public static class GeoRockTracker
    {
        public static void Hook()
        {
            if (RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.GeoRocks) return;

            On.GeoRock.OnEnable += GeoRock_OnEnable;
            On.GeoRock.SetMyID += GeoRock_SetMyID;
            ModHooks.AfterSavegameLoadHook += AfterSavegameLoadHook;
        }

        public static void Unhook()
        {
            On.GeoRock.OnEnable -= GeoRock_OnEnable;
            On.GeoRock.SetMyID -= GeoRock_SetMyID;
            ModHooks.AfterSavegameLoadHook -= AfterSavegameLoadHook;
        }

        private static void GeoRock_OnEnable(On.GeoRock.orig_OnEnable orig, GeoRock self)
        {
            orig(self);

            PlayMakerFSM geoRockFSM = self.gameObject.LocateMyFSM("Geo Rock");

            FsmUtil.AddAction(FsmUtil.GetState(geoRockFSM, "Destroy"), new TrackGeoRock(self.gameObject));
        }

        private static void GeoRock_SetMyID(On.GeoRock.orig_SetMyID orig, GeoRock self)
        {
            orig(self);

            // Rename duplicate ids
            if (self.gameObject.scene.name == "Crossroads_ShamanTemple" && self.gameObject.name == "Geo Rock 2")
            {
                if (self.transform.parent != null)
                {
                    self.geoRockData.id = "_Items/Geo Rock 2";
                }
            }

            if (self.gameObject.scene.name == "Abyss_06_Core" && self.gameObject.name == "Geo Rock Abyss")
            {
                if (self.transform.parent != null)
                {
                    self.geoRockData.id = "_Props/Geo Rock Abyss";
                }
            }
        }

        private static void AfterSavegameLoadHook(SaveGameData self)
        {
            // Update Geo Rock counter (vanilla Geo Rocks only)
            MapModS.LS.geoRockCounter = 0;

            foreach (GeoRockData grd in self.sceneData.geoRocks)
            {
                if (grd.hitsLeft == 0)
                {
                    MapModS.LS.geoRockCounter++;
                }
            }
        }
    }
}