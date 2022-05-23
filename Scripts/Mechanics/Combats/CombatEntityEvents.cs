using System;
using Armageddon.Mechanics.StatusEffects;

namespace Armageddon.Mechanics.Combats
{
    public enum CombatEntityStatusEffectsChangeType
    {
        Add,
        Remove
    }

    public class CombatEntityStatusEffectsChangedArgs : EventArgs
    {
        public CombatEntityStatusEffectsChangedArgs(CombatEntityStatusEffectsChangeType changeType,
            StatusEffect addedStatusEffect,
            StatusEffect removedStatusEffect)
        {
            ChangeType = changeType;
            AddedStatusEffect = addedStatusEffect;
            RemovedStatusEffect = removedStatusEffect;
        }

        public CombatEntityStatusEffectsChangeType ChangeType { get; }
        public StatusEffect AddedStatusEffect { get; }

        public StatusEffect RemovedStatusEffect { get; }
        // public IReadOnlyList<StatusEffect> AddedStatusEffects { get; }
        // public IReadOnlyList<StatusEffect> RemovedStatusEffects { get; }
    }
}
