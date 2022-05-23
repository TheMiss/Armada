using Armageddon.Mechanics.Items;
using Armageddon.Worlds.Actors.Characters;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace Armageddon.Sheets.Items
{
    // [CreateAssetMenu(fileName = "ItemSheet", menuName = "Items/ItemSheet", order = 0)]
    public abstract class ItemSheet : Sheet
    {
        [DisplayAsString]
        [PropertyOrder(-10)]
        [SerializeField]
        private ItemType m_type;

        [ShowIf(nameof(m_type), ItemType.Skin)]
        [PropertyOrder(-10)]
        [SerializeField]
        private CharacterSkin m_characterSkin;

        [ShowIf(nameof(ShowItemQuality))]
        [GUIColor(nameof(QualityColor))]
        [SerializeField]
        private ItemQuality m_quality;

        public ItemType Type => m_type;

        [PropertyOrder(-10)]
        [ShowInInspector]
        public bool IsEquipable => Type.IsEquipable();

        [PropertyOrder(-10)]
        [ShowInInspector]
        public bool IsUsable => Type.IsUsable();

        [PropertyOrder(-10)]
        [ShowInInspector]
        public bool IsStackable => Type.IsStackable();

        public CharacterSkin CharacterSkin => m_characterSkin;

        private bool ShowItemQuality => (int)m_type > (int)ItemType.Consumable;

        public ItemQuality Quality => m_quality;

        private Color QualityColor => m_quality.ToColor();

#if UNITY_EDITOR

        public void _SetType(ItemType type)
        {
            m_type = type;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
