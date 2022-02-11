using ItemChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapModS.Data
{
    public class ItemDef
    {
        public ItemDef(AbstractItem item)
        {
            id = item.RandoItemId();
            itemName = item.RandoItemName();
            poolGroup = DataLoader.GetItemPoolGroup(item.RandoItemName());
            persistent = item.IsPersistent();
        }

        public int id;
        public string itemName;
        public PoolGroup poolGroup;
        public bool persistent = false;
    }
}
