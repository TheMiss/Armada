using Armageddon.Sheets.Items;
using Armageddon.UI.Base;
using Purity.Common;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.Upgrades.Powers
{
    public enum PowerButtonState
    {
        Disable,
        Active,
        Undiscovered
    }

    public class PowerElement : SelectableElement<PowerElement>
    {
        [SerializeField]
        private Image m_icon;

        [SerializeField]
        private TextMeshProUGUI m_nameText;

        private bool m_isPowerActive;

        [ShowInPlayMode]
        public CardSheet CardSheet { get; set; }

        public override object Object => CardSheet;

        public bool IsPowerActive
        {
            get => m_isPowerActive;
            set
            {
                m_isPowerActive = value;
                UpdateDetails();
            }
        }

        public void UpdateDetails()
        {
            m_icon.sprite = CardSheet.EnemyIcon;

            Color color = Color.white;
            color.a = IsPowerActive ? 1.0f : 100 / 255f;

            m_nameText.color = color;
            m_nameText.Set(CardSheet.Name);

            m_icon.color = color;
        }
    }
}
