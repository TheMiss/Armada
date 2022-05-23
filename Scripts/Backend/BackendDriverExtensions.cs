using System.Threading.Tasks;
using Armageddon.Assistance.BackendDrivers;
using Armageddon.Backend.Functions;
using Cysharp.Threading.Tasks;
#if DEBUG
using Armageddon.Backend.Functions.Cheats;
#endif


namespace Armageddon.Backend
{
    public static class BackendDriverExtensions
    {
        public static async UniTask<LoadPlayerReply> LoadPlayerAsync(this BackendDriver driver,
            LoadPlayerRequest request)
        {
            return await driver.SendRequestAsync<LoadPlayerRequest, LoadPlayerReply>(request);
        }

        public static async UniTask<EquipHeroItemReply> EquipHeroItemAsync(this BackendDriver driver,
            EquipHeroItemRequest request)
        {
            return await driver.SendRequestAsync<EquipHeroItemRequest, EquipHeroItemReply>(request);
        }

        public static async UniTask<UnequipHeroItemReply> UnequipHeroItemAsync(this BackendDriver driver,
            UnequipHeroItemRequest request)
        {
            return await driver.SendRequestAsync<UnequipHeroItemRequest, UnequipHeroItemReply>(request);
        }

        public static async UniTask<SellItemsReply> SellItemsAsync(this BackendDriver driver, SellItemsRequest request)
        {
            return await driver.SendRequestAsync<SellItemsRequest, SellItemsReply>(request);
        }

        public static async UniTask<UnlockHeroReply> UnlockHeroAsync(this BackendDriver driver,
            UnlockHeroRequest request)
        {
            return await driver.SendRequestAsync<UnlockHeroRequest, UnlockHeroReply>(request);
        }

        public static async UniTask<SelectHeroReply> SelectHeroAsync(this BackendDriver driver,
            SelectHeroRequest request)
        {
            return await driver.SendRequestAsync<SelectHeroRequest, SelectHeroReply>(request);
        }

        public static async UniTask<UseItemReply> UseItemAsync(this BackendDriver driver,
            UseItemRequest request)
        {
            return await driver.SendRequestAsync<UseItemRequest, UseItemReply>(request);
        }

        public static async UniTask<UpgradePlayerAbilityReply> UpgradePlayerAbilityAsync(this BackendDriver driver,
            UpgradePlayerAbilityRequest request)
        {
            return await driver.SendRequestAsync<UpgradePlayerAbilityRequest, UpgradePlayerAbilityReply>(request);
        }

        public static async UniTask<StartGameReply> StartGameAsync(this BackendDriver driver,
            StartGameRequest request)
        {
            return await driver.SendRequestAsync<StartGameRequest, StartGameReply>(request);
        }

        public static async UniTask<EndGameReply> EndGameAsync(this BackendDriver driver,
            EndGameRequest request)
        {
            return await driver.SendRequestAsync<EndGameRequest, EndGameReply>(request);
        }

        public static async UniTask<ExchangeTokensReply> ExchangeTokensAsync(this BackendDriver driver,
            ExchangeTokensRequest request)
        {
            return await driver.SendRequestAsync<ExchangeTokensRequest, ExchangeTokensReply>(request);
        }

        public static async UniTask<BuyPremiumShopItemReply> BuyPremiumShopItemAsync(this BackendDriver driver,
            BuyPremiumShopItemRequest request)
        {
            return await driver.SendRequestAsync<BuyPremiumShopItemRequest, BuyPremiumShopItemReply>(request);
        }

        public static async UniTask<ResetShopsReply> ResetShopsAsync(this BackendDriver driver,
            ResetShopsRequest request)
        {
            return await driver.SendRequestAsync<ResetShopsRequest, ResetShopsReply>(request);
        }

        public static async UniTask<BuyShopItemReply> BuyShopItemAsync(this BackendDriver driver,
            BuyShopItemRequest request)
        {
            return await driver.SendRequestAsync<BuyShopItemRequest, BuyShopItemReply>(request);
        }

        public static async UniTask<ClaimMailsReply> ClaimMailsAsync(this BackendDriver driver,
            ClaimMailsRequest request)
        {
            return await driver.SendRequestAsync<ClaimMailsRequest, ClaimMailsReply>(request);
        }
        
        public static async UniTask<DeleteMailsReply> DeleteMailsAsync(this BackendDriver driver,
            DeleteMailsRequest request)
        {
            return await driver.SendRequestAsync<DeleteMailsRequest, DeleteMailsReply>(request);
        }

#if DEBUG || DEVELOPMENT_BUILD
        // Cheat

        public static async UniTask<GetEquipableItemsReply> GetEquipableItemsAsync(this BackendDriver driver,
            GetEquipableItemsRequest request)
        {
            return await driver.SendRequestAsync<GetEquipableItemsRequest, GetEquipableItemsReply>(request);
        }

        public static async UniTask<GetConsumablesReply> GetConsumablesAsync(this BackendDriver driver,
            GetConsumablesRequest request)
        {
            return await driver.SendRequestAsync<GetConsumablesRequest, GetConsumablesReply>(request);
        }

        public static async Task<GetSkinDyesReply> GetSkinDyesAsync(this BackendDriver driver,
            GetSkinDyesRequest request)
        {
            return await driver.SendRequestAsync<GetSkinDyesRequest, GetSkinDyesReply>(request);
        }

        public static async Task<GetCardsReply> GetCardsAsync(this BackendDriver driver, GetCardsRequest request)
        {
            return await driver.SendRequestAsync<GetCardsRequest, GetCardsReply>(request);
        }

        public static async Task<GetLootBoxesReply> GetLootBoxesAsync(this BackendDriver driver,
            GetLootBoxesRequest request)
        {
            return await driver.SendRequestAsync<GetLootBoxesRequest, GetLootBoxesReply>(request);
        }

        public static async Task<GetCurrenciesReply> GetCurrenciesAsync(this BackendDriver driver,
            GetCurrenciesRequest request)
        {
            return await driver.SendRequestAsync<GetCurrenciesRequest, GetCurrenciesReply>(request);
        }
#endif
    }
}
