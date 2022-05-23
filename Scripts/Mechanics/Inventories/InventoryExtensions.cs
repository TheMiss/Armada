using Armageddon.Mechanics.Items;

namespace Armageddon.Mechanics.Inventories
{
    public static class InventoryExtensions
    {
        public static Item GetItem(this Inventory inventory, string instanceId)
        {
            foreach (InventorySlot slot in inventory.Slots)
            {
                if (slot.Object is Item item)
                {
                    if (item.InstanceId == instanceId)
                    {
                        return item;
                    }
                }
            }

            // Debug.Log($"Cannot find item {serverId} ({Utility.Int64ToHex(serverId)})");

            return null;
        }
    }
}
