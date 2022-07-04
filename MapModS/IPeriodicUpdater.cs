using System.Collections;

namespace MapModS
{
    public interface IPeriodicUpdater
    {
        float UpdateWaitSeconds { get; }
        IEnumerator PeriodicUpdate();
    }
}
