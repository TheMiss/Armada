#if DEBUG

using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;
using Armageddon.Mechanics.Items;

namespace Armageddon.Backend.Functions.Cheats
{
    [Exchange(AddConvertExtension = false)]
    public class GetEquipableItemsRequestEntry
    {
        public int ItemGeneratorId;
        public int Star;
        public int Level;
        public ItemType ItemType;
        public ItemQuality MinQuality;
        public ItemQuality MaxQuality;
    }

    [FunctionRequest]
    public class GetEquipableItemsRequest : BackendRequest
    {
        public GetEquipableItemsRequestEntry[] Entries;
    }

    [FunctionReply]
    public class GetEquipableItemsReply : BackendReply
    {
        public ItemPayload[] Items;
    }
}

#endif
