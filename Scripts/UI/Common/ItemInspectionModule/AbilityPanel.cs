using Armageddon.UI.Base;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Armageddon.UI.Common.ItemInspectionModule
{
    public class AbilityPanel : Widget
    {
        [SerializeField]
        private TextMeshProUGUI m_titleText;

        [SerializeField]
        private TextMeshProUGUI m_detailsText;

        public void SetTitleText(string titleText)
        {
            m_titleText.Set(titleText);
        }

        public void SetDetailsText(string valueText)
        {
            m_detailsText.Set(valueText);
        }
    }
}
