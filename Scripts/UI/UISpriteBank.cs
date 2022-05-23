using System;
using Armageddon.Backend.Payloads;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.UI
{
    /// <summary>
    ///     Should contain only frequently uses like Energy, Red Gem, and so on.
    /// </summary>
    public class UISpriteBank : ScriptableObject
    {
        [Required]
        [AssetsOnly]
        [PreviewField]
        [SerializeField]
        private Sprite m_goldShard;

        [Required]
        [AssetsOnly]
        [PreviewField]
        [SerializeField]
        private Sprite m_redGem;

        public Sprite GoldShard => m_goldShard;

        public Sprite RedGem => m_redGem;

        public Sprite GetCurrencyIcon(CurrencyType currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.GoldShard:
                    return GoldShard;
                case CurrencyType.RedGem:
                    return RedGem;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currencyType), currencyType, null);
            }
        }
    }
}
