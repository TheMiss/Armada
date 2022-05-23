using System.Collections.Generic;
using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Attributes;
using Armageddon.Backend.Payloads;

namespace Armageddon.Backend.Functions
{
    public class LoadPlayerRequest : BackendRequest
    {
    }

    [FunctionReply]
    public class LoadPlayerReply : BackendReply
    {
        public int Level;
        public long Exp;
        public HeroPayload[] Heroes = { };
        public LockedHeroPayload[] LockedHeroes = { };
        public string HeroInstanceId;
        public int MaxInventoryCount;
        public ItemPayload[] Items = { };
        public HeroInventoryPayload HeroInventory;
        public Dictionary<string, int> Abilities = new();
        public Dictionary<string, CurrencyPayload> Balances = new();
        public MapPayload Map;
        public int[] CardPowerIds = { };
        public PremiumShopPayload PremiumShop;
        public ShopPayload[] Shops = { };
        public MailPayload[] Mails = { };
    }
}
