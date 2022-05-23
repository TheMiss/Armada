using System.Collections.Generic;
using System.Linq;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Armageddon.Worlds.Actors.Characters
{
    /// <summary>
    ///     To honor Unity2D animation SpriteResolver, we use the word 'Resolver' as well
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SpriteResolver))]
    public class PartResolver : FastMonoBehaviour
    {
        [ReadOnly]
        [SerializeField]
        private SpriteRenderer m_spriteRenderer;

        [ReadOnly]
        [SerializeField]
        private SpriteResolver m_spriteResolver;

        public SpriteRenderer SpriteRenderer => m_spriteRenderer;

        public SpriteResolver SpriteResolver => m_spriteResolver;

        private void OnEnable()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (m_spriteRenderer == null)
            {
                m_spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (m_spriteResolver == null)
            {
                m_spriteResolver = GetComponent<SpriteResolver>();
            }
        }

        public void SetDye(CharacterSkin characterSkin)
        {
            SpriteRenderer.color = characterSkin.Color;
        }

        /// <summary>
        ///     Set variant by Label
        /// </summary>
        /// <param name="spriteLibrary"></param>
        /// <param name="variantIndex"></param>
        public void SetVariant(SpriteLibrary spriteLibrary, int variantIndex)
        {
            SetVariant(spriteLibrary.spriteLibraryAsset, variantIndex);
        }

        /// <summary>
        ///     Usually, labels will be 'Variant1', 'Variant2', ...
        /// </summary>
        /// <param name="spriteLibraryAsset"></param>
        /// <param name="variantIndex"></param>
        private void SetVariant(SpriteLibraryAsset spriteLibraryAsset, int variantIndex)
        {
            string categoryName = $"{SpriteResolver.name}";

            // If this causes performance issue, we can cache it.
            List<string> labels = spriteLibraryAsset.GetCategoryLabelNames(categoryName).ToList();

            SpriteResolver.SetCategoryAndLabel(categoryName, labels[variantIndex]);
        }

        /// <summary>
        ///     Use Category as variant, see the SpriteLibrary
        /// </summary>
        /// <param name="spriteLibrary"></param>
        /// <param name="variantName"></param>
        public void SetVariant_Old(SpriteLibrary spriteLibrary, string variantName)
        {
            SetVariant_Old(spriteLibrary.spriteLibraryAsset, variantName);
        }

        private void SetVariant_Old(SpriteLibraryAsset spriteLibraryAsset, string variantName)
        {
            string categoryName = $"{SpriteResolver.name} {variantName}";

            List<string> labels = spriteLibraryAsset.GetCategoryLabelNames(categoryName).ToList();

            // Back in the day, I designed the game that we night have multiple frames for animation...
            // But right now just use one frame, hence labels[0]...
            SpriteResolver.SetCategoryAndLabel(categoryName, labels[0]);
        }
    }
}
