using System.Collections.Generic;
using Armageddon.Backend.Payloads;
using Armageddon.Externals.OdinInspector;
using Armageddon.UI.Base;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.PremiumShop.ShardPacks
{
    public class ShardsPanel : Panel
    {
        [SerializeField]
        private List<CurrencyPanelItem> m_items;

        public void AddButtonListener(UnityAction<PremiumShopItemButton> callback)
        {
            for (int i = 0; i < EnumEx.GetSize<ShardPackType>(); i++)
            {
                PremiumShopItemButton premiumShopItemButton = m_items[i];
                Button button = premiumShopItemButton.Button;
                button.onClick.AddListener(() => callback(premiumShopItemButton));
            }
        }

        [Button]
        [GUIColorDefaultButton]
        private void SetExampleThb()
        {
            CurrencyPanelItem item = m_items[0];
            item.SetProduct("shardPack1", 1500);
            item.SetPrice(CurrencyType.RedGem, 60, 60);
            item.SetBonusAmount(1500);

            item = m_items[1];
            item.SetProduct("shardPack2", 4400);
            item.SetPrice(CurrencyType.RedGem, 160, 100);
            item.SetBonusAmount(4400);

            item = m_items[2];
            item.SetProduct("shardPack3", 12000);
            item.SetPrice(CurrencyType.RedGem, 400, 200);
            item.SetBonusAmount(12000);
        }

        [Button]
        [GUIColorDefaultButton]
        private void HideBonuses()
        {
            for (int i = 0; i < EnumEx.GetSize<ShardPackType>(); i++)
            {
                CurrencyPanelItem item = m_items[i];
                item.BonusBadgeObject.SetActive(false);
            }
        }

        [Button]
        [GUIColorDefaultButton]
        private void ShowBonuses()
        {
            for (int i = 0; i < EnumEx.GetSize<ShardPackType>(); i++)
            {
                CurrencyPanelItem item = m_items[i];
                item.BonusBadgeObject.SetActive(true);
            }
        }

        public void SetPacks(CurrencyPackPayload[] shardPacks)
        {
            // Assume that the size of both are matched!
            for (int i = 0; i < shardPacks.Length; i++)
            {
                CurrencyPackPayload shardPack = shardPacks[i];
                CurrencyPayload price = shardPack.Price;
                CurrencyPanelItem item = m_items[i];
                item.SetProduct(shardPack.ProductId, shardPack.Currencies[0].Amount);
                item.SetPrice(price.Type, price.Amount, price.Amount);

                if (shardPack.BonusCurrencies.Length > 0)
                {
                    item.SetBonusAmount(shardPack.BonusCurrencies[0].Amount);
                }
            }
        }
    }
}
