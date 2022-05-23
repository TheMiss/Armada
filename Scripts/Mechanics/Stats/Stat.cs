using System;
using System.Collections.Generic;
using Purity.Common;
using Sirenix.OdinInspector;

namespace Armageddon.Mechanics.Stats
{
    [Serializable]
    public class Stat
    {
        [TableColumnWidth(150)]
        [ShowAsString]
        public readonly StatId Id;

        private double m_baseValue;

        private double m_value;

        [TableColumnWidth(300)]
        [PropertyOrder(100)]
        [ShowInInspector]
        private List<StatModifier> m_modifiers = new();

        public Stat(StatId id, double baseValue)
        {
            Id = id;
            BaseValue = baseValue;
        }

        // [ShowAsString]
        [ShowDelayedProperty]
        public double BaseValue
        {
            get => m_baseValue;
            set
            {
                m_baseValue = value;
                IsDirty = true;
            }
        }

        public bool IsDirty { private set; get; } = true;

        [ShowAsString]
        public virtual double Value
        {
            get
            {
                if (IsDirty)
                {
                    // Can add ForceCalculateFinalValue() if needed.
                    m_value = CalculateFinalValue();
                    IsDirty = false;
                }

                return m_value;
            }
        }

        public IReadOnlyList<StatModifier> Modifiers => m_modifiers;

        public long ValueLong => (long)Value;

        public virtual void AddModifier(StatModifier modifier)
        {
            StatModifier existingModifier = m_modifiers.Find(x => x.Source == modifier.Source);
            //
            // if (existingModifier != null)
            // {
            //     Debug.LogWarning($"Trying to add modifier from the same source ({existingModifier.Source})");
            //     return;
            // }

            modifier.ValueChanged += OnModifierValueChanged;
            m_modifiers.Add(modifier);

            IsDirty = true;
        }

        private void OnModifierValueChanged(StatModifier modifier)
        {
            IsDirty = true;
        }

        public virtual bool RemoveModifier(StatModifier modifier)
        {
            if (m_modifiers.Remove(modifier))
            {
                modifier.ValueChanged -= OnModifierValueChanged;
                IsDirty = true;
                return true;
            }

            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(IStatSource source)
        {
            int removalCount = m_modifiers.RemoveAll(mod => mod.Source == source);

            if (removalCount > 0)
            {
                IsDirty = true;
                return true;
            }

            return false;
        }

        private static int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order)
            {
                return -1;
            }

            if (a.Order > b.Order)
            {
                return 1;
            }

            return 0; //if (a.Order == b.Order)
        }

        protected virtual double CalculateFinalValue()
        {
            double finalValue = BaseValue;
            double sumPercentAdd = 0;

            m_modifiers.Sort(CompareModifierOrder);

            for (int i = 0; i < m_modifiers.Count; i++)
            {
                StatModifier modifier = m_modifiers[i];

                switch (modifier.Type)
                {
                    case StatModifierType.Flat:
                    {
                        finalValue += modifier.Value;
                        break;
                    }
                    case StatModifierType.Percentage:
                    {
                        sumPercentAdd += modifier.Value;

                        if (i + 1 >= m_modifiers.Count || m_modifiers[i + 1].Type != StatModifierType.Percentage)
                        {
                            finalValue *= 1 + sumPercentAdd;
                            sumPercentAdd = 0;
                        }

                        break;
                    }
                    // case StatModifierType.PercentMultiply:
                    // {
                    //     finalValue *= 1 + modifier.Value;
                    //     break;
                    // }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return finalValue;
            // Workaround for float calculation errors, like displaying 12.00001 instead of 12
            // return Mathf.Round(finalValue);
        }

        public static implicit operator double(Stat stat)
        {
            return stat.Value;
        }

        // public static implicit operator int(Stat stat)
        // {
        //     return stat.ValueAsInt;
        // }
    }
}
