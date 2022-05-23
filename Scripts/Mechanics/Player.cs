using System.Collections.Generic;
using System.Linq;
using System.Text;
using Armageddon.AssetManagement;
using Armageddon.Backend;
using Armageddon.Backend.Functions;
using Armageddon.Backend.Payloads;
using Armageddon.Backend.Payloads.Local;
using Armageddon.Extensions;
using Armageddon.Localization;
using Armageddon.Mechanics.Abilities;
using Armageddon.Mechanics.Bonuses;
using Armageddon.Mechanics.Characters;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.Mechanics.Mails;
using Armageddon.Mechanics.Maps;
using Armageddon.Mechanics.Shops;
using Armageddon.Sheets.Abilities;
using Armageddon.Sheets.Items;
using Armageddon.UI;
using Cysharp.Threading.Tasks;
using Purity.Common;
using UnityEngine;

namespace Armageddon.Mechanics
{
    /// <summary>
    ///     Persistent data of Player
    /// </summary>
    public class Player : GameAccessibleObject
    {
        public const string PlayerProfileFileName = "PlayerProfile";
        public const string PlayerInventoryFileName = "PlayerInventory";
        public const string LocalBadgeManagerFileName = "LocalBadgeManager";

        // If these are not readonly or const, when used on OnApplicationQuit in Editor, they become false.
        public static readonly bool CompressPlayerProfileFile = true;
        public static readonly bool CompressPlayerInventoryFile = true;
        public static readonly bool CompressLocalBadgeManagerFile = true;

        private static UISystem m_ui;

        public int CurrentMapId = 1;

        public Player()
        {
            Debug.Log("Player Constructor");

            RegisterService(this);
        }

        private static UISystem UI
        {
            get
            {
                if (m_ui == null)
                {
                    m_ui = GetService<UISystem>();
                }

                return m_ui;
            }
        }

        public int Level { get; set; }
        public long Exp { get; set; }
        public long NextLevelExp { get; set; }
        public Dictionary<string, Currency> Currencies { get; private set; } = new();
        public List<Hero> Heroes { get; private set; } = new();
        public List<int> LockedHeroes { get; private set; } = new();
        public string HeroInstanceId { get; private set; }
        public PlayerInventory PlayerInventory { private set; get; }
        public HeroInventory HeroInventory { private set; get; }
        public int MaxInventoryCount { private set; get; }
        public Hero CurrentHero => GetSelectedHero();
        public int TotalItemCount => PlayerInventory.UsedSlotCount + HeroInventory.UsedSlotCount;
        public List<PlayerAbility> Abilities { get; } = new();
        public List<CardPower> CardPowers { get; private set; } = new();
        public List<Map> Maps { get; private set; } = new();
        public PremiumShopPayload PremiumShop { get; private set; }
        public List<Shop> Shops { get; private set; } = new();
        public List<Mail> Mails { get; private set; } = new();
        public LocalBadgeManager LocalBadgeManager { get; private set; } = new();

