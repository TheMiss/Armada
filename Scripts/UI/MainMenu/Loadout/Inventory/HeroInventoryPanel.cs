using System;
using System.Collections.Generic;
using Armageddon.Extensions;
using Armageddon.Externals.OdinInspector;
using Armageddon.Games;
using Armageddon.Mechanics;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.UI.Common.InventoryModule;
using Armageddon.UI.Common.InventoryModule.Slot;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Armageddon.UI.MainMenu.Loadout.Inventory
{
    [Serializable]
    public class SlotIconAssetReference
    {
        [TableColumnWidth(100, Resizable = false)]
        [ReadOnly]
        public EquipmentSlotType Type;

        public AssetReference IconAsset;
    }

    public class HeroInventoryPanel : InventoryPanel
    {
        [ListDrawerSettings(HideRemoveButton = true, HideAddButton = true)]
        [TableList(IsReadOnly = true)]
        [SerializeField]
        private SlotIconAssetReference[] m_slotIcons;

        [BoxGroupPrefabs]
        [SerializeField]
        private ObjectHolder m_objectHolderPrefab;

        [BoxGroupPrefabs]
        [SerializeField]
        private SlotWidget m_slotWidgetPrefab;

        [SerializeField]
        private RectTransform m_slotContentTransform;

        public RectTransform SlotContentTransform => m_slotContentTransform;

        protected override void OnEnable()
        {
            var game = GetService<Game>();
            Player player = game.Player;

            if (player == null)
            {
                return;
            }

            // Get Inventory before anything else.
            Inventory = player.HeroInventory;

            SlotContentTransform.DestroyDesignRemnant();

            // The order here is important
            base.OnEnable();

            if (SlotWidgets.Count != Inventory.Slots.Count)
            {
                CreateSlotWidgets();
            }
        }

        private void OnValidate()
        {
            if (m_slotIcons == null)
            {
                m_slotIcons = new SlotIconAssetReference[8];

                for (int i = 0; i < 9; i++)
                {
                    var slotIconAssetReference = new SlotIconAssetReference
                    {
                        Type = (EquipmentSlotType)i,
                        IconAsset = null
                    };

                    m_slotIcons[i] = slotIconAssetReference;
                }
            }
        }

        private void CreateSlotWidgets()
        {
            SlotWidget[] slotWidgets = SlotContentTransform.GetComponentsInChildren<SlotWidget>(true);
            SlotWidgets.AddRange(slotWidgets);

            int index = 0;
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                slotWidget.Index = index++;
            }

            foreach (InventorySlot slot in Inventory.Slots)
            {
                var objectHolderItem = ObjectHolderItem.Create(slot.Object, m_objectHolderPrefab);
                objectHolderItem.SetItem((Item)slot.Object);

                SlotWidget slotWidget = SlotWidgets.Find(x => x.Index == slot.Index);
                slotWidget.Inventory = Inventory;
                slotWidget.Slot = slot;
                slotWidget.ObjectHolder = objectHolderItem;
                slotWidget.SetObjectHolderPosition(objectHolderItem, true);
            }
        }

        protected override SlotWidget GetSlotWidgetPrefab()
        {
            return m_slotWidgetPrefab;
        }

        protected override SlotWidget CreateSlotWidget(int index)
        {
            SlotWidget slotWidget = base.CreateSlotWidget(index);
            LoadSlotObjectAsync(slotWidget, index).Forget();

            return slotWidget;
        }

        private async UniTaskVoid LoadSlotObjectAsync(SlotWidget slotWidget, int slotIndex)
        {
            GameObject iconObject = await m_slotIcons[slotIndex].IconAsset.InstantiateAsync();
            slotWidget.SetIconObject(iconObject);
        }

        public void HighlightSlotsByItem(Item item, float otherSlotTypesAlpha = 1.0f, float duration = 0.5f)
        {
            List<EquipmentSlotType> slotTypes = this.GetEquipSlotTypesByItem(item);
            HighlightSlotsBySlotTypes(slotTypes, otherSlotTypesAlpha, duration);
        }

        private void HighlightSlotsBySlotTypes(List<EquipmentSlotType> slotTypes,
            float otherSlotTypesAlpha, float duration)
        {
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                if (slotTypes != null && slotTypes.Contains((EquipmentSlotType)slotWidget.Slot.Index))
                {
                    // slotWidget.ShowMarker(true);
                }
                else
                {
                    slotWidget.SetAlpha(otherSlotTypesAlpha, duration);
                }
            }
        }

        /// <summary>
        ///     The SlotWidget itself not the SlotObjectHolder
        /// </summary>
        public void SetAllowSelectSlotsByItem(Item item)
        {
            if (item == null)
            {
                Debug.LogWarning("SDFDSFDFS");
                return;
            }

            List<EquipmentSlotType> slotTypes = this.GetEquipSlotTypesByItem(item);

            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                var slotType = (EquipmentSlotType)slotWidget.Slot.Index;
                slotWidget.AllowSelect = slotTypes.Contains(slotType);
            }
        }

        public override void OnWillInsertObjectHolder(SlotWidget slotWidget, ObjectHolder objectHolder)
        {
            slotWidget.ShowIconObject(false);

            if (objectHolder is ObjectHolderItem objectHolderItem)
            {
                objectHolderItem.SetItem((Item)objectHolderItem.Object);
            }
        }

        public override void OnWillRemoveObjectHolder(SlotWidget slotWidget, ObjectHolder objectHolder)
        {
            slotWidget.ShowIconObject(true);

            if (objectHolder is ObjectHolderItem objectHolderItem)
            {
                objectHolderItem.SetItem(null);
            }
        }

        public bool IsEquippingItem(Item item)
        {
            foreach (SlotWidget slotWidget in SlotWidgets)
            {
                if (slotWidget.ObjectHolder == null)
                {
                    continue;
                }

                if (slotWidget.ObjectHolder.Object == item)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
