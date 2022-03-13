using ItemChanger;
using CMI = ConnectionMetadataInjector.ConnectionMetadataInjector;
using ConnectionMetadataInjector;

namespace MapModS.Data
{
    public class ItemDef
    {
        public ItemDef(AbstractItem item)
        {
            id = item.RandoItemId();
            itemName = item.RandoItemName();
            poolGroup = SupplementalMetadata.Of(item).Get(CMI.ItemPoolGroup);
            persistent = item.IsPersistent();
        }

        public int id;
        public string itemName;
        public string poolGroup = "Unknown";
        public bool persistent = false;
    }
}
