using Armageddon.Mechanics.StatusEffects;
using Armageddon.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.Worlds.Actors.Characters
{
    public class StatusEffectIcon : Widget
    {
        [SerializeField]
        private Image m_icon;

        [SerializeField]
        private Image m_background;

        public Image Icon => m_icon;

        public Image Background => m_background;

        public StatusEffect StatusEffect { get; set; }
    }
}
