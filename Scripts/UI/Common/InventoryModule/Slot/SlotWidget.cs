using System;
using Armageddon.Configuration;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Inventories;
using Armageddon.UI.Base;
using DG.Tweening;
using Purity.Common.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Armageddon.UI.Common.InventoryModule.Slot
{
    public class SlotWidget : Widget, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [ChildGameObjectsOnly]
        [SerializeField]
        private GameObject m_normalObject;

        [Optional]
        [ChildGameObjectsOnly]
        [SerializeField]
        private GameObject m_selectedObject;

        [ChildGameObjectsOnly]
        [SerializeField]
        private GameObject m_markerObject;

        [ChildGameObjectsOnly]
        [SerializeField]
        private Image m_hitBox;

        [ChildGameObjectsOnly]
        [SerializeField]
        private Transform m_slotIconContainerTransform;

        [ChildGameObjectsOnly]
        [SerializeField]
        private Transform m_objectHolderContainerTransform;

        [SerializeField]
        private CanvasGroup m_canvasGroup;

        [ReadOnly]
        [ShowInInspector]
        private int m_index;

        private InventorySlot m_slot;
        // TODO: Bring this back when the bug is fixed
        // private bool m_isPointerDown;
        // private float m_pointerDownDuration;
        // private bool m_isObjectLifting;
        // private PointerEventData m_pointerEventData;

        public InventoryPanel InventoryPanel { set; get; }
        public Inventory Inventory { get; set; }

        public int Index
        {
            set
            {
                m_index = value;
                name = $"Slot{m_index}";
            }
            get => m_index;
        }

        /// <summary>
        ///     Slot will also register event to UISlot
        ///     And this should be assigned only once when it was created.
        /// </summary>
        public InventorySlot Slot
        {
            set
            {
                // if (m_slot != null)
                // {
                //     Debug.LogWarning($"{name} has assigned Data already");
                //     return;
                // }

                m_slot = value;
                if (m_slot != null)
                {
                    Index = m_slot.Index;
                    AdjustState();
                }

                // // m_slot.StateChanged += OnSlotStateChanged;
                // m_slot.IndexChanged += OnIndexChanged;
                // AdjustState();
            }
            get => m_slot;
        }

        public ObjectHolder ObjectHolder { set; get; }

        /// <summary>
        ///     Enable this to let the user select SlotWidget and to prevent from hit test of ObjectHolder
        /// </summary>
        public bool IsHitBoxEnabled
        {
            set => m_hitBox.raycastTarget = value;
            get => m_hitBox.raycastTarget;
        }

        /// <summary>
        ///     Even though HitBoxEnabled is true, we still can prevent from the SlotWidget emit select (Used to select slot rather
        ///     than the object within the slot, for example.)
        /// </summary>
        [ShowInInspector]
        public bool AllowSelect { set; get; } = true;

        public Transform ObjectHolderContainerTransform => m_objectHolderContainerTransform;

        protected override void Awake()
        {
            base.Awake();

            CanTick = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // m_isPointerDown = false;
            // m_pointerDownDuration = 0.0f;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (m_slot != null)
            {
                // m_slot.StateChanged -= OnSlotStateChanged;
                m_slot.IndexChanged -= OnIndexChanged;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // TODO: Bring this back when the bug is fixed
            // TODO: There's bug here (Unity 2020.3.4f). OnPointerUp will come after OnPointerDown (Happen to OnPointerClick too)
            if (eventData.dragging)
            {
            }

            // SlotManager.DeselectAll();
            // SlotManager.SelectSlot(Slot.Index, eventData.clickCount);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // TODO: Bring this back when the bug is fixed
            //if (!m_isPointerDown)
            // {
            //     m_isPointerDown = true;
            //     m_pointerDownDuration = 0.0f;
            //     m_pointerEventData = eventData;
            //     
            //     Debug.Log($"{name}.OnPointerDown");
            // }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!AllowSelect || eventData.dragging)
            {
                return;
            }

            Inventory.DeselectAll();
            Inventory.SelectSlot(Slot.Index, eventData.clickCount);

            // TODO: Bring this back when the bug is fixed
            // TODO: There's bug here (Unity 2020.3.4f). OnPointerUp will come after OnPointerDown (Happen to OnPointerClick too)
            // Wait for someone to file an issue. I'm not doing it...
            // if (m_isPointerDown)
            // // if (m_isPointerDown && !eventData.dragging)
            // {
            //     m_isPointerDown = false;
            //     
            //     if (ObjectHolder != null)
            //     {
            //         ObjectHolder.SetDragLifting(false, eventData);
            //         // Debug.Log($"{name}.OnPointerUp ---XXX ");
            //     }
            // }

            // Debug.Log($"{name}.OnPointerUp");
        }

        [Button]
        private void DisableRaycastTargets()
        {
            SetRaycastTargetsInChildren(gameObject, false);
        }

        public override void Tick()
        {
            // TODO: Bring this back when the bug is fixed
            // if (m_isPointerDown)
            // {
            //     m_pointerDownDuration += Time.unscaledDeltaTime;
            //
            //     // TODO: Well, this is a hardcode.
            //     if (m_pointerDownDuration > 1.0f)
            //     {
            //         if (ObjectHolder != null)
            //         {
            //             ObjectHolder.SetDragLifting(true, m_pointerEventData);
            //             Debug.Log($"{name}.Tick");
            //         }
            //     }
            // }
        }

        private void OnSlotStateChanged(object sender, SlotStateChangedArgs e)
        {
            AdjustState();
        }

        private void OnIndexChanged(object sender, SlotIndexChangedArgs e)
        {
            Index = e.NewIndex;
        }

        private void Select()
        {
            // Debug.Log($"{Slot.Index} Select for {SlotManager.GetType().Name}");

            if (m_selectedObject != null)
            {
                m_selectedObject.SetActive(true);
            }

            m_normalObject.SetActive(false);

            const float scale = 1.15f;
            Transform.DOScale(new Vector3(scale, scale), 0.2f);
        }

        private void Deselect()
        {
            // Debug.Log($"{Slot.Index} Deselect for {SlotManager.GetType().Name}");

            if (m_selectedObject != null)
            {
                m_selectedObject.SetActive(false);
            }

            m_normalObject.SetActive(true);

            const float scale = 1.0f;
            Transform.DOScale(new Vector3(scale, scale), 0.2f);
        }

        public void AdjustState()
        {
            if (Slot == null)
            {
                Deselect();
                return;
            }

            SlotState state = Slot.State;

            switch (state)
            {
                case SlotState.Selected:
                    Select();
                    break;
                case SlotState.Deselected:
                    Deselect();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetObjectHolderPosition(ObjectHolder objectHolder, bool firstTimeCreate)
        {
            Vector2 previousPosition = objectHolder.Transform.position;

            // Instantly snap first...
            objectHolder.Transform.SetParent(m_objectHolderContainerTransform, false);
            objectHolder.Transform.localScale = Vector3.one;
            objectHolder.Transform.ResetAnchorToStretchAll();

            if (firstTimeCreate || UISettings.InstantSwapItem)
            {
                return;
            }

            Vector2 newPosition = objectHolder.Transform.position;
            objectHolder.Transform.position = previousPosition;
            var canvas = objectHolder.GetComponent<Canvas>();
            canvas.overrideSorting = true;

            objectHolder.Transform.DOMove(newPosition, 0.25f).OnComplete(() => canvas.overrideSorting = false);
        }

        public void SetIconObject(GameObject iconObject)
        {
            m_slotIconContainerTransform.DestroyChildren();

            iconObject.transform.SetParent(m_slotIconContainerTransform, false);
            iconObject.transform.ResetAnchorToStretchAll();

            if (Slot.Object == null)
            {
                m_slotIconContainerTransform.gameObject.SetActive(true);
            }
        }

        public void ShowIconObject(bool value)
        {
            m_slotIconContainerTransform.gameObject.SetActive(value);
        }

        private void SetEnableChildrenImages()
        {
        }

        public void SetAlpha(float alpha, float duration = 0.2f)
        {
            float currentAlpha = m_canvasGroup.alpha;
            if (currentAlpha < alpha)
            {
            }

            DOTween.To(() => m_canvasGroup.alpha, x => { m_canvasGroup.alpha = x; }, alpha, duration);

            // m_canvasGroup.alpha = alpha;
        }

        public void ShowMarker(bool value)
        {
            m_markerObject.SetActive(value);
        }
    }
}
