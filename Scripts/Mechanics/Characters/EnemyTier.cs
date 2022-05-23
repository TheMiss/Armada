using Armageddon.Backend.Attributes;

namespace Armageddon.Mechanics.Characters
{
    [Exchange]
    public enum EnemyTier
    {
        /// <summary>
        ///     Regular, Superior, and Champion
        /// </summary>
        Minion,

        /// <summary>
        ///     Elite and Ruler (a.k.a boss tier)
        /// </summary>
        Noble,

        /// <summary>
        ///     Demigod and Deity (a.k.a super boss tier)
        /// </summary>
        Celestial
    }
}