        public async UniTask<bool> ReinitializeAsync(LoadPlayerReply reply)
        {
            InitializeLocalFiles();

            Debug.Log("Player.ReinitializeAsync");

            Level = reply.Level;
            Exp = reply.Exp;

            Currencies.Clear();

            foreach (KeyValuePair<string, CurrencyPayload> kvp in reply.Balances)
            {
                var currency = new Currency(kvp.Value);
                Currencies.Add(kvp.Key, currency);
            }

            MaxInventoryCount = reply.MaxInventoryCount;

            Heroes.Clear();

            foreach (HeroPayload heroObject in reply.Heroes)
            {
                var hero = new Hero();
                await hero.InitializeAsync(heroObject);

                Heroes.Add(hero);
            }

            LockedHeroes.Clear();

            foreach (LockedHeroPayload lockedHeroObject in reply.LockedHeroes)
            {
                // TODO: Should be -1 as comment below
                const long instanceId = 0;

                // Make a dummy with instanceId -1
                var heroObject = new HeroPayload
                {
                    SheetId = lockedHeroObject.SheetId,
                    DyeIds = new[] { 0 },
                    InstanceId = instanceId.ToHex()
                };

                var hero = new Hero();
                await hero.InitializeAsync(heroObject);
                hero.Prices = lockedHeroObject.Prices;

                Heroes.Add(hero);
            }

            Heroes.Sort((x, y) => x.SheetId > y.SheetId ? 1 : -1);

            HeroInstanceId = reply.HeroInstanceId;

            PlayerInventory = await InitializeInventory(reply);

            HeroInventory = new HeroInventory();
            HeroInventory.InitializeFixedSlots(PlayerInventory, reply.HeroInventory);

            PlayerInventory.ShiftAllObjects();

            foreach (KeyValuePair<string, int> kvp in reply.Abilities)
            {
                if (!int.TryParse(kvp.Key, out int sheetId))
                {
                    Debug.LogError($"Cannot parse Ability {kvp.Key}");
                    continue;
                }

                var abilitySheet = await Assets.LoadSheetAsync<AbilitySheet>(sheetId);
                // AbilitySheet abilitySheet = await Assets.LoadAbilitySheetAsync(sheetId);

                var ability = new PlayerAbility(this, abilitySheet, kvp.Value)
                {
                    UpgradeableLevel = 5,
                    MaxUpgradeableLevel = 10
                };

                Abilities.Add(ability);
                // Abilities.Add(key, kvp.Value);
            }

            await InitializeCardPowersAsync(reply.CardPowerIds);

            var map = new Map(reply.Map);
            Maps.Add(map);

            // TODO: Change from storing payload to a real class like Shop and almost every else.
            PremiumShop = reply.PremiumShop;
            Game.ReinitializePremiumShop();

            await InitializeShopsAsync(reply.Shops);

            await InitializeMailboxAsync(reply.Mails);

            return true;
        }

        private void InitializeLocalFiles()
        {
            if (DeviceFile.Exists(LocalBadgeManagerFileName))
            {
                LocalBadgeManager = DeviceFile.ReadObjectFromJson<LocalBadgeManager>(
                    LocalBadgeManagerFileName, CompressLocalBadgeManagerFile);
            }

            LocalBadgeManager ??= new LocalBadgeManager
            {
                IsDataChanged = true
            };
        }

        private async UniTask<PlayerInventory> InitializeInventory(LoadPlayerReply reply)
        {
            var playerInventory = new PlayerInventory();
            playerInventory.AddSlotRange(reply.Items.Length);

            var slots = new List<SlotPayload>();

            // Load player inventory if the file exists.
            // Since it's fine that if a player tries to modify this...
            var playerLocalInventory = DeviceFile.ReadObjectFromJson<PlayerLocalInventory>(
                PlayerInventoryFileName, CompressPlayerInventoryFile);

            if (playerLocalInventory != null)
            {
                string[] instanceIds = playerLocalInventory.Slots.Split(',');

                foreach (string instanceId in instanceIds)
                {
                    var slot = new SlotPayload { InstanceId = instanceId };
                    slots.Add(slot);
                }
            }

            var noIndexItems = new List<Item>();

            foreach (ItemPayload itemObject in reply.Items)
            {
                var item = await Item.CreateAsync(itemObject);

                bool foundSlot = false;
                int index = 0;
                foreach (SlotPayload slot in slots)
                {
                    if (slot.InstanceId == item.InstanceId)
                    {
                        playerInventory.InsertObject(item, index);
                        foundSlot = true;
                        break;
                    }

                    index++;

                    // In case the save file is corrupt
                    if (index >= playerInventory.Slots.Count)
                    {
                        break;
                    }
                }

                if (!foundSlot)
                {
                    noIndexItems.Add(item);
                }
            }

            foreach (Item noIndexItem in noIndexItems)
            {
                playerInventory.InsertObject(noIndexItem);
            }

            var playerLocalProfile = DeviceFile.ReadObjectFromJson<PlayerLocalProfile>(
                PlayerProfileFileName, CompressPlayerProfileFile);

            if (playerLocalProfile != null)
            {
                foreach (InventorySlot slot in playerInventory.Slots)
                {
                    if (slot.Object is Item item)
                    {
                        if (playerLocalProfile.UsingItemInstances.Contains(item.InstanceId))
                        {
                            item.UseWhenStartGame = true;
                        }
                    }
                }
            }

            return playerInventory;
        }

