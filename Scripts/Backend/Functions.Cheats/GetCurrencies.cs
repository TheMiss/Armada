#if DEBUG

using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions.Cheats
{
    [Exchange(AddConvertExtension = false)]
    public class GetCurrenciesRequestEntry
    {
        public CurrencyType CurrencyType;
        public int Amount;
    }

    [FunctionRequest]
    public class GetCurrenciesRequest : BackendRequest
    {
        public GetCurrenciesRequestEntry[] Entries;
    }

    [FunctionReply]
    public class GetCurrenciesReply : BackendReply
    {
        public ModifiedCurrencyPayload[] ModifiedCurrencies;
    }
}

#endif
