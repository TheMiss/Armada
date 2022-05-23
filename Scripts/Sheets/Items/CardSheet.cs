using System.Collections.Generic;
using Armageddon.Sheets.Effects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Sheets.Items
{
    public class CardSheet : ItemSheet
    {
        [PreviewField]
        [SerializeField]
        private Sprite m_enemyIcon;

        [HideReferenceObjectPicker]
        [ListDrawerSettings(Expanded = true)]
        [ValueDropdown(nameof(DropdownItems), AppendNextDrawer = true)]
        [SerializeReference]
        private List<Effect> m_effects;

        private static List<ValueDropdownItem<Effect>> DropdownItems => EffectDetailsRow.DropdownItems;

        public List<Effect> Effects => m_effects;

        public Sprite EnemyIcon => m_enemyIcon;
    }
}