        public void CompileAbilitiesFromVariousSources()
        {
            foreach (PlayerAbility ability in Abilities)
            {
                ability.ExtraLevel = 0;
            }

            foreach (InventorySlot inventorySlot in HeroInventory.Slots)
            {
                if (inventorySlot.Object is Item item)
                {
                    foreach (Bonus bonus in item.Bonuses)
                    {
                        if (bonus is AbilityBonus abilityBonus)
                        {
                            PlayerAbility foundAbility = Abilities.Find(x => x.Sheet == abilityBonus.Sheet);
                            if (foundAbility != null)
                            {
                                foundAbility.ExtraLevel += abilityBonus.Level;
                            }
                        }
                    }
                }
            }
        }

        public async UniTask InitializeCardPowersAsync(int[] cardPowerIds)
        {
            CardPowers.Clear();

            foreach (int cardPowerId in cardPowerIds)
            {
                var cardPower = await CardPower.CreateAsync(cardPowerId);
                CardPowers.Add(cardPower);
            }
        }

        private async UniTask InitializeShopsAsync(ShopPayload[] shopPayloads)
        {
            Shops.Clear();

            foreach (ShopPayload shopPayload in shopPayloads)
            {
                var shop = new Shop();
                await shop.ReinitializeAsync(shopPayload);

                Shops.Add(shop);
            }
        }

        private async UniTask InitializeMailboxAsync(MailPayload[] mailPayloads)
        {
            Mails.Clear();

            foreach (MailPayload mailPayload in mailPayloads)
            {
                var mail = new Mail();
                await mail.InitializeAsync(mailPayload);

                Mails.Add(mail);
            }

            LocalBadgeManager.InitializeMailBadges(Mails);
        }

        public void RefreshMailBadges()
        {
            LocalBadgeManager.SetToLocals(Mails);
        }

        private async UniTask ReinitializeShopAsync(ShopPayload shopPayload)
        {
            Shop shop = Shops.Find(x => x.Type == shopPayload.Type);
            await shop.ReinitializeAsync(shopPayload);
        }

        private async UniTask AddCardPowerAsync(int addedCardPowerId)
        {
            var cardPower = await CardPower.CreateAsync(addedCardPowerId);
            CardPowers.Add(cardPower);
        }

        private async UniTask AddCardPowersAsync(int[] addedCardPowerIds)
        {
            foreach (int addedCardPowerId in addedCardPowerIds)
            {
                var cardPower = await CardPower.CreateAsync(addedCardPowerId);
                CardPowers.Add(cardPower);
            }
        }

        private Hero GetSelectedHero()
        {
            return Heroes.FirstOrDefault(x => x.InstanceId == HeroInstanceId);
        }

        public bool HasHero(int heroSheetId)
        {
            return Heroes.FirstOrDefault(x => x.SheetId == heroSheetId) != null;
        }

        public void AddCharacter(Character character)
        {
            switch (character)
            {
                case Hero hero:
                    Heroes.Add(hero);
                    break;
                default:
                    Debug.LogWarning($"{character.GetType()} is not handled properly.");
                    break;
            }

            // Update: Set SkinPresetIndex and default color were set from backend
            // TODO Set SkinPresetIndex and default color here
        }

