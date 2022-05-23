using Armageddon.Backend.Attributes;

namespace Armageddon.Mechanics.Characters
{
    [Exchange]
    public enum EnemyRank
    {
        /// <summary>
        ///     Just normal...
        /// </summary>
        Regular,

        /// <summary>
        ///     Enhanced with Attack Modifier
        /// </summary>
        Superior,

        /// <summary>
        ///     Enhanced with Attack Modifier and increased stats
        /// </summary>
        Champion,

        /// <summary>
        ///     Bosses
        /// </summary>
        Elite,

        /// <summary>
        ///     Enhanced bosses.
        /// </summary>
        Ruler,

        /// <summary>
        ///     Even more special
        /// </summary>
        Demigod,

        /// <summary>
        ///     Godlike
        /// </summary>
        Deity
    }
}
