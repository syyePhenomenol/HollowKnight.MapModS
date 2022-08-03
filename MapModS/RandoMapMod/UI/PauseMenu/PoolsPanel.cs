using MapChanger.UI;
using RandoMapMod.Pins;

namespace RandoMapMod.UI
{
    internal class PoolsPanel : ExtraButtonPanel
    {
        internal static PoolsPanel Instance { get; private set; }

        public PoolsPanel() : base("Pools Panel", "RandoMapMod", 415f, 10)
        {
            Instance = this;
        }

        protected override void MakeButtons()
        {
            foreach (string poolGroup in RmmPins.AllPoolGroups)
            {
                PoolButton poolButton = new(poolGroup);
                poolButton.Make();
                ExtraButtonsGrid.Children.Add(poolButton.Button);
                ExtraButtons.Add(poolButton);
            }
        }
    }
}
