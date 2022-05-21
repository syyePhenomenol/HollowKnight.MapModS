using MagicUI.Core;
using MagicUI.Elements;
using MapModS.Data;
using MapModS.Map;
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

        private static bool poolsPanelActive = false;

        private static readonly Dictionary<string, Tuple<Action<Button>, Action<Button>>> _mainButtons = new()
        {
            { "Enabled", new(ToggleEnabled, UpdateEnabled) },
            { "Spoilers", new(ToggleSpoilers, UpdateSpoilers) },
            { "Randomized", new(ToggleRandomized, UpdateRandomized) },
            { "Others", new(ToggleOthers, UpdateOthers) },
            { "Style", new(ToggleStyle, UpdateStyle) },
            { "Size", new(ToggleSize, UpdateSize) },
            { "Mode", new(ToggleMode, UpdateMode) },
            { "Customize Pins", new(ToggleCustomizePins, UpdateCustomizePins) }
        };

        private static readonly Dictionary<string, Tuple<KeyCode, Action<Button>>> _hotkeys = new()
        {
            { "Spoilers", new(KeyCode.Alpha1, ToggleSpoilers) },
            { "Randomized", new(KeyCode.Alpha2, ToggleRandomized) },
            { "Others", new(KeyCode.Alpha3, ToggleOthers) },
            { "Style", new(KeyCode.Alpha4, ToggleStyle) },
            { "Size", new(KeyCode.Alpha5, ToggleSize) }
        };

        private static readonly Dictionary<string, Tuple<Action<Button>, Action<Button>>> _auxButtons = new()
        {
            { "Persistent", new(TogglePersistent, UpdatePersistent) },
            { "Group By", new(ToggleGroupBy, UpdateGroupBy) }
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
                    FontSize = 20,
                    Font = MagicUI.Core.UI.TrajanBold,
                    Padding = new(10.5f, 840f, 10f, 10f),
                    Text = "MapModS",
                };

                DynamicUniformGrid mainButtons = new(layout, "Main Buttons")
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Orientation = Orientation.Vertical,
                    Padding = new(10.5f, 865f, 10f, 10f),
                    HorizontalSpacing = 5f,
                    VerticalSpacing = 5f
                };

                mainButtons.ChildrenBeforeRollover = 4;

                foreach (KeyValuePair<string, Tuple<Action<Button>, Action<Button>>> kvp in _mainButtons)
                {
                    Button button = new(layout, kvp.Key)
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        BorderColor = Color.white,
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

            DynamicUniformGrid panelButtons = new(layout, "Panel Buttons")
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Orientation = Orientation.Vertical,
                Padding = new(415.5f, 865f, 10f, 10f),
                HorizontalSpacing = 0f,
                VerticalSpacing = 5f
            };

            panelButtons.ChildrenBeforeRollover = 10;

            foreach (string group in new List<string>(MainData.usedPoolGroups) { "Benches" })
            {
                Button button = new(layout, group)
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    BorderColor = Color.black,
                    MinHeight = 28f,
                    MinWidth = 85f,
                    Content = L.Localize(group).Replace(" ", "\n"),
                    Font = MagicUI.Core.UI.TrajanNormal,
                    FontSize = 11,
                    Margin = 0f
                };

                button.Click += TogglePool;
                panelButtons.Children.Add(button);
            }

            StackLayout auxButtons = new(layout, "Aux Buttons")
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Orientation = Orientation.Horizontal,
                Padding = new(210.5f, 931f, 10f, 10f),
                Spacing = 5f
            };

            foreach (KeyValuePair<string, Tuple<Action<Button>, Action<Button>>> kvp in _auxButtons)
            {
                Button button = new(layout, kvp.Key)
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    BorderColor = Color.black,
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

            foreach (KeyValuePair<string, Tuple<KeyCode, Action<Button>>> kvp in _hotkeys)
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
            layout.Destroy();
            layout = null;
        }

        private static void UpdateAll()
        {
            foreach (KeyValuePair<string, Tuple<Action<Button>, Action<Button>>> kvp in _mainButtons)
            {
                Button button = (Button)layout.GetElement(kvp.Key);

                kvp.Value.Item2.Invoke(button);

                if (kvp.Key == "Enabled") continue;
                
                if (MapModS.LS.ModEnabled)
                {
                    button.Visibility = Visibility.Visible;
                }
                else
                {
                    button.Visibility = Visibility.Hidden;
                }
            }

            foreach (string group in new List<string>(MainData.usedPoolGroups) { "Benches" })
            {
                Button button = (Button)layout.GetElement(group);

                UpdatePool(button);

                if (MapModS.LS.ModEnabled && poolsPanelActive)
                {
                    button.Visibility = Visibility.Visible;
                }
                else
                {
                    button.Visibility = Visibility.Hidden;
                }
            }

            foreach (KeyValuePair<string, Tuple<Action<Button>, Action<Button>>> kvp in _auxButtons)
            {
                Button button = (Button)layout.GetElement(kvp.Key);

                kvp.Value.Item2.Invoke(button);

                if (MapModS.LS.ModEnabled && poolsPanelActive)
                {
                    button.Visibility = Visibility.Visible;
                }
                else
                {
                    button.Visibility = Visibility.Hidden;
                }
            }
        }

        public static void ToggleEnabled(Button sender)
        {
            MapModS.LS.ToggleModEnabled();

            if (!GameManager.instance.IsGamePaused())
            {
                UIManager.instance.checkpointSprite.Show();
                UIManager.instance.checkpointSprite.Hide();
            }

            if (GUI.AnyMapOpen())
            {
                MapText.SetToRefresh();
            }
            else
            {
                MapText.UpdateAll();
            }

            if (!MapModS.LS.ModEnabled)
            {
                Transition.ResetMapColors(GameManager.instance.gameMap);
                poolsPanelActive = false;
            }

            //TransitionText.SetTexts();

            UpdateAll();
        }

        private static void UpdateEnabled(Button sender)
        {
            if (MapModS.LS.ModEnabled)
            {
                sender.ContentColor = Color.green;
                sender.Content = $"{L.Localize("Mod")}\n{L.Localize("Enabled")}";
            }
            else
            {
                sender.ContentColor = Color.red;
                sender.Content = $"{L.Localize("Mod")}\n{L.Localize("Disabled")}";
            }
        }

        public static void ToggleSpoilers(Button sender)
        {
            MapModS.LS.ToggleSpoilers();
            WorldMap.CustomPins.SetSprites();
            LookupText.UpdateSelectedPin();

            UpdateAll();
            MapText.UpdateAll();
        }

        private static void UpdateSpoilers(Button sender)
        {
            if (MapModS.LS.SpoilerOn)
            {
                sender.ContentColor = Color.green;
                sender.Content = $"{L.Localize("Spoilers")}:\n{L.Localize("on")}";
            }
            else
            {
                sender.ContentColor = Color.white;
                sender.Content = $"{L.Localize("Spoilers")}:\n{L.Localize("off")}";
            }
        }

        public static void ToggleRandomized(Button sender)
        {
            MapModS.LS.ToggleRandomizedOn();
            WorldMap.CustomPins.ResetPoolSettings();
            WorldMap.CustomPins.SetPinsActive();

            UpdateAll();
            MapText.UpdateAll();
        }

        private static void UpdateRandomized(Button sender)
        {
            if (WorldMap.CustomPins == null) return;

            string text = $"{L.Localize("Randomized")}:\n";

            if (MapModS.LS.randomizedOn)
            {
                sender.ContentColor = Color.green;
                text += L.Localize("on");
            }
            else
            {
                sender.ContentColor = Color.white;
                text += L.Localize("off");
            }

            if (WorldMap.CustomPins.IsRandomizedCustom())
            {
                sender.ContentColor = Color.yellow;
                text += $" ({L.Localize("custom")})";
            }

            sender.Content = text;
        }

        public static void ToggleOthers(Button sender)
        {
            MapModS.LS.ToggleOthersOn();
            WorldMap.CustomPins.ResetPoolSettings();
            WorldMap.CustomPins.SetPinsActive();

            UpdateAll();
            MapText.UpdateAll();
        }

        private static void UpdateOthers(Button sender)
        {
            if (WorldMap.CustomPins == null) return;

            string text = $"{L.Localize("Others")}:\n";

            if (MapModS.LS.othersOn)
            {
                sender.ContentColor = Color.green;
                text += L.Localize("on");
            }
            else
            {
                sender.ContentColor = Color.white;
                text += L.Localize("off");
            }

            if (WorldMap.CustomPins.IsOthersCustom())
            {
                sender.ContentColor = Color.yellow;
                text += $" ({L.Localize("custom")})";
            }

            sender.Content = text;
        }

        public static void ToggleStyle(Button sender)
        {
            MapModS.GS.TogglePinStyle();
            WorldMap.CustomPins.SetSprites();

            UpdateAll();
            MapText.UpdateAll();
        }

        private static void UpdateStyle(Button sender)
        {
            string text = $"{L.Localize("Pin Style")}:\n";

            switch (MapModS.GS.pinStyle)
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

            sender.Content = text;
        }

        public static void ToggleSize(Button sender)
        {
            MapModS.GS.TogglePinSize();

            if (WorldMap.CustomPins != null)
            {
                WorldMap.CustomPins.ResizePins("None selected");
            }

            if (MapModS.LS.lookupOn)
            {
                LookupText.UpdateSelectedPin();
            }

            UpdateAll();
            MapText.UpdateAll();
        }

        private static void UpdateSize(Button sender)
        {
            string text = $"{L.Localize("Pin Size")}:\n";

            switch (MapModS.GS.pinSize)
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

            sender.Content = text;
        }

        public static void ToggleMode(Button sender)
        {
            if (GameManager.instance.gameMap != null
                && (MapModS.LS.mapMode == MapMode.TransitionRando
                    || MapModS.LS.mapMode == MapMode.TransitionRandoAlt))
            {
                Transition.ResetMapColors(GameManager.instance.gameMap);
            }

            MapModS.LS.ToggleFullMap();

            UpdateAll();
            MapText.UpdateAll();
        }

        private static void UpdateMode(Button sender)
        {
            string text = $"{L.Localize("Mode")}:\n";

            switch (MapModS.LS.mapMode)
            {
                case MapMode.FullMap:
                    sender.ContentColor = Color.green;
                    text += L.Localize("Full Map");
                    break;

                case MapMode.AllPins:
                    sender.ContentColor = Color.white;
                    text += L.Localize("All Pins");
                    break;

                case MapMode.PinsOverMap:
                    sender.ContentColor = Color.white;
                    text += L.Localize("Pins Over Map");
                    break;

                case MapMode.TransitionRando:
                    sender.ContentColor = Color.cyan;
                    text += L.Localize("Transition");
                    break;

                case MapMode.TransitionRandoAlt:
                    sender.ContentColor = Color.cyan;
                    text += L.Localize("Transition") + " 2";
                    break;
            }

            sender.Content = text;
        }

        public static void CollapsePanel()
        {
            poolsPanelActive = false;

            UpdateAll();
        }

        public static void ToggleCustomizePins(Button sender)
        {
            poolsPanelActive = !poolsPanelActive;

            UpdateAll();
        }

        private static void UpdateCustomizePins(Button sender)
        {
            if (MapModS.LS.ModEnabled && poolsPanelActive)
            {
                sender.ContentColor = Color.yellow;
            }
            else
            {
                sender.ContentColor = Color.white;
            }

            sender.Content = $"{L.Localize("Customize")}\n{L.Localize("Pins")}";
        }

        public static void TogglePool(Button sender)
        {
            if (sender.Name == "Benches")
            {
                if (!PlayerData.instance.GetBool("hasPinBench")) return;

                MapModS.LS.ToggleBench();
            }
            else
            {
                MapModS.LS.TogglePoolGroupSetting(sender.Name);

                WorldMap.CustomPins.GetRandomizedOthersGroups();

                UpdateAll();
                MapText.UpdateAll();
            }
        }

        private static void UpdatePool(Button sender)
        {
            if (WorldMap.CustomPins == null) return;

            if (sender.Name == "Geo Rocks" && !RM.RS.GenerationSettings.PoolSettings.GeoRocks)
            {
                sender.Content = $"{L.Localize("Geo Rocks")}:\n" + MapModS.LS.GeoRockCounter + " / " + "207";
            }

            if (sender.Name == "Benches")
            {
                if (PlayerData.instance.GetBool("hasPinBench"))
                {
                    sender.ContentColor = Color.green;
                }
                else
                {
                    sender.ContentColor = Color.red;
                }
            }
            else
            {
                switch (MapModS.LS.GetPoolGroupSetting(sender.Name))
                {
                    case PoolGroupState.Off:
                        sender.ContentColor = Color.white;
                        break;
                    case PoolGroupState.On:
                        sender.ContentColor = Color.green;
                        break;
                    case PoolGroupState.Mixed:
                        sender.ContentColor = Color.yellow;
                        break;
                }
            }
        }

        public static void ToggleGroupBy(Button sender)
        {
            MapModS.LS.ToggleGroupBy();

            WorldMap.CustomPins.GetRandomizedOthersGroups();
            WorldMap.CustomPins.ResetPoolSettings();

            UpdateAll();
        }

        private static void UpdateGroupBy(Button sender)
        {
            switch (MapModS.LS.groupBy)
            {
                case GroupBy.Location:
                    sender.Content = $"{L.Localize("Group by")}:\n{L.Localize("Location")}";
                    break;

                case GroupBy.Item:
                    sender.Content = $"{L.Localize("Group by")}:\n{L.Localize("Item")}";
                    break;
            }
        }

        public static void TogglePersistent(Button sender)
        {
            MapModS.GS.TogglePersistentOn();

            UpdateAll();
        }

        private static void UpdatePersistent(Button sender)
        {
            if (MapModS.GS.persistentOn)
            {
                sender.ContentColor = Color.green;
                sender.Content = $"{L.Localize("Persistent\nitems")}: {L.Localize("On")}";
            }
            else
            {
                sender.ContentColor = Color.white;
                sender.Content = $"{L.Localize("Persistent\nitems")}: {L.Localize("Off")}";
            }
        }
    }
}