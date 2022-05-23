using System.Collections.Generic;
using System.Linq;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.UI.Base;
using Armageddon.UI.Common.InventoryModule.Slot;
using UnityEngine;

namespace Armageddon.UI.Common.InventoryModule
{
    public enum SignType
    {
        Add,
        Minus,
        QuestionMark
    }

    public class SignData : IInventoryObject
    {
        public SignData(SignType type)
        {
            Type = type;
        }

        public SignType Type { get; }
        public string InstanceId => string.Empty;
    }

    public abstract class InventoryPanel : Widget
    {
        public int UsedSlotWidgetCount => SlotWidgets.Count(slotWidget => slotWidget.ObjectHolder != null);

        public Inventory Inventory { protected set; get; }

        public List<SlotWidget> SlotWidgets { get; } = new();

        protected override void OnEnable()
        {
            base.OnEnable();

            if (Inventory == null)
            {
                Debug.Log("SlotsHandler is null.");
                return;
            }

            Inventory.ObjectInserted += OnObjectInserted;
            Inventory.ObjectUpdated += OnObjectUpdated;
            Inventory.ObjectRemoved += OnObjectRemoved;
            Inventory.SlotAdded += OnSlotAdded;
            Inventory.SlotRemoved += OnSlotRemoved;
            Inventory.SlotSelected += OnSlotSelected;
            Inventory.SlotDeselected += OnSlotDeselected;
            Inventory.SlotsCleared += OnSlotsCleared;
        }

        protected override void OnDisable()
        {
            if (Inventory == null)
            {
                //Debug.Log("Inventory is null.");
                return;
            }

            Inventory.ObjectInserted -= OnObjectInserted;
            Inventory.ObjectUpdated -= OnObjectUpdated;
            Inventory.ObjectRemoved -= OnObjectRemoved;
            Inventory.SlotAdded -= OnSlotAdded;
            Inventory.SlotRemoved -= OnSlotRemoved;
            Inventory.SlotSelected -= OnSlotSelected;
            Inventory.SlotDeselected -= OnSlotDeselected;
            Inventory.SlotsCleared -= OnSlotsCleared;

            base.OnDisable();
        }

        public override void OnResourcesUnloading()
        {
            // SlotWidgets.Clear();
            // m_slotContentTransform.DestroyChildren();
        }

        private void Clear()
        {
        }

        protected virtual void OnObjectInserted(object sender, ObjectInsertedArgs e)
        {
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                if (slotWidget.ObjectHolder == null)
                {
                    continue;
                }

                if (slotWidget.Index == e.Slot.Index)
                {
                    slotWidget.ObjectHolder.Object = e.Object;

                    OnWillInsertObjectHolder(slotWidget, slotWidget.ObjectHolder);
                    break;
                }
            }
        }

        private void OnObjectUpdated(object sender, ObjectUpdatedArgs e)
        {
            UpdateObject(e.Slot.Object, e.Slot.Index);
        }

