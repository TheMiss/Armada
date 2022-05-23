using Armageddon.Backend.Attributes;

namespace Armageddon.Mechanics.Characters
{
    [Exchange]
    public enum CharacterElement
    {
        /// <summary>
        ///     This element has a flaming attack modifier.
        /// </summary>
        Fire,

        /// <summary>
        ///     This element has a frosty attack modifier.
        /// </summary>
        Ice,

        /// <summary>
        ///     This element has a shock attack modifier.
        /// </summary>
        Lightning,

        /// <summary>
        ///     This element has a corrosive attack modifier.
        /// </summary>
        Toxin,

        /// <summary>
        ///     This element has a darkling attack modifier.
        /// </summary>
        Dark,

        /// <summary>
        /// </summary>
        Light,

        /// <summary>
        /// </summary>
        Specter,

        /// <summary>
        /// </summary>
        Undead
    }
}
