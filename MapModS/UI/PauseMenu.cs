﻿using MagicUI.Core;
using MagicUI.Elements;
using MapModS.Data;
//using MapModS.Map;
using MapModS.Pins;
using MapChanger;
using MapModS.Settings;
using System;
using System.Collections.Generic;
using UnityEngine;
using L = RandomizerMod.Localization;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.UI
{
    internal class PauseMenu
    {
        private static LayoutRoot layout;

        private static bool panelActive = false;

        private static readonly Dictionary<string, (Action<Button>, Action<Button>)> _mainButtons = new()
        {
            { "Enabled", (ToggleEnabled, UpdateEnabled) },
            { "Spoilers", (ToggleSpoilers, UpdateSpoilers) },
            { "Randomized", (ToggleRandomized, UpdateRandomized) },
            { "Others", (ToggleVanilla, UpdateOthers) },
            { "Style", (ToggleStyle, UpdateStyle) },
            { "Size", (ToggleSize, UpdateSize) },
            { "Mode", (ToggleMode, UpdateMode) },
            { "Customize Pins", (ToggleCustomizePins, UpdateCustomizePins) }
        };

        private static readonly Dictionary<string, (KeyCode, Action<Button>)> _hotkeys = new()
        {
            { "Spoilers", (KeyCode.Alpha1, ToggleSpoilers) },
            { "Randomized", (KeyCode.Alpha2, ToggleRandomized) },
            { "Others", (KeyCode.Alpha3, ToggleVanilla) },
            { "Style", (KeyCode.Alpha4, ToggleStyle) },
            { "Size", (KeyCode.Alpha5, ToggleSize) }
        };

        private static readonly Dictionary<string, (Action<Button>, Action<Button>)> _auxButtons = new()
        {
            { "Persistent", (TogglePersistent, UpdatePersistent) },
            { "Group By", (ToggleGroupBy, UpdateGroupBy) }
        };

        public static void Build()
        {
            if (layout == null)
            {
                layout = new(true, "Pause Menu");
                layout.VisibilityCondition = GameManager.instance.IsGamePaused;

                TextObject title = new(layout, "MapModS")
                {
                    TextAlignment = HorizontalAlignment.Left,
                    ContentColor = Colors.GetColor(ColorSetting.UI_Neutral),
                    FontSize = 20,
                    Font = MagicUI.Core.UI.TrajanBold,
                    Padding = new(10f, 840f, 10f, 10f),
                    Text = "MapModS",
                };

                DynamicUniformGrid mainButtons = new(layout, "Main Buttons")
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Orientation = Orientation.Vertical,
                    Padding = new(10f, 865f, 10f, 10f),
                    HorizontalSpacing = 5f,
                    VerticalSpacing = 5f
                };

                mainButtons.ChildrenBeforeRollover = 4;

                foreach (KeyValuePair<string, (Action<Button>, Action<Button>)> kvp in _mainButtons)
                {
                    Button button = new(layout, kvp.Key)
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        BorderColor = Colors.GetColor(ColorSetting.UI_Borders),
                        MinHeight = 28f,
                        MinWidth = 95f,
                        Font = MagicUI.Core.UI.TrajanBold,
                        FontSize = 11,
                        Margin = 0f
                    };

                    button.Click += kvp.Value.Item1;
                    mainButtons.Children.Add(button);
                }
            }

            DynamicUniformGrid poolButtons = new(layout, "Pool Buttons")
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Orientation = Orientation.Vertical,
                Padding = new(415f, 865f, 10f, 10f),
                HorizontalSpacing = 0f,
                VerticalSpacing = 5f
            };

            poolButtons.ChildrenBeforeRollover = 10;

            //List<string> groupButtonNames = MainData.usedPoolGroups;

            //foreach (string group in MainData.usedPoolGroups)
            //{
            //    Button button = new(layout, group)
            //    {
            //        HorizontalAlignment = HorizontalAlignment.Center,
            //        VerticalAlignment = VerticalAlignment.Center,
            //        Borderless = true,
            //        MinHeight = 28f,
            //        MinWidth = 85f,
            //        Content = L.Localize(group).Replace(" ", "\n"),
            //        Font = MagicUI.Core.UI.TrajanNormal,
            //        FontSize = 11,
            //        Margin = 0f
            //    };

            //    button.Click += TogglePool;
            //    poolButtons.Children.Add(button);
            //}

            //if (!Dependencies.HasBenchRando() || !BenchRandoInterop.IsBenchRandoEnabled())
            //{
            //    Button button = new(layout, "Benches (Vanilla)")
            //    {
            //        HorizontalAlignment = HorizontalAlignment.Center,
            //        VerticalAlignment = VerticalAlignment.Center,
            //        Borderless = true,
            //        MinHeight = 28f,
            //        MinWidth = 85f,
            //        Content = L.Localize("Benches (Vanilla)").Replace(" ", "\n"),
            //        Font = MagicUI.Core.UI.TrajanNormal,
            //        FontSize = 11,
            //        Margin = 0f
            //    };

            //    button.Click += ToggleBench;
            //    poolButtons.Children.Add(button);
            //}

            StackLayout auxButtons = new(layout, "Aux Buttons")
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Orientation = Orientation.Horizontal,
                Padding = new(210f, 931f, 10f, 10f),
                Spacing = 5f
            };

            foreach (KeyValuePair<string, (Action<Button>, Action<Button>)> kvp in _auxButtons)
            {
                Button button = new(layout, kvp.Key)
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Borderless = true,
                    MinHeight = 28f,
                    MinWidth = 95f,
                    Font = MagicUI.Core.UI.TrajanBold,
                    FontSize = 11,
                    Margin = 0f
                };

                button.Click += kvp.Value.Item1;
                auxButtons.Children.Add(button);
            }

            layout.ListenForHotkey(KeyCode.M, () =>
            {
                ToggleEnabled((Button)layout.GetElement("Enabled"));
            }, ModifierKeys.Ctrl);

            foreach (KeyValuePair<string, (KeyCode, Action<Button>)> kvp in _hotkeys)
            {
                layout.ListenForHotkey(kvp.Value.Item1, () =>
                {
                    kvp.Value.Item2.Invoke((Button)layout.GetElement(kvp.Key));
                }, ModifierKeys.Ctrl, () => MapModS.LS.ModEnabled);
            }

            UpdateAll();
        }

        public static void Destroy()
        {
            layout?.Destroy();
            layout = null;
        }

        private static void UpdateAll()
        {
            //foreach (KeyValuePair<string, (Action<Button>, Action<Button>)> kvp in _mainButtons)
            //{
            //    Button button = (Button)layout.GetElement(kvp.Key);

            //    kvp.Value.Item2.Invoke(button);

            //    if (kvp.Key == "Enabled") continue;

            //    if (MapModS.LS.ModEnabled)
            //    {
            //        button.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        button.Visibility = Visibility.Hidden;
            //    }
            //}

            //foreach (string group in MainData.usedPoolGroups)
            //{
            //    if (layout.GetElement(group) == null) continue;

            //    UpdatePool((Button)layout.GetElement(group));
            //}

            if (layout.GetElement("Benches (Vanilla)") != null)
            {
                UpdateBench((Button)layout.GetElement("Benches (Vanilla)"));
            }

            if (MapModS.LS.ModEnabled && panelActive)
            {
                layout.GetElement("Pool Buttons").Visibility = Visibility.Visible;
                layout.GetElement("Aux Buttons").Visibility = Visibility.Visible;
            }
            else
            {
                layout.GetElement("Pool Buttons").Visibility = Visibility.Hidden;
                layout.GetElement("Aux Buttons").Visibility = Visibility.Hidden;
            }

            foreach (KeyValuePair<string, (Action<Button>, Action<Button>)> kvp in _auxButtons)
            {
                kvp.Value.Item2.Invoke((Button)layout.GetElement(kvp.Key));
            }
        }

        public static void ToggleEnabled(Button sender)
        {
            if (GUI.lockToggleEnable) return;

            MapModS.LS.ToggleModEnabled();

            UIManager.instance.checkpointSprite.Show();
            UIManager.instance.checkpointSprite.Hide();

            //if (GUI.worldMapOpen || GUI.quickMapOpen)
            //{
            //    GUI.lockToggleEnable = true;
            //    MapText.SetToRefresh();
            //}
            //else
            //{
            //    MapText.UpdateAll();
            //}

            //if (!MapModS.LS.ModEnabled)
            //{
            //    WorldMap.pinGroup.gameObject.SetActive(false);
            //    WorldMap.goExtraRooms.SetActive(false);
            //    //FullMap.PurgeMap();
            //    MapRooms.ResetMapColors(GameManager.instance.gameMap);
            //    panelActive = false;
            //}

            UpdateAll();
        }

        private static void UpdateEnabled(Button sender)
        {
            if (MapModS.LS.ModEnabled)
            {
                sender.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                sender.Content = $"{L.Localize("Mod")}\n{L.Localize("Enabled")}";
            }
            else
            {
                sender.ContentColor = Colors.GetColor(ColorSetting.UI_Disabled);
                sender.Content = $"{L.Localize("Mod")}\n{L.Localize("Disabled")}";
            }
        }

        public static void ToggleSpoilers(Button sender)
        {
            MapModS.LS.ToggleSpoilers();
            MapModS.randoPinGroup.Set();
            //WorldMap.CustomPins.SetSprites();
            //InfoPanels.UpdateSelectedPin();

            //UpdateAll();
            //MapText.UpdateAll();
        }

        private static void UpdateSpoilers(Button sender)
        {
            if (MapModS.LS.SpoilerOn)
            {
                sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                sender.Content = $"{L.Localize("Spoilers")}:\n{L.Localize("on")}";
            }
            else
            {
                sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                sender.Content = $"{L.Localize("Spoilers")}:\n{L.Localize("off")}";
            }
        }

        public static void ToggleRandomized(Button sender)
        {
            MapModS.LS.ToggleRandomizedOn();
            MapModS.randoPinGroup.Set();

            //WorldMap.CustomPins.ResetPoolSettings();
            //WorldMap.CustomPins.SetPinsActive();
            //WorldMap.CustomPins.SetSprites();

            //UpdateAll();
            //MapText.UpdateAll();
        }

        private static void UpdateRandomized(Button sender)
        {
            //if (WorldMap.CustomPins == null) return;

            //string text = $"{L.Localize("Randomized")}:\n";

            //if (MapModS.LS.randomizedOn)
            //{
            //    sender.ContentColor = Colors.GetColor(ColorSetting.UI_On);
            //    text += L.Localize("on");
            //}
            //else
            //{
            //    sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
            //    text += L.Localize("off");
            //}

            //if (WorldMap.CustomPins.IsRandomizedCustom())
            //{
            //    sender.ContentColor = Colors.GetColor(ColorSetting.UI_Custom);
            //    text += $" ({L.Localize("custom")})";
            //}

            //sender.Content = text;
        }

        public static void ToggleVanilla(Button sender)
        {
            MapModS.LS.ToggleOthersOn();
            MapModS.randoPinGroup.Set();
            //WorldMap.CustomPins.ResetPoolSettings();
            //WorldMap.CustomPins.SetPinsActive();
            //WorldMap.CustomPins.SetSprites();

            //UpdateAll();
            //MapText.UpdateAll();
        }

        private static void UpdateOthers(Button sender)
        {
            //if (WorldMap.CustomPins == null) return;

            //string text = $"{L.Localize("Others")}:\n";

            //if (MapModS.LS.othersOn)
            //{
            //    sender.ContentColor = Colors.GetColor(ColorSetting.UI_On);
            //    text += L.Localize("on");
            //}
            //else
            //{
            //    sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
            //    text += L.Localize("off");
            //}

            //if (WorldMap.CustomPins.IsOthersCustom())
            //{
            //    sender.ContentColor = Colors.GetColor(ColorSetting.UI_Custom);
            //    text += $" ({L.Localize("custom")})";
            //}

            //sender.Content = text;
        }

        public static void ToggleStyle(Button sender)
        {
            MapModS.GS.TogglePinStyle();
            MapModS.randoPinGroup.Set();
            //WorldMap.CustomPins.SetSprites();

            //UpdateAll();
            //MapText.UpdateAll();
        }

        private static void UpdateStyle(Button sender)
        {
            string text = $"{L.Localize("Pin Style")}:\n";

            switch (MapModS.GS.PinStyle)
            {
                case PinStyle.Normal:
                    text += L.Localize("normal");
                    break;

                case PinStyle.Q_Marks_1:
                    text += $"{L.Localize("q marks")} 1";
                    break;

                case PinStyle.Q_Marks_2:
                    text += $"{L.Localize("q marks")} 2";
                    break;

                case PinStyle.Q_Marks_3:
                    text += $"{L.Localize("q marks")} 3";
                    break;
            }

            sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
            sender.Content = text;
        }

        public static void ToggleSize(Button sender)
        {
            //MapModS.GS.TogglePinSize();

            //if (WorldMap.CustomPins != null)
            //{
            //    WorldMap.CustomPins.ResizePins("None selected");
            //}

            //if (MapModS.GS.lookupOn)
            //{
            //    InfoPanels.UpdateSelectedPin();
            //}

            //UpdateAll();
            //MapText.UpdateAll();
        }

        private static void UpdateSize(Button sender)
        {
            string text = $"{L.Localize("Pin Size")}:\n";

            switch (MapModS.GS.PinSize)
            {
                case PinSize.Small:
                    text += L.Localize("small");
                    break;

                case PinSize.Medium:
                    text += L.Localize("medium");
                    break;

                case PinSize.Large:
                    text += L.Localize("large");
                    break;
            }

            sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
            sender.Content = text;
        }

        public static void ToggleMode(Button sender)
        {
            //if (GameManager.instance.gameMap != null
            //    && (MapModS.LS.Mode == MapMode.Transition
            //        || MapModS.LS.Mode == MapMode.TransitionVisitedOnly))
            //{
            //    MapRooms.ResetMapColors(GameManager.instance.gameMap);
            //}

            MapModS.LS.ToggleMode();
            MapChanger.Settings.ToggleMode();
            UpdateAll();
            //MapText.UpdateAll();
            //ControlPanel.UpdateAll();
            //MapKey.UpdateAll();
            //InfoPanels.selectedScene = "None";
            //TransitionPersistent.ResetRoute();
            //RouteCompass.UpdateCompass();
        }

        private static void UpdateMode(Button sender)
        {
            string text = $"{L.Localize("Mode")}:\n";

            switch (MapModS.LS.Mode)
            {
                case MapMode.FullMap:
                    sender.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                    text += L.Localize("Full Map");
                    break;

                case MapMode.AllPins:
                    sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                    text += L.Localize("All Pins");
                    break;

                case MapMode.PinsOverMap:
                    sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                    text += L.Localize("Pins Over Map");
                    break;

                case MapMode.Transition:
                    sender.ContentColor = Colors.GetColor(ColorSetting.UI_Special);
                    text += L.Localize("Transition");
                    break;

                case MapMode.TransitionVisitedOnly:
                    sender.ContentColor = Colors.GetColor(ColorSetting.UI_Special);
                    text += L.Localize("Transition") + " 2";
                    break;
            }

            sender.Content = text;
        }

        public static void CollapsePanel()
        {
            panelActive = false;

            UpdateAll();
        }

        public static void ToggleCustomizePins(Button sender)
        {
            panelActive = !panelActive;

            UpdateAll();
        }

        private static void UpdateCustomizePins(Button sender)
        {
            if (MapModS.LS.ModEnabled && panelActive)
            {
                sender.ContentColor = Colors.GetColor(ColorSetting.UI_Custom);
            }
            else
            {
                sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
            }

            sender.Content = $"{L.Localize("Customize")}\n{L.Localize("Pins")}";
        }

        public static void TogglePool(Button sender)
        {
            MapModS.LS.TogglePoolGroupSetting(sender.Name);

            //WorldMap.CustomPins.GetRandomizedOthersGroups();

            UpdateAll();
            //MapText.UpdateAll();
        }

        private static void UpdatePool(Button sender)
        {
            //if (WorldMap.CustomPins == null) return;

            //if (sender.Name == "Geo Rocks" && !RM.RS.GenerationSettings.PoolSettings.GeoRocks)
            //{
            //    sender.Content = $"{L.Localize("Geo Rocks")}:\n" + MapModS.LS.geoRockCounter + " / " + "207";
            //}

            switch (MapModS.LS.GetPoolGroupSetting(sender.Name))
            {
                case PoolState.Off:
                    sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                    break;
                case PoolState.On:
                    sender.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                    break;
                case PoolState.Mixed:
                    sender.ContentColor = Colors.GetColor(ColorSetting.UI_Custom);
                    break;
            }
        }

        public static void ToggleBench(Button sender)
        {
            if (!PlayerData.instance.GetBool("hasPinBench")) return;

            MapModS.LS.ToggleBench();

            UpdateAll();
        }

        public static void UpdateBench(Button sender)
        {
            if (PlayerData.instance.GetBool("hasPinBench"))
            {
                if (MapModS.LS.ShowBenchPins)
                {
                    sender.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                }
                else
                {
                    sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                }
            }
            else
            {
                sender.ContentColor = Colors.GetColor(ColorSetting.UI_Disabled);
            }
        }

        public static void TogglePersistent(Button sender)
        {
            MapModS.GS.TogglePersistentOn();

            UpdateAll();
        }

        private static void UpdatePersistent(Button sender)
        {
            if (MapModS.GS.PersistentOn)
            {
                sender.ContentColor = Colors.GetColor(ColorSetting.UI_On);
                sender.Content = $"{L.Localize("Persistent\nitems")}: {L.Localize("On")}";
            }
            else
            {
                sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
                sender.Content = $"{L.Localize("Persistent\nitems")}: {L.Localize("Off")}";
            }
        }

        public static void ToggleGroupBy(Button sender)
        {
            MapModS.LS.ToggleGroupBy();

            //WorldMap.CustomPins.GetRandomizedOthersGroups();
            //WorldMap.CustomPins.ResetPoolSettings();

            UpdateAll();
        }

        private static void UpdateGroupBy(Button sender)
        {
            switch (MapModS.LS.GroupBy)
            {
                case GroupBySetting.Location:
                    sender.Content = $"{L.Localize("Group by")}:\n{L.Localize("Location")}";
                    break;

                case GroupBySetting.Item:
                    sender.Content = $"{L.Localize("Group by")}:\n{L.Localize("Item")}";
                    break;
            }

            sender.ContentColor = Colors.GetColor(ColorSetting.UI_Neutral);
        }
    }
}