using System;
using Armageddon.Mechanics.Stats;
using Purity.Common;

namespace Armageddon.Mechanics.Bonuses
{
    [Serializable]
    public class StatBonus : Bonus
    {
        public StatBonus(object source, StatId statId, StatModifierType modifierType,
            float value, float minValue, float maxValue) : base(BonusType.Stat)
        {
            Source = source;
            StatId = statId;
            ModifierType = modifierType;
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;

            // StatModifier = new StatModifier(value, statModifierType, source);
        }

        public object Source { get; }

        [ShowAsString]
        public StatId StatId { get; }

        [ShowAsString]
        public StatModifierType ModifierType { get; }

        [ShowAsString]
        public double Value { get; }

        public float MinValue { get; }

        public float MaxValue { get; }

        // public StatModifier StatModifier { get; }

        public int ValueAsLong => (int)Value;
        public int MinValueAsLong => (int)MinValue;
        public int MaxValueAsLong => (int)MaxValue;
    }
}
