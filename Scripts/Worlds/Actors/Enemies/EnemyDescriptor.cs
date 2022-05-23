using System;
using System.Collections.Generic;
using Armageddon.Backend.Payloads;
using Armageddon.Mechanics.Characters;
using Armageddon.Mechanics.Items;
using Armageddon.Sheets;
using Armageddon.Sheets.Actors;
using Armageddon.Worlds.Actors.Characters;

namespace Armageddon.Worlds.Actors.Enemies
{
    public class StageDropCurrency
    {
        public Sheet Sheet { get; set; }
        public ItemQuality Quality { get; set; }
        public int Quantity { get; set; }
    }

    public class StageDropItem
    {
        public Sheet Sheet { get; set; }
        public ItemQuality Quality { get; set; }
        public int Quantity { get; set; }
    }

    public class StageDrop
    {
        public StageDrop(int id, StageDropCurrency currency)
        {
            Id = id;
            Type = DropType.Currency;
            Currency = currency;
        }

        public StageDrop(int id, StageDropItem item)
        {
            Id = id;
            Type = DropType.Item;
            Item = item;
        }

        public int Id { get; }
        public DropType Type { get; }
        public StageDropCurrency Currency { get; }
        public StageDropItem Item { get; }

        public override string ToString()
        {
            return Type switch
            {
                DropType.Currency => $"{Currency}",
                DropType.Item => $"{Item.Sheet.Name} x{Item.Quantity} ({Id})",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public class EnemyDescriptor : CharacterDescriptor
    {
        public EnemySheet Sheet { get; set; }
        public EnemyRank Rank { get; set; }
        public List<StageDrop> Drops { get; set; }
    }
}
