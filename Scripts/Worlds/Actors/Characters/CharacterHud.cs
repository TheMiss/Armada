using System.Collections.Generic;
using Armageddon.Mechanics.StatusEffects;
using Armageddon.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.Worlds.Actors.Characters
{
    public class CharacterHud : Widget
    {
        private const float DisplayBubbleTextDuration = 1.5f;

        [SerializeField]
        private StatusEffectIcon m_statusEffectIconPrefab;

        [SerializeField]
        private Image m_healthBarFill;

        [SerializeField]
        private GameObject m_statusEffectBarObject;

        private readonly List<StatusEffectIcon> m_statusEffectIcons = new();

        public void SetHealth(long currentHealth, long health)
        {
            m_healthBarFill.fillAmount = (float)(currentHealth / (double)health);
        }

        public void AddStatusEffect(StatusEffect statusEffect)
        {
            // Need pooling
            StatusEffectIcon statusEffectIcon =
                Instantiate(m_statusEffectIconPrefab, m_statusEffectBarObject.transform);
            statusEffectIcon.Icon.sprite = statusEffect.Sheet.Icon;
            statusEffectIcon.StatusEffect = statusEffect;

            m_statusEffectIcons.Add(statusEffectIcon);
        }

        public void RemoveStatusEffect(StatusEffect statusEffect)
        {
            StatusEffectIcon statusEffectIcon = m_statusEffectIcons.Find(x => x.StatusEffect == statusEffect);
            DestroyGameObject(statusEffectIcon);

            m_statusEffectIcons.Remove(statusEffectIcon);
        }

        // public void SetHealth(float percent)
        // {
        //     m_healthBarFill.fillAmount = percent;
        // }
    }
}
