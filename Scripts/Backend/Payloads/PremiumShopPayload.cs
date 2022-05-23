using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    public class CurrencyPackPayload
    {
        public string ProductId;
        public CurrencyPayload[] Currencies = { };
        public CurrencyPayload[] BonusCurrencies = { };
        public CurrencyPayload Price;
        public bool IsFirstTime;
    }

    [Exchange]
    public class ChestPackXInfoPayload
    {
        public string ProductId;
        public CurrencyPayload OriginalPrice;
        public CurrencyPayload CurrentPrice;
    }

    [Exchange]
    public class ChestPackPayload
    {
        public ChestPackXInfoPayload PackXOne;
        public ChestPackXInfoPayload PackXTen;
        public int CurrentChestId;
        public int ObtainedCount;
        public bool IsUnlocked;
        public int NextChestId;
        public int UnlockNextChestAmount;
    }

    [Exchange]
    public class PremiumShopPayload
    {
        public CurrencyPackPayload[] GemPacks = { };
        public CurrencyPackPayload[] ShardPacks = { };
        public ChestPackPayload[] ChestPacks = { };
    }
}
