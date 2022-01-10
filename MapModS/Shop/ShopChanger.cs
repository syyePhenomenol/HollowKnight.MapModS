﻿using System.Collections.Generic;
using UnityEngine;

// This code was heavily borrowed from RandomizerMod 3.0
namespace MapModS.Shop
{
    public static class ShopChanger
    {
        //private static ShopDef[] _shopItems;

        public static void Hook()
        {
            On.PlayMakerFSM.OnEnable += PlayMakerFSM_OnEnable;
        }

        private static void PlayMakerFSM_OnEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            if (!MapModS.LS.ModEnabled) return;

            if (self.gameObject.scene.name != "Room_mapper" || self.gameObject.name != "Shop Menu") return;

            //_shopItems = DataLoader.GetShopArray();

            RefreshIseldaShop();
        }

        private static readonly List<string> ShopItemsToRemove = new()
        {
            "Shop Item PinBench",
            "Shop Item PinCocoon",
            "Shop Item PinDreamPlant",
            "Shop Item PinGhost",
            "Shop Item PinShop",
            "Shop Item PinStag",
            "Shop Item PinTram",
            "Shop Item PinSpa",
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
            //GameObject itemPrefab = UnityEngine.Object.Instantiate(shop.stock[0]);
            //itemPrefab.SetActive(false);

            List<GameObject> newStock = new();

            foreach (GameObject item in shop.stock)
            {
                MapModS.Instance.Log(item.GetComponent<ShopItemStats>().name);

                // (specialType: 0 = lantern, elegant key, quill; 1 = mask, 2 = charm, 3 = vessel, 4-7 = relics, 8 = notch, 9 = map, 10 = simple key, 11 = egg, 12-14 = repair fragile, 15 = salubra blessing, 16 = map pin, 17 = map marker)

                // Remove Vanilla Pins and Map Markers from the shop
                if (!ShopItemsToRemove.Contains(item.GetComponent<ShopItemStats>().name))
                {
                    newStock.Add(item);
                }
            }

            //foreach (ShopDef shopItem in _shopItems)
            //{
            //    if (!Enum.TryParse(shopItem.playerDataBoolName, out Pool pool))
            //    {
            //        MapModS.Instance.LogError("Shop: bool name not recognized as an enum");
            //        continue;
            //    }

            //    if (MapModS.LS.GetHasFromGroup(pool)) continue;

            //    // Create a new shop item for this item def
            //    GameObject newItemObj = UnityEngine.Object.Instantiate(itemPrefab);
            //    newItemObj.SetActive(false);

            //    // Apply all the stored values
            //    ShopItemStats stats = newItemObj.GetComponent<ShopItemStats>();
            //    stats.playerDataBoolName = shopItem.playerDataBoolName;
            //    stats.nameConvo = shopItem.nameConvo;
            //    stats.descConvo = shopItem.descConvo;
            //    stats.dungDiscount = false;
            //    stats.cost = shopItem.cost;

            //    // Need to set all these to make sure the item doesn't break in one of various ways
            //    stats.priceConvo = string.Empty;
            //    stats.specialType = 16;
            //    stats.charmsRequired = 0;
            //    stats.relic = false;
            //    stats.relicNumber = 0;
            //    stats.relicPDInt = string.Empty;

            //    // Apply the sprite for the UI
            //    stats.transform.Find("Item Sprite").gameObject.GetComponent<SpriteRenderer>().sprite = SpriteManager.GetSprite(shopItem.spriteName);
            //    stats.transform.Find("Item Sprite").localPosition = new Vector2(0.08f, 0.0f);

            //    newStock.Add(newItemObj);
            //}

            shop.stock = newStock.ToArray();
        }
    }
}