        protected virtual void OnObjectRemoved(object sender, ObjectRemovedArgs e)
        {
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                if (slotWidget.ObjectHolder == null)
                {
                    continue;
                }

                if (slotWidget.Index == e.Slot.Index)
                {
                    OnWillRemoveObjectHolder(slotWidget, slotWidget.ObjectHolder);
                    break;
                }
            }
        }

        protected virtual void OnSlotAdded(object sender, SlotAddedArgs e)
        {
            AddEmptySlots(e.AddedSlotCount);
        }

        private void OnSlotRemoved(object sender, SlotRemovedArgs e)
        {
            RemoveSlotWidget(e.RemovedIndex);
        }

        private void OnSlotSelected(object sender, SlotSelectedArgs e)
        {
            //    SetSelectedHero(e.Slot.Object, e.Slot.Index);
        }

        private void OnSlotDeselected(object sender, SlotDeselectedArgs e)
        {
            //    SetSelectedHero(e.Slot.Object, e.Slot.Index);
            SlotWidget slotWidget = GetSlotWidgetAt(e.Slot.Index);
            if (slotWidget != null)
            {
                slotWidget.AdjustState();
            }
        }

        private void OnSlotsCleared(object sender, SlotsClearedArgs e)
        {
            foreach (SlotWidget slot in SlotWidgets)
            {
                Destroy(slot.gameObject);
            }

            SlotWidgets.Clear();
        }

        protected void AddEmptySlots(int slotCount)
        {
            int maxSlotCount = SlotWidgets.Count + slotCount;

            while (SlotWidgets.Count < maxSlotCount)
            {
                int index = SlotWidgets.Count;

                SlotWidget slotWidget = CreateSlotWidget(index);
                SlotWidgets.Add(slotWidget);
            }
        }

        private bool RemoveSlotWidget(int removedIndex)
        {
            SlotWidget removingSlotWidget = SlotWidgets.FirstOrDefault(x => x.Index == removedIndex);

            if (removingSlotWidget != null)
            {
                SlotWidgets.Remove(removingSlotWidget);
                Destroy(removingSlotWidget.gameObject);

                return true;
            }

            return false;
        }

        protected virtual SlotWidget GetSlotWidgetPrefab()
        {
            Debug.Log("You must override GetSlotWidgetPrefab().");
            return null;
        }

        protected virtual RectTransform GetSlotContentTransform()
        {
            Debug.Log("You must override GetSlotContentTransform().");
            return null;
        }

        protected virtual SlotWidget CreateSlotWidget(int index)
        {
            SlotWidget slotWidget = Instantiate(GetSlotWidgetPrefab(), GetSlotContentTransform());
            slotWidget.InventoryPanel = this;
            slotWidget.Inventory = Inventory;
            slotWidget.Index = index;

            InventorySlot slot = Inventory.GetSlotAt(index);
            slotWidget.Slot = slot;

            return slotWidget;
        }

        public virtual void OnWillInsertObjectHolder(SlotWidget slotWidget, ObjectHolder objectHolder)
        {
        }

        private void RemoveEmptySlots()
        {
            var removingSlotWidgets = new List<SlotWidget>();
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                if (slotWidget.ObjectHolder == null)
                {
                    removingSlotWidgets.Add(slotWidget);
                    // RemoveSlotWidget(slotWidget.Index);
                }
            }

            foreach (SlotWidget removingSlotWidget in removingSlotWidgets)
            {
                RemoveSlotWidget(removingSlotWidget.Index);
            }
        }

        protected virtual SlotWidget UpdateObject(IInventoryObject obj, int index)
        {
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                if (slotWidget.Index == index)
                {
                    slotWidget.ObjectHolder.Object = obj;

                    ObjectHolder objectHolder = slotWidget.ObjectHolder;

                    if (objectHolder != null)
                    {
                        objectHolder.SetObject(obj);
                    }

                    return slotWidget;
                }
            }

            Debug.LogWarning("UpdateObject: Available slot not found!");

            return null;
        }

        public virtual void OnWillRemoveObjectHolder(SlotWidget slotWidget, ObjectHolder objectHolder)
        {
        }

        public ObjectHolder GetObjectHolder(Item item)
        {
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                if (slotWidget.ObjectHolder == null)
                {
                    continue;
                }

                if (slotWidget.ObjectHolder.Object == item)
                {
                    return slotWidget.ObjectHolder;
                }
            }

            return null;
        }

        public SlotWidget GetSlotWidget(Item item)
        {
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                if (slotWidget.ObjectHolder == null)
                {
                    continue;
                }

                if (slotWidget.ObjectHolder.Object == item)
                {
                    return slotWidget;
                }
            }

            return null;
        }

        public SlotWidget GetSlotWidget(InventorySlot slot)
        {
            return SlotWidgets.FirstOrDefault(uiSlot => uiSlot.Index == slot.Index);
        }

        public SlotWidget GetSlotWidgetAt(int index)
        {
            SlotWidget slotWidget = SlotWidgets.FirstOrDefault(x => x.Index == index);
            return slotWidget;
        }

        public SlotWidget GetEmptySlotWidget()
        {
            SlotWidget slotWidget = SlotWidgets.FirstOrDefault(x => x.ObjectHolder == null);
            return slotWidget;
        }

        public static void Swap(SlotWidget x, SlotWidget y)
        {
            IInventoryObject tempObjectX = null;
            ObjectHolder tempObjectHolderX = x.ObjectHolder;

            if (tempObjectHolderX != null)
            {
                tempObjectX = tempObjectHolderX.Object;
            }

            SlotState tempStateX = x.Slot.State;

            IInventoryObject tempObjectY = null;
            ObjectHolder tempObjectHolderY = y.ObjectHolder;

            if (tempObjectHolderY != null)
            {
                tempObjectY = tempObjectHolderY.Object;
            }

            SlotState tempStateY = y.Slot.State;

            if (tempObjectX != null)
            {
                x.Inventory.RemoveObject(tempObjectX);
            }

            if (tempObjectY != null)
            {
                y.Inventory.RemoveObject(tempObjectY);
            }

            if (tempObjectY != null)
            {
                x.Inventory.InsertObject(tempObjectY, x.Index);
            }

            // No longer swap
            // x.Slot.State = tempStateY;

            if (tempObjectX != null)
            {
                y.Inventory.InsertObject(tempObjectX, y.Index);
            }

            // No longer swap
            // y.Slot.State = tempStateX;
        }

        public static void Move(SlotWidget source, SlotWidget target)
        {
            if (source.ObjectHolder == null)
            {
                Debug.LogError("Move: source.ObjectHolder == null");
                return;
            }

            IInventoryObject obj = source.ObjectHolder.Object;
            SlotState tempSourceSlotState = source.Slot.State;

            source.Inventory.RemoveObjectAt(source.Index);
            source.Slot.State = SlotState.Deselected;

            target.Inventory.InsertObject(obj, target.Index);

            // No longer swap
            // destination.Slot.State = tempSourceSlotState;
        }

        public static void Remove(SlotWidget source)
        {
            if (source.ObjectHolder == null)
            {
                Debug.LogError("Remove: source.ObjectHolder == null");
                return;
            }

            source.Inventory.RemoveObjectAt(source.Index);
            source.Slot.State = SlotState.Deselected;
        }

        public virtual void ResetAllSlotStates()
        {
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                slotWidget.SetAlpha(1.0f);
                slotWidget.ShowMarker(false);
            }
        }

        public bool CanSwap(SlotWidget source, SlotWidget target)
        {
            if (source.ObjectHolder.Object is Item item)
            {
                if (target.Inventory is HeroInventory heroInventory)
                {
                    if (!heroInventory.CanEquip(target.Index, item))
                    {
                        Debug.LogWarning($"Can't equip {item.Type} into " +
                            $"{(EquipmentSlotType)target.Index}");
                        return false;
                    }
                }

                if (!item.IsEquipable)
                {
                    Debug.Log(
                        $"source.ObjectHolder.Object is not equipable [{item.InstanceId}.]");
                    return false;
                }
            }
            else
            {
                Debug.Log(
                    $"source.ObjectHolder.Object is not Item [{source.ObjectHolder.Object.GetType()}.]");
                return false;
            }

            return true;
        }

        public void DeselectAllSlots()
        {
            Inventory.DeselectAll();
        }

        public void UnhighlightSlots()
        {
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                slotWidget.SetAlpha(1.0f);
            }
        }

        public void AdjustSlotStates()
        {
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                slotWidget.AdjustState();
            }
        }

        public void HideSlotMarkers()
        {
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                slotWidget.ShowMarker(false);
            }
        }

        public void SetAllowSelectSlots(bool value)
        {
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                slotWidget.AllowSelect = value;
            }
        }

        public void ResetScrollBar()
        {
            var rectTransform = GetSlotContentTransform().GetComponent<RectTransform>();
            Vector2 anchoredPosition = rectTransform.anchoredPosition;
            anchoredPosition.y = 0.0f;
            rectTransform.anchoredPosition = anchoredPosition;
        }
    }
}
