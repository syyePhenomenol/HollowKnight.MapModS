﻿using System.Collections.Generic;
using MagicUI.Core;
using MagicUI.Elements;
using UnityEngine;

namespace MapChanger.UI
{
    internal class PauseMenu : UILayer
    {
        internal static PauseMenu Instance { get; private set; } = new();

        internal static List<Title> Titles { get; private set; } = new();
        internal static GridLayout MainButtonsGrid { get; private set; }
        internal static List<MainButton> MainButtons { get; private set; } = new();
        internal static List<ExtraButtonPanel> ExtraButtonPanels { get; private set; } = new();

        internal static void OnEnterGame()
        {
            Instance.Build();

            On.HeroController.Pause += OnPause;
        }

        internal static void OnQuitToMenu()
        {
            Instance.Destroy();

            Instance = new();
            Titles = new();
            MainButtons = new();
            ExtraButtonPanels = new();

            On.HeroController.Pause -= OnPause;
        }

        public override bool Condition()
        {
            return GameManager.instance.IsGamePaused();
        }

        public override void BuildLayout()
        {
            MainButtonsGrid = new(Root, "Main Buttons")
            {
                RowDefinitions =
                {
                    new GridDimension(1, GridUnit.AbsoluteMin),
                    new GridDimension(1, GridUnit.AbsoluteMin),
                    new GridDimension(1, GridUnit.AbsoluteMin)
                },
                ColumnDefinitions =
                {
                    new GridDimension(1, GridUnit.AbsoluteMin),
                    new GridDimension(1, GridUnit.AbsoluteMin),
                    new GridDimension(1, GridUnit.AbsoluteMin),
                    new GridDimension(1, GridUnit.AbsoluteMin)
                },
                MinHeight = 33f,
                MinWidth = 100f,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new(10f, 865f, 10f, 10f)
            };

            Update();
        }

        public override void Update()
        {
            foreach (Title title in Titles)
            {
                title.Update();
            }

            foreach (MainButton mainButton in MainButtons)
            {
                mainButton.Update();
            }

            foreach (ExtraButtonPanel ebp in ExtraButtonPanels)
            {
                ebp.Update();
            }
        }

        private static void OnPause(On.HeroController.orig_Pause orig, HeroController self)
        {
            orig(self);
            MapChangerMod.Instance.LogDebug("Pause Game");

            foreach (ExtraButtonPanel ebp in ExtraButtonPanels)
            {
                ebp.Hide();
            }

            Instance.Update();
        }

        internal static void HideOtherPanels(ExtraButtonPanel visiblePanel)
        {
            foreach (ExtraButtonPanel ebp in ExtraButtonPanels)
            {
                if (ebp != visiblePanel)
                {
                    ebp?.Hide();
                }
            }
        }
    }
}
