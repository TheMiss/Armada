using System;
using System.Collections.Generic;
using Armageddon.Mechanics.Abilities;
using Armageddon.Mechanics.Characters;
using Armageddon.Mechanics.Items;
using Armageddon.Sheets.Effects;
using Purity.Common;
using Purity.Common.Extensions;

namespace Armageddon.Mechanics.Combats
{
    public class Attack
    {
        public CombatEntity Attacker { get; private set; }
        public long Damage { get; private set; }
        public double CriticalChance { get; private set; }
        public double CriticalDamage { get; private set; }
        public double MasteryDamage { get; private set; }
        private double[] TierMultipliers { get; } = ArrayEx.NewWithValues<double>(EnumEx.GetSize<EnemyTier>());
        private double[] SizeMultipliers { get; } = ArrayEx.NewWithValues<double>(EnumEx.GetSize<CharacterSize>());
        private double[] RaceMultipliers { get; } = ArrayEx.NewWithValues<double>(EnumEx.GetSize<CharacterRace>());

        private double[] ElementMultipliers { get; } =
            ArrayEx.NewWithValues<double>(EnumEx.GetSize<CharacterElement>());

        public List<AttackModifier> Modifiers { get; set; } = new();

        public void SetValues(Weapon weapon, CombatEntity attacker)
        {
            double attackPowerPerShot = attacker.AttackPower / weapon.FireRate;

            Attacker = attacker;
            Damage = (long)(weapon.DamagePerShot + attackPowerPerShot);
            CriticalChance = attacker.CriticalChance;
            CriticalDamage = attacker.CriticalDamage;
            MasteryDamage = attacker.GetWeaponMasteryDamage(weapon.Sheet.WeaponType);

            switch (attacker.Type)
            {
                case CombatEntityType.Hero:
                case CombatEntityType.Companion:
                    attacker.FillTierDamageMultipliers(TierMultipliers);
                    attacker.FillSizeDamageMultipliers(SizeMultipliers);
                    attacker.FillRaceDamageMultipliers(RaceMultipliers);
                    attacker.FillElementDamageMultipliers(ElementMultipliers);
                    break;
                case CombatEntityType.Enemy:
                    TierMultipliers.SetValues(1);
                    SizeMultipliers.SetValues(1);
                    RaceMultipliers.SetValues(1);
                    ElementMultipliers.SetValues(1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Modifiers.Clear();

            foreach (Ability ability in attacker.Abilities)
            {
                foreach (Effect effect in ability.Effects)
                {
                    if (effect.Type == EffectType.AddStatusEffectWhenAttack)
                    {
                        var modifier = new AttackModifier(effect, ability.Level);
                        Modifiers.Add(modifier);
                    }
                }
            }
        }

        public double GetTierMultiplier(CombatEntity target)
        {
            switch (target.Type)
            {
                case CombatEntityType.Hero:
                case CombatEntityType.Companion:
                    return 1;
                case CombatEntityType.Enemy:
                    return TierMultipliers[target.Tier];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public double GetSizeMultiplier(CombatEntity target)
        {
            switch (target.Type)
            {
                case CombatEntityType.Hero:
                case CombatEntityType.Companion:
                    return 1;
                case CombatEntityType.Enemy:
                    return SizeMultipliers[(int)target.Size];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public double GetRaceMultiplier(CombatEntity target)
        {
            switch (target.Type)
            {
                case CombatEntityType.Hero:
                case CombatEntityType.Companion:
                    return 1;
                case CombatEntityType.Enemy:
                    return RaceMultipliers[(int)target.Race];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public double GetElementMultiplier(CombatEntity target)
        {
            switch (target.Type)
            {
                case CombatEntityType.Hero:
                case CombatEntityType.Companion:
                    return 1;
                case CombatEntityType.Enemy:
                    return ElementMultipliers[(int)target.Element];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
