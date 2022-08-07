using MagicUI.Core;
using MagicUI.Elements;
using MagicUI.Graphics;
using MapChanger;
using MapChanger.MonoBehaviours;
using MapChanger.UI;
using RandoMapMod.Modes;
using RandoMapMod.Pins;
using RandoMapMod.Rooms;

namespace RandoMapMod.UI
{
    internal class InfoPanels : WorldMapStack
    {
        protected override HorizontalAlignment StackHorizontalAlignment => HorizontalAlignment.Right;

        private static Panel lookupPanel;
        private static TextObject lookupPanelText;

        private static Panel uncheckedPanel;
        private static TextObject uncheckedPanelText;

        protected override bool Condition()
        {
            return base.Condition() && Conditions.RandoMapModEnabled();
        }

        protected override void BuildStack()
        {
            lookupPanel = new(Root, SpriteManager.Instance.GetTexture("GUI.PanelRight").ToSlicedSprite(100f, 50f, 200f, 50f), "Lookup Panel")
            {
                Borders = new(30f, 30f, 30f, 30f),
                MinWidth = 400f,
                MinHeight = 100f,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };

            ((Image)Root.GetElement("Lookup Panel Background")).Tint = RmmColors.GetColor(RmmColorSetting.UI_Borders);

            lookupPanelText = new(Root, "Lookup Panel Text")
            {
                ContentColor = RmmColors.GetColor(RmmColorSetting.UI_Neutral),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlignment = HorizontalAlignment.Left,
                Font = MagicUI.Core.UI.Perpetua,
                FontSize = 20,
                MaxWidth = 450f
            };

            lookupPanel.Child = lookupPanelText;

            Stack.Children.Add(lookupPanel);

            uncheckedPanel = new(Root, SpriteManager.Instance.GetTexture("GUI.PanelRight").ToSlicedSprite(100f, 50f, 250f, 50f), "Unchecked Panel")
            {
                Borders = new(30f, 30f, 30f, 30f),
                MinWidth = 200f,
                MinHeight = 100f,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };

            ((Image)Root.GetElement("Unchecked Panel Background")).Tint = RmmColors.GetColor(RmmColorSetting.UI_Borders);

            uncheckedPanelText = new(Root, "Unchecked Panel Text")
            {
                ContentColor = Colors.GetColor(ColorSetting.UI_Neutral),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Font = MagicUI.Core.UI.TrajanNormal,
                FontSize = 14
            };

            uncheckedPanel.Child = uncheckedPanelText;

            Stack.Children.Add(uncheckedPanel);
        }

        public override void Update()
        {
            UpdateLookupPanel();
            UpdateUncheckedPanel();
        }

        internal static void UpdateLookupPanel()
        {
            if (RandoMapMod.GS.LookupOn && RmmPinSelector.Instance.SelectedObjectKey is not Selector.NONE_SELECTED)
            {
                lookupPanelText.Text = RmmPinSelector.GetLookupText();
                lookupPanel.Visibility = Visibility.Visible;
            }
            else
            {
                lookupPanel.Visibility = Visibility.Collapsed;
            }
        }

        internal static void UpdateUncheckedPanel()
        {
            if (Conditions.TransitionModeEnabled()
                && RandoMapMod.GS.ShowUncheckedPanel
                && TransitionRoomSelector.Instance.SelectedObjectKey is not Selector.NONE_SELECTED)
            {
                uncheckedPanelText.Text = TransitionRoomSelector.GetUncheckedPanelText();
                uncheckedPanel.Visibility = Visibility.Visible;
            }
            else
            {
                uncheckedPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
