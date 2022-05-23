using Armageddon.Backend.Attributes;

namespace Armageddon.Mechanics.Stats
{
    [Exchange(AssignEnumValue = true)]
    public enum StatId
    {
        // Primary Stats (Basic Stats a.k.a. Attributes)
        Dexterity = 0,
        Vitality = 1,
        Perception = 2,
        Leadership = 3,

        // Secondary Stats (Derived Stats)
        Health = 100,
        HealthRegeneration = 101,
        Armor = 102,
        AttackPower = 103,
        UltimatePower = 104,
        CriticalChance = 105,
        CriticalDamage = 106,
        CriticalResistance = 107,
        StatusResistance = 108,

        // Extra stats
        MagicFind = 200,
        // Lifesteal = 201,

        // Mastery stats
        BlasterDamageBonus = 300,
        MissileDamageBonus = 301,
        BeamerDamageBonus = 302,
        MeleeDamageBonus = 303,

        // Damage Addition on Tier
        DamageOnMinionIncrease = 400,
        DamageOnNobleIncrease = 401,
        DamageOnCelestialBeingIncrease = 402,

        // Damage Addition on Size
        DamageOnSmallIncrease = 500,
        DamageOnMediumIncrease = 501,
        DamageOnLargeIncrease = 502,
        DamageOnGiganticIncrease = 503,

        // Damage Addition on Race
        DamageOnAngelIncrease = 600,
        DamageOnDemonIncrease = 601,
        DamageOnDragonIncrease = 602,
        DamageOnLeviathanIncrease = 603,
        DamageOnDeviantIncrease = 604,

        // Damage Addition on Element
        DamageOnFireIncrease = 700,
        DamageOnIceIncrease = 701,
        DamageOnLightningIncrease = 702,
        DamageOnToxinIncrease = 703,
        DamageOnDarkIncrease = 704,
        DamageOnLightIncrease = 705,
        DamageOnSpecterIncrease = 706,
        DamageOnUndeadIncrease = 707,

        // Damage Reduction from Tier
        DamageFromMinionReduction = 800,
        DamageFromNobleReduction = 801,
        DamageFromCelestialBeingReduction = 802,

        // Damage Reduction from Size
        DamageFromSmallReduction = 900,
        DamageFromMediumReduction = 901,
        DamageFromLargeReduction = 902,
        DamageFromGiganticReduction = 903,

        // Damage Reduction from Race
        DamageFromAngelReduction = 1000,
        DamageFromDemonReduction = 1001,
        DamageFromDragonReduction = 1002,
        DamageFromLeviathanReduction = 1003,
        DamageFromDeviantReduction = 1004,

        // Damage Reduction from Element
        DamageFromFireReduction = 1100,
        DamageFromIceReduction = 1101,
        DamageFromLightningReduction = 1102,
        DamageFromToxinReduction = 1103,
        DamageFromDarkReduction = 1104,
        DamageFromLightReduction = 1105,
        DamageFromSpecterReduction = 1106,
        DamageFromUndeadReduction = 1107,

        // Primarily used for bonus stats
        AllAttributes = 1200,
        WeaponDamage = 1200
    }
}