        private static void WriteFile<T>(string filePath, T data, 
            bool compressData, bool uploadToServerIfDataChanged = true)
        {
            string previousJsonString = string.Empty;

            if (DeviceFile.Exists(filePath))
            {
                previousJsonString = DeviceFile.ReadAllText(filePath);

                if (compressData)
                {
                    previousJsonString = Zip.DecompressFromBase64(previousJsonString);
                }
            }

            Debug.Log($"Writing {filePath}");

            string newJsonString = DeviceFile.WriteObjectToJson(filePath, data, compressData);

            if (!uploadToServerIfDataChanged)
            {
                return;
            }

            // Can go with this https://www.dotnetperls.com/file-equals
            // But the files are so small right now.
            if (previousJsonString.Length == newJsonString.Length)
            {
                if (previousJsonString == newJsonString)
                {
                    Debug.Log($"{filePath}'s content has not been changed.");
                    return;
                }
            }

            byte[] payload = new UTF8Encoding(true).GetBytes(newJsonString);

            Debug.Log($"Uploading {filePath} ({payload.Length} bytes.) since its content has been changed. ");
            Game.BackendDriver.UploadFileAsync(filePath);
        }

        // TODO: Write when start game as well.
        public void WriteAllLocalSaveFiles()
        {
            WritePlayerInventoryFile();
            WritePlayerProfileFile();
            // WriteLocalBadgeManagerFile();
        }

        public void WritePlayerInventoryFile()
        {
            var stringBuilder = new StringBuilder(string.Empty);

            foreach (InventorySlot playerInventorySlot in PlayerInventory.Slots)
            {
                string instanceId = playerInventorySlot.ReferenceId;
                stringBuilder.Append($"{instanceId},");
            }

            // for (int i = 0; i < 200; i++)
            // {
            //     stringBuilder.Append($"{PlayerInventory.Slots[0].ReferenceId.ToHex()},");
            // }

            if (stringBuilder.Length > 2)
            {
                stringBuilder.Remove(stringBuilder.Length - 2, 2);
            }

            var playerInventory = new PlayerLocalInventory
            {
                Slots = stringBuilder.ToString()
            };

            WriteFile(PlayerInventoryFileName, playerInventory, CompressPlayerInventoryFile);
        }

        public void WritePlayerProfileFile()
        {
            var usingItemInstances = new List<string>();

            foreach (InventorySlot slot in PlayerInventory.Slots)
            {
                if (slot.Object is Item { UseWhenStartGame: true } item)
                {
                    usingItemInstances.Add(item.InstanceId);
                }
            }

            var playerProfile = new PlayerLocalProfile
            {
                CurrentMapId = CurrentMapId,
                UsingItemInstances = usingItemInstances
            };

            WriteFile(PlayerProfileFileName, playerProfile, CompressPlayerProfileFile);
        }

        public void WriteLocalBadgeManagerFile()
        {
            LocalBadgeManager.SetToLocals(Mails);
            WriteFile(LocalBadgeManagerFileName, LocalBadgeManager, CompressLocalBadgeManagerFile);
        }

        public Hero GetHero(string instanceId)
        {
            return Heroes.Find(x => x.InstanceId == instanceId);
        }

        public void SetBalance(CurrencyType currencyType, int balance, int balanceChange)
        {
            Currency currency = Currencies[currencyType.ToCurrencyCode()];

            int addedBalance = currency.AddBalance(balanceChange);
            if (addedBalance != balance)
            {
                // Something's fishy here.
                // They may be modifying the JSON, but who cares...

                Debug.LogError($"{addedBalance} != {balance}");
            }
        }

        public long GetBalance(CurrencyType currencyType)
        {
            return Currencies[currencyType.ToCurrencyCode()].Balance;
        }

        public async UniTask<bool> SellItemAsync(Item item)
        {
            if (item == null)
            {
                Debug.LogError("Player.SellItemAsync: item == null");
                return false;
            }

            var request = new SellItemsRequest
            {
                Items = new[]
                {
                    new SellItemsRequestItem
                    {
                        InstanceId = item.InstanceId,
                        Amount = item.Quantity
                    }
                }
            };

            SellItemsReply reply = await Game.BackendDriver.SellItemsAsync(request);

            if (!Game.ValidateReply(reply))
            {
                return false;
            }

            PlayerInventory.RemoveObject(item);
            PlayerInventory.ShiftAllObjects();

            SetBalance(reply.CurrencyType, reply.Balance, reply.BalanceChange);

            return true;
        }

