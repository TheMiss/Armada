using Armageddon.Backend.Payloads;
using Armageddon.Localization;
using Armageddon.UI.Base;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.PremiumShop
{
    public class PremiumShopPricePanel : Widget
    {
        [SerializeField]
        private TextMeshProUGUI m_originalPriceText;

        [SerializeField]
        private TextMeshProUGUI m_currentPriceText;

        [SerializeField]
        private Image m_strikethroughImage;

        private bool m_isPriceSet;

        protected override void Awake()
        {
            base.Awake();

            // In case that we somehow got an error and it would show the number we used to test.
            // As we know, Awake is not really a constructor, and SetPrice() is typically called before Awake().
            if (!m_isPriceSet)
            {
                m_originalPriceText.gameObject.SetActive(false);
                m_currentPriceText.gameObject.SetActive(false);
            }
        }

        public void SetPrice(CurrencyType currencyType, int originalPrice, int currentPrice)
        {
            if (currentPrice < originalPrice)
            {
                m_originalPriceText.gameObject.SetActive(true);
                string text = Lexicon.Currency(currencyType, originalPrice);
                m_originalPriceText.Set(text);
            }
            else
            {
                m_originalPriceText.gameObject.SetActive(false);
            }

            if (currentPrice > 0)
            {
                string text = Lexicon.Currency(currencyType, currentPrice);
                m_currentPriceText.Set(text);
                m_isPriceSet = true;
            }
        }

        public void SetRealMoneyPriceText(string priceText)
        {
            m_originalPriceText.gameObject.SetActive(false);
            m_currentPriceText.Set(priceText);
        }
    }
}
