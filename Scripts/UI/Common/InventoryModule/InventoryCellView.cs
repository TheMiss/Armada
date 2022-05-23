using System.Collections.Generic;
using Armageddon.Mechanics.Inventories;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace Armageddon.UI.Common.InventoryModule
{
    public delegate void SelectedDelegate(InventoryRowCellView rowCellView);

    public class InventoryCellView : EnhancedScrollerCellView
    {
        [SerializeField]
        private List<InventoryRowCellView> m_rowCellViews;

        public List<InventoryRowCellView> RowCellViews => m_rowCellViews;

        public void SetSlots(IReadOnlyList<InventorySlot> slots, int startingIndex)
        {
            if (RowCellViews.Count == 0)
            {
                Debug.Log("m_rowCellViews.Count should be at least 1");
            }

            for (int i = 0; i < RowCellViews.Count; i++)
            {
                // if the sub cell is outside the bounds of the data, we pass null to the sub cell
                int slotIndex = startingIndex + i;
                RowCellViews[i].SetSlot(slotIndex < slots.Count ? slots[slotIndex] : null);
            }
        }
    }
}
