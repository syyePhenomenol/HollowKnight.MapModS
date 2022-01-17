using System.Collections.Generic;
using UnityEngine;

// This code was heavily borrowed from RandomizerMod 3.0
namespace MapModS.Shop
{
    public static class ShopChanger
    {
        public static void Hook()
        {
            On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        }

        public static void Unhook()
        {
            On.PlayMakerFSM.OnEnable -= PlayMakerFSM_OnEnable;
        }

        private static void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            if (!MapModS.LS.ModEnabled) return;

            if (self.gameObject.scene.name != "Room_mapper" || self.gameObject.name != "Shop Menu") return;

            RefreshIseldaShop();
        }

        private static readonly List<string> ShopItemsToRemove = new()
        {
            //"Shop Item PinBench",
            //"Shop Item PinCocoon",
            //"Shop Item PinDreamPlant",
            //"Shop Item PinGhost",
            //"Shop Item PinShop",
            //"Shop Item PinStag",
            //"Shop Item PinTram",
            //"Shop Item PinSpa",
            "Shop Item MarkerB",
            "Shop Item MarkerR",
            "Shop Item MarkerY",
            "Shop Item MarkerW"
        };

        public static void RefreshIseldaShop()
        {
            GameObject shopObj = GameObject.Find("Shop Menu");

            if (shopObj == null) return;

            ShopMenuStock shop = shopObj.GetComponent<ShopMenuStock>();

            List<GameObject> newStock = new();

            foreach (GameObject item in shop.stock)
            {
                // Remove Map Markers from the shop
                if (!ShopItemsToRemove.Contains(item.GetComponent<ShopItemStats>().name))
                {
                    newStock.Add(item);
                }
            }

            shop.stock = newStock.ToArray();
        }
    }
}