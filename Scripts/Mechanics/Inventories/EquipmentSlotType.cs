using Armageddon.Backend.Attributes;

namespace Armageddon.Mechanics.Inventories
{
    [Exchange(AssignEnumValue = true)]
    public enum EquipmentSlotType
    {
        PrimaryWeapon = 0,
        SecondaryWeapon = 1,
        Kernel = 2,
        Armor = 3,
        AccessoryLeft = 4,
        AccessoryRight = 5,
        CompanionLeft = 6,
        CompanionRight = 7
    }
}
