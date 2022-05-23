using System;
using System.Collections.Generic;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.StatusEffects;
using Armageddon.Sheets.Effects;
using I2.Loc;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Sheets.StatusEffects
{
    public abstract class StatusEffectSheet : Sheet
    {
        [ReadOnly]
        [SerializeField]
        private StatusEffectType m_type;

        [MinValue(1)]
        [SerializeField]
        private int m_maxStack;

        [PropertyOrder(20)]
        [OnCollectionChanged(nameof(OnCollectionChanged))]
        [TableList]
        [SerializeField]
        private List<EffectDetailsRow> m_detailsRows;

        [ShowAsStringOverflow]
        public string Description => LocalizationManager.GetTranslation($"StatusEffect/Description/StatusEffect{Id}");

        public StatusEffectType Type => m_type;

        public int MaxStack => m_maxStack;

        public List<EffectDetailsRow> DetailsRows => m_detailsRows;

        [ButtonGroup]
        [GUIColorDefaultButton]
        [PropertyOrder(10)]
        private void ToggleDescriptionOnly()
        {
            Effect.ShowDescriptionOnly = !Effect.ShowDescriptionOnly;
        }

        private void OnCollectionChanged()
        {
            int level = 1;
            foreach (EffectDetailsRow row in m_detailsRows)
            {
                row._SetLevel(level++);
            }
        }

        public List<Effect> GetEffects(int level)
        {
            try
            {
                EffectDetailsRow row = m_detailsRows[level - 1];
                return row.Effects;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return new List<Effect>();
            }
        }

#if UNITY_EDITOR
        public void _SetType(StatusEffectType type)
        {
            m_type = type;
        }

        public void _SetEffectDetailsRows(List<EffectDetailsRow> detailsRows)
        {
            m_detailsRows = detailsRows;
        }
#endif
    }
}
