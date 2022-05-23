using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Sheets.Effects
{
    public enum DamageOverTimeType
    {
        DamageFlat,
        DamagePercentOfCurrentHealth,
        DamagePercentOfMaxHealth
    }

    [Serializable]
    public class DamageOverTimeEffect : Effect
    {
        [HideIf(nameof(ShowDescriptionOnly))]
        [HorizontalGroup(280)]
        [LabelWidth(130)]
        [SerializeField]
        private float m_totalDamage = 0.1f;

        [HideIf(nameof(ShowDescriptionOnly))]
        [HorizontalGroup(280)]
        [LabelWidth(40)]
        [LabelText("Type")]
        [SerializeField]
        private DamageOverTimeType m_damageOverTimeType;

        [HideIf(nameof(ShowDescriptionOnly))]
        [HorizontalGroup]
        [LabelWidth(80)]
        [MinValue(0.1)]
        [SuffixLabel("s")]
        [SerializeField]
        private float m_triggerInterval = 0.5f;

        public DamageOverTimeEffect() : base(EffectType.DamageOverTime)
        {
        }

        public override string Description
        {
            get
            {
                string damageText = DamageOverTimeType switch
                {
                    DamageOverTimeType.DamageFlat => $"{m_totalDamage}",
                    DamageOverTimeType.DamagePercentOfCurrentHealth => $"{m_totalDamage * 100.0f}% of current health",
                    DamageOverTimeType.DamagePercentOfMaxHealth => $"{m_totalDamage * 100.0f}% of max health",
                    _ => throw new ArgumentOutOfRangeException()
                };

                return $"Deals {damageText} over its duration and triggers every {m_triggerInterval} seconds.";
            }
        }

        public DamageOverTimeType DamageOverTimeType => m_damageOverTimeType;

        public float TriggerInterval => m_triggerInterval;

        // public float TotalDamage => m_totalDamage;

        public float GetTotalDamage(float duration, long currentHealth, long health)
        {
            return DamageOverTimeType switch
            {
                DamageOverTimeType.DamageFlat => m_totalDamage * duration,
                DamageOverTimeType.DamagePercentOfCurrentHealth => m_totalDamage * currentHealth,
                DamageOverTimeType.DamagePercentOfMaxHealth => m_totalDamage * health,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
