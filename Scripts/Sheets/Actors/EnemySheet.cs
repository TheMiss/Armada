using Armageddon.Mechanics.Characters;
using Armageddon.Worlds.Actors.Enemies;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using Purity.Common.Editor;
#endif

namespace Armageddon.Sheets.Actors
{
    public class EnemySheet : Sheet
    {
        [Required]
        [InlineButton(nameof(OpenPrefab))]
        [SerializeField]
        private EnemyActor m_prefab;

        [SerializeField]
        private EnemyTier m_tier;

        [SerializeField]
        private CharacterSize m_size = CharacterSize.Medium;

        [SerializeField]
        private CharacterRace m_race = CharacterRace.Demon;

        [SerializeField]
        private CharacterElement m_element;

        [HorizontalGroup("HHealth")]
        [SerializeField]
        private float m_baseHealth;

        [HorizontalGroup("HHealth")]
        [SerializeField]
        private float m_growthHealth;

        [HorizontalGroup("HArmor")]
        [SerializeField]
        private float m_baseArmor;

        [HorizontalGroup("HArmor")]
        [SerializeField]
        private float m_growthArmor;

        [HorizontalGroup("HExp")]
        [SerializeField]
        private float m_baseExp;

        [HorizontalGroup("HExp")]
        [SerializeField]
        private float m_growthExp;

        [HorizontalGroup("HDexterity")]
        [SerializeField]
        private float m_baseDexterity;

        [HorizontalGroup("HDexterity")]
        [SerializeField]
        private float m_growthDexterity;

        [HorizontalGroup("HVitality")]
        [SerializeField]
        private float m_baseVitality;

        [HorizontalGroup("HVitality")]
        [SerializeField]
        private float m_growthVitality;

        [HorizontalGroup("HPerception")]
        [SerializeField]
        private float m_basePerception;

        [HorizontalGroup("HPerception")]
        [SerializeField]
        private float m_growthPerception;

        [HorizontalGroup("HLeadership")]
        [SerializeField]
        private float m_baseLeadership;

        [HorizontalGroup("HLeadership")]
        [SerializeField]
        private float m_growthLeadership;

        [HorizontalGroup("HGoldShard")]
        [SerializeField]
        private float m_baseGoldShard;

        [HorizontalGroup("HGoldShard")]
        [SerializeField]
        private float m_growthGoldShard;

        public EnemyActor Prefab => m_prefab;

        public EnemyTier Tier => m_tier;

        public CharacterSize Size => m_size;

        public CharacterRace Race => m_race;

        public CharacterElement Element => m_element;

        public float BaseHealth => m_baseHealth;

        public float GrowthHealth => m_growthHealth;

        public float BaseArmor => m_baseArmor;

        public float GrowthArmor => m_growthArmor;

        public float BaseExp => m_baseExp;

        public float GrowthExp => m_growthExp;

        public float BaseDexterity => m_baseDexterity;

        public float GrowthDexterity => m_growthDexterity;

        public float BaseVitality => m_baseVitality;

        public float GrowthVitality => m_growthVitality;

        public float BasePerception => m_basePerception;

        public float GrowthPerception => m_growthPerception;

        public float BaseLeadership => m_baseLeadership;

        public float GrowthLeadership => m_growthLeadership;

        public float BaseGoldShard => m_baseGoldShard;

        public float GrowthGoldShard => m_growthGoldShard;

        public void OpenPrefab()
        {
#if UNITY_EDITOR
            PrefabStageUtilityEx.OpenPrefab(m_prefab.gameObject);
#endif
        }
    }
}
