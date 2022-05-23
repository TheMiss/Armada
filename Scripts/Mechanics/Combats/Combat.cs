using Armageddon.Sheets.Effects;
using Purity.Common;
using UnityEngine;

namespace Armageddon.Mechanics.Combats
{
    public static class Combat
    {
        public static AttackHit PerformAttackOnTarget(Attack attack, CombatEntity target)
        {
            ApplyEffects(attack, target);

            long variance = (long)(attack.Damage * 0.1f);
            long halfVariance = variance / 2;
            long minDamage = attack.Damage - halfVariance;
            long maxDamage = attack.Damage + halfVariance;

            minDamage = Clamp(minDamage, 1, minDamage);

            long mainDamage = Range(minDamage, maxDamage + 1);

            const float minCriticalChance = 0.05f;
            const float maxCriticalChance = 0.9f;
            float criticalChance = (float)Clamp(attack.CriticalChance - target.CriticalResistance,
                minCriticalChance, maxCriticalChance);
            bool procCriticalHit = Random.value <= criticalChance;

            double criticalMultiplier = procCriticalHit ? attack.CriticalDamage : 1.0f;
            double masterDamage = attack.MasteryDamage;
            double tierMultiplier = attack.GetTierMultiplier(target);
            double sizeMultiplier = attack.GetSizeMultiplier(target);
            double raceMultiplier = attack.GetRaceMultiplier(target);
            double elementMultiplier = attack.GetElementMultiplier(target);

            double totalMultiplier = tierMultiplier * sizeMultiplier * raceMultiplier * elementMultiplier;

            long damage = (long)((mainDamage * criticalMultiplier + masterDamage) * totalMultiplier);

            double armor = target.Armor;

            const float factor = 0.5f;

            long damageDealt = (long)
                (factor * (damage * damage) / (armor + factor * damage));

            damageDealt = Clamp(damageDealt, 1, damageDealt);

            target.CurrentHealth -= damageDealt;

            var attackHit = new AttackHit
            {
                Type = procCriticalHit ? AttackHitType.CriticalHit : AttackHitType.NormalHit,
                DamageDealt = damageDealt
            };

            return attackHit;
        }

        private static void ApplyEffects(Attack attack, CombatEntity target)
        {
            foreach (AttackModifier modifier in attack.Modifiers)
            {
                if (modifier.Effect is AddStatusEffectOnAttackEffect effect)
                {
                    bool procEffect = Random.value <= effect.Chance;

                    if (!procEffect)
                    {
                        continue;
                    }

                    if (effect.AffectOn == AffectOnType.Target)
                    {
                        float resistance = Clamp((float)target.StatusResistance, 0, 1f);
                        float effectiveness = 1 - resistance;
                        float duration = effect.Duration;

                        target.AddOrRefreshStatusEffect(effect.StatusEffectSheet, modifier.Level,
                            duration, effectiveness);
                    }
                    else
                    {
                        Debug.Log("No implementation");
                    }
                }
            }
        }

        public static double Clamp(double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static long Clamp(long value, long min, long max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static double Range(double minInclusive, double maxInclusive)
        {
            return CustomRandom.Range(minInclusive, maxInclusive);
        }

        public static long Range(long minInclusive, long maxExclusive)
        {
            return CustomRandom.Range(minInclusive, maxExclusive);
        }
    }
}
