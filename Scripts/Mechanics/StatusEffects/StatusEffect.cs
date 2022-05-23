using System.Collections.Generic;
using Armageddon.Mechanics.Combats;
using Armageddon.Sheets.Effects;
using Armageddon.Sheets.StatusEffects;
using Cysharp.Threading.Tasks;
using Purity.Common;
using UnityEngine;

namespace Armageddon.Mechanics.StatusEffects
{
    public class StatusEffect
    {
        private bool m_runningEffects;

        public StatusEffect(CombatEntity target, StatusEffectSheet sheet, int level, float duration,
            float effectiveness)
        {
            Target = target;
            Sheet = sheet;
            Level = level;
            StackCount = 1;
            Duration = duration;
            HasDuration = duration > 0;
            Effectiveness = effectiveness;
        }

        public CombatEntity Target { get; }
        public StatusEffectSheet Sheet { get; }

        [ShowAsString]
        public string Name => Sheet.Name;

        [ShowAsString]
        public int Level { get; }

        // [ShowAsString]
        public bool HasDuration { get; }

        public float Effectiveness { get; set; }

        public float RemainingTime { get; private set; }

        [ShowAsString]
        public float Duration { get; }

        public int StackCount { get; set; }

        public void RefreshDuration()
        {
            RemainingTime = Duration * Effectiveness;
            Debug.Log($"{Target} is affected(refreshed) by {Sheet} for {RemainingTime} seconds");
        }

        private void OnStart()
        {
            Debug.Log($"{Target} is affected by {Sheet} for {RemainingTime} seconds");
        }

        private void OnEnd()
        {
            Debug.Log($"{Target} is no longer affected by {Sheet}");
        }

        public async UniTaskVoid ApplyEffectsAsync()
        {
            RemainingTime = Duration * Effectiveness;

            OnStart();
            m_runningEffects = true;

            List<Effect> effects = Sheet.GetEffects(Level);

            foreach (Effect effect in effects)
            {
                if (effect is DamageOverTimeEffect damageOverTimeEffect)
                {
                    ApplyDamageOverTimeEffectAsync(damageOverTimeEffect).Forget();
                }
            }

            while (RemainingTime > 0)
            {
                RemainingTime -= Time.deltaTime;
                await UniTask.Yield();
            }


            OnEnd();
            m_runningEffects = false;

            Target.RemoveStatusEffect(this);
        }

        private async UniTaskVoid ApplyDamageOverTimeEffectAsync(DamageOverTimeEffect effect)
        {
            int interval = (int)(effect.TriggerInterval * 1000);
            double totalDamage = effect.GetTotalDamage(Duration, Target.CurrentHealth, (long)Target.Health);
            double damagerPerTrigger = totalDamage / (Duration / effect.TriggerInterval);

            double damageDealt = 0;

            // float applyingTime = 0;
            // while (m_runningEffects)
            // {
            //     applyingTime -= Time.deltaTime;
            //     
            //     if (applyingTime <= 0)
            //     {
            //         damageDealt += damagerPerTrigger;
            //         Target.CurrentHealth -= (long)damagerPerTrigger;
            //         Debug.Log($"{Target} took damage {damagerPerTrigger}");
            //         
            //         applyingTime = effect.TriggerInterval;
            //     }
            //
            //     // Warning, we should have a CancellationToken to stop the task when m_runningEffects is false
            //     await UniTask.Yield();
            // }

            while (m_runningEffects)
            {
                damageDealt += damagerPerTrigger;
                Target.CurrentHealth -= (long)damagerPerTrigger;
                Debug.Log($"{Target} took damage {damagerPerTrigger}");

                // Otherwise, we would have to use the code above with a CancellationToken. Let's go with this for now.
                if (damageDealt >= totalDamage * Effectiveness)
                {
                    break;
                }

                await UniTask.Delay(interval);
            }

            Debug.Log($"Damage dealt = {damageDealt}");

            // We discard the remaining damage as the trigger interval is not reached yet.
            // float effectiveTotalDamage = effect.TotalDamage * Effectiveness;
            //
            // if (damageDealt < effectiveTotalDamage)
            // {
            //     damageDealt += effectiveTotalDamage - damageDealt;
            // }
        }
    }
}
