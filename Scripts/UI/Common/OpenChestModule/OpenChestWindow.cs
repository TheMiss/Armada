using System;
using System.Collections.Generic;
using Armageddon.Backend.Functions;
using Armageddon.Backend.Payloads;
using Armageddon.Extensions;
using Armageddon.Externals.OdinInspector;
using Armageddon.Localization;
using Armageddon.Mechanics;
using Armageddon.Mechanics.Items;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using Purity.Common;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common.OpenChestModule
{
    public enum OpenLootBoxWindowResult
    {
        Back,
        Open,
        OpenX
    }

    // TODO: Rename to OpenChestWindow
    public class OpenChestWindow : Window
    {
        private const int MaxOpeningLootBoxCount = 10;

        [BoxGroupPrefabs]
        [SerializeField]
        private DropElement m_dropElementPrefab;

        [SerializeField]
        private Button m_openButton;

        [SerializeField]
        private Button m_openXButton;

        [SerializeField]
        private Button m_backButton;

        [SerializeField]
        private TextMeshProUGUI m_openXText;

        [SerializeField]
        private TextMeshProUGUI m_lootBoxNameText;

        [SerializeField]
        private TextMeshProUGUI m_lootBoxCountText;

        [SerializeField]
        private Image m_lootBoxImage;

        [SerializeField]
        private Image m_lootBoxGlowingImage;

        [SerializeField]
        private Animator m_lootBoxAnimator;

        [SerializeField]
        private RectTransform m_dropsContentTransform;

        [SerializeField]
        private GameObject m_separatorObject;

        private LootBox m_openingLootBox;
        private int m_openingLootBoxCount;
        private UseItemReply m_useItemReply;

        public OpenLootBoxWindowResult? WindowResult { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_openButton.onClick.AddListener(() => OnButtonClicked(OpenLootBoxWindowResult.Open));
            m_openXButton.onClick.AddListener(() => OnButtonClicked(OpenLootBoxWindowResult.OpenX));
            m_backButton.onClick.AddListener(() => OnButtonClicked(OpenLootBoxWindowResult.Back));

            m_dropsContentTransform.DestroyDesignRemnant();

            m_useItemReply = null;
        }

        protected override void OnDisable()
        {
            m_openButton.onClick.RemoveAllListeners();
            m_openXButton.onClick.RemoveAllListeners();
            m_backButton.onClick.RemoveAllListeners();

            base.OnDisable();
        }

        private void OnButtonClicked(OpenLootBoxWindowResult result)
        {
            switch (result)
            {
                case OpenLootBoxWindowResult.Back:
                    DialogResult = false;
                    ApplyReply();
                    break;
                case OpenLootBoxWindowResult.Open:
                    ApplyReply();
                    PlayOpenLootBoxAnimationAsync(1).Forget();
                    break;
                case OpenLootBoxWindowResult.OpenX:
                    ApplyReply();
                    PlayOpenLootBoxAnimationAsync(m_openingLootBoxCount).Forget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }

            WindowResult = OpenLootBoxWindowResult.Back;
        }

        private async UniTask<List<DropElement>> CreateDropElementsAsync(DropPayload[] dropObjects)
        {
            var stopwatch = new Stopwatch(nameof(CreateDropElementsAsync));

            List<DropElement> dropElements = await DropElement.CreateManyAsync(dropObjects,
                m_dropElementPrefab, m_dropsContentTransform);

            // var droppedItemElements = new List<DroppedItemElement>();
            //
            // foreach (DropObject dropObject in dropObjects)
            // {
            //     DroppedItemElement droppedItemElement = Instantiate(m_droppedItemElementPrefab, 
            //         m_lootedItemsContentTransform);
            //     
            //     droppedItemElement.gameObject.SetActive(false);
            //     droppedItemElements.Add(droppedItemElement);
            //
            //     switch (dropObject.Type)
            //     {
            //         case DropType.Item:
            //         {
            //             var item = await Item.CreateAsync(dropObject.Item);
            //             droppedItemElement.Initialize(item);
            //             break;
            //         }
            //         case DropType.Currency:
            //             droppedItemElement.Initialize(dropObject.Currency.Type, dropObject.Currency.Amount);
            //             break;
            //         default:
            //             throw new ArgumentOutOfRangeException();
            //     }
            // }

            stopwatch.Stop();

            return dropElements;
        }

        private async UniTask PlayOpenLootBoxAnimationAsync(int quantity)
        {
            m_dropsContentTransform.DestroyChildren();
            m_separatorObject.SetActive(false);

            var player = GetService<Player>();

            m_useItemReply = await player.UseItemAsync(m_openingLootBox, quantity, true, true);

            // We don't want to do async here as we need to load while playing an animation.
            UniTask<List<DropElement>> createTask = CreateDropElementsAsync(m_useItemReply.Drops);

            await m_lootBoxAnimator.PlayAsync(this, "Open");

            Debug.Log("Finished Play LootBox!");

            while (createTask.Status == UniTaskStatus.Pending)
            {
                await UniTask.Yield();
            }

            m_separatorObject.SetActive(true);
            m_lootBoxAnimator.Play("Idle");

            SetTexts(m_openingLootBox);

            List<DropElement> dropElements = createTask.GetAwaiter().GetResult();
            foreach (DropElement dropElement in dropElements)
            {
                dropElement.gameObject.SetActive(true);
                dropElement.Transform.PlayScaleAnimation();

                await UniTask.Delay(60);
            }
        }

        private void SetTexts(LootBox lootBox)
        {
            m_lootBoxNameText.Set(lootBox.Sheet.Name);
            m_lootBoxCountText.Set($"{lootBox.Quantity}");

            m_openXButton.gameObject.SetActive(false);

            if (lootBox.Quantity > 1)
            {
                m_openXButton.gameObject.SetActive(true);
                m_openingLootBoxCount = lootBox.Quantity;

                if (m_openingLootBoxCount > MaxOpeningLootBoxCount)
                {
                    m_openingLootBoxCount = MaxOpeningLootBoxCount;
                }

                m_openXText.Set($"{Texts.UI.Open.ToUpper()} x{m_openingLootBoxCount}");
            }
        }

        private void ApplyReply()
        {
            if (m_useItemReply == null)
            {
                return;
            }

            var player = GetService<Player>();
            player.UpdateBalances(m_useItemReply.ModifiedCurrencies);
            player.InsertOrUpdateItems(m_useItemReply.GetItemsFromDrops()).Forget();

            m_useItemReply = null;
        }

        public async UniTask<OpenLootBoxWindowResult?> ShowLootBoxAsync(LootBox lootBox)
        {
            m_dropsContentTransform.DestroyChildren();
            m_separatorObject.SetActive(false);

            m_openingLootBox = lootBox;
            m_lootBoxImage.sprite = lootBox.Sheet.Image;
            m_lootBoxGlowingImage.sprite = lootBox.Sheet.Image;

            SetTexts(lootBox);

            // Required to play animation on Animator as Animator needs its gameObject to be active!
            Show();
            m_lootBoxAnimator.Play("Idle");

            WindowResult = null;

            // We don't want this to be modal(Not showing Blocker and OpenLootBoxWindow is a fullscreen anyway.)
            await ShowDialogAsync(true, false);

            return WindowResult;
        }
    }
}
