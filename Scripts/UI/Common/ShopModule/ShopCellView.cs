using System.Collections.Generic;
using Armageddon.Mechanics.Shops;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.Events;

namespace Armageddon.UI.Common.ShopModule
{
    public delegate void SelectedDelegate(ShopRowCellView rowCellView);

    public class ShopCellView : EnhancedScrollerCellView
    {
        [SerializeField]
        private List<ShopRowCellView> m_rowCellViews;

        public List<ShopRowCellView> RowCellViews => m_rowCellViews;

        public void SetShopItems(IReadOnlyList<ShopItem> shopItems, int startingIndex)
        {
            if (RowCellViews.Count == 0)
            {
                Debug.Log("m_rowCellViews.Count should be at least 1");
            }

            for (int i = 0; i < RowCellViews.Count; i++)
            {
                // if the sub cell is outside the bounds of the data, we pass null to the sub cell
                int slotIndex = startingIndex + i;
                RowCellViews[i].SetShopItem(slotIndex < shopItems.Count ? shopItems[slotIndex] : null);
            }
        }

        public void AddOnClickListener(UnityAction<ShopRowCellView> callback)
        {
            foreach (ShopRowCellView rowCellView in RowCellViews)
            {
                rowCellView.ItemButton.Button.onClick.AddListener(() => callback.Invoke(rowCellView));
            }
        }
    }
}
