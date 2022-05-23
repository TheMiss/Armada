using System;
using Armageddon.Backend.Attributes;
using Armageddon.Mechanics.Items;
using Armageddon.Mechanics.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Backend.Payloads
{
    [Exchange(AddConvertExtension = true)]
    [Serializable]
    public class ItemPayload
    {
        public string InstanceId;
        public int SheetId;
        public int? Level;
        public ItemQuality? Quality;
        public ItemStatPayload DamagePerSecond = new();
        public ItemStatPayload FireRate = new();
        public ItemStatPayload DexterityMultiplier = new();
        public ItemStatPayload VitalityMultiplier = new();
        public ItemStatPayload PerceptionMultiplier = new();
        public ItemStatPayload LeadershipMultiplier = new();
        public ItemStatPayload Armor = new();
        public ItemStatPayload CriticalChance = new();
        public ItemStatPayload CriticalDamage = new();
        public StatBonusPayload[] StatBonuses = { };
        public AbilityBonusPayload[] AbilityBonuses = { };
        public int? Quantity;
        public int? BuyPrice;
        public CurrencyPayload ShopSellPrice = new();
    }

    [InlineProperty(LabelWidth = 60)]
    [Exchange(AddConvertExtension = true)]
    [Serializable]
    public class ItemStatPayload
    {
        public double Value;
        
        [HideInInspector]
        public double? MinValue;
        
        [HideInInspector]
        public double? MaxValue;
    }

    [Exchange(AddConvertExtension = true)]
    [Serializable]
    public class StatBonusPayload
    {
        public StatId StatId;
        public StatModifierType StatModifierType = StatModifierType.Flat;
        public float Value;
        
        [HideInInspector]
        public float? MinValue;
        
        [HideInInspector]
        public float? MaxValue;
    }

    [Exchange(AddConvertExtension = true)]
    [Serializable]
    public class AbilityBonusPayload
    {
        public int SheetId;
        public int Level;
        
        [HideInInspector]
        public int? MinLevel;
        
        [HideInInspector]
        public int? MaxLevel;
    }
}
