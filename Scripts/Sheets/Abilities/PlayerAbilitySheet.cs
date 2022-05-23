using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Armageddon.Sheets.Abilities
{
    [Serializable]
    public class PlayerAbilityUpgradeDetailsRow
    {
        [DisplayAsString]
        [TableColumnWidth(60, false)]
        [SerializeField]
        private int m_level;

        [InlineProperty]
        [SerializeField]
        private Price m_price;

        public int Level => m_level;

        public Price Price => m_price;


#if UNITY_EDITOR
        public void _SetLevel(int level)
        {
            m_level = level;
        }

        public void _SetPrice(Price price)
        {
            m_price = price;
        }
#endif
    }

    public class PlayerAbilitySheet : AbilitySheet
    {
        [TableList]
        [SerializeField]
        private List<PlayerAbilityUpgradeDetailsRow> m_upgradeDetailsRows;

        public List<PlayerAbilityUpgradeDetailsRow> UpgradeDetailsRows => m_upgradeDetailsRows;

#if UNITY_EDITOR
        public void _SetUpgradeDetailsRow(List<PlayerAbilityUpgradeDetailsRow> rows)
        {
            m_upgradeDetailsRows = rows;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
