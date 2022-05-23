using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions
{
    [FunctionRequest]
    public class BuyPremiumShopItemRequest : BackendRequest
    {
        public string ProductId;
        public int Amount;
    }

    [FunctionReply]
    public class BuyPremiumShopItemReply : BackendReply
    {
        public PremiumShopPayload PremiumShop;
        public ModifiedCurrencyPayload[] ModifiedCurrencies = { };
        public ItemPayload[] UpdatedItems = { };
        public ItemPayload[] BoughtItems = { };
    }
}
