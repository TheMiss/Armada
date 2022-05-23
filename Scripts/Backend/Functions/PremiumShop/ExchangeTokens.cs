using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions
{
    [Exchange]
    public enum ExchangeTokensRequestTokenType
    {
        GemPack
    }

    [FunctionRequest]
    public class ExchangeTokensRequest : BackendRequest
    {
        public ExchangeTokensRequestTokenType TokenType;
    }

    [FunctionReply]
    public class ExchangeTokensReply : BackendReply
    {
        public PremiumShopPayload PremiumShop;
        public ModifiedCurrencyPayload[] ModifiedCurrencies = { };
    }
}
