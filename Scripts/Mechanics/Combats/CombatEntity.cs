using System;
using System.Collections.Generic;
using System.Linq;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Abilities;
using Armageddon.Mechanics.Bonuses;
using Armageddon.Mechanics.Characters;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.Mechanics.Stats;
using Armageddon.Mechanics.StatusEffects;
using Armageddon.Sheets.Effects;
using Armageddon.Sheets.StatusEffects;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Mechanics.Combats
{
    [Serializable]
    public class CombatEntity
    {
        public const float DexterityPerLevel = 3.5f;
        public const float VitalityPerLevel = 2.2f;
        public const float PerceptionPerLevel = 2.5f;
        public const float LeadershipPerLevel = 1.8f;

        public const float AttackPowerPerDexterity = 2;
        public const float UltimatePowerPerDexterity = 1 / 5f;
        public const float CriticalDamagePerDexterity = 0.1f / 200f;

        public const float HealthPerVitality = 20;
        public const float ArmorPerVitality = 1 / 8f;
        public const float StatusResistancePerVitality = 1 / 10f;

        public const float PrimaryDamagePerDexterity = 1.2f;
        public const float SecondaryDamagePerDexterity = 1 / 10f;
        public const float CriticalChancePerPerception = 0.1f / 5f;

        public const float CompanionDamagePerLeadership = 1.2f;
        public const float StatusResistancePerLeadership = 1 / 15f;
        public const float CriticalResistancePerLeadership = 1 / 10f;

        private int m_level;
        private long m_currentHealth;
        private List<StatusEffect> m_statusEffects = new();

        public CombatEntity(CombatEntityType type, string name, bool createAdvancedStats)
        {
            List<Stat> CreateStatsFromRange(StatId start, StatId end, float baseValue = 0)
            {
                List<Stat> stats = EnumEx.GetRangeValues(start, end).Select(statType => new Stat(statType, baseValue))
                    .ToList();
                AllStats.AddRange(stats);

                return stats;
            }

            Type = type;
            Name = name;
            Attributes = CreateStatsFromRange(StatId.Dexterity, StatId.Leadership);
            DerivedStats = CreateStatsFromRange(StatId.Health, StatId.StatusResistance);

            switch (type)
            {
                case CombatEntityType.Hero:
                    Weapons = ListEx.NewWithValues<Weapon>(2);
                    WeaponStatsEntries = ListEx.NewWithValues<WeaponStatsEntry>(2);
                    break;
                case CombatEntityType.Enemy:
                    break;
                case CombatEntityType.Companion:
                    Weapons = ListEx.NewWithValues<Weapon>(1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            if (createAdvancedStats)
            {
                ExtraStats = CreateStatsFromRange(StatId.MagicFind, StatId.MagicFind);
                MasteryStats = CreateStatsFromRange(StatId.BlasterDamageBonus, StatId.MeleeDamageBonus);

                // Damage Increase
                DamageOnTierIncreaseStats = CreateStatsFromRange(
                    StatId.DamageOnMinionIncrease,
                    StatId.DamageOnCelestialBeingIncrease);

                DamageOnSizeStats = CreateStatsFromRange(
                    StatId.DamageOnSmallIncrease,
                    StatId.DamageOnGiganticIncrease);

                DamageOnRaceIncreaseStats = CreateStatsFromRange(
                    StatId.DamageOnAngelIncrease,
                    StatId.DamageOnDeviantIncrease);

                DamageOnElementIncreaseStats = CreateStatsFromRange(
                    StatId.DamageOnFireIncrease,
                    StatId.DamageOnUndeadIncrease);

                // Damage Reduction
                DamageFromTierReductionStats = CreateStatsFromRange(
                    StatId.DamageFromMinionReduction,
                    StatId.DamageFromCelestialBeingReduction);

                DamageFromSizeStats = CreateStatsFromRange(
                    StatId.DamageFromSmallReduction,
                    StatId.DamageFromGiganticReduction);

                DamageFromRaceReductionStats = CreateStatsFromRange(
                    StatId.DamageFromAngelReduction,
                    StatId.DamageFromDeviantReduction);

                DamageFromElementReductionStats = CreateStatsFromRange(
                    StatId.DamageFromFireReduction,
                    StatId.DamageFromUndeadReduction);
            }
        }

        public string Name { get; }

        [ShowAsString]
        public int Level
        {
            get => m_level;
            set
            {
                m_level = value;
                OnLevelChanged();
            }
        }

        [ShowAsString]
        public long CurrentHealth
        {
            get => m_currentHealth;
            set
            {
                m_currentHealth = value;
                if (m_currentHealth < 0)
                {
                    m_currentHealth = 0;
                }
                else if (m_currentHealth > Health)
                {
                    m_currentHealth = (long)Health;
                }
            }
        }

        [ShowReadOnlyTableList]
        public List<WeaponStatsEntry> WeaponStatsEntries { get; set; } = new();

        [ShowReadOnlyTableList]
        public IReadOnlyList<Stat> Attributes { get; private set; }

        [ShowReadOnlyTableList]
        public IReadOnlyList<Stat> DerivedStats { get; private set; }

        [ShowReadOnlyTableList]
        public IReadOnlyList<Stat> ExtraStats { get; private set; }

        [ShowReadOnlyTableList]
        public IReadOnlyList<Stat> MasteryStats { get; private set; }

        [ShowReadOnlyTableList]
        public IReadOnlyList<Stat> DamageOnTierIncreaseStats { get; private set; }

        [ShowReadOnlyTableList]
        public IReadOnlyList<Stat> DamageOnSizeStats { get; private set; }

        [ShowReadOnlyTableList]
        public IReadOnlyList<Stat> DamageOnRaceIncreaseStats { get; private set; }

        [ShowReadOnlyTableList]
        public IReadOnlyList<Stat> DamageOnElementIncreaseStats { get; private set; }

        [ShowReadOnlyTableList]
        public IReadOnlyList<Stat> DamageFromTierReductionStats { get; private set; }

        [ShowReadOnlyTableList]
        public IReadOnlyList<Stat> DamageFromSizeStats { get; private set; }

        [ShowReadOnlyTableList]
        public IReadOnlyList<Stat> DamageFromRaceReductionStats { get; private set; }

        [ShowReadOnlyTableList]
        public IReadOnlyList<Stat> DamageFromElementReductionStats { get; private set; }

        public List<Stat> AllStats { get; private set; } = new();

        [ShowReadOnlyTableList]
        public List<Bonus> Bonuses { get; private set; } = new();

        [ShowReadOnlyTableList]
        public List<Ability> Abilities { get; set; } = new();

        [ShowReadOnlyTableList]
        public IReadOnlyList<StatusEffect> StatusEffects => m_statusEffects;

        public CombatEntityInventory Inventory { get; } = new();

        public List<Weapon> Weapons { get; private set; }

        public bool HasCustomAttributes { get; set; }
        public float StartingDexterity { get; set; } = 6;
        public float StartingVitality { get; set; } = 10;
        public float StartingPerception { get; set; } = 5;
        public float StartingLeadership { get; set; } = 4;
        public float StartingHealth { get; set; } = 100;
        public float StartingHealthRegeneration { get; set; }
        public float StartingArmor { get; set; }
        public float StartingAttackPower { get; set; } = 5;
        public float StartingSecondaryDamage { get; set; } = 2;
        public float StartingCompanionDamage { get; set; }
        public float StartingBlasterMastery { get; set; }
        public float StartingMissileMastery { get; set; }
        public float StartingBeamMastery { get; set; }
        public float StartingCriticalDamage { get; set; } = 1.2f;
        public float StartingCriticalChance { get; set; } = 0.05f;
        public float StartingCriticalResistance { get; set; }
        public float StartingStatusResistance { get; set; }
        public float StartingMagicFind { get; set; }

        public CombatEntityType Type { get; }

        // TODO: Upgrade to init property in C# 9.0
        public int Tier { get; set; }
        public CharacterSize Size { get; set; }
        public CharacterRace Race { get; set; }
        public CharacterElement Element { get; set; }

        // =============================================
        // Quick access for Attributes and Derived Stats
        // =============================================
        public Stat Dexterity => this[StatId.Dexterity];
        public Stat Vitality => this[StatId.Vitality];
        public Stat Perception => this[StatId.Perception];
        public Stat Leadership => this[StatId.Leadership];
        public Stat Health => this[StatId.Health];
        public Stat HealthRegeneration => this[StatId.HealthRegeneration];
        public Stat Armor => this[StatId.Armor];

        public Stat AttackPower => this[StatId.AttackPower];

        public Stat CriticalChance => this[StatId.CriticalChance];
        public Stat CriticalDamage => this[StatId.CriticalDamage];
        public Stat CriticalResistance => this[StatId.CriticalResistance];
        public Stat StatusResistance => this[StatId.StatusResistance];
        public Stat MagicFind => this[StatId.MagicFind];

        private Stat this[StatId statId]
        {
            get
            {
                int type = (int)statId;

                if (type >= (int)StatId.Dexterity && type < (int)StatId.Health)
                {
                    return Attributes[type];
                }

                if (type >= (int)StatId.Health && type < (int)StatId.MagicFind)
                {
                    return DerivedStats[type - 100];
                }

                if (type >= (int)StatId.MagicFind && type < (int)StatId.BlasterDamageBonus)
                {
                    return ExtraStats[type - 200];
                }

                if (type >= (int)StatId.BlasterDamageBonus && type < (int)StatId.DamageOnMinionIncrease)
                {
                    return MasteryStats[type - 300];
                }

                if (type >= (int)StatId.DamageOnMinionIncrease && type < (int)StatId.DamageOnSmallIncrease)
                {
                    return DamageOnTierIncreaseStats[type - 400];
                }

                if (type >= (int)StatId.DamageOnSmallIncrease && type < (int)StatId.DamageOnAngelIncrease)
                {
                    return DamageOnSizeStats[type - 500];
                }

                if (type >= (int)StatId.DamageOnAngelIncrease && type < (int)StatId.DamageOnFireIncrease)
                {
                    return DamageOnRaceIncreaseStats[type - 600];
                }

                if (type >= (int)StatId.DamageOnFireIncrease && type < (int)StatId.DamageFromMinionReduction)
                {
                    return DamageOnElementIncreaseStats[type - 700];
                }

                if (type >= (int)StatId.DamageFromMinionReduction && type < (int)StatId.DamageFromSmallReduction)
                {
                    return DamageFromTierReductionStats[type - 800];
                }

                if (type >= (int)StatId.DamageFromSmallReduction && type < (int)StatId.DamageFromAngelReduction)
                {
                    return DamageFromSizeStats[type - 900];
                }

                if (type >= (int)StatId.DamageFromAngelReduction && type < (int)StatId.DamageFromFireReduction)
                {
                    return DamageFromRaceReductionStats[type - 1000];
                }

                if (type >= (int)StatId.DamageFromFireReduction && type < (int)StatId.AllAttributes)
                {
                    return DamageFromElementReductionStats[type - 1100];
                }

                // TODO: Need a fix for all stats and the rest 
                Debug.LogError($"Cannot get {statId}");

                return null;
            }

            #region Old

            // get
            // {
            //     int type = (int)statType;
            //     int index = type;
            //
            //     const int primaryCount = (int)StatType.Leadership + 1 - (int)StatType.Dexterity;
            //     const int secondaryCount = (int)StatType.StatusResistance + 1 - (int)StatType.Health;
            //     const int damageAdditionCount = 0;
            //     // const int damageAdditionCount =
            //     //     (int)StatType.IncreaseDamageOnUndead + 1 - (int)StatType.IncreaseDamageOnMinion;
            //
            //     if (type >= 300)
            //     {
            //         index = type - 300 + primaryCount + secondaryCount + damageAdditionCount;
            //     }
            //     else if (type >= 200)
            //     {
            //         index = type - 200 + primaryCount + secondaryCount;
            //     }
            //     else if (type >= 100)
            //     {
            //         index = type - 100 + primaryCount;
            //     }
            //
            //     return BasicStats[index];
            // }

            #endregion
        }

        public Stat this[StatBonus stat] => this[stat.StatId];

        public event EventHandler<CombatEntityStatusEffectsChangedArgs> StatusEffectsChanged;

        private void OnLevelChanged()
        {
            CalculateBaseAttributes();
        }

        [Button]
        [GUIColorDefaultButton]
        [PropertyOrder(-100)]
        public void CompileStats()
        {
            if (HasCustomAttributes)
            {
                Dexterity.BaseValue = StartingDexterity;
                Vitality.BaseValue = StartingVitality;
                Perception.BaseValue = StartingPerception;
                Leadership.BaseValue = StartingLeadership;
            }
            else
            {
                CalculateBaseAttributes();
            }

            // RemoveStatModifiersFromAbilities();
            // AddStatModifiersFromAbilities();

            CalculateBaseAttackPower();
            CalculateBaseUltimatePower();

            CalculateBaseHealth();
            CalculateBaseHealthRegeneration();
            CalculateBaseArmor();
            CalculateBaseStatusResistance();

            CalculateBaseCriticalChance();
            CalculateBaseCriticalDamage();
            CalculateBaseCriticalResistance();

            CurrentHealth = Health.ValueLong;

            UpdateWeaponStatsEntries();
        }

        private void CalculateBaseAttributes()
        {
            double kernelDexterity = 0;
            double kernelVitality = 0;
            double kernelPerception = 0;
            double kernelLeadership = 0;

            Item item = Inventory[EquipmentSlotType.Kernel];

            if (item is Kernel kernel)
            {
                kernelDexterity += kernel.DexterityMultiplier;
                kernelVitality += kernel.VitalityMultiplier;
                kernelPerception += kernel.PerceptionMultiplier;
                kernelLeadership += kernel.LeadershipMultiplier;
            }

            Dexterity.BaseValue = (int)(Level * DexterityPerLevel + kernelDexterity);
            Vitality.BaseValue = (int)(Level * VitalityPerLevel + kernelVitality);
            Perception.BaseValue = (int)(Level * PerceptionPerLevel + kernelPerception);
            Leadership.BaseValue = (int)(Level * LeadershipPerLevel + kernelLeadership);
        }

        private void CalculateBaseHealth()
        {
            double kernelHealth = 0;

            // Item item = Inventory[EquipmentSlotType.Kernel];
            //
            // if (item is Kernel kernel)
            // {
            //     kernelHealth += kernel.BaseHealth;
            // }

            Health.BaseValue = StartingHealth + Vitality.Value * HealthPerVitality + kernelHealth;
        }

        private void CalculateBaseHealthRegeneration()
        {
            HealthRegeneration.BaseValue = StartingHealthRegeneration + HealthRegeneration;
        }

        private void CalculateBaseArmor()
        {
            double armorBaseArmor = 0;

            // Item item = Inventory[EquipmentSlotType.Armor];
            //
            // if (item is Armor armor)
            // {
            //     armorBaseArmor += armor.ArmorBonus;
            // }

            Armor.BaseValue = StartingArmor + Vitality.Value * ArmorPerVitality + armorBaseArmor;
        }

        private void CalculateBaseAttackPower()
        {
            AttackPower.BaseValue = StartingAttackPower +
                Dexterity.Value * AttackPowerPerDexterity +
                Perception.Value * PrimaryDamagePerDexterity;

            // TODO: Add AttackPower from item
            // ....
            // ...

            // float primaryWeaponDamage = 0;
            //
            // Item item = Inventory[EquipmentSlotType.PrimaryWeapon];
            //
            // if (item is Weapon weapon)
            // {
            //     primaryWeaponDamage = weapon.DamagePerShot;
            // }
            //
            // AttackPower.BaseValue = StartingAttackPower +
            //     Dexterity.Value * AttackPowerPerDexterity +
            //     Perception.Value * PrimaryDamagePerDexterity +
            //     primaryWeaponDamage;
        }

        private void CalculateBaseUltimatePower()
        {
            // SecondaryDamage.BaseValue = StartingSecondaryDamage +
            //     Dexterity.Value * UltimatePowerPerDexterity +
            //     Perception.Value * SecondaryDamagePerDexterity;
        }

        private void CalculateBaseCriticalDamage()
        {
            CriticalDamage.BaseValue = StartingCriticalDamage +
                Dexterity.Value * CriticalDamagePerDexterity;
        }

        private void CalculateBaseCriticalChance()
        {
            CriticalChance.BaseValue = StartingCriticalChance +
                Perception.Value * CriticalChancePerPerception;
        }

        private void CalculateBaseStatusResistance()
        {
            StatusResistance.BaseValue = StartingStatusResistance +
                Vitality.Value * StatusResistancePerVitality +
                Leadership.Value * StatusResistancePerLeadership;
        }

        private void CalculateBaseCriticalResistance()
        {
            CriticalResistance.BaseValue = StartingCriticalResistance +
                Leadership.Value * CriticalResistancePerLeadership;
        }

        private void UpdateWeaponStatsEntries()
        {
            foreach (WeaponStatsEntry entry in WeaponStatsEntries)
            {
                entry?.UpdateStats();
            }
        }

        public void EquipItem(EquipmentSlotType slotType, Item item)
        {
            if (!item.Type.CanEquipToSlotType(slotType))
            {
                Debug.LogWarning(
                    $"You're trying to equip {item.InstanceId}({item.Type}) to {slotType} slotType");
                return;
            }

            UnequipItem(slotType);

            AddBonusesFromItem(item);

            Inventory[slotType] = item;

            if (Type == CombatEntityType.Hero)
            {
                if (item is Weapon weapon)
                {
                    AttachWeapon(weapon, (int)weapon.InstallationType);
                }
            }
        }

        public void UnequipItem(EquipmentSlotType slotType)
        {
            Item item = Inventory[slotType];

            if (item == null)
            {
                return;
            }

            RemoveAllBonusesFromSource(item);

            Inventory[slotType] = null;

            if (Type == CombatEntityType.Hero)
            {
                if (item is Weapon weapon)
                {
                    DetachWeapon((int)weapon.InstallationType);
                }
            }
        }

        public void AddBonusesFromItem(Item item)
        {
            if (item is Armor armor)
            {
                var armorModifier = new StatModifier(armor.ArmorBonus, StatModifierType.Flat, armor);
                this[StatId.Armor].AddModifier(armorModifier);
            }

            List<Bonus> bonuses = item.Bonuses;

            foreach (Bonus bonus in bonuses)
            {
                if (bonus is StatBonus statBonus)
                {
                    var modifier = new StatModifier(statBonus.Value, statBonus.ModifierType, item);
                    this[statBonus].AddModifier(modifier);
                }
                else if (bonus is AbilityBonus abilityBonus)
                {
                    AddAbility(abilityBonus);
                }
            }

            Bonuses.AddRange(bonuses);
        }

        public void RemoveAllBonusesFromSource(IStatSource source)
        {
            foreach (Stat stat in AllStats)
            {
                stat.RemoveAllModifiersFromSource(source);
            }

            if (source is Item item)
            {
                foreach (Bonus bonus in item.Bonuses)
                {
                    if (bonus is AbilityBonus abilityBonus)
                    {
                        RemoveAbility(abilityBonus);
                    }
                }
            }

            for (int i = Bonuses.Count - 1; i >= 0; i--)
            {
                Bonus bonus = Bonuses[i];
                if (bonus is StatBonus statBonus)
                {
                    if (statBonus.Source == source)
                    {
                        Bonuses.RemoveAt(i);
                    }
                }
                else if (bonus is AbilityBonus abilityBonus)
                {
                    if (abilityBonus.Source == source)
                    {
                        Bonuses.RemoveAt(i);
                    }
                }
            }
        }

        private void AddAbility(AbilityBonus abilityBonus)
        {
            Ability ability = Abilities.Find(x => x.Sheet == abilityBonus.Sheet);

            if (ability == null)
            {
                ability = new Ability(abilityBonus.Source, abilityBonus.Sheet, abilityBonus.Level);
                Abilities.Add(ability);
            }
            else
            {
                ability.Level += abilityBonus.Level;
            }

            // Refresh the stats from increased ability level
            RemoveStatModifiersFromAbility(ability);
            AddStatModifiersFromAbility(ability);
        }

        private void RemoveAbility(AbilityBonus abilityBonus)
        {
            for (int i = Abilities.Count - 1; i >= 0; i--)
            {
                Ability ability = Abilities[i];

                if (ability.Sheet == abilityBonus.Sheet)
                {
                    ability.Level -= abilityBonus.Level;

                    if (ability.Level <= 0)
                    {
                        Abilities.RemoveAt(i);
                    }

                    // Refresh the stats from reduced ability level
                    RemoveStatModifiersFromAbility(ability);
                    AddStatModifiersFromAbility(ability);
                }
            }
        }

        private void AddStatModifiersFromAbilities()
        {
            foreach (Ability ability in Abilities)
            {
                RemoveStatModifiersFromAbility(ability);
                AddStatModifiersFromAbility(ability);
            }
        }

        private void AddStatModifiersFromAbility(Ability ability)
        {
            // Active can also give stat modifiers.
            // if (ability.Type != AbilityType.Passive)
            // {
            //     Debug.Log($"{ability} is not passive.");
            //     return;
            // }

            foreach (Effect effect in ability.Effects)
            {
                if (effect is AddStatModifierEffect addStatModifierEffect)
                {
                    var modifier = new StatModifier(addStatModifierEffect.Value,
                        addStatModifierEffect.ModifierType, ability);

                    this[addStatModifierEffect.StatId].AddModifier(modifier);
                }
            }
        }

        private void RemoveStatModifiersFromAbilities()
        {
            foreach (Ability ability in Abilities)
            {
                RemoveStatModifiersFromAbility(ability);
            }
        }

        private void RemoveStatModifiersFromAbility(Ability ability)
        {
            foreach (Stat stat in AllStats)
            {
                stat.RemoveAllModifiersFromSource(ability);
            }
        }

        public void AttachWeapon(Weapon weapon, int slotIndex)
        {
            Weapons[slotIndex] = weapon;

            var weaponStatsEntry = new WeaponStatsEntry(this, weapon);
            WeaponStatsEntries[slotIndex] = weaponStatsEntry;
        }

        public void DetachWeapon(int slotIndex)
        {
            Weapons[slotIndex] = null;
            WeaponStatsEntries[slotIndex] = null;
        }

        public double GetWeaponMasteryDamage(WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Blaster => this[StatId.BlasterDamageBonus],
                WeaponType.Missile => this[StatId.MissileDamageBonus],
                WeaponType.Beamer => this[StatId.BeamerDamageBonus],
                WeaponType.Melee => this[StatId.MeleeDamageBonus],
                _ => throw new ArgumentOutOfRangeException(nameof(weaponType), weaponType, null)
            };
        }

        public void FillTierDamageMultipliers(double[] tierMultipliers)
        {
            for (int i = 0; i < DamageOnTierIncreaseStats.Count; i++)
            {
                tierMultipliers[i] = 1 + DamageOnTierIncreaseStats[i];
            }
        }

        public void FillSizeDamageMultipliers(double[] sizeMultipliers)
        {
            for (int i = 0; i < DamageOnSizeStats.Count; i++)
            {
                sizeMultipliers[i] = 1 + DamageOnSizeStats[i];
            }
        }

        public void FillRaceDamageMultipliers(double[] raceMultipliers)
        {
            for (int i = 0; i < DamageOnRaceIncreaseStats.Count; i++)
            {
                raceMultipliers[i] = 1 + DamageOnRaceIncreaseStats[i];
            }
        }

        public void FillElementDamageMultipliers(double[] elementMultipliers)
        {
            for (int i = 0; i < DamageOnElementIncreaseStats.Count; i++)
            {
                elementMultipliers[i] = 1 + DamageOnElementIncreaseStats[i];
            }
        }

        public void AddOrRefreshStatusEffect(StatusEffectSheet statusEffectSheet, int level, float duration,
            float effectiveness)
        {
            StatusEffect statusEffect = m_statusEffects.Find(x => x.Sheet == statusEffectSheet);

            bool addNewStatusEffect = false;

            if (statusEffect == null)
            {
                addNewStatusEffect = true;
            }
            else
            {
                if (statusEffectSheet.MaxStack == 1)
                {
                    // Refresh its duration if MaxStack is 1
                    statusEffect.RefreshDuration();
                }
                else
                {
                    addNewStatusEffect = true;
                }
            }

            if (addNewStatusEffect)
            {
                statusEffect = new StatusEffect(this, statusEffectSheet, level, duration, effectiveness);
                statusEffect.ApplyEffectsAsync().Forget();

                AddStatusEffect(statusEffect);
            }
        }

        public void AddStatusEffect(StatusEffect statusEffect)
        {
            m_statusEffects.Add(statusEffect);

            var e = new CombatEntityStatusEffectsChangedArgs(CombatEntityStatusEffectsChangeType.Add,
                statusEffect, null);
            StatusEffectsChanged?.Invoke(this, e);
        }

        public void RemoveStatusEffect(StatusEffect statusEffect)
        {
            m_statusEffects.Remove(statusEffect);

            var e = new CombatEntityStatusEffectsChangedArgs(CombatEntityStatusEffectsChangeType.Remove,
                null, statusEffect);
            StatusEffectsChanged?.Invoke(this, e);
        }

        public void Tick()
        {
            if (HealthRegeneration > 0)
            {
                CurrentHealth += (long)(HealthRegeneration * Time.deltaTime);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///     Intentionally used for show stats in Inspector
        /// </summary>
        public class WeaponStatsEntry
        {
            private readonly CombatEntity m_owner;
            private readonly Weapon m_weapon;

            public WeaponStatsEntry(CombatEntity owner, Weapon weapon)
            {
                m_owner = owner;
                m_weapon = weapon;
            }

            [ShowAsString]
            public string DPS { get; set; }

            [ShowAsString]
            public string Damage { get; set; }

            [ShowAsString]
            public double FireRate { get; set; }

            [ShowAsString]
            public string TotalDamage { get; set; }

            [ShowAsString]
            public string TotalDPS { get; set; }

            public void UpdateStats()
            {
                double shotsPerFire = m_weapon.ShotsPerFire;
                double fireRate = m_weapon.FireRate;

                {
                    double damagePerSecond = m_weapon.DamagePerSecond;
                    DPS = $"{damagePerSecond}";

                    double damagePerShot = m_weapon.DamagePerShot;
                    Damage = $"{damagePerShot:F}x{shotsPerFire}";

                    FireRate = fireRate;
                }
                {
                    double attackPowerPerShot = m_owner.AttackPower / fireRate;
                    double calculatedDamagePerShot = attackPowerPerShot + m_weapon.DamagePerShot;
                    TotalDamage = $"{calculatedDamagePerShot:F}x{shotsPerFire}";

                    double totalDamagePerSecond = calculatedDamagePerShot * shotsPerFire * fireRate;
                    TotalDPS = $"{totalDamagePerSecond:F}";
                }
            }
        }
    }
}
