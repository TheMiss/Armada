using System;
using System.Collections.Generic;
using Purity.Common;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Unused
{
    public class DamageableHitArgs : EventArgs
    {
        public DamageableHitArgs(Damageable damageable)
        {
            Damageable = damageable;
        }

        public Damageable Damageable { get; }
    }

    public class Damager : Context
    {
        [SerializeField]
        private Collider2D m_collider2D;

        private readonly List<Collider2D> m_overlapColliders = new();

        private readonly RaycastHit2D[] m_raycastResults = new RaycastHit2D[5];

        public int Damage { set; get; } = 1;

        public ContactFilter2D ContactFilter2D { private set; get; }

        // Sometimes, ContactFilter is not enough. For example, we might want to ignore some objects in the same layer.
        public List<GameObject> IgnoredObjects { get; } = new();

        protected override void Awake()
        {
            base.Awake();

            CanTick = true;
        }

        public event EventHandler<DamageableHitArgs> DamageableHit;

        public void SetContactFilter(params int[] layers)
        {
            int layerMask = LayerMaskEx.GetMask(layers);

            var contactFilter2D = new ContactFilter2D();
            contactFilter2D.SetLayerMask(layerMask);

            ContactFilter2D = contactFilter2D;
        }

        public override void Tick()
        {
            if (!m_collider2D.enabled)
            {
                return;
            }

            if (Physics2D.OverlapCollider(m_collider2D, ContactFilter2D, m_overlapColliders) <= 0)
            {
                return;
            }

            foreach (Collider2D other in m_overlapColliders)
            {
                var otherDamageable = other.GetComponent<Damageable>();

                if (otherDamageable == null)
                {
                    // Collision mask should be set!
                    //Debug.LogWarning($"{name} hit {other.name}, but it has no damageable component attached!");
                    continue;
                }

                foreach (GameObject ignoredObject in IgnoredObjects)
                {
                    if (otherDamageable.gameObject == ignoredObject)
                    {
                        return;
                    }
                }

                Vector2 original = transform.position;
                Vector2 direction = (Vector2)other.transform.position - original;

                if (Physics2D.RaycastNonAlloc(original, direction, m_raycastResults) <= 0)
                {
                    Debug.Log($"{name}: How can this happen?");
                    continue;
                }

                // TODO: Check if it's sorted already.
                RaycastHit2D firstResult = m_raycastResults[0];

                otherDamageable.OnDamagerHit(this, firstResult.point);
                DamageableHit?.Invoke(this, new DamageableHitArgs(otherDamageable));
                break;
            }
        }
    }
}
