#if DEBUG

using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions.Cheats
{
    [Exchange(AddConvertExtension = false)]
    public class GetCardsRequestEntry
    {
        public int ItemId;
    }

    [FunctionRequest]
    public class GetCardsRequest : BackendRequest
    {
        public GetCardsRequestEntry[] Entries;
    }

    [FunctionReply]
    public class GetCardsReply : BackendReply
    {
        public ItemPayload[] Items;
    }
}

#endif
