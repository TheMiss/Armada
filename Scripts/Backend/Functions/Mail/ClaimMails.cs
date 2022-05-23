using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions
{
    [Exchange(AddConvertExtension = false)]
    public class ClaimMailsRequestEntry
    {
        public string InstanceId;
    }

    [FunctionRequest]
    public class ClaimMailsRequest : BackendRequest
    {
        public ClaimMailsRequestEntry[] Entries;
    }

    [FunctionReply]
    public class ClaimMailsReply : BackendReply
    {
        public ModifiedCurrencyPayload[] ModifiedCurrencies = { };
        public ItemPayload[] UpdatedItems = { };
        public ItemPayload[] ClaimedItems = { };
        public string[] RemovedMailInstanceIds = { };
    }
}
