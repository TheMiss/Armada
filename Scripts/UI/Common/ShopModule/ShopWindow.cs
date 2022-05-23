using System;
using System.Threading;
using Armageddon.Backend.Functions;
using Armageddon.Backend.Payloads;
using Armageddon.Externals.OdinInspector;
using Armageddon.Localization;
using Armageddon.Mechanics.Items;
using Armageddon.Mechanics.Shops;
using Armageddon.UI.Base;
using Armageddon.UI.Common.ItemInspectionModule;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using Purity.Common;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common.ShopModule
{
    // TODO: Make this auto refresh when remaining time is zero.
    public class ShopWindow : Window, IEnhancedScrollerDelegate
    {
        [BoxGroupPrefabs]
        [SerializeField]
        private EnhancedScrollerCellView m_cellViewPrefab;

        [SerializeField]
        private EnhancedScroller m_scroller;
        
        [SerializeField]
        private Button m_resetButton;

        [SerializeField]
        private Button m_backButton;

        [SerializeField]
        private TextMeshProUGUI m_titleText;

        [SerializeField]
        private TextMeshProUGUI m_refreshTimerText;

        [SerializeField]
        private Toggle m_dailyShopToggle;

        [SerializeField]
        private Toggle m_weeklyShopToggle;

        [SerializeField]
        private Toggle m_specialShopToggle;

        [SerializeField]
        private Toggle m_adsShopToggle;

        [SerializeField]
        private int m_numberOfCellsPerRow = 4;

        [SerializeField]
        private int m_cellSize = 275;

        [ShowInPlayMode]
        private Shop m_currentShop;

        protected override void Awake()
        {
            base.Awake();

            m_resetButton.onClick.AddListener(OnResetButtonClicked);
            m_backButton.onClick.AddListener(OnBackButtonClicked);
            m_dailyShopToggle.onValueChanged.AddListener(value => OnToggleValueChanged(m_dailyShopToggle));
            m_weeklyShopToggle.onValueChanged.AddListener(value => OnToggleValueChanged(m_weeklyShopToggle));
            m_specialShopToggle.onValueChanged.AddListener(value => OnToggleValueChanged(m_specialShopToggle));
            m_adsShopToggle.onValueChanged.AddListener(value => OnToggleValueChanged(m_adsShopToggle));
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_dailyShopToggle.isOn = true;
            m_weeklyShopToggle.isOn = false;
            m_specialShopToggle.isOn = false;
            m_adsShopToggle.isOn = false;

            SetShop(ShopType.Daily);
        }

        int IEnhancedScrollerDelegate.GetNumberOfCells(EnhancedScroller scroller)
        {
            return Mathf.CeilToInt(m_currentShop.Items.Count / (float)m_numberOfCellsPerRow);
        }

        float IEnhancedScrollerDelegate.GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return m_cellSize;
        }

        EnhancedScrollerCellView IEnhancedScrollerDelegate.GetCellView(EnhancedScroller scroller,
            int dataIndex, int cellIndex)
        {
            // first, we get a cell from the scroller by passing a prefab.
            // if the scroller finds one it can recycle it will do so, otherwise
            // it will create a new cell.
            var cellView = (ShopCellView)scroller.GetCellView(m_cellViewPrefab);

            // data index of the first sub cell
            int rowIndex = dataIndex * m_numberOfCellsPerRow;
            cellView.name = $"Cell {rowIndex} to {rowIndex + m_numberOfCellsPerRow - 1}";

            // pass in a reference to our data set with the offset for this cell
            cellView.SetShopItems(m_currentShop.Items, rowIndex);

            // return the cell to the scroller
            return cellView;
        }

        private void SetShop(ShopType shopType)
        {
            m_currentShop = Game.Player.Shops[(int)shopType];

            m_scroller.Delegate = this;
            m_scroller.cellViewInstantiated = (scroller, view) =>
            {
                var cellView = (ShopCellView)view;
                cellView.AddOnClickListener(OnCellViewClicked);
            };
            m_scroller.ReloadData();

            m_titleText.Set(Lexicon.Shop(shopType));

            StopAllUniTasks();
            UpdateRemainingTimeAsync().Forget();
        }

        private void OnResetButtonClicked()
        {
            async UniTask Async()
            {
                string titleText = $"{Texts.UI.Attention}!";
                string messageText = Lexicon.ConfirmResetShop(m_currentShop.Type, m_currentShop.ResetPrice);
                string acceptButtonText = Texts.UI.Yes;
                string rejectButtonText = Texts.UI.Cancel;

                bool? dialogResult = await UI.AlertDialog.ShowInfoDialogAsync(titleText, messageText,
                    acceptButtonText, rejectButtonText);

                if (dialogResult == true)
                {
                    ShopType shopType = m_currentShop.Type;
                    await Game.Player.ResetShopAsync(m_currentShop);

                    // m_currentShop is set again here.
                    SetShop(shopType);
                }
            }

            Async().Forget();
        }

        private void OnBackButtonClicked()
        {
            Hide();
        }

        private void OnCellViewClicked(ShopRowCellView cellView)
        {
            ShowCellViewAsync(cellView).Forget();
        }

        private async UniTask ShowCellViewAsync(ShopRowCellView cellView)
        {
            ShopItem shopItem = cellView.ShopItem;
            Item item = shopItem.Item;
            Debug.Log($"Clicked on {item.Name}({item.InstanceId})");

            // We use InspectItemWindow's Blocker, so no longer need this.
            // AddBlocker(null, Transform.GetSiblingIndex() + 1, new Color(0, 0, 0, 240 / 255f));

            InspectItemWindow inspectItemWindow = UI.InspectItemWindow;
            inspectItemWindow.Mode = InspectItemWindowMode.InShop;
            inspectItemWindow.InspectAsync(item, false).Forget();

            ShopConfirmDialog shopConfirmDialog = UI.ShopConfirmDialog;
            shopConfirmDialog.TransformIndexChanged = window =>
            {
                inspectItemWindow.Transform.SetSiblingIndex(shopConfirmDialog.Transform.GetSiblingIndex());
            };

            bool? dialogResult = await shopConfirmDialog.ShowDialogAsync(shopItem);

            if (dialogResult == false)
            {
                return;
            }

            Debug.Log($"Buying {shopConfirmDialog.ShopItem.Item.Name} X{shopConfirmDialog.SelectedQuantity}.");

            BuyShopItemReply reply =
                await Game.Player.BuyShopItemAsync(m_currentShop, shopItem, shopConfirmDialog.SelectedQuantity);
            if (reply != null)
            {
                m_scroller.ReloadData();
            }

            // RemoveBlocker();
        }

        private async UniTask UpdateRemainingTimeAsync()
        {
            DateTime nextResetTime = m_currentShop.NextResetTime;
            Debug.Log($"nextResetTime: {nextResetTime.ToStringEx()}");

            m_refreshTimerText.Set(nextResetTime.GetRemainingTimeString());

            CancellationToken token = GetCancellationToken(nameof(UpdateRemainingTimeAsync));

            while (DialogResult == null)
            {
                m_refreshTimerText.Set(nextResetTime.GetRemainingTimeString());
                await UniTask.Delay(1000, true, cancellationToken: token);
            }
        }

        private void OnToggleValueChanged(Toggle toggle)
        {
            if (!toggle.isOn)
            {
                return;
            }

            if (toggle == m_dailyShopToggle)
            {
                SetShop(ShopType.Daily);
            }
            else if (toggle == m_weeklyShopToggle)
            {
                SetShop(ShopType.Weekly);
            }
            else if (toggle == m_specialShopToggle)
            {
                SetShop(ShopType.Special);
            }
            else if (toggle == m_adsShopToggle)
            {
                SetShop(ShopType.Ads);
            }
        }
    }
}
