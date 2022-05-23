using Armageddon.Mechanics.Characters;
using Armageddon.UI.Base;
using Purity.Common;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.Loadout.HeroSelection
{
    public class HeroElement : Widget
    {
        [SerializeField]
        private Toggle m_toggle;

        [SerializeField]
        private GameObject m_hardSelectObject;

        [SerializeField]
        private Image m_icon;

        [SerializeField]
        private TextMeshProUGUI m_titleText;

        public Toggle Toggle => m_toggle;

        [ShowInPlayMode]
        public Hero Hero { set; get; }

        public void SetIcon(Sprite icon)
        {
            m_icon.sprite = icon;
        }

        public void SetTitle(string title)
        {
            m_titleText.Set(title);
        }

        public void Select()
        {
            m_hardSelectObject.SetActive(true);
        }

        public void Deselect()
        {
            m_hardSelectObject.SetActive(false);
        }
    }
}
