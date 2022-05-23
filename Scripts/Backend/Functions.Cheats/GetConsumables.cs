#if DEBUG

using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions.Cheats
{
    [Exchange(AddConvertExtension = false)]
    public class GetConsumablesRequestEntry
    {
        public int ItemId;
        public int Quantity = 1;
    }

    [FunctionRequest]
    public class GetConsumablesRequest : BackendRequest
    {
        public GetConsumablesRequestEntry[] Entries;
    }

    [FunctionReply]
    public class GetConsumablesReply : BackendReply
    {
        public ItemPayload[] Items;
    }
}

#endif
