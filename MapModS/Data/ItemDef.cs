using ConnectionMetadataInjector;
using ItemChanger;

namespace MapModS.Data
{
    public class ItemDef
    {
        public ItemDef(AbstractItem item)
        {
            id = item.RandoItemId();
            itemName = item.RandoItemName();
            poolGroup = SupplementalMetadata.Of(item).Get(InjectedProps.ItemPoolGroup);
            persistent = item.IsPersistent();
        }

        public int id;
        public string itemName;
        public string poolGroup = "Unknown";
        public bool persistent = false;
    }
}
