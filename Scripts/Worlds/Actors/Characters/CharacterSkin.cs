using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Characters
{
    public enum DyeType
    {
        Color,
        Material
    }

    /// <summary>
    ///     Sharable with ItemSheet and Enemy Dye
    /// </summary>
    [CreateAssetMenu(fileName = "Dye", menuName = "Dyes/Dye", order = 0)]
    public class CharacterSkin : IdentifiableObject
    {
        [SerializeField]
        private DyeType m_type;

        [ShowIf(nameof(m_showColor))]
        [SerializeField]
        private Color m_color;

        [ShowIf(nameof(m_showMaterial))]
        [SerializeField]
        private Material m_material;

        private bool m_showColor;

        private bool m_showMaterial;

        public DyeType Type => m_type;

        public Color Color => m_color;

        public Material Material => m_material;
    }
}
