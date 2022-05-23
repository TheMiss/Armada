using System;
using System.Linq;
using Armageddon.UI.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common.InventoryModule
{
    public class InventoryItemFilterBar : Widget
    {
        public enum ItemFilterType
        {
            All = 0,
            PrimaryWeapon = 1,
            SecondaryWeapon = 2,
            Kernel = 3,
            Armor = 4,
            Accessory = 5,
            Companion = 6,
            Consumable = 7,
            Skin = 8,
            Card = 9,
            LootBox = 10,
            Misc = 11
        }

        [SerializeField]
        private Toggle[] m_toggles;
        
        [SerializeField]
        private InventoryPanel m_inventoryPanel;

        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (Toggle toggle in m_toggles)
            {
                toggle.isOn = false;

                string typeName = toggle.name.Replace("Toggle", string.Empty);
                var filterType = (ItemFilterType)Enum.Parse(typeof(ItemFilterType), typeName);
                toggle.onValueChanged.AddListener(value =>
                {
                    if (value)
                    {
                        OnFilterTypeChanged(filterType);
                    }
                    else
                    {
                        Toggle activeFilter = m_toggles.FirstOrDefault(x => x.isOn);

                        if (activeFilter == null)
                        {
                            OnFilterTypeChanged(ItemFilterType.All);
                        }
                    }
                });
            }
        }

        protected override void OnDisable()
        {
            foreach (Toggle toggle in m_toggles)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }

            base.OnDisable();
        }

        [Button]
        private void RefreshToggles()
        {
            m_toggles = GetComponentsInChildren<Toggle>();

            foreach (Toggle toggle in m_toggles)
            {
                string typeName = toggle.name.Replace("Toggle", string.Empty);

                try
                {
                    var filterType = (ItemFilterType)Enum.Parse(typeof(ItemFilterType), typeName);
                    Debug.Log($"filterType = {filterType}");
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e.Message);
                }
            }
        }

        private void OnFilterTypeChanged(ItemFilterType type)
        {
            Debug.Log($"Selected {type}");

            m_inventoryPanel.DeactivateSlotByItemFilterType(type);
            m_inventoryPanel.ResetScrollBar();
        }
    }
}