        public bool CanUnlockHero(Hero hero, CurrencyType currencyType)
        {
            if (!Game.CheckRequirementBeforeRequest)
            {
                return true;
            }

            long price = hero.Prices[currencyType.ToCurrencyCode()];

            return CanAfford(currencyType, (int)price);

            // TODO: Remove
            // Currency currency = Game.Player.Currencies[currencyType.ToCurrencyCode()];
            // long price = hero.Prices[currencyType.ToCurrencyCode()];
            //
            // return currency.Balance >= price;
        }

        private bool CanAfford(Currency price)
        {
            if (!Game.CheckRequirementBeforeRequest)
            {
                return true;
            }

            Currency currency = Game.Player.Currencies[price.Type.ToCurrencyCode()];

            return currency.Balance >= price.Amount;
        }

        private bool CanAfford(CurrencyType currencyType, int amount)
        {
            Currency currency = Game.Player.Currencies[currencyType.ToCurrencyCode()];

            return currency.Balance >= amount;
        }

        public async UniTask<bool> UnlockHeroAsync(Hero hero, CurrencyType currencyType)
        {
            if (hero == null)
            {
                Debug.LogError("Player.UnlockHero: hero == null");
                return false;
            }

            var request = new UnlockHeroRequest
            {
                HeroSheetId = hero.SheetId,
                CurrencyType = currencyType
            };

            UnlockHeroReply reply = await Game.BackendDriver.UnlockHeroAsync(request);

            if (!Game.ValidateReply(reply))
            {
                return false;
            }

            SetBalance(reply.CurrencyType, reply.Balance, reply.BalanceChange);
            hero.InstanceId = reply.HeroInstanceId;

            return true;
        }

        public async UniTask<bool> SelectHeroAsync(Hero hero)
        {
            if (hero == null)
            {
                Debug.LogError("Player.SelectHero: hero == null");
                return false;
            }

            var request = new SelectHeroRequest
            {
                HeroSheetId = hero.SheetId,
                HeroInstanceId = hero.InstanceId
            };

            SelectHeroReply reply = await Game.BackendDriver.SelectHeroAsync(request);

            if (!Game.ValidateReply(reply))
            {
                return false;
            }

            HeroInstanceId = reply.HeroInstanceId;

            return true;
        }

        private bool CanUseItem(Item item, out string reason)
        {
            reason = string.Empty;

            if (!Game.CheckRequirementBeforeRequest)
            {
                return true;
            }

            if (item.Type == ItemType.Card)
            {
                CardPower cardPower = CardPowers.Find(x => x.Sheet.Id == item.Sheet.Id);

                if (cardPower != null)
                {
                    // TODO: Localize
                    reason = $"You already have {item.Name}";
                    return false;
                }
            }

            return true;
        }

        private void ShowWaitingUI()
        {
            UI.WaitForServerResponse.Show();
        }

        private void HideWaitingUI()
        {
            UI.WaitForServerResponse.Hide();
        }

