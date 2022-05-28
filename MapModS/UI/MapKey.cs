using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapModS.Data;
using MapModS.Map;
using System.Collections.Generic;
using UnityEngine;

namespace MapModS.UI
{
    internal class MapKey
    {
        private static LayoutRoot layout;

        private static Panel panel;
        private static StackLayout panelContents;
        private static GridLayout pinKey;
        private static GridLayout roomKey;

        private static readonly Dictionary<PinBorderColor, string> _pinKey = new()
        {
            { PinBorderColor.Normal, "pinBlank" },
            { PinBorderColor.Previewed, "pinBlankGreen" },
            { PinBorderColor.Out_of_logic, "pinBlankRed" },
            { PinBorderColor.Persistent, "pinBlankCyan" }
        };

        public static bool Condition()
        {
            return GUI.worldMapOpen
                && MapModS.LS.ModEnabled
                && !GUI.lockToggleEnable;
        }

        public static void Build()
        {
            if (layout == null)
            {
                layout = new(true, "Map Key");
                layout.VisibilityCondition = Condition;

                panel = new(layout, GUIController.Instance.Images["panelLeft"].ToSlicedSprite(200f, 50f, 100f, 50f), "Panel")
                {
                    MinHeight = 0f,
                    MinWidth = 0f,
                    Borders = new(0f, 20f, 20f, 20f),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Padding = new(160f, 170f, 10f, 10f)
                };

                panelContents = new(layout, "Panel Contents")
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Orientation = Orientation.Vertical,
                    Padding = Padding.Zero,
                    Spacing = 5f
                };

                panel.Child = panelContents;

                pinKey = new(layout, "Pin Key")
                {
                    MinWidth = 200f,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    RowDefinitions =
                    {
                        new GridDimension(1, GridUnit.Proportional),
                        new GridDimension(1, GridUnit.Proportional),
                        new GridDimension(1, GridUnit.Proportional),
                        new GridDimension(1, GridUnit.Proportional)
                    },
                    ColumnDefinitions =
                    {
                        new GridDimension(1, GridUnit.Proportional),
                        new GridDimension(1.6f, GridUnit.Proportional)
                    },
                };

                panelContents.Children.Add(pinKey);

                int counter = 0;

                foreach(KeyValuePair<PinBorderColor, string> kvp in _pinKey)
                {
                    Image pin = new Image(layout, SpriteManager.GetSprite(kvp.Value), kvp.Key.ToString() + " Pin")
                    {
                        Width = 50f,
                        Height = 50f,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }.WithProp(GridLayout.Column, 0).WithProp(GridLayout.Row, counter);

                    TextObject text = new TextObject(layout, kvp.Key.ToString() + " Text")
                    {
                        Text = Utils.ToCleanName(kvp.Key.ToString()),
                        Padding = new(10f, 0f, 0f, 0f),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    }.WithProp(GridLayout.Column, 1).WithProp(GridLayout.Row, counter);

                    pinKey.Children.Add(pin);
                    pinKey.Children.Add(text);

                    counter++;
                }

                roomKey = new(layout, "Room Key")
                {
                    MinWidth = 200f,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    RowDefinitions =
                    {
                        new GridDimension(1, GridUnit.Proportional),
                        new GridDimension(1, GridUnit.Proportional),
                        new GridDimension(1, GridUnit.Proportional),
                        new GridDimension(1, GridUnit.Proportional),
                        new GridDimension(1, GridUnit.Proportional),
                        new GridDimension(1, GridUnit.Proportional)
                    },
                    ColumnDefinitions =
                    {
                        new GridDimension(1, GridUnit.Proportional),
                        new GridDimension(1.6f, GridUnit.Proportional)
                    },
                };

                panelContents.Children.Add(roomKey);

                Sprite roomCopy = GameManager.instance.gameMap.transform.GetChild(12).transform.GetChild(26).GetComponent<SpriteRenderer>().sprite;

                counter = 0;

                foreach (KeyValuePair<Transition.RoomState, Vector4> kvp in Transition.roomColor)
                {
                    if (kvp.Key == Transition.RoomState.Debug) continue;

                    Image room = new Image(layout, roomCopy, kvp.Key.ToString() + " Room")
                    {
                        Width = 40f,
                        Height = 40f,
                        Tint = kvp.Value,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new(0f, 5f, 17f, 5f),
                    }.WithProp(GridLayout.Column, 0).WithProp(GridLayout.Row, counter);

                    TextObject text = new TextObject(layout, kvp.Key.ToString() + " Text")
                    {
                        Text = Utils.ToCleanName(kvp.Key.ToString()),
                        Padding = new(10f, 0f, 0f, 0f),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    }.WithProp(GridLayout.Column, 1).WithProp(GridLayout.Row, counter);

                    roomKey.Children.Add(room);
                    roomKey.Children.Add(text);

                    counter++;
                }

                Image roomHighlight = new Image(layout, roomCopy, "Highlighted Room")
                {
                    Width = 40f,
                    Height = 40f,
                    Tint = new(255, 255, 255, 1f),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new(0f, 5f, 17f, 5f),
                }.WithProp(GridLayout.Column, 0).WithProp(GridLayout.Row, counter);

                TextObject textHighlight = new TextObject(layout, "Highlighted Text")
                {
                    Text = "Contains\nunchecked\ntransitions",
                    Padding = new(10f, 0f, 0f, 0f),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                }.WithProp(GridLayout.Column, 1).WithProp(GridLayout.Row, counter);

                roomKey.Children.Add(roomHighlight);
                roomKey.Children.Add(textHighlight);

                UpdateAll();
            }
        }

        public static void Destroy()
        {
            layout?.Destroy();
            layout = null;
        }

        public static void UpdateAll()
        {
            UpdatePanel();
        }

        public static void UpdatePanel()
        {
            if (MapModS.LS.mapKeyOn)
            {
                panel.Visibility = Visibility.Visible;
            }
            else
            {
                panel.Visibility = Visibility.Hidden;
            }

            if (TransitionData.TransitionModeActive())
            {
                roomKey.Visibility = Visibility.Visible;
            }
            else
            {
                roomKey.Visibility = Visibility.Collapsed;
            }
        }
    }
}
