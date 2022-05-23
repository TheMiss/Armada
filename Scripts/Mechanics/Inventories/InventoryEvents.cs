using System;

namespace Armageddon.Mechanics.Inventories
{
    public class ObjectInsertedArgs : EventArgs
    {
        public ObjectInsertedArgs(InventorySlot slot)
        {
            Slot = slot;
            Object = slot.Object;
        }

        public InventorySlot Slot { get; }
        public IInventoryObject Object { get; }
    }

    public class ObjectUpdatedArgs : EventArgs
    {
        public ObjectUpdatedArgs(InventorySlot slot)
        {
            Slot = slot;
            Object = slot.Object;
        }

        public InventorySlot Slot { get; }
        public IInventoryObject Object { get; }
    }

    public class ObjectRemovedArgs : EventArgs
    {
        public ObjectRemovedArgs(InventorySlot slot)
        {
            Slot = slot;
            Object = slot.Object;
        }

        public InventorySlot Slot { get; }
        public IInventoryObject Object { get; }
    }

    public class SlotSelectedArgs : EventArgs
    {
        public SlotSelectedArgs(InventorySlot slot, int clickCount)
        {
            Slot = slot;
            Object = slot.Object;
            ClickCount = clickCount;
        }

        public InventorySlot Slot { get; }
        public IInventoryObject Object { get; }
        public int ClickCount { get; }
    }

    public class SlotDeselectedArgs : EventArgs
    {
        public SlotDeselectedArgs(InventorySlot slot)
        {
            Slot = slot;
            Object = slot.Object;
        }

        public InventorySlot Slot { get; }
        public IInventoryObject Object { get; }
    }

    public class ObjectsClearedArgs : EventArgs
    {
    }

    public class SlotAddedArgs : EventArgs
    {
        public SlotAddedArgs(int addedSlotCount)
        {
            AddedSlotCount = addedSlotCount;
        }

        public int AddedSlotCount { get; }
    }

    public class SlotRemovedArgs : EventArgs
    {
        public SlotRemovedArgs(int removedIndex)
        {
            RemovedIndex = removedIndex;
        }

        public int RemovedIndex { get; }
    }

    public class SlotsClearedArgs : EventArgs
    {
    }
}
