using System;
using Purity.Common;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Unused
{
    public class DamagerHitArgs : EventArgs
    {
        public DamagerHitArgs(Damager hitter, Vector2 hitPosition)
        {
            Hitter = hitter;
            HitPosition = hitPosition;
        }

        public Damager Hitter { get; }
        public Vector2 HitPosition { get; }
    }

    public class DamageableDiedArgs : EventArgs
    {
        public DamageableDiedArgs(Damager killer)
        {
            Killer = killer;
        }

        public Damager Killer { get; }
    }

    public class Damageable : Context
    {
        [SerializeField]
        private int m_startingHealth;

        private float m_invulnerableDuration;
        private float m_invulnerableTimer;

        public int StartingHealth => m_startingHealth;
        public int CurrentHealth { private set; get; }

        public bool IsInvulnerable { private set; get; }

        /// <summary>
        ///     Perfectly works well with ObjectPool
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            CurrentHealth = m_startingHealth;

            CanTick = true;
        }

        public event EventHandler<DamageableDiedArgs> Died;

        public event EventHandler BecomeVulnerable;

        public event EventHandler<DamagerHitArgs> DamagerHit;

        public static event EventHandler<DamageableDiedArgs> DamageableDied;

        public void SetStartHealth(int startingHealth, bool setCurrentHealth = true)
        {
            m_startingHealth = startingHealth;

            if (setCurrentHealth)
            {
                CurrentHealth = startingHealth;
            }
        }

        public void SetInvulnerable(float duration)
        {
            IsInvulnerable = true;
            m_invulnerableDuration = duration;
            m_invulnerableTimer = 0.0f;
        }

        /// <summary>
        ///     We directly raise this
        /// </summary>
        /// <param name="damager"></param>
        /// <param name="hitPosition"></param>
        public void OnDamagerHit(Damager damager, Vector2 hitPosition)
        {
            if (CurrentHealth <= 0 || IsInvulnerable)
            {
                return;
            }

            CurrentHealth -= damager.Damage;

            if (CurrentHealth > StartingHealth)
            {
                CurrentHealth = StartingHealth;
            }

            //TakeDamage?.Invoke(damager, hitPosition);
            DamagerHit?.Invoke(this, new DamagerHitArgs(damager, hitPosition));

            // HealthChanged event...

            if (CurrentHealth <= 0)
            {
                Died?.Invoke(this, new DamageableDiedArgs(damager));
                DamageableDied?.Invoke(this, new DamageableDiedArgs(damager));
            }
        }

        public override void Tick()
        {
            if (!IsInvulnerable)
            {
                return;
            }

            m_invulnerableTimer += Time.deltaTime;

            if (m_invulnerableTimer > m_invulnerableDuration)
            {
                BecomeVulnerable?.Invoke(this, EventArgs.Empty);
                IsInvulnerable = false;
                m_invulnerableTimer -= m_invulnerableDuration; // Unnecessary
            }
        }
    }
}
