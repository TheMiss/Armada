using System.Collections.Generic;
using Armageddon.Backend.Attributes;
using Armageddon.Mechanics.Items;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    public class StageDropItemPayload
    {
        public int SheetId;
        public int? Level;
        public ItemQuality? Quality;
        public int? Quantity;
        public Dictionary<string, string> CustomData;
    }

    [Exchange]
    public class StageDropCurrencyPayload
    {
        public CurrencyType Type;
        public int Amount;
    }

    [Exchange]
    public class StageDropPayload
    {
        public int Id;
        public DropType Type;
        public StageDropCurrencyPayload Currency;
        public StageDropItemPayload Item;
    }
}
