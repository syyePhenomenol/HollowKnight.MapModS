using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapModS.Data
{
    public enum PinLocationState
    {
        UncheckedUnreachable,
        UncheckedReachable,
        NonRandomizedUnchecked,
        OutOfLogicReachable,
        Previewed,
        Cleared,
        ClearedPersistent
    }
}
