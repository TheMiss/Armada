using Armageddon.Backend.Payloads;
using Armageddon.Localization;
using Armageddon.Mechanics;
using Armageddon.UI.Base;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Armageddon.UI.Common.ShopModule
{
    public class ShopPricePanel : Widget
    {
        [SerializeField]
        private TextMeshProUGUI m_currentPriceText;

        private bool m_isPriceSet;

        public Currency Price { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            // In case that we somehow got an error and it would show the number we used to test.
            // As we know, Awake is not really a constructor, and SetPrice() is typically called before Awake().
            if (!m_isPriceSet)
            {
                m_currentPriceText.gameObject.SetActive(false);
            }
        }

        public void SetPrice(Currency price)
        {
            if (price == null)
            {
                SetEmptyPrice();
                return;
            }

            SetPrice(price.Type, price.Amount);
        }

        private void SetPrice(CurrencyType currencyType, int currentPrice)
        {
            if (currentPrice <= 0)
            {
                SetEmptyPrice();
                return;
            }

            m_currentPriceText.gameObject.SetActive(true);
            string text = Lexicon.Currency(currencyType, currentPrice);
            m_currentPriceText.Set(text);
            m_isPriceSet = true;
        }

        public void SetEmptyPrice()
        {
            Price = new Currency(CurrencyType.RealMoney, 0);
            m_currentPriceText.Set(string.Empty);
        }
    }
}
