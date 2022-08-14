﻿using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapChanger;
using MapChanger.MonoBehaviours;
using MapChanger.UI;
using RandoMapMod.Modes;
using RandoMapMod.Pins;
using RandoMapMod.Rooms;
using L = RandomizerMod.Localization;

namespace RandoMapMod.UI
{
    internal class SelectionPanels : WorldMapStack
    {
        // TODO: where to indicate lock selections?
        protected override HorizontalAlignment StackHorizontalAlignment => HorizontalAlignment.Right;

        private static Panel lookupPanel;
        private static TextObject pinPanelText;

        private static Panel roomPanel;
        private static TextObject roomPanelText;

        //private static Panel benchwarpPanel;
        //private static TextObject benchwarpPanelText;

        protected override bool Condition()
        {
            return base.Condition() && Conditions.RandoMapModEnabled();
        }

        protected override void BuildStack()
        {
            lookupPanel = new(Root, SpriteManager.Instance.GetTexture("GUI.PanelRight").ToSlicedSprite(100f, 50f, 200f, 50f), "Lookup Panel")
            {
                Borders = new(30f, 30f, 30f, 30f),
                MinWidth = 200f,
                MinHeight = 100f,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };

            ((Image)Root.GetElement("Lookup Panel Background")).Tint = RmmColors.GetColor(RmmColorSetting.UI_Borders);

            pinPanelText = new(Root, "Pin Panel Text")
            {
                ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Font = MagicUI.Core.UI.Perpetua,
                FontSize = 20,
                MaxWidth = 450f
            };

            lookupPanel.Child = pinPanelText;

            Stack.Children.Add(lookupPanel);

            //benchwarpPanel = new(Root, SpriteManager.Instance.GetTexture("GUI.PanelRight").ToSlicedSprite(100f, 50f, 250f, 50f), "Benchwarp Panel")
            //{
            //    Borders = new(30f, 30f, 30f, 30f),
            //    MinWidth = 200f,
            //    MinHeight = 100f,
            //    HorizontalAlignment = HorizontalAlignment.Right,
            //    VerticalAlignment = VerticalAlignment.Center
            //};

            //((Image)Root.GetElement("Benchwarp Panel Background")).Tint = RmmColors.GetColor(RmmColorSetting.UI_Borders);

            //benchwarpPanelText = new(Root, "Benchwarp Panel Text")
            //{
            //    ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral),
            //    HorizontalAlignment = HorizontalAlignment.Left,
            //    VerticalAlignment = VerticalAlignment.Center,
            //    Font = MagicUI.Core.UI.TrajanNormal,
            //    FontSize = 14,
            //    MaxWidth = 450f
            //};

            //benchwarpPanel.Child = benchwarpPanelText;

            //Stack.Children.Add(benchwarpPanel);

            roomPanel = new(Root, SpriteManager.Instance.GetTexture("GUI.PanelRight").ToSlicedSprite(100f, 50f, 250f, 50f), "Room Panel")
            {
                Borders = new(30f, 30f, 30f, 30f),
                MinWidth = 200f,
                MinHeight = 100f,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };

            ((Image)Root.GetElement("Room Panel Background")).Tint = RmmColors.GetColor(RmmColorSetting.UI_Borders);

            roomPanelText = new(Root, "Room Panel Text")
            {
                ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Font = MagicUI.Core.UI.TrajanNormal,
                FontSize = 14,
                MaxWidth = 450f
            };

            roomPanel.Child = roomPanelText;

            Stack.Children.Add(roomPanel);
        }

        public override void Update()
        {
            UpdatePinPanel();
            //UpdateBenchwarpPanel();
            UpdateRoomPanel();
        }

        internal static void UpdatePinPanel()
        {
            if (RandoMapMod.GS.PinSelectionOn && RmmPinSelector.Instance.SelectedObjectKey is not Selector.NONE_SELECTED)
            {
                pinPanelText.Text = RmmPinSelector.Instance.GetText();
                lookupPanel.Visibility = Visibility.Visible;
            }
            else
            {
                lookupPanel.Visibility = Visibility.Collapsed;
            }
        }

        //internal static void UpdateBenchwarpPanel()
        //{
        //    if (Conditions.ItemRandoModeEnabled()
        //        && RandoMapMod.GS.BenchwarpSelectionOn
        //        && BenchwarpRoomSelector.Instance.SelectedObjectKey is not Selector.NONE_SELECTED)
        //    {
        //        benchwarpPanelText.Text = BenchwarpRoomSelector.Instance.GetText();
        //        benchwarpPanel.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        benchwarpPanel.Visibility = Visibility.Collapsed;
        //    }
        //}

        internal static void UpdateRoomPanel()
        {
            if (Conditions.TransitionRandoModeEnabled()
                && RandoMapMod.GS.RoomSelectionOn
                && TransitionRoomSelector.Instance.SelectedObjectKey is not Selector.NONE_SELECTED)
            {
                roomPanelText.Text = TransitionRoomSelector.Instance.GetText();
                roomPanel.Visibility = Visibility.Visible;
            }
            else
            {
                roomPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
