using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions
{
    [FunctionRequest]
    public class BuyShopItemRequest : BackendRequest
    {
        public ShopType ShopType;
        public string ItemInstanceId;
        public int Quantity;
    }

    [FunctionReply]
    public class BuyShopItemReply : BackendReply
    {
        public ShopPayload Shop;
        public ModifiedCurrencyPayload[] ModifiedCurrencies = { };
        public ItemPayload[] UpdatedItems = { };
        public ItemPayload[] BoughtItems = { };
    }
}
