namespace Armageddon.Backend.Payloads
{
    // [Exchange]
    public class UpdateInventoryPayload
    {
        public ItemPayload[] UpdatedUsedItems = { };
        public ModifiedCurrencyPayload[] ModifiedCurrencies = { };
    }
}
