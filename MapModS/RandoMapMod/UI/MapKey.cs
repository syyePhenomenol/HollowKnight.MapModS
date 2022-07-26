using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapModS.Data;
using MapModS.Map;
using MapModS.Settings;
using UnityEngine;
using L = RandomizerMod.Localization;

namespace MapModS.UI
{
    internal class MapKey
    {
        private static LayoutRoot layout;

        private static Panel panel;
        private static StackLayout panelContents;
        private static GridLayout pinKey;
        private static GridLayout roomKey;

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

                ((Image)layout.GetElement("Panel Background")).Tint = Colors.GetColor(ColorSetting.UI_Borders);

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

                foreach(ColorSetting colorSetting in Colors.pinColors)
                {
                    Panel pinPanel = new Panel(layout, SpriteManager.GetSprite("pinBlank"), colorSetting.ToString() + "Panel")
                    {
                        MinHeight = 50f,
                        MinWidth = 50f,
                        Borders = new(0f, 0f, 0f, 0f),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new(0f, 0f, 0f, 0f)
                    }.WithProp(GridLayout.Column, 0).WithProp(GridLayout.Row, counter);

                    Image pin = new Image(layout, SpriteManager.GetSprite("pinBorder"), colorSetting.ToString() + " Pin")
                    {
                        Width = 50f,
                        Height = 50f,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }.WithProp(GridLayout.Column, 0).WithProp(GridLayout.Row, counter);

                    ((Image)layout.GetElement(colorSetting.ToString() + " Pin")).Tint = Colors.GetColor(colorSetting);

                    pinPanel.Child = pin;

                    TextObject text = new TextObject(layout, colorSetting.ToString() + " Text")
                    {
                        Text = L.Localize(Utils.ToCleanName(colorSetting.ToString().Replace("Pin_", ""))),
                        Padding = new(10f, 0f, 0f, 0f),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    }.WithProp(GridLayout.Column, 1).WithProp(GridLayout.Row, counter);

                    pinKey.Children.Add(pinPanel);
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

                foreach (ColorSetting color in Colors.roomColors)
                {
                    string cleanRoomColor = Utils.ToCleanName(color.ToString().Replace("Room_", ""));

                    Image room = new Image(layout, roomCopy, cleanRoomColor + " Room")
                    {
                        Width = 40f,
                        Height = 40f,
                        Tint = Colors.GetColor(color),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new(0f, 5f, 17f, 5f),
                    }.WithProp(GridLayout.Column, 0).WithProp(GridLayout.Row, counter);

                    TextObject text = new TextObject(layout, cleanRoomColor + " Text")
                    {
                        Text = L.Localize(cleanRoomColor),
                        Padding = new(10f, 0f, 0f, 0f),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    }.WithProp(GridLayout.Column, 1).WithProp(GridLayout.Row, counter);

                    roomKey.Children.Add(room);
                    roomKey.Children.Add(text);

                    counter++;
                }

                Vector4 highlighted = Colors.GetColor(ColorSetting.Room_Normal);
                highlighted.w = 1f;

                Image roomHighlight = new Image(layout, roomCopy, "Highlighted Room")
                {
                    Width = 40f,
                    Height = 40f,
                    Tint = highlighted,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new(0f, 5f, 17f, 5f),
                }.WithProp(GridLayout.Column, 0).WithProp(GridLayout.Row, counter);

                TextObject textHighlight = new TextObject(layout, "Highlighted Text")
                {
                    Text = L.Localize("Contains\nunchecked\ntransitions"),
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
            if (MapModS.GS.MapKeyOn)
            {
                panel.Visibility = Visibility.Visible;
            }
            else
            {
                panel.Visibility = Visibility.Hidden;
            }

            if (MapModS.LS.Mode == MapMode.Transition
                || MapModS.LS.Mode == MapMode.TransitionVisitedOnly)
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
