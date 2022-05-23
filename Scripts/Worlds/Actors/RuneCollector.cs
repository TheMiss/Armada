using System;
using System.Collections.Generic;
using Armageddon.Games;
using Armageddon.Worlds.Actors.Runes;
using UnityEngine;

namespace Armageddon.Worlds.Actors
{
    public class RuneHitArgs : EventArgs
    {
        public RuneHitArgs(RuneActor runeActor)
        {
            RuneActor = runeActor;
        }

        public RuneActor RuneActor { get; }
    }

    public class RuneCollector : GameContext
    {
        [SerializeField]
        private Collider2D m_collider2D;

        private readonly List<Collider2D> m_overlapColliders = new();

        public ContactFilter2D ContactFilter2D { set; get; }

        protected override void Awake()
        {
            base.Awake();
            ContactFilter2D = GetDefaultContactFilter2D();

            CanTick = true;
        }

        public event EventHandler<RuneHitArgs> ItemHit;

        private static ContactFilter2D GetDefaultContactFilter2D()
        {
            // Detect items only
            int itemMask = 1 << LayerMask.NameToLayer("Rune");
            int layerMask = itemMask;

            var contactFilter2D = new ContactFilter2D();
            contactFilter2D.SetLayerMask(layerMask);

            return contactFilter2D;
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
                var item = other.GetComponent<RuneActor>();

                if (item == null)
                {
                    continue;
                }

                ItemHit?.Invoke(this, new RuneHitArgs(item));
                break;
            }
        }
    }
}
