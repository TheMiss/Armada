using System;
using System.Collections.Generic;
using System.Linq;
using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend;
using Armageddon.Backend.Functions;
using Armageddon.Backend.Functions.Cheats;
using Armageddon.Backend.Payloads;
using Armageddon.Games;
using Armageddon.Localization;
using Armageddon.Mechanics;
using Armageddon.UI.Base;
using Armageddon.UI.MainMenu.PremiumShop.Chests;
using Armageddon.UI.MainMenu.PremiumShop.GemPacks;
using Armageddon.UI.MainMenu.PremiumShop.ShardPacks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.PremiumShop
{
    public class PremiumShopTabWindow : TabWindow, IStoreListener
    {
        [SerializeField]
        private ScrollRect m_scrollRect;

        [SerializeField]
        private GemsPanel m_gemsPanel;

        [SerializeField]
        private ShardsPanel m_shardsPanel;

        [SerializeField]
        private ChestsPanel m_chestsPanel;

        [SerializeField]
        private List<GameObject> m_scrollViewObjects;

        [SerializeField]
        private List<Toggle> m_toggles;
        
        private IStoreController m_storeController;

        public bool IsInitialized => m_storeController != null;

        protected override void Awake()
        {
            base.Awake();

            // m_scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);

            m_gemsPanel.AddOnButtonClickListener(OnGemPackButtonClicked);
            m_shardsPanel.AddButtonListener(OnShardPackButtonClicked);
            m_chestsPanel.AddButtonListener(OnChestPackButtonClicked);

            int index = 0;
            foreach (Toggle toggle in m_toggles)
            {
                int toggleIndex = index;
                toggle.onValueChanged.AddListener(value =>
                {
                    if (value)
                    {
                        OnToggleValueChanged(toggle, toggleIndex);
                    }
                });

                index++;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Set chests scroll view as a default.
            m_scrollViewObjects[0].SetActive(false);
            m_scrollViewObjects[1].SetActive(true);
            m_scrollViewObjects[2].SetActive(false);

            // TODO: Reset scroll bar position to the beginning.

            m_toggles[0].isOn = false;
            m_toggles[1].isOn = true;
            m_toggles[2].isOn = false;
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("OnInitialized");

            m_storeController = controller;
            m_gemsPanel.SetProductPrices(controller.products.all);
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log($"OnInitializeFailed InitializationFailureReason:{error}");

            string titleText = $"{Texts.UI.Error}!";
            string messageText = $"{Texts.Message.PurchaseFailed} ({error})";
            string acceptButtonText = Texts.UI.GotIt;
            UI.AlertDialog.ShowErrorDialogAsync(titleText, messageText, acceptButtonText).Forget();
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs e)
        {
            if (!IsInitialized)
            {
                return PurchaseProcessingResult.Complete;
            }

            // Test edge case where product is unknown
            if (e.purchasedProduct == null)
            {
                Debug.LogWarning("Attempted to process purchase with unknown product. Ignoring");
                return PurchaseProcessingResult.Complete;
            }

            // Test edge case where purchase has no receipt
            if (string.IsNullOrEmpty(e.purchasedProduct.receipt))
            {
                Debug.LogWarning("Attempted to process purchase with no receipt: ignoring");
                return PurchaseProcessingResult.Complete;
            }

            Debug.Log($"Processing transaction: {e.purchasedProduct.transactionID}");

#if UNITY_EDITOR
            int validateMethod = 0;

            string productId = e.purchasedProduct.definition.id;
            if (productId == "com.bibeshox.arma.gempack1" ||
                productId == "com.bibeshox.arma.gempack2" ||
                productId == "com.bibeshox.arma.gempack3")
            {
                validateMethod = 1;
            }

            if (validateMethod == 1)
            {
                ValidateReceiptEditor(e).Forget();
            }
            else
            {
                ValidateReceipt(e).Forget();
            }
#else
            ValidateReceipt(e).Forget();
#endif

            return PurchaseProcessingResult.Complete;
        }

        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log("OnPurchaseFailed: FAIL. " +
                $"Product: '{product.definition.storeSpecificId}', " +
                $"PurchaseFailureReason: {failureReason}");

            UI.WaitForServerResponse.Hide();

            string titleText = $"{Texts.UI.Error}!";
            string messageText = Texts.Message.PurchaseFailed;
            string acceptButtonText = Texts.UI.GotIt;
            UI.AlertDialog.ShowErrorDialogAsync(titleText, messageText, acceptButtonText).Forget();
        }

        private void OnToggleValueChanged(Toggle toggle, int index)
        {
            for (int i = 0; i < m_scrollViewObjects.Count; i++)
            {
                GameObject scrollViewObject = m_scrollViewObjects[i];
                scrollViewObject.SetActive(i == index);
            }
        }
        
        public void InitializePurchasing()
        {
            // Note: this should be initialized once, even though the player switch accounts.
            // If IAP is already initialized, return gently
            if (IsInitialized)
            {
                return;
            }

            // Create a builder for IAP service
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));

            // Register each item from PremiumShop
            PremiumShopPayload premiumShop = UI.Game.Player.PremiumShop;
            List<CurrencyPackPayload> allPacks = premiumShop.GemPacks.ToList();
            allPacks.AddRange(premiumShop.ShardPacks);

            foreach (CurrencyPackPayload pack in allPacks)
            {
                builder.AddProduct(pack.ProductId, ProductType.Consumable);
            }

            // Trigger IAP service initialization
            UnityPurchasing.Initialize(this, builder);
        }

        public void RefreshItems()
        {
            PremiumShopPayload premiumShop = UI.Game.Player.PremiumShop;
            m_gemsPanel.SetPacks(premiumShop.GemPacks);
            m_shardsPanel.SetPacks(premiumShop.ShardPacks);
            m_chestsPanel.SetPacks(premiumShop.ChestPacks);
        }

        private void OnGemPackButtonClicked(PremiumShopItemButton button)
        {
            Debug.Log($"Clicked on {button.name} ({button.ProductId})");

            BuyIapProductId(button.ProductId);
        }

        /// <summary>
        ///     Only GemPack Tokens are available at the moment.
        /// </summary>
        private void BuyIapProductId(string productId)
        {
            // If IAP service has not been initialized, fail hard
            if (!IsInitialized)
            {
                Debug.LogError("IAP Service is not initialized!");
                return;
            }

            // Pass in the product id to initiate purchase
            m_storeController.InitiatePurchase(productId);
        }

