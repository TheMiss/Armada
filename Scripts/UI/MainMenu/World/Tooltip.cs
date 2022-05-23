using Armageddon.UI.Base;
using TMPro;
using UnityEngine;

namespace Armageddon.UI.MainMenu.World
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

        public GameObject ArrowObject => m_arrowObject;

        public void SetArrowPositionX(float x)
        {
            m_arrowObject.transform.position = new Vector3(x, m_arrowObject.transform.position.y, 0);
        }
    }
}
