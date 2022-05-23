using System.Threading;
using Armageddon.Configuration;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Cysharp.Threading.Tasks;
using Purity.Common.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common.InventoryModule.Slot
{
    public class ObjectHolderItem : ObjectHolder
    {
        [InlineEditor]
        [SerializeField]
        private ObjectHolderItemPrefabBank m_prefabBank;

        [ReadOnly]
        [SerializeField]
        private GameObject m_qualityFrameObject;

        [ReadOnly]
        [SerializeField]
        private GameObject m_qualityIconObject;

        [SerializeField]
        private RectTransform m_qualityIconParentTransform;

        [SerializeField]
        private Image m_itemTypeIcon;

        [SerializeField]
        private GameObject m_bottomRightPanelObject;

        [SerializeField]
        private TextMeshProUGUI m_bottomRightText;

        private int m_quantity;

        public ObjectHolderItemPrefabBank PrefabBank => m_prefabBank;

        // TODO: It is basically just SetItem() now. So remove this!
        public void Initialize(Item item)
        {
            SetItem(item);
        }

        public override void SetObject(IInventoryObject obj)
        {
            SetItem((Item)obj, true);
        }

        public void SetItem(Item item, bool animate = false)
        {
            if (item == null)
            {
                gameObject.name = "None";
                gameObject.SetActive(false);
                return;
            }

            gameObject.name = $"{item.Name}({item.InstanceId})";

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            SetIcon(item.Sheet.Icon);
            SetQuality(item.Quality);
            SetItemTypeIcon(item.Type);

            if (item.IsEquipable)
            {
                SetLevel(item.Level);
            }
            else if (item.IsStackable)
            {
                SetQuantityAsync(item.Quantity, animate).Forget();
            }
            else
            {
                SetNoBottomRightText();
            }

            Object = item;
        }

        public void ShowBottomRight(bool value)
        {
            m_bottomRightPanelObject.SetActive(value);
        }


        private void SetItemTypeIcon(ItemType type)
        {
            m_itemTypeIcon.sprite = PrefabBank.ItemTypeIcons[(int)type];
        }

        private void SetLevel(int itemLevel)
        {
            m_bottomRightText.Set($"Lv.{itemLevel}");
        }

        private async UniTaskVoid SetQuantityAsync(int quantity, bool animate)
        {
            if (animate)
            {
                CancellationToken token = GetCancellationToken(nameof(SetQuantityAsync));
                //CancellationToken cts = this.GetCancellationTokenOnDestroy();
                m_quantity = await TweenUtility.ChangeValue(m_quantity, quantity, m_bottomRightText, "x{0}", token);
            }
            else
            {
                m_quantity = quantity;
                m_bottomRightText.Set($"x{quantity}");
            }
        }

        private void SetNoBottomRightText()
        {
            m_bottomRightText.Set(string.Empty);
        }

        public static ObjectHolderItem Create(IInventoryObject obj, ObjectHolder objectHolderPrefab)
        {
            var objectHolderItem = (ObjectHolderItem)Instantiate(objectHolderPrefab);
            objectHolderItem.gameObject.SetActive(false);
            objectHolderItem.name = "None";
            objectHolderItem.AllowDrag = true;
            // objectHolderItem.AllowSelect = true;
            objectHolderItem.AllowReselect = true;
            objectHolderItem.Object = obj;

            if (obj != null)
            {
                if (obj is Item item)
                {
                    objectHolderItem.Initialize(item);
                }
                else
                {
                    Debug.LogWarning($"slotObject is {obj}");
                }
            }

            if (!UISettings.InstantSwapItem)
            {
                // Note: We can optimize this actually like create a single canvas so we don't need to bother with sortingOrder
                var canvas = objectHolderItem.gameObject.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = 500;
            }

            return objectHolderItem;
        }

        protected override bool OnEndDragValidating(SlotWidget source, SlotWidget target)
        {
            InventoryPanel inventoryPanel = SlotWidget.InventoryPanel;

            if (!inventoryPanel.CanSwap(source, target))
            {
                return false;
            }

            Debug.LogWarning("Dragging is no longer supported at the moment"!);
            // slotWidgetManager.SwapAsync is split into EquipItemAsync and UnequipItemAsync
            // slotWidgetManager.SwapAsync(source, target).Forget();

            // Not allowed end drag which would return the slot object to its previous slot,
            // but let SwapAsync does its job.
            return false;
        }
    }
}
