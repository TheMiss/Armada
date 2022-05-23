using System;

namespace Armageddon.Mechanics.Inventories
{
    public enum SlotState
    {
        Deselected,
        Selected
    }

    public class SlotStateChangedArgs : EventArgs
    {
        public SlotStateChangedArgs(SlotState state)
        {
            State = state;
        }

        public SlotState State { get; }
    }

    public class SlotIndexChangedArgs : EventArgs
    {
        public SlotIndexChangedArgs(int newIndex)
        {
            NewIndex = newIndex;
        }

        public int NewIndex { get; }
    }

    public class InventorySlot
    {
        private int m_index;

        private SlotState m_state;

        public InventorySlot(Inventory inventory, int index)
        {
            Inventory = inventory;
            m_index = index;
        }

        public Inventory Inventory { set; get; }

        public int Index
        {
            set => ChangeIndex(value);
            get => m_index;
        }

        public SlotState State
        {
            set => ChangeState(value);
            get => m_state;
        }

        public string ReferenceId => Object?.InstanceId;

        public IInventoryObject Object { set; get; }

        private bool ChangeState(SlotState state)
        {
            if (m_state == state)
            {
                return false;
            }

            m_state = state;
            StateChanged?.Invoke(this, new SlotStateChangedArgs(m_state));

            return true;
        }

        private bool ChangeIndex(int index)
        {
            if (m_index == index)
            {
                return false;
            }

            m_index = index;
            IndexChanged?.Invoke(this, new SlotIndexChangedArgs(m_index));

            return true;
        }

        public event EventHandler<SlotStateChangedArgs> StateChanged;
        public event EventHandler<SlotIndexChangedArgs> IndexChanged;
    }
}
