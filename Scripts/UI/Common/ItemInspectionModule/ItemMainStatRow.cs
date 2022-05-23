using Armageddon.UI.Base;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Armageddon.UI.Common.ItemInspectionModule
{
    public class ItemMainStatRow : Widget
    {
        [SerializeField]
        private TextMeshProUGUI m_statNameText;

        [SerializeField]
        private TextMeshProUGUI m_statValueText;

        public void SetStatValue(string valueText)
        {
            m_statValueText.Set(valueText);
        }
    }
}
