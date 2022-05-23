using System.Globalization;
using Armageddon.Localization;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Armageddon.UI.MainMenu.PremiumShop
{
    public class CurrencyPanelItem : PremiumShopItemButton
    {
        [SerializeField]
        private TextMeshProUGUI m_amountText;

        [SerializeField]
        private TextMeshProUGUI m_bonusText;

        [SerializeField]
        private GameObject m_bonusBadgeObject;

        public TextMeshProUGUI AmountText => m_amountText;

        public TextMeshProUGUI BonusText => m_bonusText;

        public GameObject BonusBadgeObject => m_bonusBadgeObject;

        public void SetProduct(string productId, int amount)
        {
            ProductId = productId;

            // TODO: Localize price text
            // Create a CultureInfo object for English in the U.S.
            // var t = UI.Game.Localization;
            var us = new CultureInfo("en-US");

            string text = amount.ToString("N0", us);
            m_amountText.Set(text);
        }

        public void SetBonusAmount(int amount)
        {
            var us = new CultureInfo("en-US");
            string text = $"{Texts.UI.Bonus}!\n+{amount.ToString("N0", us)}";

            m_bonusText.Set(text);
        }
    }
}
