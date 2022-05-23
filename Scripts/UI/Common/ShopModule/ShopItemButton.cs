using Armageddon.Mechanics;
using Armageddon.UI.Base;
using Armageddon.UI.Common.InventoryModule.Slot;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common.ShopModule
{
    public class ShopItemButton : Widget
    {
        [SerializeField]
        private Button m_button;

        [SerializeField]
        private ObjectHolderItem m_objectHolder;

        [SerializeField]
        private ShopPricePanel m_shopPricePanel;

        public Button Button => m_button;

        public ObjectHolderItem ObjectHolder => m_objectHolder;

        public ShopPricePanel PricePanel => m_shopPricePanel;

        // public void SetSellPrice(CurrencyType currencyType, int currentPrice)
        // {
        //     m_shopPricePanel.SetPrice(currencyType, currentPrice);
        // }

        public void SetSellPrice(Currency price)
        {
            PricePanel.SetPrice(price);
        }

        private void SetEmptySellPrice()
        {
            PricePanel.SetEmptyPrice();
        }
    }
}
