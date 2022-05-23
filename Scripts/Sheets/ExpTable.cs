using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Sheets
{
    [Serializable]
    public class ExpTableDetailsRow
    {
        [DisplayAsString]
        [SerializeField]
        private int m_level;

        [DisplayAsString]
        [SerializeField]
        private long m_totalExp;

        [SerializeField]
        private long m_expForNextLevel;

        [JsonIgnore]
        [ShowAsEnabledString]
        [SerializeField]
        private string m_rise;

        public int Level => m_level;

        public long TotalExp => m_totalExp;

        public long ExpForNextLevel => m_expForNextLevel;

        public string Rise => m_rise;

#if UNITY_EDITOR

        public void _SetLevel(int level)
        {
            m_level = level;
        }

        public void _SetTotalExp(long totalExp)
        {
            m_totalExp = totalExp;
        }

        public void _SetExpForNextLevel(long expForNextLevel)
        {
            m_expForNextLevel = expForNextLevel;
        }

        public void _SetRise(string rise)
        {
            m_rise = rise;
        }
#endif
    }

    public class ExpTable : ScriptableObject
    {
        [TableList(AlwaysExpanded = true, HideToolbar = true, IsReadOnly = true)]
        public List<ExpTableDetailsRow> Rows;

        private Dictionary<int, ExpTableDetailsRow> m_rowDictionary;

        public bool GetRow(int level, out ExpTableDetailsRow row)
        {
            if (m_rowDictionary == null)
            {
                m_rowDictionary = new Dictionary<int, ExpTableDetailsRow>();

                foreach (ExpTableDetailsRow r in Rows)
                {
                    m_rowDictionary.Add(r.Level, r);
                }
            }

            return m_rowDictionary.TryGetValue(level, out row);
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            long totalExp = 0;
            for (int i = 0; i < Rows.Count; i++)
            {
                ExpTableDetailsRow row = Rows[i];

                if (i == 0)
                {
                    row._SetTotalExp(0);
                    row._SetRise("-");
                    continue;
                }

                totalExp += Rows[i - 1].ExpForNextLevel;
                row._SetTotalExp(totalExp);

                if (i < Rows.Count - 1)
                {
                    double rise = (double)row.ExpForNextLevel / Rows[i - 1].ExpForNextLevel;
                    row._SetRise($"{(rise - 1) * 100:F}%");
                }
                else
                {
                    row._SetRise("-");
                }
            }
        }

        /// <summary>
        ///     First time set up only
        /// </summary>
        // [ButtonGroup]
        // [Button(ButtonSizes.Large)]
        // [GUIColorDefaultButton]
        private void SetUpLevel1ToLevel40()
        {
            Rows = new List<ExpTableDetailsRow>();

            long totalExp = 0;
            long expForNextLevel = 550;

            for (int i = 0; i < 40; i++)
            {
                int level = i + 1;

                var row = new ExpTableDetailsRow();
                row._SetLevel(level);
                row._SetTotalExp(totalExp);
                row._SetExpForNextLevel(expForNextLevel);

                Rows.Add(row);

                totalExp += expForNextLevel;
                expForNextLevel = (long)(expForNextLevel * 1.20f);
            }

            Rows[Rows.Count - 1]._SetExpForNextLevel(0);

            OnValidate();
        }
#endif
    }
}
