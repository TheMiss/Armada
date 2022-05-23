using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Armageddon.Backend.Functions
{
    [FunctionRequest]
    public class UnlockHeroRequest : BackendRequest
    {
        public int HeroSheetId;
        public CurrencyType CurrencyType;
    }

    [FunctionReply]
    public class UnlockHeroReply : BackendReply
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencyType CurrencyType;

        public int Balance;
        public int BalanceChange;
        public int HeroSheetId;
        public string HeroInstanceId;
    }
}
