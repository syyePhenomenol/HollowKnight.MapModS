namespace RandoMapMod.Settings
{
    public enum PinSize
    {
        Small,
        Medium,
        Large
    }

    public enum PinStyle
    {
        Normal,
        Q_Marks_1,
        Q_Marks_2,
        Q_Marks_3
    }

    public enum RouteTextInGame
    {
        Hide,
        Show,
        NextTransitionOnly
    }

    public enum OffRouteBehaviour
    {
        Keep,
        Cancel,
        Reevaluate
    }

    public enum RMMMode
    {
        Full_Map,
        All_Pins,
        Pins_Over_Map,
        Transition_1,
        Transition_2,
        Transition_3,
    }

    public enum GroupBySetting
    {
        Location,
        Item
    }

    public enum PoolState
    {
        Off,
        On,
        Mixed
    }
}
