using System;
using Armageddon.Backend.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    [Serializable]
    public class ModifiedCurrencyPayload
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencyType CurrencyType;

        public int Balance;
        public int BalanceChange;
    }
}