        public async UniTask<UseItemReply> UseItemAsync(Item item, int quantity, bool delayUpdateItems = false,
            bool delayUpdateCurrencies = false)
        {
            if (item == null)
            {
                Debug.LogError("Player.UseItemAsync: item == null");
                return null;
            }

            if (!CanUseItem(item, out string reason))
            {
                // TODO: Localize
                string titleText = string.Empty;
                string acceptButtonText = Texts.UI.GotIt;

                UI.AlertDialog.ShowWarningDialogAsync(titleText, reason, acceptButtonText).Forget();
                return null;
            }

            ShowWaitingUI();

            var request = new UseItemRequest
            {
                ItemInstanceId = item.InstanceId,
                Quantity = quantity
            };

            UseItemReply reply = await Game.BackendDriver.UseItemAsync(request);

            if (!Game.ValidateReply(reply))
            {
                return null;
            }

            await InsertOrUpdateItems(reply.UpdatedUsedItems);

            ItemPayload updatedUsedItem = reply.UpdatedUsedItems.FirstOrDefault(
                x => x.InstanceId == item.InstanceId);

            if (updatedUsedItem != null)
            {
                item.Quantity = updatedUsedItem.Quantity ?? 0;
            }

            if (reply.AddedCardPowerId != null)
            {
                string message = $"CardPower: {reply.AddedCardPowerId} has been added";
                Debug.Log(message);

                await AddCardPowerAsync(reply.AddedCardPowerId.Value);
            }

            if (!delayUpdateCurrencies)
            {
                UpdateBalances(reply.ModifiedCurrencies);
            }

            if (!delayUpdateItems)
            {
                await InsertOrUpdateItems(reply.GetItemsFromDrops());
            }

            HideWaitingUI();

            return reply;
        }

        public void UpdateBalances(ModifiedCurrencyPayload[] modifiedCurrencies)
        {
            foreach (ModifiedCurrencyPayload modifiedCurrency in modifiedCurrencies)
            {
                Debug.Log(
                    $"Get {modifiedCurrency.BalanceChange} {modifiedCurrency.CurrencyType}, total = {modifiedCurrency.Balance}");
                Game.Player.SetBalance(modifiedCurrency.CurrencyType, modifiedCurrency.Balance,
                    modifiedCurrency.BalanceChange);
            }
        }

        public async UniTask InsertOrUpdateItems(ItemPayload[] itemPayloads)
        {
            var items = new List<Item>();

            foreach (ItemPayload itemPayload in itemPayloads)
            {
                var item = await Item.CreateAsync(itemPayload);
                items.Add(item);
            }

            foreach (Item item in items)
            {
                if (item.Quantity == 0)
                {
                    InventorySlot desireSlot = PlayerInventory.Slots.FirstOrDefault(
                        x => x.Object.InstanceId == item.InstanceId);

                    if (desireSlot != null)
                    {
                        PlayerInventory.RemoveObject(desireSlot.Object);
                        PlayerInventory.ShiftAllObjects();
                    }
                    else
                    {
                        Debug.LogWarning($"UpdateItems: Cannot find {item.InstanceId}");
                    }
                }
                else
                {
                    if (item.IsStackable)
                    {
                        PlayerInventory.InsertOrUpdateObject(item);
                    }
                    else
                    {
                        PlayerInventory.InsertObject(item);
                    }
                }
            }
        }


        public bool CanUpgradeAbility(PlayerAbility playerAbility, out string reason)
        {
            reason = string.Empty;

            if (!Game.CheckRequirementBeforeRequest)
            {
                return true;
            }

            List<PlayerAbilityUpgradeDetailsRow> detailsRows = playerAbility.Sheet.UpgradeDetailsRows;

            if (playerAbility.Level + 1 > detailsRows.Count)
            {
                // Upgrade Button should be disable at this point.
                // The only way to get through is cheating...
                reason = "No more level to up.";
                return false;
            }

            PlayerAbilityUpgradeDetailsRow detailsRow = detailsRows[playerAbility.Level];
            CurrencyType currencyType = detailsRow.Price.CurrencyType;
            long currencyBalance = GetBalance(currencyType);
            long price = detailsRow.Price.Amount;

            if (currencyBalance < price)
            {
                reason = Lexicon.InsufficientCurrencyDetails(currencyType);
                return false;
            }

            return true;
        }

        public async UniTask<bool> UpgradeAbilityAsync(PlayerAbility playerAbility)
        {
            if (playerAbility == null)
            {
                Debug.LogError("Player.UpgradeAbilityAsync: ability == null");
                return false;
            }

            var request = new UpgradePlayerAbilityRequest
            {
                AbilityId = playerAbility.Sheet.Id
            };

            UpgradePlayerAbilityReply reply = await Game.BackendDriver.UpgradePlayerAbilityAsync(request);

            if (!Game.ValidateReply(reply))
            {
                return false;
            }

            SetBalance(reply.CurrencyType, reply.Balance, reply.BalanceChange);
            playerAbility.Level = reply.AbilityLevel;

            return true;
        }

