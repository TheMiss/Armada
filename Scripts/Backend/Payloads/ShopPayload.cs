using System;
using Armageddon.Backend.Attributes;

namespace Armageddon.Backend.Payloads
{
    [Exchange]
    public enum ShopType
    {
        Daily,
        Weekly,
        Special,
        Ads
    }

    [Exchange]
    public class ShopItemPayload
    {
        public ItemPayload Item;
        // Add more information here, for example, daily purchase availability.
    }

    [Exchange]
    public class ShopPayload
    {
        public ShopType Type;
        public string Name;
        public DateTime NextFreeResetTime;
        public CurrencyPayload ResetPrice;

        /// <summary>
        ///     Usually, this would be called "Items," which can make a code reader pause a bit for recognizing it is an object of
        ///     ShopItem, not just an object of Item class. So breaking consistency here is not a bad idea.
        /// </summary>
        public ShopItemPayload[] ShopItems = { };
    }
}
