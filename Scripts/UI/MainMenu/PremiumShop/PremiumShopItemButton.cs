using Armageddon.Backend.Payloads;
using Armageddon.Mechanics;
using Armageddon.UI.Base;
using Purity.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.PremiumShop
{
    public class PremiumShopItemButton : Widget
    {
        [SerializeField]
        private Button m_button;
        
        [SerializeField]
        private PremiumShopPricePanel m_premiumShopPricePanel;

        public Button Button => m_button;

        [ShowInPlayMode]
        public string ProductId { get; protected set; }

        public Currency Price { get; private set; }

        public void SetRealMoneyPriceText(string priceText)
        {
            Price = new Currency(CurrencyType.RealMoney, 0);
            m_premiumShopPricePanel.SetRealMoneyPriceText(priceText);
        }

        public void SetPrice(CurrencyType currencyType, int originalPrice, int currentPrice)
        {
            Price = new Currency(currencyType, currentPrice);
            m_premiumShopPricePanel.SetPrice(currencyType, originalPrice, currentPrice);
        }
    }
}
