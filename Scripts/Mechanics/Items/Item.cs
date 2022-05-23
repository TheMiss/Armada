using System;
using System.Collections.Generic;
using Armageddon.AssetManagement;
using Armageddon.Backend.Payloads;
using Armageddon.Mechanics.Bonuses;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Stats;
using Armageddon.Sheets.Abilities;
using Armageddon.Sheets.Items;
using Armageddon.Worlds.Actors.Characters;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Armageddon.Mechanics.Items
{
    public class ItemStat
    {
        public readonly double Value;
        public readonly double MinValue;
        public readonly double MaxValue;

        public ItemStat(ItemStatPayload itemStatPayload)
        {
            Value = itemStatPayload.Value;
            MinValue = itemStatPayload.MinValue ?? 0;
            MaxValue = itemStatPayload.MaxValue ?? 0;
        }

        public ItemStat(double value)
        {
            Value = value;
        }

        public static implicit operator double(ItemStat stat)
        {
            return stat.Value;
        }

        public string AsStatMultiplier()
        {
            return $"{Value:F1}/Lv";
        }

        public string AsFireRate()
        {
            return $"{Value:F1}/s";
        }

        public string AsCriticalChance()
        {
            return $"{(int)Math.Round(Value * 100)}%";
        }

        public string AsCriticalDamage()
        {
            return $"{(int)Math.Round(Value * 100)}%";
        }

        public string AsDps()
        {
            return $"{(long)Value}";
        }
    }

    public abstract class Item : GameAccessibleObject, IInventoryObject, IStatSource
    {
        public string Name => Sheet.Name;

        public ItemType Type => Sheet.Type;

        public int Level { get; private set; }

        public ItemQuality Quality { get; private set; }

        public ItemSheet Sheet { get; protected set; }

        public List<Bonus> Bonuses { get; private set; } = new();

        public Currency ShopSellPrice { get; set; }

        public int BuyPrice { get; set; }

        public int Quantity { set; get; }

        public bool UseWhenStartGame { get; set; }

        public bool IsStackable => Sheet.IsStackable;

        public bool IsEquipable => Sheet.IsEquipable;

        public bool IsUsable => Sheet.IsUsable;

        public CharacterSkin CharacterSkin => Sheet.CharacterSkin;

        public string InstanceId { get; private set; }

        protected virtual async UniTask InitializeStatsAsync(ItemSheet sheet, ItemPayload itemPayload)
        {
            await UniTask.CompletedTask;
        }

        private static Item CreateItem(ItemType itemType)
        {
            return itemType switch
            {
                ItemType.PrimaryWeapon => new Weapon(),
                ItemType.SecondaryWeapon => new Weapon(),
                ItemType.Kernel => new Kernel(),
                ItemType.Armor => new Armor(),
                ItemType.Accessory => new Accessory(),
                ItemType.Companion => new Companion(),
                ItemType.Consumable => new Consumable(),
                ItemType.Skin => new Skin(),
                ItemType.Card => new Card(),
                ItemType.Chest => new LootBox(),
                ItemType.Misc => new Misc(),
                _ => throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null)
            };
        }

        public static async UniTask<Item> CreateAsync(ItemPayload itemPayload)
        {
            ItemSheet sheet = await Assets.LoadItemSheetAsync(itemPayload.SheetId);

            if (sheet == null)
            {
                Debug.LogError($"Wait what?!... No ItemSheet {itemPayload.SheetId}");
                return null;
            }

            Item item = CreateItem(sheet.Type);
            item.Sheet = sheet;
            item.InstanceId = itemPayload.InstanceId;
            item.Level = itemPayload.Level ?? 0;
            item.Quality = itemPayload.Quality ?? ItemQuality.Common;
            item.Quantity = itemPayload.Quantity ?? 0;
            item.BuyPrice = itemPayload.BuyPrice ?? 0;
            item.ShopSellPrice = new Currency(itemPayload.ShopSellPrice);


            if (!item.IsEquipable)
            {
                item.Quality = sheet.Quality;
            }
            // if ((int)item.Type > (int)ItemType.Consumable)
            // {
            //     item.Quality = sheet.Quality;
            // }

            await item.InitializeStatsAsync(sheet, itemPayload);

            var bonuses = new List<Bonus>();

            foreach (StatBonusPayload statBonusObject in itemPayload.StatBonuses)
            {
                var statBonus = new StatBonus(item, statBonusObject.StatId, statBonusObject.StatModifierType,
                    statBonusObject.Value, statBonusObject.MinValue ?? 0, statBonusObject.MaxValue ?? 0);

                bonuses.Add(statBonus);
            }

            foreach (AbilityBonusPayload abilityBonusObject in itemPayload.AbilityBonuses)
            {
                var abilitySheet = await Assets.LoadSheetAsync<AbilitySheet>(abilityBonusObject.SheetId);
                // AbilitySheet abilitySheet = await Assets.LoadAbilitySheetAsync(abilityBonusObject.sheetId);

                var abilityBonus = new AbilityBonus(item, abilitySheet,
                    abilityBonusObject.Level,
                    abilityBonusObject.MinLevel ?? 0,
                    abilityBonusObject.MaxLevel ?? 0);

                bonuses.Add(abilityBonus);
            }

            item.Bonuses = bonuses;

            return item;
        }

        public override string ToString()
        {
            return $"{Name}({InstanceId})";
        }
    }
}
