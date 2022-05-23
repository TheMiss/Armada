using Armageddon.Backend.Payloads;
using Purity.Common;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Armageddon.UI.MainMenu.PremiumShop.Chests
{
    public class ChestPanelRowItemButton : PremiumShopItemButton
    {
        [SerializeField]
        private TextMeshProUGUI m_titleText;

        [ShowInPlayMode]
        public int Amount { get; private set; }

        public void SetTitleText(string text)
        {
            m_titleText.Set(text);
        }

        // public void SetProduct(string productId, int amount)
        // {
        //     ProductId = productId;
        //     Amount = amount;
        // }

        public void SetPackX(ChestPackXInfoPayload packX)
        {
            ProductId = packX.ProductId;
            SetPrice(packX.CurrentPrice.Type, packX.OriginalPrice.Amount, packX.CurrentPrice.Amount);
            Amount = 1;
        }
    }
}