        public bool CanStartGame(Stage stage)
        {
            if (!Game.CheckRequirementBeforeRequest)
            {
                return true;
            }

            int balance = Currencies[CurrencyCode.Energy].Balance;

            return balance >= stage.Payload.EnergyCost;
        }

        private void SetModifiedCurrencies(ModifiedCurrencyPayload[] modifiedCurrencies)
        {
            foreach (ModifiedCurrencyPayload modifiedCurrency in modifiedCurrencies)
            {
                string sign = modifiedCurrency.BalanceChange >= 0 ? "+" : string.Empty;
                Debug.Log($"{sign}{modifiedCurrency.BalanceChange} {modifiedCurrency.CurrencyType}, " +
                    $"total = {modifiedCurrency.Balance}");

                SetBalance(modifiedCurrency.CurrencyType, modifiedCurrency.Balance,
                    modifiedCurrency.BalanceChange);
            }
        }

        private async UniTask ShowCannotAffordDialogAsync(Currency currency)
        {
            string titleText = $"{Texts.UI.Attention}!";
            string message = Lexicon.InsufficientCurrency(currency.Type);
            string acceptButtonText = Texts.UI.GotIt;

            await UI.AlertDialog.ShowWarningDialogAsync(titleText, message, acceptButtonText);
        }

        /// <summary>
        ///     Use this as a template for adding more methods.
        /// </summary>
        public async UniTask<BuyPremiumShopItemReply> BuyPremiumShopItemAsync(string productId, int amount,
            Currency price)
        {
            if (!CanAfford(price))
            {
                ShowCannotAffordDialogAsync(price).Forget();
                return null;
            }

            ShowWaitingUI();

            var request = new BuyPremiumShopItemRequest
            {
                ProductId = productId,
                Amount = amount
            };

            BuyPremiumShopItemReply reply = await Game.BackendDriver.BuyPremiumShopItemAsync(request);
            if (Game.ValidateReply(reply))
            {
                SetModifiedCurrencies(reply.ModifiedCurrencies);

                await Game.Player.InsertOrUpdateItems(reply.UpdatedItems);

                PremiumShop = reply.PremiumShop;
                Game.ReinitializePremiumShop();
            }

            HideWaitingUI();

            return reply;
        }

        public async UniTask<ResetShopsReply> ResetShopAsync(Shop shop)
        {
            if (!CanAfford(shop.ResetPrice))
            {
                ShowCannotAffordDialogAsync(shop.ResetPrice).Forget();
                return null;
            }

            ShowWaitingUI();

            var request = new ResetShopsRequest
            {
                Entries = new[]
                {
                    new ResetShopsRequestEntry { ShopType = shop.Type, SpendCurrency = true }
                }
            };

            ResetShopsReply reply = await Game.BackendDriver.ResetShopsAsync(request);
            if (Game.ValidateReply(reply))
            {
                SetModifiedCurrencies(reply.ModifiedCurrencies);

                await InitializeShopsAsync(reply.Shops);
            }

            HideWaitingUI();

            return reply;
        }

        public async UniTask<BuyShopItemReply> BuyShopItemAsync(Shop shop, ShopItem shopItem, int quantity)
        {
            Item buyingItem = shopItem.Item;
            if (!CanAfford(buyingItem.ShopSellPrice))
            {
                ShowCannotAffordDialogAsync(buyingItem.ShopSellPrice).Forget();
                return null;
            }

            ShowWaitingUI();

            var request = new BuyShopItemRequest
            {
                ShopType = shop.Type,
                ItemInstanceId = buyingItem.InstanceId,
                Quantity = quantity
            };

            BuyShopItemReply reply = await Game.BackendDriver.BuyShopItemAsync(request);
            if (Game.ValidateReply(reply))
            {
                SetModifiedCurrencies(reply.ModifiedCurrencies);

                await ReinitializeShopAsync(reply.Shop);
                await InsertOrUpdateItems(reply.UpdatedItems);

                foreach (ItemPayload boughtItem in reply.BoughtItems)
                {
                    ItemSheet itemSheet = await Assets.LoadItemSheetAsync(boughtItem.SheetId);
                    Debug.Log($"You have just bought {boughtItem.Quantity} {itemSheet}(s)");
                }
            }

            HideWaitingUI();

            return reply;
        }

