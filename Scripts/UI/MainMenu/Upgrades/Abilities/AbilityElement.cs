using Armageddon.Mechanics.Abilities;
using Armageddon.UI.Base;
using Purity.Common;
using Purity.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.Upgrades.Abilities
{
    public class AbilityElement : SelectableElement<AbilityElement>
    {
        [SerializeField]
        private Image m_icon;

        [SerializeField]
        private TextMeshProUGUI m_levelText;

        [SerializeField]
        private TextMeshProUGUI m_nameText;

        [ShowInPlayMode]
        public PlayerAbility PlayerAbility { get; set; }

        public override object Object => PlayerAbility;

        public void UpdateDetails()
        {
            m_icon.sprite = PlayerAbility.Sheet.Icon;

            int level = PlayerAbility.Level;
            Color color = Color.white;

            if (PlayerAbility.ExtraLevel > 0)
            {
                level += PlayerAbility.ExtraLevel;
                color = new Color(20 / 255f, 255 / 255f, 0);
            }

            m_levelText.Set($"{level}/{PlayerAbility.MaxUpgradeableLevel}");
            m_levelText.color = color;

            m_nameText.Set(PlayerAbility.Name);
        }
    }
}
