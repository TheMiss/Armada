using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Mechanics.Inventories;

namespace Armageddon.Backend.Functions
{
    [FunctionRequest]
    public class EquipHeroItemRequest : BackendRequest
    {
        public string ItemInstanceId;
        public EquipmentSlotType SlotType;
    }

    [FunctionReply]
    public class EquipHeroItemReply : BackendReply
    {
        public string EquippedItemInstanceId;
        public string UnequippedItemInstanceId;
        public EquipmentSlotType SlotType;
    }
}
