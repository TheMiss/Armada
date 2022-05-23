using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Armageddon.Backend.Functions
{
    [Exchange(AddConvertExtension = false)]
    public class SellItemsRequestItem
    {
        public string InstanceId;
        public int Amount;
    }

    [FunctionRequest]
    public class SellItemsRequest : BackendRequest
    {
        public SellItemsRequestItem[] Items;
    }

    [FunctionReply]
    public class SellItemsReply : BackendReply
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencyType CurrencyType;

        public int Balance;
        public int BalanceChange;
    }
}
