using System.Collections.Generic;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.UI.Common.InventoryModule.Slot;
using UnityEngine;

namespace Armageddon.UI.Common.InventoryModule
{
    public static class InventoryWindowExtensions
    {
        public static List<EquipmentSlotType> GetEquipSlotTypesByItem(this InventoryPanel inventoryPanel,
            Item item)
        {
            var slotTypes = new List<EquipmentSlotType>();

            switch (item.Type)
            {
                case ItemType.PrimaryWeapon:
                    slotTypes.Add(EquipmentSlotType.PrimaryWeapon);
                    break;
                case ItemType.SecondaryWeapon:
                    slotTypes.Add(EquipmentSlotType.SecondaryWeapon);
                    break;
                case ItemType.Kernel:
                    slotTypes.Add(EquipmentSlotType.Kernel);
                    break;
                case ItemType.Armor:
                    slotTypes.Add(EquipmentSlotType.Armor);
                    break;
                case ItemType.Accessory:
                    slotTypes.Add(EquipmentSlotType.AccessoryLeft);
                    slotTypes.Add(EquipmentSlotType.AccessoryRight);
                    break;
                case ItemType.Companion:
                    slotTypes.Add(EquipmentSlotType.CompanionLeft);
                    slotTypes.Add(EquipmentSlotType.CompanionRight);
                    break;

                default:
                    Debug.LogWarning($"GetEquipSlotTypesByItem: {item.Type} is not available!");
                    break;
            }

            return slotTypes;
        }

        public static void HighlightSlotsByItemType(this InventoryPanel inventoryPanel, ItemType itemType,
            float otherTypesAlpha = 0.3f, float duration = 0.5f)
        {
            foreach (SlotWidget slotWidget in inventoryPanel.SlotWidgets)
            {
                if (slotWidget.ObjectHolder == null)
                {
                    slotWidget.SetAlpha(otherTypesAlpha, duration);
                    continue;
                }

                if (slotWidget.ObjectHolder.Object is Item item)
                {
                    if (item.Type == itemType)
                    {
                        slotWidget.SetAlpha(1.0f, 0f);
                    }
                    else
                    {
                        slotWidget.SetAlpha(otherTypesAlpha, duration);
                    }
                }
            }
        }

        public static void ShowSlotMarkersByItem(this InventoryPanel inventoryPanel, Item item)
        {
            List<EquipmentSlotType> slotTypes = inventoryPanel.GetEquipSlotTypesByItem(item);
            inventoryPanel.ShowSlotMarkersBySlotTypes(slotTypes);
        }

        public static void ShowSlotMarkersByItemType(this InventoryPanel inventoryPanel, Item selectedItem)
        {
            ItemType itemType = selectedItem.Type;

            foreach (SlotWidget slotWidget in inventoryPanel.SlotWidgets)
            {
                if (slotWidget.ObjectHolder == null)
                {
                    continue;
                }

                if (slotWidget.ObjectHolder.Object is Item item)
                {
                    if (item.Type == itemType && selectedItem != item)
                    {
                        slotWidget.ShowMarker(true);
                    }
                    else
                    {
                        slotWidget.ShowMarker(false);
                    }
                }
            }
        }

        public static void ShowSlotMarkersBySlotTypes(this InventoryPanel inventoryPanel,
            List<EquipmentSlotType> slotTypes)
        {
            foreach (SlotWidget slotWidget in inventoryPanel.SlotWidgets)
            {
                slotWidget.ShowMarker(slotTypes.Contains((EquipmentSlotType)slotWidget.Slot.Index));
            }
        }

        public static void HighlightSlot(this InventoryPanel inventoryPanel, Item highlightedItem,
            float otherAlpha = 0.5f, float duration = 0.3f)
        {
            foreach (SlotWidget slotWidget in inventoryPanel.SlotWidgets)
            {
                if (slotWidget.ObjectHolder == null)
                {
                    slotWidget.SetAlpha(otherAlpha, duration);
                    continue;
                }

                if (slotWidget.ObjectHolder.Object is Item item)
                {
                    if (item == highlightedItem)
                    {
                        slotWidget.SetAlpha(1.0f, 0f);
                    }
                    else
                    {
                        slotWidget.SetAlpha(otherAlpha, duration);
                    }
                }
            }
        }

        public static void SetAllowSelectSlotByItem(this InventoryPanel inventoryPanel, Item item)
        {
            foreach (SlotWidget slotWidget in inventoryPanel.SlotWidgets)
            {
                if (slotWidget.ObjectHolder != null)
                {
                    slotWidget.AllowSelect = slotWidget.ObjectHolder.Object == item;
                }
                else
                {
                    slotWidget.AllowSelect = false;
                }
            }
        }

        public static void SetAllowSelectSlotsByItemType(this InventoryPanel inventoryPanel, Item selectedItem)
        {
            ItemType itemType = selectedItem.Type;

            foreach (SlotWidget slotWidget in inventoryPanel.SlotWidgets)
            {
                if (slotWidget.ObjectHolder == null)
                {
                    continue;
                }

                if (slotWidget.ObjectHolder.Object is Item item)
                {
                    slotWidget.AllowSelect = item.Type == itemType;
                }
            }
        }

        public static void DeactivateSlotByItemFilterType(this InventoryPanel inventoryPanel,
            InventoryItemFilterBar.ItemFilterType filterType)
        {
            foreach (SlotWidget slotWidget in inventoryPanel.SlotWidgets)
            {
                ObjectHolder objectHolder = slotWidget.ObjectHolder;

                if (objectHolder != null)
                {
                    if (objectHolder.Object is Item item)
                    {
                        if (filterType == InventoryItemFilterBar.ItemFilterType.All ||
                            item.Type == (ItemType)filterType - 1)
                        {
                            slotWidget.gameObject.SetActive(true);
                        }
                        else
                        {
                            slotWidget.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}
