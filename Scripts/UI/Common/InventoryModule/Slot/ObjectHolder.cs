using System;
using System.Collections.Generic;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.UI.Base;
using Purity.Common.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Armageddon.UI.Common.InventoryModule.Slot
{
    public abstract class ObjectHolder : Widget
        , IBeginDragHandler
        , IDragHandler
        , IEndDragHandler
    {
        public static Vector2 DragLiftingOffset = new(-50, 50);

        [SerializeField]
        private Image m_hitBoxImage;

        [SerializeField]
        private Image m_icon;

        [DisableInPlayMode]
        [SerializeField]
        private List<QualityEntry> m_qualityEntries;

        private Canvas m_draggingCanvas;

        public Image Icon => m_icon;

        public SlotWidget SlotWidget { set; get; }

        public bool AllowDrag { set; get; } = true;

        // public bool AllowSelect { set; get; }

        public bool AllowReselect { set; get; }

        public IInventoryObject Object { set; get; }

        public Canvas DraggingCanvas
        {
            get
            {
                if (m_draggingCanvas == null)
                {
                    // Trying to locate the top most for dragging object holder
                    Canvas canvas = null;
                    Transform parent = Transform.parent;
                    while (canvas == null && parent != null)
                    {
                        canvas = parent.GetComponent<Canvas>();

                        if (canvas != null)
                        {
                            m_draggingCanvas = canvas;
                            break;
                        }

                        parent = parent.parent;
                    }

                    if (m_draggingCanvas == null)
                    {
                        Debug.LogError("m_draggingCanvas == null");
                    }
                }

                return m_draggingCanvas;
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!AllowDrag)
            {
                return;
            }

            if (SlotWidget == null)
            {
                Debug.LogWarning("m_initialSlot should be not null!");
                return;
            }

            // Remove this item's transform into Canvas's Transform
            Transform.SetParent(DraggingCanvas.transform, true);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (AllowDrag)
            {
                Transform.position = eventData.pointerCurrentRaycast.screenPosition + DragLiftingOffset;
            }
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (SlotWidget == null)
            {
                return;
            }

            if (!AllowDrag)
            {
                return;
            }

            var hitSlotWidget = TestHit<SlotWidget>(eventData);

            bool allowSwap = hitSlotWidget != null && SlotWidget != hitSlotWidget;

            if (allowSwap && OnEndDragValidating(SlotWidget, hitSlotWidget))
            {
                if (hitSlotWidget.ObjectHolder != null)
                {
                    InventoryPanel.Swap(SlotWidget, hitSlotWidget);
                }
                else
                {
                    InventoryPanel.Move(SlotWidget, hitSlotWidget);
                }

                Transform.SetParent(hitSlotWidget.ObjectHolderContainerTransform, false);
                Transform.localPosition = Vector3.zero;
                Transform.ResetAnchorToStretchAll();
            }
            else
            {
                // Return to original slot
                Transform.SetParent(SlotWidget.ObjectHolderContainerTransform, false);
                Transform.localPosition = Vector3.zero;
                Transform.ResetAnchorToStretchAll();
            }
        }

        [Button]
        private void DisableRaycastTargets()
        {
            SetRaycastTargetsInChildren(gameObject, false);
        }

        public void SetIcon(Sprite sprite)
        {
            Icon.sprite = sprite;
        }

        public void SetColor(Color color)
        {
            Icon.color = color;
        }

        /// <summary>
        ///     Returns true to allow slot swap
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected virtual bool OnEndDragValidating(SlotWidget source, SlotWidget target)
        {
            SetDragLifting(false, null);
            return true;
        }

        public void SetDragLifting(bool lift, PointerEventData eventData)
        {
            m_hitBoxImage.raycastTarget = lift;
            //Transform.position = eventData.pointerCurrentRaycast.screenPosition + DragLiftingOffset;
        }

        public virtual void SetObject(IInventoryObject obj)
        {
        }

        [Button("Common")]
        [ButtonGroup("ItemQuality")]
        [GUIColorCommon]
        private void SetQualityCommon()
        {
            SetQuality(ItemQuality.Common);
        }

        [Button("Uncommon")]
        [ButtonGroup("ItemQuality")]
        [GUIColorUncommon]
        private void SetQualityUncommon()
        {
            SetQuality(ItemQuality.Uncommon);
        }

        [Button("Rare")]
        [ButtonGroup("ItemQuality")]
        [GUIColorRare]
        private void SetQualityRare()
        {
            SetQuality(ItemQuality.Rare);
        }

        [Button("Epic")]
        [ButtonGroup("ItemQuality")]
        [GUIColorEpic]
        private void SetQualityEpic()
        {
            SetQuality(ItemQuality.Epic);
        }

        [Button("Legendary")]
        [ButtonGroup("ItemQuality")]
        [GUIColorLegendary]
        private void SetQualityLegendary()
        {
            SetQuality(ItemQuality.Legendary);
        }

        [Button("Immortal")]
        [ButtonGroup("ItemQuality")]
        [GUIColorImmortal]
        private void SetQualityImmortal()
        {
            SetQuality(ItemQuality.Immortal);
        }

        [Button("Ancient")]
        [ButtonGroup("ItemQuality")]
        [GUIColorAncient]
        private void SetQualityAncient()
        {
            SetQuality(ItemQuality.Ancient);
        }

        protected virtual void SetQuality(ItemQuality quality)
        {
            foreach (QualityEntry qualityEntry in m_qualityEntries)
            {
                qualityEntry.SetActive(false);
            }

            int index = (int)quality;
            m_qualityEntries[index].SetActive(true);
        }


        [Serializable]
        public class QualityEntry
        {
            public bool Active;
            public bool UseSmallFrame;
            public GameObject BigFrameObject;

            [ShowIf(nameof(UseSmallFrame))]
            public GameObject SmallFrameObject;

            public void SetActive(bool active, bool forceSet = false)
            {
                if (Active == active && !forceSet)
                {
                    return;
                }

                Active = active;
                BigFrameObject.SetActive(active);

                if (SmallFrameObject != null)
                {
                    SmallFrameObject.SetActive(active);
                }
            }
        }
    }
}
