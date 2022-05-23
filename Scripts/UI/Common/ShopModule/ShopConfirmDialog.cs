using Armageddon.Mechanics;
using Armageddon.Mechanics.Items;
using Armageddon.Mechanics.Shops;
using Armageddon.UI.Common.InventoryModule.Slot;
using Armageddon.UI.Common.ItemInspectionModule;
using Cysharp.Threading.Tasks;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.Common.ShopModule
{
    public enum ShopConfirmDialogMode
    {
        Buy,
        Sell
    }

    public class ShopConfirmDialog : AlertDialog
    {
        // [SerializeField]
        // private TextMeshProUGUI m_titleText;
        //
        [SerializeField]
        private Button m_iconButton;

        // [SerializeField]
        // private Button m_acceptButton;
        //
        // [SerializeField]
        // private Button m_rejectButton;

        [SerializeField]
        private ObjectHolderItem m_objectHolderItem;

        [SerializeField]
        private GameObject m_sliderPanelObject;

        [SerializeField]
        private Slider m_slider;

        [SerializeField]
        private Button m_decreaseButton;

        [SerializeField]
        private Button m_increaseButton;

        [SerializeField]
        private TextMeshProUGUI m_amountTooltipText;

        [SerializeField]
        private TextMeshProUGUI m_buyButtonText;

        [SerializeField]
        private ShopPricePanel m_pricePanel;

        // public TextMeshProUGUI TitleText => m_titleText;

        public ObjectHolderItem ObjectHolderItem => m_objectHolderItem;

        public int SelectedQuantity { get; private set; }

        public Currency TotalPrice { get; private set; }

        public ShopItem ShopItem { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            m_iconButton.onClick.AddListener(OnIconButtonClicked);
            m_decreaseButton.onClick.AddListener(OnDecreaseButtonClicked);
            m_increaseButton.onClick.AddListener(OnIncreaseButtonClicked);

            m_slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnIconButtonClicked()
        {
            InspectItemWindow inspectItemWindow = UI.InspectItemWindow;
            inspectItemWindow.Mode = InspectItemWindowMode.InShop;
            inspectItemWindow.InspectAsync(ShopItem.Item, false).Forget();
        }

        private void OnDecreaseButtonClicked()
        {
            m_slider.value -= 1;
            SetSelectedQuantity((int)m_slider.value, ShopItem.Item.Quantity);
        }

        private void OnIncreaseButtonClicked()
        {
            m_slider.value += 1;
            SetSelectedQuantity((int)m_slider.value, ShopItem.Item.Quantity);
        }

        private void OnSliderValueChanged(float value)
        {
            SetSelectedQuantity((int)value, ShopItem.Item.Quantity);
        }

        // public async UniTask<bool?> ShowBuyAsync(Item item)
        // {
        //     return await ShowAsync(ShopConfirmDialogMode.Buy, item);
        // }
        //
        // public async UniTask<bool?> ShowSellAsync(Item item)
        // {
        //     return await ShowAsync(ShopConfirmDialogMode.Sell, item);
        // }

        private void SetShopItem(ShopItem shopItem)
        {
            ShopItem = shopItem;
            Item item = shopItem.Item;
            TitleText.Set(item.Name);
            ObjectHolderItem.SetItem(item);
            m_slider.maxValue = item.Quantity;
            m_slider.value = item.Quantity;
            m_sliderPanelObject.transform.parent.gameObject.SetActive(item.IsStackable);
            SetSelectedQuantity(1, item.Quantity, true);
            // SetSelectedQuantity(item.Quantity, item.Quantity);
        }

        private void SetSelectedQuantity(int amount, int maxAmount, bool setSliderValue = false)
        {
            if (amount <= 0)
            {
                amount = 1;
                m_slider.SetValueWithoutNotify(amount);
            }

            if (setSliderValue)
            {
                m_slider.SetValueWithoutNotify(amount);
            }

            SelectedQuantity = amount;
            Item item = ShopItem.Item;
            TotalPrice = null;
            if (item.ShopSellPrice != null)
            {
                TotalPrice = new Currency(item.ShopSellPrice.Type, SelectedQuantity * item.ShopSellPrice.Amount);
            }

            m_amountTooltipText.Set($"{amount}/{maxAmount}");

            // TODO: Localize
            string x = string.Empty;
            if (amount > 1)
            {
                x = $" X{amount}";
            }

            m_buyButtonText.Set($"Buy{x}");
            m_pricePanel.SetPrice(TotalPrice);
        }

        // public async UniTask<bool?> ShowAsync(ShopConfirmDialogMode mode, Item item)
        // {
        //     string titleText = "Attention!";
        //     string buyText = $"Are you sure you want to buy {item.Name} at {item.BuyPrice}";
        //     string sellText = $"Are you sure you want to sell {item.Name} at {item.SellPrice} this cannot be undone.";
        //     string messageText = mode == ShopConfirmDialogMode.Buy ? buyText : sellText;
        //     string acceptText = Texts.UI.Yes;
        //     string rejectText = Texts.UI.No;
        //
        //     return await ShowAsync(item, titleText, messageText, acceptText, rejectText);
        // }

        public async UniTask<bool?> ShowDialogAsync(ShopItem shopItem)
        {
            SetShopItem(shopItem);

            transform.SetAsLastSibling();

            bool? result = await ShowDialogAsync();

            UI.InspectItemWindow.Hide();

            return result;
        }
    }
}
