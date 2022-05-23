using Armageddon.UI.Base;
using TMPro;
using UnityEngine;

namespace Armageddon.UI.MainMenu.Upgrades
{
    public abstract class Tooltip : Widget
    {
        [SerializeField]
        private TextMeshProUGUI m_titleText;

        [SerializeField]
        private TextMeshProUGUI m_detailsText;

        [SerializeField]
        private GameObject m_arrowObject;

        protected TextMeshProUGUI TitleText => m_titleText;

        protected TextMeshProUGUI DetailsText => m_detailsText;

        public void SetArrowAtLeftSide()
        {
            m_arrowObject.transform.localPosition = new Vector3(-300, 0, 0);
            m_arrowObject.transform.localScale = new Vector3(-1, 1, 1);
        }

        public void SetArrowAtRightSide()
        {
            m_arrowObject.transform.localPosition = new Vector3(300, 0, 0);
            m_arrowObject.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
