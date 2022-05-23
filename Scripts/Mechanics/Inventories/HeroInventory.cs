using System;
using Armageddon.Backend;
using Armageddon.Backend.Functions;
using Armageddon.Backend.Payloads;
using Armageddon.Mechanics.Items;
using Cysharp.Threading.Tasks;
using Purity.Common;
using UnityEngine;

namespace Armageddon.Mechanics.Inventories
{
    /// <summary>
    ///     IMPORTANT: This must not be confused with CombatEntityInventory!
    ///     You can think of this as Hero Equipment.
    ///     Also, this is used to manage items in inventory between client and server.
    ///     CombatEntityInventory will be used for in game inventory for all combat entities.
    /// </summary>
    public class HeroInventory : Inventory
    {
        public HeroInventory() : base(false)
        {
        }

        public void InitializeFixedSlots(PlayerInventory playerInventory, HeroInventoryPayload inventoryPayload)
        {
            ClearSlots();

            int slotSize = EnumEx.GetSize<EquipmentSlotType>();
            // int slotSize = Enum.GetNames(typeof(EquipmentSlotType)).Length;

            AddSlotRange(slotSize);

            int index = 0;
            foreach (SlotPayload slot in inventoryPayload.Slots)
            {
                if (slot == null)
                {
                    index++;
                    continue;
                }

                string instanceId = slot.InstanceId;
                Item item = playerInventory.GetItem(instanceId);

                // Let's swap
                InsertObject(item, index);
                playerInventory.RemoveObject(item);

                index++;
            }
        }

        public bool CheckIntegrity(HeroInventoryPayload inventoryPayload)
        {
            bool slotsChanged = false;
            int index = 0;
            foreach (SlotPayload slotObject in inventoryPayload.Slots)
            {
                if (slotObject == null)
                {
                    index++;
                    continue;
                }

                InventorySlot slot = GetSlotAt(index);
                if (slot.ReferenceId != slotObject.InstanceId)
                {
                    Debug.LogWarning($"{slot.ReferenceId} != {slotObject.InstanceId}");
                    slotsChanged = true;
                }

                index++;
            }

            return !slotsChanged;
        }

        public InventorySlot GetEquippedSlot(Item item)
        {
            foreach (InventorySlot slot in Slots)
            {
                if (slot.Object == item)
                {
                    return slot;
                }
            }

            return null;
        }

        /// <summary>
        ///     Locally check before asking the backend
        /// </summary>
        public bool CanEquip(int slotIndex, Item item)
        {
            if (!Game.CheckRequirementBeforeRequest)
            {
                return true;
            }

            if (!item.IsEquipable)
            {
                Debug.LogWarning($"Trying to equip non equipable {item.Type} ({item.InstanceId})");
                return false;
            }

            var slotType = (EquipmentSlotType)slotIndex;

            return item.Type switch
            {
                ItemType.PrimaryWeapon when slotType == EquipmentSlotType.PrimaryWeapon => true,
                ItemType.SecondaryWeapon when slotType == EquipmentSlotType.SecondaryWeapon => true,
                ItemType.Kernel when slotType == EquipmentSlotType.Kernel => true,
                ItemType.Armor when slotType == EquipmentSlotType.Armor => true,
                ItemType.Accessory when slotType == EquipmentSlotType.AccessoryLeft ||
                    slotType == EquipmentSlotType.AccessoryRight => true,
                ItemType.Companion when slotType == EquipmentSlotType.CompanionLeft ||
                    slotType == EquipmentSlotType.CompanionRight => true,
                _ => false
            };
        }

        public async UniTask<bool> EquipItemAsync(InventorySlot source, InventorySlot target)
        {
            if (target.Inventory is HeroInventory)
            {
                var item = (Item)source.Object;
                var slotType = (EquipmentSlotType)target.Index;

                var request = new EquipHeroItemRequest
                {
                    ItemInstanceId = item.InstanceId,
                    SlotType = slotType
                };

                EquipHeroItemReply reply = await Game.BackendDriver.EquipHeroItemAsync(request);

                if (!Game.ValidateReply(reply))
                {
                    return false;
                }

                InventorySlot freeSlot = source.Inventory.GetEmptySlot();

                // Move the previous equipped fist if any.
                if (target.Object != null)
                {
                    Move(target, freeSlot);
                }

                Move(source, target);
            }
            else
            {
                Debug.LogError($"{target.Inventory} is not HeroInventory");
            }

            return true;
        }

        public async UniTask<bool> UnequipItemAsync(InventorySlot source, InventorySlot target)
        {
            if (source.Inventory is HeroInventory && target.Inventory is PlayerInventory)
            {
                var slotType = (EquipmentSlotType)source.Index;
                var request = new UnequipHeroItemRequest
                {
                    SlotType = (int)slotType
                };

                UnequipHeroItemReply reply = await Game.BackendDriver.UnequipHeroItemAsync(request);

                if (!Game.ValidateReply(reply))
                {
                    return false;
                }

                switch (reply.Result)
                {
                    case UnequipHeroItemResult.Success:
                    {
                        InventorySlot freeSlot = target.Inventory.GetEmptySlot();
                        Move(source, freeSlot);

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                Debug.LogError("UnequipItemAsync: " +
                    "!(source.SlotManager is HeroInventory && target.SlotManager is PlayerInventory)");
                return false;
            }

            return true;
        }

        public Item GetItemAtSlot(EquipmentSlotType slotType)
        {
            return (Item)Slots[(int)slotType].Object;
        }
    }
}
