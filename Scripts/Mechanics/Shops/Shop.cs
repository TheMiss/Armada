using System;
using System.Collections.Generic;
using Armageddon.Backend.Payloads;
using Armageddon.Mechanics.Items;
using Cysharp.Threading.Tasks;

namespace Armageddon.Mechanics.Shops
{
    public class ShopItem
    {
        public Item Item { get; set; }
    }

    public class Shop
    {
        public ShopType Type { get; private set; }
        public DateTime NextResetTime { get; private set; }
        public List<ShopItem> Items { get; } = new();
        public Currency ResetPrice { get; set; }

        public async UniTask ReinitializeAsync(ShopPayload payload)
        {
            Items.Clear();

            Type = payload.Type;
            NextResetTime = payload.NextFreeResetTime;
            ResetPrice = new Currency(payload.ResetPrice);

            foreach (ShopItemPayload shopItemPayload in payload.ShopItems)
            {
                var item = await Item.CreateAsync(shopItemPayload.Item);
                var shopItem = new ShopItem
                {
                    Item = item
                };

                Items.Add(shopItem);
            }
        }
    }
}
