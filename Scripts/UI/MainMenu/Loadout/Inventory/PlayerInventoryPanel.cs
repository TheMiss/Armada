using Armageddon.Externals.OdinInspector;
using Armageddon.Games;
using Armageddon.Mechanics;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.UI.Common.InventoryModule;
using Armageddon.UI.Common.InventoryModule.Slot;
using EnhancedUI.EnhancedScroller;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.Loadout.Inventory
{
    public class PlayerInventoryPanel : InventoryPanel, IEnhancedScrollerDelegate
    {
        [SerializeField]
        private TextMeshProUGUI m_currentQuantityText;

        [SerializeField]
        private TextMeshProUGUI m_limitQuantityText;

        [SerializeField]
        private ScrollRect m_scrollRect;

        [SerializeField]
        private EnhancedScroller m_scroller;

        [BoxGroupPrefabs]
        [SerializeField]
        private EnhancedScrollerCellView m_cellViewPrefab;

        [SerializeField]
        private int m_numberOfCellsPerRow = 3;

        [SerializeField]
        private int m_cellSize = 210;

        protected override void OnEnable()
        {
            var game = GetService<Game>();
            Player player = game.Player;

            if (player == null)
            {
                return;
            }

            // Get Inventory before anything else.
            Inventory = player.PlayerInventory;

            // The order here is important
            base.OnEnable();

            // SlotWidgets.Clear();
            m_scroller.Delegate = this;
            m_scroller.cellViewInstantiated = (scroller, view) =>
            {
                var cellView = (InventoryCellView)view;

                foreach (InventoryRowCellView rowCellView in cellView.RowCellViews)
                {
                    SlotWidgets.Add(rowCellView.SlotWidget);
                }
            };

            m_scroller.ReloadData();
        }

        int IEnhancedScrollerDelegate.GetNumberOfCells(EnhancedScroller scroller)
        {
            return Mathf.CeilToInt(Inventory.Slots.Count / (float)m_numberOfCellsPerRow);
        }

        float IEnhancedScrollerDelegate.GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return m_cellSize;
        }

        EnhancedScrollerCellView IEnhancedScrollerDelegate.GetCellView(EnhancedScroller scroller, int dataIndex,
            int cellIndex)
        {
            // first, we get a cell from the scroller by passing a prefab.
            // if the scroller finds one it can recycle it will do so, otherwise
            // it will create a new cell.
            var cellView = (InventoryCellView)scroller.GetCellView(m_cellViewPrefab);

            // data index of the first sub cell
            int rowIndex = dataIndex * m_numberOfCellsPerRow;
            cellView.name = $"Cell {rowIndex} to {rowIndex + m_numberOfCellsPerRow - 1}";

            // pass in a reference to our data set with the offset for this cell
            // cellView.SetData(ref _data, di, null);
            cellView.SetSlots(Inventory.Slots, rowIndex);

            // return the cell to the scroller
            return cellView;
        }

        protected override void OnSlotAdded(object sender, SlotAddedArgs e)
        {
            m_scroller.ReloadData();
        }

        public void LockScroll()
        {
            m_scrollRect.enabled = false;
        }

        public void UnlockScroll()
        {
            m_scrollRect.enabled = true;
        }

        public void UpdateQuantityText()
        {
            var player = GetService<Player>();
            m_currentQuantityText.Set($"{player.TotalItemCount}");
            m_limitQuantityText.Set($"{player.MaxInventoryCount}");
        }

        public override void OnWillInsertObjectHolder(SlotWidget slotWidget, ObjectHolder objectHolder)
        {
            if (objectHolder is ObjectHolderItem objectHolderItem)
            {
                objectHolderItem.SetItem((Item)objectHolderItem.Object);
            }

            UpdateQuantityText();
        }

        public override void OnWillRemoveObjectHolder(SlotWidget slotWidget, ObjectHolder objectHolder)
        {
            // if (objectHolder is ObjectHolderItem objectHolderItem)
            // {
            //     objectHolderItem.gameObject.SetActive(false);
            //     // objectHolderItem.SetItem(null);
            // }

            objectHolder.gameObject.SetActive(false);
            UpdateQuantityText();
        }
    }
}
