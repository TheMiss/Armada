#if DEBUG

using System;
using System.Collections.Generic;
using Armageddon.AssetManagement;
using Armageddon.Backend;
using Armageddon.Backend.Functions.Cheats;
using Armageddon.Backend.Payloads;
using Armageddon.Games;
using Armageddon.Mechanics.Items;
using Armageddon.Sheets.Items;
using Cysharp.Threading.Tasks;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Armageddon.Mechanics
{
    public class CheatCode : Context
    {
        [SaveOnValueChanged]
        public int ItemGeneratorId = 1;

        [SaveOnValueChanged]
        public int ItemGeneratorCount = 1;

        [SaveOnValueChanged]
        public int Star = 1;

        [SaveOnValueChanged]
        public int Level = 1;

        [SaveOnValueChanged]
        public ItemQuality MinQuality = ItemQuality.Common;

        [SaveOnValueChanged]
        public ItemQuality MaxQuality = ItemQuality.Legendary;

        public ItemType ItemType = ItemType.PrimaryWeapon;

        [SaveOnValueChanged]
        public int LootBoxLevel = 5;

        [SaveOnValueChanged]
        public int LootBoxQuantity = 5;

        private static Game Game { set; get; }

        public static CheatCode Instance { get; private set; }

        public static void Create(Game game)
        {
            Game = game;
            var cheatCodeObject = new GameObject("CheatCode");
            Instance = cheatCodeObject.AddComponent<CheatCode>();
            Instance.CanTick = true;

            LoadValues();
        }

        private static void LoadValues()
        {
            Instance.ItemGeneratorId = PlayerPrefs.GetInt(nameof(ItemGeneratorId), 1);
            Instance.ItemGeneratorCount = PlayerPrefs.GetInt(nameof(ItemGeneratorCount), 1);
            Instance.Star = PlayerPrefs.GetInt(nameof(Star), 1);
            Instance.Level = PlayerPrefs.GetInt(nameof(Level), 1);
            Instance.MinQuality = (ItemQuality)PlayerPrefs.GetInt(nameof(MinQuality));
            Instance.MaxQuality = (ItemQuality)PlayerPrefs.GetInt(nameof(MaxQuality));
            Instance.LootBoxLevel = PlayerPrefs.GetInt(nameof(LootBoxLevel), 5);
            Instance.LootBoxQuantity = PlayerPrefs.GetInt(nameof(LootBoxQuantity), 1);
        }

        private static void SaveValues()
        {
            PlayerPrefs.SetInt(nameof(ItemGeneratorId), Instance.ItemGeneratorId);
            PlayerPrefs.SetInt(nameof(ItemGeneratorCount), Instance.ItemGeneratorCount);
            PlayerPrefs.SetInt(nameof(Star), Instance.Star);
            PlayerPrefs.SetInt(nameof(Level), Instance.Level);
            PlayerPrefs.SetInt(nameof(MinQuality), (int)Instance.MinQuality);
            PlayerPrefs.SetInt(nameof(MaxQuality), (int)Instance.MaxQuality);
            PlayerPrefs.SetInt(nameof(LootBoxLevel), Instance.LootBoxLevel);
            PlayerPrefs.SetInt(nameof(LootBoxQuantity), Instance.LootBoxQuantity);

            PlayerPrefs.Save();
        }

        public static async UniTaskVoid GetAllEquipableItemsAsync()
        {
            var items = new List<GetEquipableItemsRequestEntry>();

            int itemQualitySize = EnumEx.GetSize<ItemQuality>();

            for (int i = 0; i < itemQualitySize; i++)
            {
                int itemTypeSize = EnumEx.GetSize<ItemType>();
                for (int j = 0; j < itemTypeSize; j++)
                {
                    var itemType = (ItemType)j;
                    var minQuality = (ItemQuality)i;
                    var maxQuality = (ItemQuality)i;

                    var parameters = new GetEquipableItemsRequestEntry
                    {
                        Star = 1,
                        Level = Random.Range(1, 100),
                        ItemType = itemType,
                        MinQuality = minQuality,
                        MaxQuality = maxQuality
                    };

                    items.Add(parameters);
                }
            }

            var request = new GetEquipableItemsRequest
            {
                Entries = items.ToArray()
            };

            await GetEquipableItemsAsync(request);
        }

        public static async UniTaskVoid GetEquipableItemsAsync(List<GetEquipableItemsRequestEntry> entries)
        {
            var request = new GetEquipableItemsRequest
            {
                Entries = entries.ToArray()
            };

            await GetEquipableItemsAsync(request);
        }

        public static async UniTaskVoid GetAllConsumablesAsync()
        {
            List<ConsumableSheet> consumableSheets = await Assets.LoadConsumableSheetsAsync();
            var entries = new List<GetConsumablesRequestEntry>();

            foreach (ConsumableSheet consumableSheet in consumableSheets)
            {
                var entry = new GetConsumablesRequestEntry
                {
                    ItemId = consumableSheet.Id,
                    Quantity = Random.Range(1, 11)
                };

                entries.Add(entry);
            }

            var request = new GetConsumablesRequest
            {
                Entries = entries.ToArray()
            };

            await GetConsumablesAsync(request);
        }

        public static async UniTaskVoid GetAllCardsAsync()
        {
            List<CardSheet> cardSheets = await Assets.LoadCardSheetsAsync();
            var entries = new List<GetCardsRequestEntry>();

            foreach (CardSheet cardSheet in cardSheets)
            {
                var entry = new GetCardsRequestEntry
                {
                    ItemId = cardSheet.Id
                };

                entries.Add(entry);
            }

            var request = new GetCardsRequest
            {
                Entries = entries.ToArray()
            };

            await GetCardsAsync(request);
        }

        public static async UniTaskVoid GetAllLootBoxesAsync()
        {
            List<ChestSheet> lootBoxSheets = await Assets.LoadLootBoxSheetsAsync();
            var entries = new List<GetLootBoxesRequestEntry>();

            foreach (ChestSheet lootBoxSheet in lootBoxSheets)
            {
                var entry = new GetLootBoxesRequestEntry
                {
                    ItemId = lootBoxSheet.Id,
                    Quantity = Instance.LootBoxQuantity
                };

                entries.Add(entry);
            }

            var request = new GetLootBoxesRequest
            {
                Entries = entries.ToArray()
            };

            await GetLootBoxesAsync(request);
        }

        public static async UniTask GetCurrenciesAsync(params (CurrencyType Type, int Amount)[] currencies)
        {
            Game.UI.WaitForServerResponse.Show();

            // var request = new GetCurrenciesRequest
            // {
            //     currencyType = (int)currencyType,
            //     amount = amount
            // };

            var entries = new List<GetCurrenciesRequestEntry>();

            foreach ((CurrencyType type, int amount) in currencies)
            {
                var entry = new GetCurrenciesRequestEntry
                {
                    CurrencyType = type,
                    Amount = amount
                };

                entries.Add(entry);
            }

            var request = new GetCurrenciesRequest
            {
                Entries = entries.ToArray()
            };

            GetCurrenciesReply reply = await Game.BackendDriver.GetCurrenciesAsync(request);

            if (Game.ValidateReply(reply))
            {
                foreach (ModifiedCurrencyPayload modifiedCurrency in reply.ModifiedCurrencies)
                {
                    Debug.Log(
                        $"Get {modifiedCurrency.BalanceChange} {modifiedCurrency.CurrencyType}, total = {modifiedCurrency.Balance}");
                    Game.Player.SetBalance(modifiedCurrency.CurrencyType, modifiedCurrency.Balance,
                        modifiedCurrency.BalanceChange);
                }

                // Debug.Log($"Get {reply.balanceChange} {reply.currencyType}, total = {reply.balance}");
                // Game.Player.SetBalance(reply.currencyType, reply.balance, reply.balanceChange);
            }

            Game.UI.WaitForServerResponse.Hide();
        }

        private static async UniTask GetEquipableItemsAsync(GetEquipableItemsRequest request)
        {
            Game.UI.WaitForServerResponse.Show();

            GetEquipableItemsReply reply = await Game.BackendDriver.GetEquipableItemsAsync(request);

            if (!Game.ValidateReply(reply))
            {
                return;
            }

            Debug.Log($"{nameof(GetEquipableItemsAsync)}: reply.Items.Length = {reply.Items.Length}");

            await Game.Player.InsertOrUpdateItems(reply.Items);

            Game.UI.WaitForServerResponse.Hide();
        }

        private static async UniTask GetConsumablesAsync(GetConsumablesRequest request)
        {
            Game.UI.WaitForServerResponse.Show();

            GetConsumablesReply reply = await Game.BackendDriver.GetConsumablesAsync(request);
            Debug.Log($"{nameof(GetConsumablesAsync)}: reply.Items.Length = {reply.Items.Length}");

            await Game.Player.InsertOrUpdateItems(reply.Items);

            Game.UI.WaitForServerResponse.Hide();
        }

        private static async UniTask GetSkinDyesAsync(GetSkinDyesRequest request)
        {
            Game.UI.WaitForServerResponse.Show();

            GetSkinDyesReply reply = await Game.BackendDriver.GetSkinDyesAsync(request);
            Debug.Log($"GetSkinDyesAsync: reply.Items.Length = {reply.Items.Length}");

            await Game.Player.InsertOrUpdateItems(reply.Items);

            Game.UI.WaitForServerResponse.Hide();
        }

        private static async UniTask GetCardsAsync(GetCardsRequest request)
        {
            Game.UI.WaitForServerResponse.Show();

            GetCardsReply reply = await Game.BackendDriver.GetCardsAsync(request);
            Debug.Log($"GetCardsAsync: reply.Items.Length = {reply.Items.Length}");

            await Game.Player.InsertOrUpdateItems(reply.Items);

            Game.UI.WaitForServerResponse.Hide();
        }

        private static async UniTask GetLootBoxesAsync(GetLootBoxesRequest request)
        {
            Game.UI.WaitForServerResponse.Show();

            GetLootBoxesReply reply = await Game.BackendDriver.GetLootBoxesAsync(request);
            Debug.Log($"{nameof(GetLootBoxesAsync)}: reply.Items.Length = {reply.Items.Length}");

            await Game.Player.InsertOrUpdateItems(reply.Items);

            Game.UI.WaitForServerResponse.Hide();
        }

        [Button("Get Equipable Items (F1)")]
        private void GetEquipableItems()
        {
            var entries = new List<GetEquipableItemsRequestEntry>();

            for (int i = 0; i < ItemGeneratorCount; i++)
            {
                var entry = new GetEquipableItemsRequestEntry
                {
                    ItemGeneratorId = ItemGeneratorId,
                    Star = Star,
                    Level = Level,
                    ItemType = ItemType,
                    MinQuality = MinQuality,
                    MaxQuality = MaxQuality
                };

                entries.Add(entry);
            }

            GetEquipableItemsAsync(entries).Forget();
        }

        [Button("Get All Consumables (F2)")]
        private void GetAllConsumables()
        {
            GetAllConsumablesAsync().Forget();
        }

        [Button("Get All Cards (F4)")]
        private void GetAllCards()
        {
            GetAllCardsAsync().Forget();
        }

        [Button("Get All LootBoxes (F5)")]
        private void GetAllLootBoxes()
        {
            GetAllLootBoxesAsync().Forget();
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                GetEquipableItems();
                // GetAllEquipableItemsAsync().Forget();
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                GetAllConsumables();
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                GetAllCards();
            }
            else if (Input.GetKeyDown(KeyCode.F5))
            {
                GetAllLootBoxes();
            }
            else if (Input.GetKeyDown(KeyCode.F9))
            {
                GetCurrenciesAsync((CurrencyType.Energy, 100)).Forget();
            }
            else if (Input.GetKeyDown(KeyCode.F10))
            {
                GetCurrenciesAsync((CurrencyType.GoldShard, 500000)).Forget();
            }
            else if (Input.GetKeyDown(KeyCode.F11))
            {
                GetCurrenciesAsync((CurrencyType.GoldShard, 4000), (CurrencyType.RedGem, 400)).Forget();
            }
            else if (Input.GetKeyDown(KeyCode.F12))
            {
                GetCurrenciesAsync((CurrencyType.RedGem, 6000)).Forget();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                var game = GetService<Game>();
                game.Player.WriteAllLocalSaveFiles();
                // game.BackendDriver.UploadGameSaveAsync(Player.PlayerInventoryFileName).Forget();
            }
        }

        [IncludeMyAttributes]
        [OnValueChanged(nameof(SaveValues))]
        private class SaveOnValueChangedAttribute : Attribute
        {
        }
    }
}

#endif
