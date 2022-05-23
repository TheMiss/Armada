using Armageddon.Backend.Attributes;
using Armageddon.Mechanics.Inventories;

namespace Armageddon.Mechanics.Items
{
    [Exchange]
    public enum ItemType
    {
        /// <summary>
        ///     A non-stackable, equipable for Main Weapon slot.
        /// </summary>
        PrimaryWeapon = 0,

        /// <summary>
        ///     A non-stackable, equipable for Sub Weapon Slot.
        /// </summary>
        SecondaryWeapon = 1,

        /// <summary>
        ///     A non-stackable, equipable for Kernel slot.
        /// </summary>
        Kernel = 2,

        /// <summary>
        ///     A non-stackable, equipable for Armor slot.
        /// </summary>
        Armor = 3,

        /// <summary>
        ///     A non-stackable, equipable for Accessory slot.
        /// </summary>
        Accessory = 4,

        /// <summary>
        ///     A non-stackable, equipable for Companion slot.
        /// </summary>
        Companion = 5,

        /// <summary>
        ///     A stackable, one-time-use item.
        /// </summary>
        Consumable = 6,

        /// <summary>
        ///     A non-stackable one-time-use item for dying skin color of heroes.
        /// </summary>
        Skin = 7,

        /// <summary>
        ///     A sought-after non-stackable usable item that drops only from enemies.
        /// </summary>
        Card = 8,

        /// <summary>
        ///     A precious stackable loot box for redeeming precious items.
        /// </summary>
        Chest = 9,

        /// <summary>
        ///     A stackable item which can be sold or for handing out for a quest.
        /// </summary>
        Misc = 10
    }

    public static class ItemTypeExtensions
    {
        /// <summary>
        ///     A.k.a Gear or Equipment. Depends on how we should call it in the final decision.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public static bool IsEquipable(this ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.PrimaryWeapon:
                case ItemType.SecondaryWeapon:
                case ItemType.Armor:
                case ItemType.Kernel:
                case ItemType.Accessory:
                case ItemType.Companion:
                    return true;
            }

            return false;
        }

        public static bool IsUsable(this ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Consumable:
                case ItemType.Skin:
                case ItemType.Card:
                case ItemType.Chest:
                    return true;
            }

            return false;
        }

        public static bool IsStackable(this ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Consumable:
                case ItemType.Skin: // TODO: Should not be stackable as it will be treadable.
                case ItemType.Card:
                case ItemType.Chest:
                case ItemType.Misc:
                    return true;
            }

            return false;
        }

        public static bool CanEquipToSlotType(this ItemType itemType, EquipmentSlotType slotType)
        {
            return itemType switch
            {
                ItemType.PrimaryWeapon => slotType == EquipmentSlotType.PrimaryWeapon,
                ItemType.SecondaryWeapon => slotType == EquipmentSlotType.SecondaryWeapon,
                ItemType.Kernel => slotType == EquipmentSlotType.Kernel,
                ItemType.Armor => slotType == EquipmentSlotType.Armor,
                ItemType.Accessory => slotType == EquipmentSlotType.AccessoryLeft ||
                    slotType == EquipmentSlotType.AccessoryRight,
                ItemType.Companion => slotType == EquipmentSlotType.CompanionLeft ||
                    slotType == EquipmentSlotType.CompanionRight,
                _ => false
            };
        }
    }
}
