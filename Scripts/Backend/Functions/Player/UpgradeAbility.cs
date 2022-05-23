using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Armageddon.Backend.Functions
{
    [FunctionRequest]
    public class UpgradePlayerAbilityRequest : BackendRequest
    {
        public int AbilityId;
    }

    [FunctionReply]
    public class UpgradePlayerAbilityReply : BackendReply
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencyType CurrencyType;

        public int Balance;
        public int BalanceChange;
        public int AbilityLevel;
    }
}
