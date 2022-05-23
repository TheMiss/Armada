#if DEBUG

using System;
using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions.Cheats
{
    public class GetSkinDyesRequestItem
    {
        public int ItemSheetId;
        public int Quantity = 1;
    }

    public class GetSkinDyesRequest : BackendRequest
    {
        public GetSkinDyesRequestItem[] Items;
    }

    [Serializable]
    public class GetSkinDyesReply : BackendReply
    {
        public ItemPayload[] Items;
    }
}

#endif
