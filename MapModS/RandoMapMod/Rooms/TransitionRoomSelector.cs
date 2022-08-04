using System.Collections.Generic;
using MapChanger.MonoBehaviours;
using RandoMapMod.Modes;
using RandoMapMod.Transition;

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

        private void Update()
        {
            if (InputHandler.Instance.inputActions.menuSubmit.WasPressed
                && SelectedObjectKey is not NONE_SELECTED)
            {
                RouteTracker.SelectRoute(SelectedObjectKey);
            }
        }

        protected private override bool ActiveByCurrentMode()
        {
            return MapChanger.Settings.CurrentMode().GetType().IsSubclassOf(typeof(TransitionMode));
        }
    }
}
