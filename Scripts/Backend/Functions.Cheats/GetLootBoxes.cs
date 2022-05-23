#if DEBUG

using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions.Cheats
{
    [Exchange(AddConvertExtension = false)]
    public class GetLootBoxesRequestEntry
    {
        public int ItemId;
        public int Level = 1;
        public int Quantity = 1;
    }

    [FunctionRequest]
    public class GetLootBoxesRequest : BackendRequest
    {
        public GetLootBoxesRequestEntry[] Entries;
    }

    [FunctionReply]
    public class GetLootBoxesReply : BackendReply
    {
        public ItemPayload[] Items;
    }
}

#endif
