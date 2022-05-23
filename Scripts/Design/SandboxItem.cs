using System;
using Armageddon.AssetManagement;
using Armageddon.Backend.Payloads;
using Armageddon.Extensions;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Items;
using Armageddon.Sheets.Items;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Armageddon.Design
{
    [Serializable]
    public class SandboxItem : MonoBehaviour
    {
        private const string BasePath = "Design";
        public const string ItemObjectsPath = BasePath + "/ItemObjects";

        [OnValueChanged(nameof(OnStatsChanged))]
        [ShowIf(nameof(ShowKernel))]
        public double Dexterity = 1;

        [OnValueChanged(nameof(OnStatsChanged))]
        [ShowIf(nameof(ShowKernel))]
        public double Vitality = 1;

        [OnValueChanged(nameof(OnStatsChanged))]
        [ShowIf(nameof(ShowKernel))]
        public double Perception = 1;

        [OnValueChanged(nameof(OnStatsChanged))]
        [ShowIf(nameof(ShowKernel))]
        public double Leadership = 1;

        [OnValueChanged(nameof(OnStatsChanged))]
        [ShowIf(nameof(ShowArmor))]
        public double Armor;

        [OnValueChanged(nameof(OnStatsChanged))]
        [ShowIf(nameof(ShowWeapon))]
        public double Dps = 1;

        [OnValueChanged(nameof(OnStatsChanged))]
        [ShowIf(nameof(ShowWeapon))]
        public double FireRate = 1;

        [OnValueChanged(nameof(OnStatsChanged))]
        [ShowIf(nameof(ShowCompanion))]
        public float CompanionDps = 1;

        [OnValueChanged(nameof(OnStatsChanged))]
        [ShowIf(nameof(ShowCompanion))]
        public float CompanionFireRate = 1;

        [FormerlySerializedAs("ItemObject")]
        [PropertyOrder(100)]
        public ItemPayload ItemPayload;

        private ItemSheet m_sheet;

        public ItemType ItemType => Sheet.Type;

        [PropertyOrder(-100)]
        [ShowAsString]
        public long InstanceId => ItemPayload.InstanceId.ToInt64();

        [InlineEditor]
        [PropertyOrder(-100)]
        [ShowInInspector]
        public ItemSheet Sheet
        {
            get
            {
                if (m_sheet == null)
                {
                    m_sheet = LoadItemSheet(ItemPayload.SheetId);

                    if (m_sheet != null)
                    {
                        SetName();
                    }
                }

                if (m_sheet.Id != ItemPayload.SheetId)
                {
                    m_sheet = LoadItemSheet(ItemPayload.SheetId);

                    if (m_sheet != null)
                    {
                        SetName();
                    }
                }

                if (m_sheet == null)
                {
                    Debug.LogWarning($"Could not find ItemSheet {ItemPayload.SheetId}. Using the default one.");
                    m_sheet = LoadItemSheet(0);

                    SetName();
                }

                return m_sheet;
            }
        }

        public bool ShowWeapon => ItemType == ItemType.PrimaryWeapon || ItemType == ItemType.SecondaryWeapon;
        public bool ShowKernel => ItemType == ItemType.Kernel;
        public bool ShowArmor => ItemType == ItemType.Armor;
        public bool ShowCompanion => ItemType == ItemType.Companion;

        private void SetName()
        {
            name = $"{InstanceId}({m_sheet.Name})";
        }

        public void ForceSetName()
        {
            ItemSheet sheet = Sheet;
        }

        [ButtonGroup]
        [PropertyOrder(-200)]
        [GUIColorDefaultButton]
        public void Load()
        {
            if (ItemPayload == null)
            {
                return;
            }

            switch (ItemType)
            {
                case ItemType.PrimaryWeapon:
                case ItemType.SecondaryWeapon:

                    Dps = ItemPayload.DamagePerSecond.Value;
                    FireRate = ItemPayload.FireRate.Value;
                    break;
                case ItemType.Kernel:

                    Dexterity = ItemPayload.DexterityMultiplier.Value;
                    Vitality = ItemPayload.VitalityMultiplier.Value;
                    Perception = ItemPayload.PerceptionMultiplier.Value;
                    Leadership = ItemPayload.LeadershipMultiplier.Value;

                    break;
                case ItemType.Armor:
                    Armor = ItemPayload.Armor.Value;
                    break;
            }
        }

        [ButtonGroup]
        [PropertyOrder(-200)]
        [GUIColorDefaultButton]
        public void Save()
        {
            string path = $"{ItemObjectsPath}/{ItemPayload.InstanceId.ToInt64()}.json";
            DeviceFile.WriteObjectToJson(path, ItemPayload);
        }

        private static ItemSheet LoadItemSheet(int sheetId)
        {
#if UNITY_EDITOR
            string itemSheetPath = $"{Assets.ItemsPath}/{nameof(ItemSheet)}{sheetId}.asset";
            return AssetDatabase.LoadAssetAtPath<ItemSheet>(itemSheetPath);
#else
            return null;
#endif
        }

        private void OnStatsChanged()
        {
            switch (ItemType)
            {
                case ItemType.PrimaryWeapon:
                case ItemType.SecondaryWeapon:
                    ItemPayload.DamagePerSecond.Value = Dps;
                    ItemPayload.FireRate.Value = FireRate;

                    break;
                case ItemType.Kernel:
                    ItemPayload.DexterityMultiplier.Value = Dexterity;
                    ItemPayload.VitalityMultiplier.Value = Vitality;
                    ItemPayload.PerceptionMultiplier.Value = Perception;
                    ItemPayload.LeadershipMultiplier.Value = Leadership;

                    break;
                case ItemType.Armor:
                    ItemPayload.Armor.Value = Armor;

                    break;
            }
        }
    }
}
