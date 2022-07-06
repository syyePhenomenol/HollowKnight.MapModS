using System.Collections;

namespace MapChanger.Objects
{
    public interface IPeriodicUpdater
    {
        float UpdateWaitSeconds { get; }
        IEnumerator PeriodicUpdate();
    }
}
