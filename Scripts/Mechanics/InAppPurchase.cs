using Armageddon.Backend.Payloads;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Armageddon.Mechanics
{
    public class InAppPurchase : GameAccessibleObject, IStoreListener
    {
        private IStoreController m_StoreController;
        // public void RefreshIAPItems() {
        //     PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), result => {
        //         Catalog = result.Catalog;
        //
        //         // Make UnityIAP initialize
        //         InitializePurchasing();
        //     }, error => Debug.LogError(error.GenerateErrorReport()));
        // }

        public InAppPurchase()
        {
            RegisterService(this);
        }

        public bool IsInitialized => m_StoreController != null;

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            m_StoreController = controller;
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log($"OnInitializeFailed InitializationFailureReason:{error}");
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            return PurchaseProcessingResult.Complete;
        }

        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log("OnPurchaseFailed: FAIL. " +
                $"Product: '{product.definition.storeSpecificId}', " +
                $"PurchaseFailureReason: {failureReason}");

            // TODO:
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
            PremiumShopPayload premiumShop = Game.Player.PremiumShop;
            CurrencyPackPayload[] allPacks = premiumShop.GemPacks;
            allPacks.AddRange(premiumShop.ShardPacks);

            foreach (CurrencyPackPayload pack in allPacks)
            {
                builder.AddProduct(pack.ProductId, ProductType.Consumable);
            }

            // Trigger IAP service initialization
            UnityPurchasing.Initialize(this, builder);
        }
    }
}
