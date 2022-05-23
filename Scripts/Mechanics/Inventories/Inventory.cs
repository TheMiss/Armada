using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Armageddon.Mechanics.Inventories
{
    /// <summary>
    ///     IMPORTANT: This must not be confused with CombatEntityInventory!
    ///     Represents a handler that can be used to present a collection of slots.
    /// </summary>
    public class Inventory : GameAccessibleObject
    {
        private List<InventorySlot> m_slots = new();

        public Inventory(bool autoAddSlot)
        {
            AutoAddSlot = autoAddSlot;
        }

        public IReadOnlyList<InventorySlot> Slots
        {
            set => m_slots = value as List<InventorySlot>;
            get => m_slots;
        }

        public bool AutoAddSlot { get; }

        public int AvailableSlotCount => m_slots.Count(slot => slot.Object == null);

        public int UsedSlotCount => m_slots.Count(slot => slot.Object != null);

        public event EventHandler<ObjectInsertedArgs> ObjectInserted;
        public event EventHandler<ObjectUpdatedArgs> ObjectUpdated;
        public event EventHandler<ObjectRemovedArgs> ObjectRemoved;
        public event EventHandler<SlotSelectedArgs> SlotSelected;
        public event EventHandler<SlotDeselectedArgs> SlotDeselected;
        public event EventHandler<SlotAddedArgs> SlotAdded;
        public event EventHandler<SlotRemovedArgs> SlotRemoved;
        public event EventHandler<SlotsClearedArgs> SlotsCleared;

        public void ClearSlots()
        {
            m_slots.Clear();
            SlotsCleared?.Invoke(this, new SlotsClearedArgs());
        }

        public void InsertObject(IInventoryObject obj, int index = 0)
        {
            if (AutoAddSlot)
            {
                if (AvailableSlotCount == 0)
                {
                    AddSlotRange(1);
                }
            }

            InventorySlot slot = null;
            if (m_slots[index].Object == null)
            {
                m_slots[index].Object = obj;

                slot = m_slots[index];
            }
            else
            {
                bool foundAvailableSlot = false;
                for (int i = 0; i < m_slots.Count; i++)
                {
                    if (m_slots[i].Object == null)
                    {
                        index = i;
                        m_slots[index].Object = obj;

                        slot = m_slots[index];
                        foundAvailableSlot = true;
                        break;
                    }
                }

                if (!foundAvailableSlot)
                {
                    Debug.LogWarning("SlotsHandler is full.");
                    return;
                }
            }

            ObjectInserted?.Invoke(this, new ObjectInsertedArgs(slot));
        }

        public void InsertOrUpdateObject(IInventoryObject obj)
        {
            InventorySlot foundSlot = Slots.FirstOrDefault(slot => slot.Object != null &&
                slot.Object.InstanceId == obj.InstanceId);

            if (foundSlot != null)
            {
                foundSlot.Object = obj;
                ObjectUpdated?.Invoke(this, new ObjectUpdatedArgs(foundSlot));
            }
            else
            {
                InsertObject(obj);
            }
        }

        /// <summary>
        ///     Switching SlotManager will be better off with RemoveObjectAt()
        /// </summary>
        /// <param name="obj"></param>
        public int RemoveObject(IInventoryObject obj)
        {
            for (int i = 0; i < m_slots.Count; i++)
            {
                if (m_slots[i].Object == null)
                {
                    continue;
                }

                if (m_slots[i].Object.Equals(obj))
                {
                    m_slots[i].Object = null;

                    ObjectRemoved?.Invoke(this, new ObjectRemovedArgs(m_slots[i]));
                    return i;
                }
            }

            return -1;
        }

        public void RemoveObjectAt(int index, object sender = null)
        {
            for (int i = 0; i < m_slots.Count; i++)
            {
                if (m_slots[i].Object == null)
                {
                    continue;
                }

                if (m_slots[i].Index == index)
                {
                    ObjectRemoved?.Invoke(sender, new ObjectRemovedArgs(m_slots[i]));
                    m_slots[i].Object = null;

                    break;
                }
            }
        }

        public void AddSlotRange(int slotCount)
        {
            int maxSlotCount = m_slots.Count + slotCount;

            while (m_slots.Count < maxSlotCount)
            {
                int index = m_slots.Count;
                var slot = new InventorySlot(this, index);

                m_slots.Add(slot);
            }

            SlotAdded?.Invoke(this, new SlotAddedArgs(slotCount));
        }

        public void RemoveSlotAt(int slotIndex)
        {
            bool slotRemoved = false;
            for (int i = m_slots.Count - 1; i >= 0; i--)
            {
                if (m_slots[i].Index == slotIndex)
                {
                    m_slots.RemoveAt(i);
                    slotRemoved = true;
                    break;
                }
            }

            if (!slotRemoved)
            {
                return;
            }

            // Reindex
            for (int i = 0; i < m_slots.Count; i++)
            {
                m_slots[i].Index = i;
            }

            SlotRemoved?.Invoke(this, new SlotRemovedArgs(slotIndex));
        }

        public void SelectSlot(int slotIndex, int clickCount = 1, bool raiseEvent = true)
        {
            InventorySlot slot = m_slots[slotIndex];

            if (slot != null)
            {
                slot.State = SlotState.Selected;

                if (raiseEvent)
                {
                    SlotSelected?.Invoke(this, new SlotSelectedArgs(slot, clickCount));
                }
            }

            // Slot slot = m_slots[slotIndex];
            //
            // if (slot != null)
            // {
            //     slot.State = SlotState.Selected;
            //     SlotSelected?.Invoke(this, new SlotSelectedArgs(slot));
            //
            //     // This doesn't allow reselect
            //     // if (slot.ChangeState(SlotState.Selected))
            //     // {
            //     //     ObjectSelected?.Invoke(this, new ObjectSelectedArgs(slot));
            //     // }
            // }
        }

        public void SelectSlot(IInventoryObject obj, int clickCount = 1, bool raiseEvent = true)
        {
            InventorySlot slot = GetSlot(obj);

            if (slot != null)
            {
                slot.State = SlotState.Selected;

                if (raiseEvent)
                {
                    SlotSelected?.Invoke(this, new SlotSelectedArgs(slot, clickCount));
                }
            }
        }

        public void DeselectObject(int slotIndex, bool raiseEvent = true)
        {
            InventorySlot slot = m_slots[slotIndex];

            if (slot is { State: SlotState.Selected })
            {
                slot.State = SlotState.Deselected;

                if (raiseEvent)
                {
                    SlotDeselected?.Invoke(this, new SlotDeselectedArgs(slot));
                }
            }
        }

        public void DeselectAll(bool raiseEvent = true)
        {
            for (int i = 0; i < m_slots.Count; i++)
            {
                DeselectObject(i, raiseEvent);
            }
        }

        public InventorySlot GetSlotAt(int index)
        {
            return Slots.FirstOrDefault(slot => slot.Index == index);
        }

        public InventorySlot GetSlot(IInventoryObject obj)
        {
            return Slots.FirstOrDefault(slot => slot.Object == obj);
        }

        public InventorySlot GetEmptySlot()
        {
            InventorySlot emptySlot = m_slots.FirstOrDefault(x => x.Object == null);

            if (emptySlot != null)
            {
                return emptySlot;
            }

            AddSlotRange(1);

            return m_slots[m_slots.Count - 1];
        }

        public static void Move(InventorySlot source, InventorySlot target)
        {
            if (source == null)
            {
                Debug.LogError("Move: source == null");
                return;
            }

            IInventoryObject obj = source.Object;
            SlotState tempSourceSlotState = source.State;

            source.Inventory.RemoveObjectAt(source.Index);
            source.State = SlotState.Deselected;

            target.Inventory.InsertObject(obj, target.Index);
            target.State = tempSourceSlotState;
        }

        public void ShiftAllObjects(bool removeEmptySlots = true)
        {
            for (int i = 0; i < Slots.Count - 1; i++)
            {
                InventorySlot currentSlot = Slots[i];

                int nextIndex = i + 1;

                if (nextIndex >= Slots.Count)
                {
                    break;
                }

                InventorySlot nextSlot = Slots[nextIndex];
                Move(nextSlot, currentSlot);

                // if (currentSlot.Object != null)
                // {
                //     continue;
                // }

                // for (int j = i + 1; j <= Slots.Count; j++)
                // {
                //     InventorySlot nextSlot = Slots[j];
                //     if (nextSlot.Object == null)
                //     {
                //         continue;
                //     }
                //         
                //     Move(nextSlot, currentSlot);
                //     break;
                // }
            }

            // for (int i = 0; i < Slots.Count; i++)
            // {
            //     InventorySlot currentSlot = Slots[i];
            //
            //     if (currentSlot.Object != null)
            //     {
            //         continue;
            //     }
            //
            //     for (int j = i + 1; j < Slots.Count; j++)
            //     {
            //         InventorySlot nextSlot = Slots[j];
            //         if (nextSlot.Object == null)
            //         {
            //             continue;
            //         }
            //             
            //         Move(nextSlot, currentSlot);
            //         break;
            //     }
            // }
        }

        // public void ShiftAllObjects(bool removeEmptySlots = true)
        // {
        //     // for (int i = 0; i < Slots.Count - 1; i++)
        //     // {
        //     //     InventorySlot currentSlot = Slots[i];
        //     //
        //     //     if (currentSlot.Object != null)
        //     //     {
        //     //         continue;
        //     //     }
        //     //
        //     //     for (int j = i + 1; j < Slots.Count; j++)
        //     //     {
        //     //         InventorySlot nextSlot = Slots[j];
        //     //         if (nextSlot.Object == null)
        //     //         {
        //     //             continue;
        //     //         }
        //     //             
        //     //         Move(nextSlot, currentSlot);
        //     //     }
        //     // }
        //
        //     if (removeEmptySlots)
        //     {
        //         for (int i = Slots.Count - 1; i >= 0; i--)
        //         {
        //             if (Slots[i].Object == null)
        //             {
        //                 RemoveSlotAt(i);
        //             }
        //         }
        //
        //         // for (int i = 0; i < Slots.Count - 1; i++)
        //         // {
        //         //     if (Slots[i].Object == null)
        //         //     {
        //         //         RemoveSlotAt(i);
        //         //     }
        //         // }
        //     }
        // }
    }
}