#if UNITY_EDITOR
        private async UniTask ValidateReceiptEditor(PurchaseEventArgs e)
        {
            UI.WaitForServerResponse.Show();

            string productId = e.purchasedProduct.definition.id;
            int amount = productId switch
            {
                "com.bibeshox.arma.gempack1" => 320,
                "com.bibeshox.arma.gempack2" => 1000,
                "com.bibeshox.arma.gempack3" => 2400,
                _ => 0
            };

            var entry = new GetCurrenciesRequestEntry
            {
                CurrencyType = CurrencyType.RedGem,
                Amount = amount
            };

            var request = new GetCurrenciesRequest
            {
                Entries = new[] { entry }
            };

            Game game = UI.Game;
            GetCurrenciesReply reply = await game.BackendDriver.GetCurrenciesAsync(request);

            if (game.ValidateReply(reply))
            {
                SetBalances(reply.ModifiedCurrencies);
            }

            UI.WaitForServerResponse.Hide();
        }
#endif

        private async UniTask ValidateReceipt(PurchaseEventArgs e)
        {
            UI.WaitForServerResponse.Show();

            // Deserialize receipt
            GooglePurchase googleReceipt = GooglePurchase.FromJson(e.purchasedProduct.receipt);

            var args = new ValidateGooglePlayPurchaseArgs
            {
                CurrencyCode = e.purchasedProduct.metadata.isoCurrencyCode,
                PurchasePrice = (uint)(e.purchasedProduct.metadata.localizedPrice * 100),
                ReceiptJson = googleReceipt.PayloadData.json,
                Signature = googleReceipt.PayloadData.signature
            };

            ValidateGooglePlayPurchaseReply reply = await Game.BackendDriver.ValidateGooglePlayPurchase(args);

            if (!reply.HasError)
            {
                var exchangeTokensRequest = new ExchangeTokensRequest
                {
                    TokenType = ExchangeTokensRequestTokenType.GemPack
                };

                ExchangeTokensReply exchangeReply =
                    await Game.BackendDriver.ExchangeTokensAsync(exchangeTokensRequest);
                if (UI.Game.ValidateReply(exchangeReply))
                {
                    // TODO: Refresh PremiumShop
                    SetBalances(exchangeReply.ModifiedCurrencies);
                }
            }
            else
            {
                string titleText = $"{Texts.UI.Error}!";
                string messageText = reply.ErrorMessage;
                string acceptButtonText = Texts.UI.GotIt;
                UI.AlertDialog.ShowErrorDialogAsync(titleText, messageText, acceptButtonText).Forget();

                // TODO: Add analytics here
            }

            UI.WaitForServerResponse.Hide();
        }

        private void SetBalances(ModifiedCurrencyPayload[] modifiedCurrencies)
        {
            foreach (ModifiedCurrencyPayload modifiedCurrency in modifiedCurrencies)
            {
                Debug.Log($"Get {modifiedCurrency.BalanceChange} {modifiedCurrency.CurrencyType}, " +
                    $"total = {modifiedCurrency.Balance}");

                Game.Player.SetBalance(modifiedCurrency.CurrencyType, modifiedCurrency.Balance,
                    modifiedCurrency.BalanceChange);
            }
        }

        private void OnShardPackButtonClicked(PremiumShopItemButton button)
        {
            Debug.Log($"Clicked on {button.name} ({button.ProductId})");

            BuyProduct(button).Forget();
        }

        private async UniTask BuyProduct(PremiumShopItemButton button)
        {
            string productId = button.ProductId;
            int amount = 1;
            Currency price = button.Price;

            if (button is ChestPanelRowItemButton chestPanelRowItemButton)
            {
                amount = chestPanelRowItemButton.Amount;
            }

            await Game.Player.BuyPremiumShopItemAsync(productId, amount, price);
        }

        private void OnChestPackButtonClicked(PremiumShopItemButton button)
        {
            Debug.Log($"Clicked on {button.name} ({button.ProductId})");

            BuyProduct(button).Forget();
        }

        [Serializable]
        private class Section
        {
            public float StartY;
            public Toggle Toggle;
            public Panel Panel;
        }
    }
}
