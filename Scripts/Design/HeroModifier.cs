using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.Worlds.Actors.Heroes;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Design
{
    public class HeroModifier : SandboxContext
    {
        public ItemFactory ItemFactory;

        [HideInEditorMode]
        public HeroActor HeroActor;

        [OnValueChanged(nameof(OnHeroLevelChange))]
        [Delayed]
        [MinValue(1)]
        public int HeroLevel;

        [Optional]
        [InlineButton(nameof(UnequipPrimaryWeapon), "Unequip")]
        [OnValueChanged(nameof(OnPrimaryWeaponChanged))]
        [ValueDropdown(nameof(GetPrimaryWeapons))]
        public SandboxItem PrimaryWeapon;

        [Optional]
        [InlineButton(nameof(UnequipSecondaryWeapon), "Unequip")]
        [OnValueChanged(nameof(OnSecondaryWeaponChanged))]
        [ValueDropdown(nameof(GetSecondaryWeapons))]
        public SandboxItem SecondaryWeapon;

        [Optional]
        [InlineButton(nameof(UnequipKernel), "Unequip")]
        [OnValueChanged(nameof(OnKernelChanged))]
        [ValueDropdown(nameof(GetKernels))]
        public SandboxItem Kernel;

        [Optional]
        [InlineButton(nameof(UnequipArmor), "Unequip")]
        [OnValueChanged(nameof(OnArmorChanged))]
        [ValueDropdown(nameof(GetArmors))]
        public SandboxItem Armor;

        [Optional]
        [InlineButton(nameof(UnequipAccessoryLeft), "Unequip")]
        [OnValueChanged(nameof(OnAccessoryLeftChanged))]
        [ValueDropdown(nameof(GetAccessoryLefts))]
        public SandboxItem AccessoryLeft;

        [Optional]
        [InlineButton(nameof(UnequipAccessoryRight), "Unequip")]
        [OnValueChanged(nameof(OnAccessoryRightChanged))]
        [ValueDropdown(nameof(GetAccessoryRights))]
        public SandboxItem AccessoryRight;

        [Optional]
        [InlineButton(nameof(UnequipCompanionLeft), "Unequip")]
        [OnValueChanged(nameof(OnCompanionLeftChanged))]
        [ValueDropdown(nameof(GetCompanionLefts))]
        public SandboxItem CompanionLeft;

        [Optional]
        [InlineButton(nameof(UnequipCompanionRight), "Unequip")]
        [OnValueChanged(nameof(OnCompanionRightChanged))]
        [ValueDropdown(nameof(GetCompanionRights))]
        public SandboxItem CompanionRight;

        private void OnPrimaryWeaponChanged()
        {
            EquipItemAsync(EquipmentSlotType.PrimaryWeapon, PrimaryWeapon).Forget();
        }

        private void UnequipPrimaryWeapon()
        {
            UnequipItem(EquipmentSlotType.PrimaryWeapon);
            PrimaryWeapon = null;
        }

        private ValueDropdownList<SandboxItem> GetPrimaryWeapons()
        {
            return GetSandboxItems(ItemType.PrimaryWeapon);
        }

        private void OnSecondaryWeaponChanged()
        {
            EquipItemAsync(EquipmentSlotType.SecondaryWeapon, SecondaryWeapon).Forget();
        }

        private void UnequipSecondaryWeapon()
        {
            UnequipItem(EquipmentSlotType.SecondaryWeapon);
            SecondaryWeapon = null;
        }

        private ValueDropdownList<SandboxItem> GetSecondaryWeapons()
        {
            return GetSandboxItems(ItemType.SecondaryWeapon);
        }

        private void OnKernelChanged()
        {
            EquipItemAsync(EquipmentSlotType.Kernel, Kernel).Forget();
        }

        private void UnequipKernel()
        {
            UnequipItem(EquipmentSlotType.Kernel);
            Kernel = null;
        }

        private ValueDropdownList<SandboxItem> GetKernels()
        {
            return GetSandboxItems(ItemType.Kernel);
        }

        private void OnArmorChanged()
        {
            EquipItemAsync(EquipmentSlotType.Armor, Armor).Forget();
        }

        private void UnequipArmor()
        {
            UnequipItem(EquipmentSlotType.Armor);
            Armor = null;
        }

        private ValueDropdownList<SandboxItem> GetArmors()
        {
            return GetSandboxItems(ItemType.Armor);
        }

        private void OnAccessoryLeftChanged()
        {
            EquipItemAsync(EquipmentSlotType.AccessoryLeft, AccessoryLeft).Forget();
        }

        private void UnequipAccessoryLeft()
        {
            UnequipItem(EquipmentSlotType.AccessoryLeft);
            AccessoryLeft = null;
        }

        private ValueDropdownList<SandboxItem> GetAccessoryLefts()
        {
            return GetSandboxItems(ItemType.Accessory);
        }

        private void OnAccessoryRightChanged()
        {
            EquipItemAsync(EquipmentSlotType.AccessoryRight, AccessoryRight).Forget();
        }

        private void UnequipAccessoryRight()
        {
            UnequipItem(EquipmentSlotType.AccessoryRight);
            AccessoryRight = null;
        }

        private ValueDropdownList<SandboxItem> GetAccessoryRights()
        {
            return GetSandboxItems(ItemType.Accessory);
        }

        private void OnCompanionLeftChanged()
        {
            EquipItemAsync(EquipmentSlotType.CompanionLeft, CompanionLeft).Forget();
        }

        private void UnequipCompanionLeft()
        {
            UnequipItem(EquipmentSlotType.CompanionLeft);
            CompanionLeft = null;
        }

        private ValueDropdownList<SandboxItem> GetCompanionLefts()
        {
            return GetSandboxItems(ItemType.Companion);
        }

        private void OnCompanionRightChanged()
        {
            EquipItemAsync(EquipmentSlotType.CompanionRight, CompanionRight).Forget();
        }

        private void UnequipCompanionRight()
        {
            UnequipItem(EquipmentSlotType.CompanionRight);
            CompanionRight = null;
        }

        private ValueDropdownList<SandboxItem> GetCompanionRights()
        {
            return GetSandboxItems(ItemType.Companion);
        }

        private async UniTask EquipItemAsync(EquipmentSlotType slotType, SandboxItem sandboxItem,
            bool compileStats = true)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (sandboxItem == null)
            {
                Debug.Log($"No selected item to equip to {slotType}.");
                return;
            }

            Item item = await ItemFactory.GetItem(sandboxItem.InstanceId);

            Debug.Log($"Equipping {item.InstanceId} to {slotType}");

            await HeroActor.EquipItemAsync(slotType, item);

            if (compileStats)
            {
                HeroActor.CompileStats();
            }
        }

        private void UnequipItem(EquipmentSlotType slotType, bool compileStats = true)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            HeroActor.UnequipItem(slotType);

            if (compileStats)
            {
                HeroActor.CompileStats();
            }
        }

        private ValueDropdownList<SandboxItem> GetSandboxItems(ItemType type)
        {
            var filteredInstanceIds = new ValueDropdownList<SandboxItem>();

            foreach (SandboxItem sandboxItem in ItemFactory.SandboxItems)
            {
                if (sandboxItem.Sheet.Type == type)
                {
                    filteredInstanceIds.Add($"{sandboxItem.name}", sandboxItem);
                }
            }

            return filteredInstanceIds;
        }

        [ButtonGroup]
        [PropertyOrder(-100)]
        [GUIColorDefaultButton]
        private async UniTask EquipAll()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            await EquipItemAsync(EquipmentSlotType.PrimaryWeapon, PrimaryWeapon, false);
            await EquipItemAsync(EquipmentSlotType.SecondaryWeapon, SecondaryWeapon, false);
            await EquipItemAsync(EquipmentSlotType.Kernel, Kernel, false);
            await EquipItemAsync(EquipmentSlotType.Armor, Armor, false);
            await EquipItemAsync(EquipmentSlotType.AccessoryLeft, AccessoryLeft, false);
            await EquipItemAsync(EquipmentSlotType.AccessoryRight, AccessoryRight, false);
            await EquipItemAsync(EquipmentSlotType.CompanionLeft, CompanionLeft, false);
            await EquipItemAsync(EquipmentSlotType.CompanionRight, CompanionRight, false);

            HeroActor.CompileStats();
        }

        [ButtonGroup]
        [PropertyOrder(-100)]
        [GUIColorDefaultButton]
        private void UnequipAll()
        {
            UnequipItem(EquipmentSlotType.PrimaryWeapon, false);
            UnequipItem(EquipmentSlotType.SecondaryWeapon, false);
            UnequipItem(EquipmentSlotType.Kernel, false);
            UnequipItem(EquipmentSlotType.Armor, false);
            UnequipItem(EquipmentSlotType.AccessoryLeft, false);
            UnequipItem(EquipmentSlotType.AccessoryRight, false);
            UnequipItem(EquipmentSlotType.CompanionLeft, false);
            UnequipItem(EquipmentSlotType.CompanionRight, false);

            HeroActor.CompileStats();
        }

        public void Initialize()
        {
            OnHeroLevelChange();
            EquipAll().Forget();
        }

        private void OnHeroLevelChange()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            HeroActor.CombatEntity.Level = HeroLevel;
            HeroActor.CombatEntity.CompileStats();
        }
    }
}
