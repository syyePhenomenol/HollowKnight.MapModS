using ItemChanger;

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
            //cost = item.Cost();
        }

        public int id;
        public string itemName;
        public PoolGroup poolGroup;
        public bool persistent = false;
        //public string cost;
    }
}
