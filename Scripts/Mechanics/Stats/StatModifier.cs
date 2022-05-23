using System;
using Sirenix.OdinInspector;

namespace Armageddon.Mechanics.Stats
{
    [Serializable]
    public class StatModifier
    {
        public readonly int Order;
        public readonly IStatSource Source;
        public readonly StatModifierType Type;

        private double m_value;

        private StatModifier(double value, StatModifierType type, int order, IStatSource source)
        {
            Value = value;
            Type = type;
            Order = order;
            Source = source;
        }

        public StatModifier(double value, StatModifierType type, IStatSource source) :
            this(value, type, (int)type, source)
        {
        }

        public StatModifier(StatModifierType type, IStatSource source) :
            this(0, type, source)
        {
        }

        //public StatModifier(float value, StatModifierType type) : this(value, type, (int)type, null)
        //{
        //}

        //public StatModifier(float value, StatModifierType type, int order) : this(value, type, order, null)
        //{
        //}

        public Action<StatModifier> ValueChanged { get; set; }

        [ShowInInspector]
        public double Value
        {
            get => m_value;
            set
            {
                m_value = value;
                ValueChanged?.Invoke(this);
                // m_owner?.OnModifierValueChanged(this);
            }
        }
    }
}
