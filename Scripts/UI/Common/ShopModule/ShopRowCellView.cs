using Armageddon.Mechanics.Items;
using Armageddon.Mechanics.Shops;
using UnityEngine;

namespace Armageddon.UI.Common.ShopModule
{
    public class ShopRowCellView : MonoBehaviour
    {
        [SerializeField]
        private ShopItemButton m_itemButton;

        public ShopItemButton ItemButton => m_itemButton;

        public ShopItem ShopItem { get; private set; }

        public void SetShopItem(ShopItem shopItem)
        {
            ShopItem = shopItem;

            if (ShopItem == null)
            {
                m_itemButton.gameObject.SetActive(false);
                return;
            }

            if (!m_itemButton.gameObject.activeSelf)
            {
                m_itemButton.gameObject.SetActive(true);
            }

            Item item = ShopItem.Item;
            m_itemButton.ObjectHolder.SetItem(item);
            m_itemButton.SetSellPrice(item.ShopSellPrice);
        }
    }
}
