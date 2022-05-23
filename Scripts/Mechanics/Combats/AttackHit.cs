using System;
using Armageddon.Worlds.Misc;

namespace Armageddon.Mechanics.Combats
{
    /// <summary>
    ///     Can add Missed, Blocked and so on.
    /// </summary>
    public enum AttackHitType
    {
        NormalHit,
        CriticalHit
    }

    public static class AttackHitTypeExtensions
    {
        public static DamageTextType ToDamageTextType(this AttackHitType hitType)
        {
            return hitType switch
            {
                AttackHitType.NormalHit => DamageTextType.Normal,
                AttackHitType.CriticalHit => DamageTextType.Critical,
                _ => throw new ArgumentOutOfRangeException(nameof(hitType), hitType, null)
            };
        }
    }

    public struct AttackHit
    {
        public AttackHitType Type;
        public long DamageDealt;
    }
}
