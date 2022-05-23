using System;
using Armageddon.Sheets.StatusEffects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Sheets.Effects
{
    public enum AffectOnType
    {
        Self,
        Target
    }

    [Serializable]
    public class AddStatusEffectOnAttackEffect : Effect
    {
        [HideIf(nameof(ShowDescriptionOnly))]
        [HorizontalGroup]
        [LabelWidth(90)]
        [Required]
        [LabelText("$" + nameof(StatusEffectName))]
        [SerializeField]
        private StatusEffectSheet m_statusEffectSheet;

        [HideIf(nameof(ShowDescriptionOnly))]
        [HorizontalGroup(100)]
        [LabelWidth(20)]
        [LabelText("On")]
        [SerializeField]
        private AffectOnType m_affectOn = AffectOnType.Target;

        [HideIf(nameof(ShowDescriptionOnly))]
        [HorizontalGroup(80)]
        [LabelWidth(50)]
        [Range(0, 1)]
        [SerializeField]
        private float m_chance = 1;

        [HideIf(nameof(ShowDescriptionOnly))]
        [HorizontalGroup(120)]
        [LabelWidth(25)]
        [MinValue(0)]
        [LabelText("For")]
        [SuffixLabel("s")]
        [SerializeField]
        private float m_duration;

        public AddStatusEffectOnAttackEffect() : base(EffectType.AddStatusEffectWhenAttack)
        {
        }

        private string StatusEffectName => m_statusEffectSheet == null ? "none" : m_statusEffectSheet.Name;

        public override string Description
        {
            get
            {
                string odds = $"{m_chance * 100.0f:F}%";
                string duration = m_duration > 0 ? $"for {m_duration} seconds" : "permanently";
                return $"Adds {odds} chance to add {StatusEffectName} on {m_affectOn} when attack {duration}";
            }
        }

        public StatusEffectSheet StatusEffectSheet => m_statusEffectSheet;

        public AffectOnType AffectOn => m_affectOn;

        public float Chance => m_chance;

        public float Duration => m_duration;
    }
}
