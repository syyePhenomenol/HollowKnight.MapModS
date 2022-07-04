using System;
using System.Collections.Generic;
using System.Linq;

namespace MapModS.Pins
{
    public class RandoPinGroup : PinGroup<RandoPin, RandomizerModPinDef>
    {
        public const string Name = "Rando Pin Group";

        public override IEnumerable<RandomizerModPinDef> GetPinDefs()
        {
            return RandoPinData.PinDefs.Values.OrderBy(pinDef => pinDef.OffsetX).ThenBy(pinDef => pinDef.OffsetY);
        }
    }
}
