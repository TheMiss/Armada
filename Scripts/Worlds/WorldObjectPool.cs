using System;
using Armageddon.Externals.OdinInspector;
using Armageddon.Worlds.Misc;
using Purity.Common;
using UnityEngine;

namespace Armageddon.Worlds
{
    public class WorldObjectPool : ObjectPool<Context>
    {
        [BoxGroupPrefabs]
        [SerializeField]
        private DamageText m_normalHitDamageTextPrefab;

        [BoxGroupPrefabs]
        [SerializeField]
        private DamageText m_criticalHitDamageTextPrefab;

        [BoxGroupPrefabs]
        [SerializeField]
        private DropActor m_dropActorPrefab;

        protected override void Start()
        {
            base.Start();

            Initialize();
        }

        private void Initialize()
        {
            CreateEntry(m_normalHitDamageTextPrefab, 20, 10);
            CreateEntry(m_criticalHitDamageTextPrefab, 20, 10);
            CreateEntry(m_dropActorPrefab, 20, 10);
        }

        private void CreateEntry(Context source, int initialSize, int expandingSize)
        {
            var entry = new Entry
            {
                SourceObject = source,
                StartupSize = initialSize,
                ExpandingSize = expandingSize
            };

            AddEntry(entry);
        }

        public DamageText GetDamageText(DamageTextType type)
        {
            return type switch
            {
                DamageTextType.Normal => Get<DamageText>(m_normalHitDamageTextPrefab),
                DamageTextType.Critical => Get<DamageText>(m_criticalHitDamageTextPrefab),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public DropActor GetDropActor()
        {
            return Get<DropActor>(m_dropActorPrefab);
        }
    }
}
