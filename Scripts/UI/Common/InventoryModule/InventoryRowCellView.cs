using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Inventories;
using Armageddon.UI.Common.InventoryModule.Slot;
using UnityEngine;

namespace Armageddon.UI.Common.InventoryModule
{
    public class InventoryRowCellView : MonoBehaviour
    {
        [BoxGroupPrefabs]
        [SerializeField]
        private ObjectHolder m_objectHolderPrefab;

        [SerializeField]
        private SlotWidget m_slotWidget;

        public SlotWidget SlotWidget => m_slotWidget;

        public void SetSlot(InventorySlot slot)
        {
            if (slot == null)
            {
                // Hide slot widget to match slot capacity.
                SlotWidget.gameObject.SetActive(false);

                // We better do this at m_scroller.cellViewWillRecycle.
                // if (m_slotWidget.ObjectHolder != null)
                // {
                //     m_slotWidget.ObjectHolder.name = "None";
                //     m_slotWidget.ObjectHolder.gameObject.SetActive(false);
                //     m_slotWidget.AllowSelect = false;
                // }

                return;
            }

            SlotWidget.Slot = slot;
            SlotWidget.Inventory = slot.Inventory;
            SlotWidget.Index = slot.Index;
            SlotWidget.AllowSelect = slot.Object != null;

            if (!SlotWidget.gameObject.activeSelf)
            {
                SlotWidget.gameObject.SetActive(true);
            }

            if (SlotWidget.ObjectHolder == null)
            {
                var objectHolder = ObjectHolderItem.Create(slot.Object, m_objectHolderPrefab);
                objectHolder.SlotWidget = SlotWidget;
                SlotWidget.ObjectHolder = objectHolder;
                SlotWidget.SetObjectHolderPosition(objectHolder, true);
            }
            else
            {
                SlotWidget.ObjectHolder.SetObject(slot.Object);
            }
        }
    }
}
