using System.Collections.Generic;
using System.Linq;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Items;
using Purity.Bullet2D.Patterns;
using Purity.Bullet2D.Shots;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Weapons
{
    [DisallowMultipleComponent]
    public class WeaponActor : WorldContext
    {
        [ReadOnly]
        [SerializeField]
        private List<Pattern> m_patterns;

        [ReadOnly]
        [SerializeField]
        private List<ShotShooter> m_shotShooters;

        private bool m_isFiring;

        public bool IsFiring
        {
            set => SetFiring(value);
            get => m_isFiring;
        }

        [ShowInInspector]
        public float FireRate
        {
            get => GetFireRate();
            set => SetFireRate(value);
        }

        [ShowInInspector]
        public int ShotsPerFire => m_shotShooters.Count;

        [ShowInPlayMode]
        public Weapon Weapon { get; set; }

        public List<ShotShooter> ShotShooters => m_shotShooters;

        public List<Pattern> Patterns => m_patterns;

        protected override void Awake()
        {
            base.Awake();

            CollectPatternsAndShotShooters();
        }

        private float GetFireRate()
        {
            if (m_shotShooters.Count == 0)
            {
                return 0f;
            }

            return m_shotShooters[0] switch
            {
                // Always assume all shooters are the same and have the same fire rate.
                ProjectileShooter projectileShooter => projectileShooter.FireRate,
                ExpandingShooter _ => 7 * m_shotShooters.Count, // TODO Convert this hardcode "7"
                _ => 0.0f
            };
        }

        private void SetFireRate(float fireRate)
        {
            if (m_shotShooters.Count == 0)
            {
                return;
            }

            foreach (ShotShooter shotShooter in ShotShooters)
            {
                // Only projectile shooters are able to set fire rate
                if (shotShooter is ProjectileShooter projectileShooter)
                {
                    projectileShooter.SetFireRate(fireRate);
                }
            }
        }

        public void Initialize()
        {
        }

        private void SetFiring(bool value)
        {
            // if (IsFiring == value)
            // {
            //     return;
            // }

            m_isFiring = value;

            foreach (Pattern pattern in Patterns)
            {
                pattern.TriggerAutoFire = IsFiring;
            }
        }

        [Button]
        [PropertyOrder(-100)]
        [GUIColorDefaultButton]
        private void CollectPatternsAndShotShooters()
        {
            m_patterns = GetComponentsInChildren<Pattern>().ToList();
            m_shotShooters = GetComponentsInChildren<ShotShooter>().ToList();
        }
    }
}
