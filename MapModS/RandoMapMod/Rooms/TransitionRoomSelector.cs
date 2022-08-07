using System.Collections.Generic;
using System.Diagnostics;
using MapChanger.MonoBehaviours;
using RandoMapMod.Modes;
using RandoMapMod.Transition;
using RandoMapMod.UI;

namespace RandoMapMod.Rooms
{
    internal class TransitionRoomSelector : RoomSelector
    {
        internal static TransitionRoomSelector Instance;

        internal override void Initialize(IEnumerable<MapObject> rooms)
        {
            Instance = this;

            base.Initialize(rooms);
        }

        private readonly static Stopwatch attackHoldTimer = new();

        private void Update()
        {
            if (InputHandler.Instance.inputActions.menuSubmit.WasPressed
                && SelectedObjectKey is not NONE_SELECTED)
            {
                attackHoldTimer.Reset();
                RouteTracker.SelectRoute(SelectedObjectKey);
            }

            if (InputHandler.Instance.inputActions.attack.WasPressed)
            {
                attackHoldTimer.Restart();
            }

            if (InputHandler.Instance.inputActions.attack.WasReleased)
            {
                attackHoldTimer.Reset();
            }

            if (attackHoldTimer.ElapsedMilliseconds >= 500 && SelectedObjectKey is not NONE_SELECTED)
            {
                attackHoldTimer.Reset();
                RouteTracker.TryBenchwarp();
            }
        }

        public override void AfterMainUpdate()
        {
            attackHoldTimer.Reset();
        }

        protected private override bool ActiveByCurrentMode()
        {
            return Conditions.TransitionModeEnabled();
        }

        protected override void OnSelectionChanged()
        {
            InfoPanels.UpdateUncheckedPanel();
            WorldMapRouteText.UpdateInstructions();
        }

        internal static string GetUncheckedPanelText()
        {
            string sceneName = Instance.SelectedObjectKey;
            string text = TransitionData.GetUncheckedVisited(Instance.SelectedObjectKey);

            if (text is "") return sceneName;

            return $"{sceneName}\n\n{text}";
        }
    }
}
