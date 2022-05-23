using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Armageddon.Mechanics.Combats;
using Purity.Bullet2D.Collisions;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Characters
{
    public class CharacterCollisionHandler : WorldContext, IShotCollisionHandler
    {
        [SerializeField]
        private LayerMask m_collisionMask;

        [AssetList]
        [SerializeField]
        private Explosion m_laserExplosionPrefab;

        [AssetList]
        [SerializeField]
        private Explosion m_projectileExplosionPrefab;

        [SerializeField]
        private bool m_parentExplosion = true;

        [AssetList]
        [SerializeField]
        private Explosion m_deathExplosionPrefab;

        [Range(0.1f, 8f)]
        [SerializeField]
        private float m_finalExplodeFactor = 2;

        [SerializeField]
        private bool m_damageFlicker;

        [Range(5, 40)]
        [SerializeField]
        private int m_flickerDuration = 6;

        [SerializeField]
        private Color m_damageColor;

        [SerializeField]
        private List<SpriteRenderer> m_spriteRenderers;

        private Color m_normalColor;

        public LayerMask CollisionMask
        {
            get => m_collisionMask;
            set => m_collisionMask = value;
        }

        public Explosion LaserExplosionPrefab
        {
            get => m_laserExplosionPrefab;
            set => m_laserExplosionPrefab = value;
        }

        public Explosion ProjectileExplosionPrefab
        {
            get => m_projectileExplosionPrefab;
            set => m_projectileExplosionPrefab = value;
        }

        public bool ParentExplosion
        {
            get => m_parentExplosion;
            set => m_parentExplosion = value;
        }

        public Explosion DeathExplosionPrefab
        {
            get => m_deathExplosionPrefab;
            set => m_deathExplosionPrefab = value;
        }

        public bool DamageFlicker
        {
            get => m_damageFlicker;
            set => m_damageFlicker = value;
        }

        public List<SpriteRenderer> SpriteRenderers
        {
            get => m_spriteRenderers;
            set => m_spriteRenderers = value;
        }

        [ShowInPlayMode]
        public CharacterActor CharacterActor { get; private set; }

        protected override void Start()
        {
            base.Start();

            // m_spriteRenderer = GetComponent<SpriteRenderer>();

            CharacterActor = GetComponent<CharacterActor>();

            if (SpriteRenderers.Count == 0)
            {
                SpriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
            }

            if (SpriteRenderers.Count > 0)
            {
                m_normalColor = m_spriteRenderers[0].color;
            }
        }

        public virtual void OnShotHit(ShotHit shotHit)
        {
            Explosion explosion = shotHit.Type switch
            {
                ShotHitType.Beam => m_laserExplosionPrefab,
                ShotHitType.BeamPacket => m_projectileExplosionPrefab,
                ShotHitType.Projectile => null,
                _ => null
            };

            SetDamage(shotHit);

            ExplosionBank.Instance.SetExplosion(explosion, m_parentExplosion, transform,
                new Vector2(shotHit.Point.x, shotHit.Point.y), 0, this);


            StartCoroutine(SetFlicker());
        }

        protected void SetDamage(ShotHit shotHit)
        {
            if (!(shotHit.CustomData is Attack attack))
            {
                Debug.LogWarning($"{shotHit.Shot} doesn't have Attack data!");
                return;
            }

            AttackHit hit = Combat.PerformAttackOnTarget(attack, CharacterActor.CombatEntity);

            World.CreateFloatyText(hit, CharacterActor);
            World.BattleLog.LogDamage(CharacterActor, hit.DamageDealt);

            CharacterActor.UpdateHealth();

            if (CharacterActor.CurrentHealth <= 0)
            {
                if (m_deathExplosionPrefab != null)
                {
                    Explosion finalExplode = ExplosionBank.Instance.SetExplosion(m_deathExplosionPrefab);

                    finalExplode.Transform.position = transform.position;
                    finalExplode.Transform.parent = null;
                    Vector3 localScale = finalExplode.Transform.localScale;
                    localScale = new Vector2(localScale.x * m_finalExplodeFactor, localScale.y * m_finalExplodeFactor);
                    finalExplode.Transform.localScale = localScale;
                }

                // Destroy(gameObject);
            }

            // CharacterActor.Hud.SetHealth((int)CharacterActor.CurrentHealth, (int)CharacterActor.Health);
            //
            // if (CharacterActor.CurrentHealth <= 0)
            // {
            //     if (m_deathExplosionPrefab != null)
            //     {
            //         Explosion finalExplode = ExplosionBank.Instance.SetExplosion(m_deathExplosionPrefab);
            //
            //         finalExplode.Transform.position = transform.position;
            //         finalExplode.Transform.parent = null;
            //         Vector3 localScale = finalExplode.Transform.localScale;
            //         localScale = new Vector2(localScale.x * m_finalExplodeFactor, localScale.y * m_finalExplodeFactor);
            //         finalExplode.Transform.localScale = localScale;
            //     }
            //
            //     Destroy(gameObject);
            // }
        }

        protected IEnumerator SetFlicker()
        {
            if (m_spriteRenderers.Count == 0)
            {
                Debug.LogWarning("No SpriteRenderer attached. Cannot flicker during damage.", this);
                yield return null;
            }

            // TODO: Frame-base, revise this!
            if (m_damageFlicker)
            {
                bool flicker = false;
                for (int i = 0; i < m_flickerDuration * 2; i++)
                {
                    flicker = !flicker;

                    foreach (SpriteRenderer spriteRenderer in m_spriteRenderers)
                    {
                        spriteRenderer.color = flicker ? m_damageColor : m_normalColor;
                    }


                    yield return null;
                }

                foreach (SpriteRenderer spriteRenderer in m_spriteRenderers)
                {
                    spriteRenderer.color = m_normalColor;
                }
            }
        }
    }
}
