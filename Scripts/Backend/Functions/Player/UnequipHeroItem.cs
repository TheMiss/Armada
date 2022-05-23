using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Mechanics.Inventories;

namespace Armageddon.Backend.Functions
{
    public enum UnequipHeroItemResult
    {
        Success
    }

    [FunctionRequest]
    public class UnequipHeroItemRequest : BackendRequest
    {
        public int SlotType;
    }

    [FunctionReply]
    public class UnequipHeroItemReply : BackendReply
    {
        public UnequipHeroItemResult Result;
        public string UnequippedItemInstanceId;
        public EquipmentSlotType SlotType;
    }
}
