using System;
using Armageddon.Mechanics.Stats;
using Purity.Common.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Sheets.Effects
{
    [Serializable]
    public class AddStatModifierEffect : Effect
    {
        [HideIf(nameof(ShowDescriptionOnly))]
        [HorizontalGroup]
        [LabelWidth(60)]
        [SerializeField]
        private StatId m_statId;

        [HideIf(nameof(ShowDescriptionOnly))]
        [HorizontalGroup(150)]
        [LabelWidth(50)]
        [SerializeField]
        private double m_value = 1;

        [HideIf(nameof(ShowDescriptionOnly))]
        [HorizontalGroup]
        [LabelWidth(120)]
        [SerializeField]
        private StatModifierType m_modifierType = StatModifierType.Flat;

        public AddStatModifierEffect() : base(EffectType.AddStatModifier)
        {
        }

        public AddStatModifierEffect(StatId statId, double value, StatModifierType statModifierType)
            : base(EffectType.AddStatModifier)
        {
            m_statId = statId;
            m_value = value;
            m_modifierType = statModifierType;
        }

        public override string Description
        {
            get
            {
                string sign = m_value > 0 ? "+" : string.Empty;
                string percent = m_modifierType == StatModifierType.Percentage ? "%" : string.Empty;

                return $"{sign}{m_value}{percent} {m_statId.ToHumanReadable()}";
            }
        }

        public StatId StatId => m_statId;

        public double Value => m_value;

        public StatModifierType ModifierType => m_modifierType;
    }
}
