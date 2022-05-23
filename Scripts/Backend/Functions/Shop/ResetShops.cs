using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions
{
    [Exchange(AddConvertExtension = false)]
    public class ResetShopsRequestEntry
    {
        public ShopType ShopType;
        public bool SpendCurrency;
    }

    [FunctionRequest]
    public class ResetShopsRequest : BackendRequest
    {
        public ResetShopsRequestEntry[] Entries;
    }

    [FunctionReply]
    public class ResetShopsReply : BackendReply
    {
        public ShopPayload[] Shops;
        public ModifiedCurrencyPayload[] ModifiedCurrencies;
    }
}
