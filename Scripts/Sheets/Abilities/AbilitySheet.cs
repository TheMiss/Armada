using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Abilities;
using Armageddon.Sheets.Effects;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Armageddon.Sheets.Abilities
{
    public class AbilitySheet : EffectsHolderSheet
    {
        [DisplayAsString]
        [SerializeField]
        private AbilityType m_type;

        public AbilityType Type => m_type;

        [ButtonGroup]
        [GUIColorDefaultButton]
        [PropertyOrder(10)]
        private void ToggleDescriptionOnly()
        {
            Effect.ShowDescriptionOnly = !Effect.ShowDescriptionOnly;
        }

#if UNITY_EDITOR
        public void _SetType(AbilityType type)
        {
            m_type = type;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
