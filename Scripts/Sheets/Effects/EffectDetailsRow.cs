using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Sheets.Effects
{
    [Serializable]
    public class EffectDetailsRow
    {
        [DisplayAsString]
        [TableColumnWidth(60, false)]
        [SerializeField]
        private int m_level;

        [HideReferenceObjectPicker]
        [ValidateInput(nameof(ValidateEffects))]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown(nameof(DropdownItems), AppendNextDrawer = true)]
        [HorizontalGroup("Effect")]
        [SerializeReference]
        [SerializeField]
        private List<Effect> m_effects;

        public EffectDetailsRow(int level, List<Effect> effects)
        {
            m_level = level;
            m_effects = effects;
        }

        public static List<ValueDropdownItem<Effect>> DropdownItems =>
            new()
            {
                new ValueDropdownItem<Effect>
                {
                    Text = $"{EffectType.AddStatModifier}", Value = new AddStatModifierEffect()
                },
                new ValueDropdownItem<Effect>
                {
                    Text = $"{EffectType.AddStatusEffectWhenAttack}", Value = new AddStatusEffectOnAttackEffect()
                },
                new ValueDropdownItem<Effect>
                {
                    Text = $"{EffectType.DamageOverTime}", Value = new DamageOverTimeEffect()
                }
            };

        public int Level => m_level;

        public List<Effect> Effects => m_effects;

        private bool ValidateEffects(List<Effect> effects, ref string errorMessage)
        {
            var checkedItems = new List<Effect>();

            foreach (Effect effect in effects)
            {
                foreach (Effect checkedItem in checkedItems)
                {
                    if (Effect.HasTheSameContent(checkedItem, effect))
                    {
                        errorMessage = "There are duplicated effects!";
                        return false;
                    }
                }

                checkedItems.Add(effect);
            }

            return true;
        }

        public void _SetLevel(int level)
        {
            m_level = level;
        }
    }
}
