using System.Collections.Generic;
using Armageddon.Externals.OdinInspector;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Armageddon.Sheets.Effects
{
    public abstract class EffectsHolderSheet : Sheet
    {
        [PropertyOrder(20)]
        [OnCollectionChanged(nameof(OnCollectionChanged))]
        [TableList]
        [SerializeField]
        private List<EffectDetailsRow> m_detailsRows;

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

#if UNITY_EDITOR

        public void _SetDetailsRow(List<EffectDetailsRow> detailsRows)
        {
            m_detailsRows = detailsRows;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
