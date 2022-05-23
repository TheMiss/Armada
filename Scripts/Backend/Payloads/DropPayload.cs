using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    public enum DropType
    {
        Currency,
        Item
    }

    [Exchange(AddConvertExtension = false)]
    public class DropPayload
    {
        public int Id;
        public DropType Type;
        public CurrencyPayload Currency;
        public ItemPayload Item;
    }
}
