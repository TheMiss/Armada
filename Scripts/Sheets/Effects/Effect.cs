using System;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Sheets.Effects
{
    [Serializable]
    public abstract class Effect
    {
        public static bool ShowDescriptionOnly = false;

        [HideIf(nameof(ShowDescriptionOnly))]
        [DisplayAsString]
        [HorizontalGroup(200)]
        [HideLabel]
        [SerializeField]
        private EffectType m_type;

        protected Effect(EffectType type)
        {
            m_type = type;
        }

        [HideLabel]
        [ShowAsString]
        public virtual string Description { get; protected set; }

        public EffectType Type => m_type;

        public static bool HasTheSameContent(Effect x, Effect y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (x.Type != y.Type)
            {
                return false;
            }

            if (x is AddStatModifierEffect addStatModifierEffectX &&
                y is AddStatModifierEffect addStatModifierEffectY)
            {
                if (addStatModifierEffectX.StatId == addStatModifierEffectY.StatId &&
                    addStatModifierEffectX.ModifierType == addStatModifierEffectY.ModifierType)
                {
                    return true;
                }
            }
            else
            {
                Debug.Log("Not implemented.");
            }

            return false;
        }
    }
}