        public async UniTask<ClaimMailsReply> ClaimAllMailsAsync()
        {
            List<Mail> validMails = Mails.Where(mail => mail.AttachedItems.Count > 0).ToList();

            return await ClaimMailsAsync(validMails);
        }

        public async UniTask<ClaimMailsReply> ClaimMailAsync(Mail mail)
        {
            return await ClaimMailsAsync(new List<Mail> { mail });
        }

        public async UniTask<ClaimMailsReply> ClaimMailsAsync(List<Mail> mails)
        {
            ShowWaitingUI();

            var request = new ClaimMailsRequest
            {
                Entries = mails.Select(mail =>
                    new ClaimMailsRequestEntry
                    {
                        InstanceId = mail.InstanceId
                    }).ToArray()
            };

            ClaimMailsReply reply = await Game.BackendDriver.ClaimMailsAsync(request);
            if (Game.ValidateReply(reply))
            {
                SetModifiedCurrencies(reply.ModifiedCurrencies);

                foreach (ModifiedCurrencyPayload payload in reply.ModifiedCurrencies)
                {
                    Debug.Log($"You have just claimed {payload.BalanceChange} {payload.CurrencyType}(s)");
                }

                await InsertOrUpdateItems(reply.UpdatedItems);

                foreach (ItemPayload claimedItem in reply.ClaimedItems)
                {
                    ItemSheet itemSheet = await Assets.LoadItemSheetAsync(claimedItem.SheetId);
                    Debug.Log($"You have just claimed {claimedItem.Quantity} {itemSheet}(s)");
                }

                RemoveMails(reply.RemovedMailInstanceIds);
            }

            HideWaitingUI();

            return reply;
        }

        /// <summary>
        ///     Remove the elements in the list (Mails). Not to be confused with DeleteMailAsync().
        /// </summary>
        private void RemoveMails(string[] removedMailInstanceIds)
        {
            foreach (string removedMailInstanceId in removedMailInstanceIds)
            {
                Mail removedMail = Mails.Find(x => x.InstanceId == removedMailInstanceId);
                if (removedMail != null)
                {
                    Mails.Remove(removedMail);
                }
            }
        }

        public async UniTask<DeleteMailsReply> DeleteAllMailAsync()
        {
            return await DeleteMailsAsync(Mails);
        }

        public async UniTask<DeleteMailsReply> DeleteMailAsync(Mail mail)
        {
            return await DeleteMailsAsync(new List<Mail> { mail });
        }

        public async UniTask<DeleteMailsReply> DeleteMailsAsync(List<Mail> mails)
        {
            ShowWaitingUI();

            var request = new DeleteMailsRequest
            {
                Entries = mails.Select(mail =>
                    new DeleteMailsRequestEntry
                    {
                        InstanceId = mail.InstanceId
                    }).ToArray()
            };

            DeleteMailsReply reply = await Game.BackendDriver.DeleteMailsAsync(request);
            if (Game.ValidateReply(reply))
            {
                foreach (string removedMailInstanceId in reply.RemovedMailInstanceIds)
                {
                    Mail mail = Mails.Find(x => x.InstanceId == removedMailInstanceId);
                    if (mail != null)
                    {
                        Debug.Log($"You have just deleted {mail}");
                    }
                }

                RemoveMails(reply.RemovedMailInstanceIds);
            }

            HideWaitingUI();

            return reply;
        }
    }
}
