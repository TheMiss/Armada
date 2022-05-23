using System.Collections.Generic;
using Armageddon.Backend.Payloads;
using Armageddon.Externals.OdinInspector;
using Armageddon.UI.Base;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.PremiumShop.GemPacks
{
    public class GemsPanel : Panel
    {
        [SerializeField]
        private List<CurrencyPanelItem> m_items;

        public void AddOnButtonClickListener(UnityAction<PremiumShopItemButton> callback)
        {
            for (int i = 0; i < EnumEx.GetSize<GemPackType>(); i++)
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
            item.SetProduct("gems1", 320);
            item.SetRealMoneyPriceText("THB 69.00");
            item.SetBonusAmount(320);

            item = m_items[1];
            item.SetProduct("gems2", 1000);
            item.SetRealMoneyPriceText("THB 179.00");
            item.SetBonusAmount(100);

            item = m_items[2];
            item.SetProduct("gems3", 2400);
            item.SetRealMoneyPriceText("THB 349.00");
            item.SetBonusAmount(2400);

            item = m_items[3];
            item.SetProduct("gems4", 5000);
            item.SetRealMoneyPriceText("THB 729.00");
            item.SetBonusAmount(5000);

            item = m_items[4];
            item.SetProduct("gems5", 13000);
            item.SetRealMoneyPriceText("THB 1,800.00");
            item.SetBonusAmount(13000);

            item = m_items[5];
            item.SetProduct("gems6", 28000);
            item.SetRealMoneyPriceText("THB 3,700.00");
            item.SetBonusAmount(28000);
        }

        [Button]
        [GUIColorDefaultButton]
        private void HideBonuses()
        {
            for (int i = 0; i < EnumEx.GetSize<GemPackType>(); i++)
            {
                CurrencyPanelItem item = m_items[i];
                item.BonusBadgeObject.SetActive(false);
            }
        }

        [Button]
        [GUIColorDefaultButton]
        private void ShowBonuses()
        {
            for (int i = 0; i < EnumEx.GetSize<GemPackType>(); i++)
            {
                CurrencyPanelItem item = m_items[i];
                item.BonusBadgeObject.SetActive(true);
            }
        }

        public void SetPacks(CurrencyPackPayload[] gemPacks)
        {
            // Assume that the size of both are matched!
            for (int i = 0; i < gemPacks.Length; i++)
            {
                CurrencyPackPayload gemPack = gemPacks[i];
                CurrencyPanelItem item = m_items[i];
                item.SetProduct(gemPack.ProductId, gemPack.Currencies[0].Amount);
                // We set the product price after IAP initialization

                if (gemPack.BonusCurrencies.Length > 0)
                {
                    item.SetBonusAmount(gemPack.BonusCurrencies[0].Amount);
                }
            }
        }

        public void SetProductPrices(Product[] products)
        {
            foreach (Product product in products)
            {
                CurrencyPanelItem item = m_items.Find(x => x.ProductId == product.definition.id);
                if (item != null)
                {
                    ProductMetadata metadata = product.metadata;
                    string text = $"{metadata.localizedPriceString}";
                    item.SetRealMoneyPriceText(text);
                }
            }
        }
    }
}
