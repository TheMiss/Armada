using System;
using System.Collections.Generic;
using Armageddon.Backend.Payloads;
using Armageddon.Externals.OdinInspector;
using Armageddon.Mechanics.Characters;
using Armageddon.Sheets.Actors;
using Armageddon.Worlds.Actors.Enemies;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using Purity.Common.Editor;
#endif

namespace Armageddon.Design
{
    public static class EnemyTierExtensions
    {
        public static List<EnemyRank> ToPossibleRanks(this EnemyTier tier)
        {
            return tier switch
            {
                EnemyTier.Minion => new List<EnemyRank>
                {
                    EnemyRank.Regular, EnemyRank.Superior, EnemyRank.Champion
                },
                EnemyTier.Noble => new List<EnemyRank>
                {
                    EnemyRank.Elite, EnemyRank.Ruler
                },
                EnemyTier.Celestial => new List<EnemyRank>
                {
                    EnemyRank.Demigod, EnemyRank.Deity
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    [Serializable]
    public class EnemySpawnerEntry
    {
        public EnemySheet EnemySheet;

        [ValueDropdown(nameof(MatchedRanks))]
        public EnemyRank Rank;

        public UnityAction<EnemySpawnerEntry> SpawnButtonClicked;

        public EnemySpawnerEntry(EnemySheet sheet)
        {
            EnemySheet = sheet;
        }

        private List<EnemyRank> MatchedRanks => EnemySheet.Tier.ToPossibleRanks();

        [VerticalGroup("Actions")]
        [Button]
        public void Spawn()
        {
            SpawnButtonClicked?.Invoke(this);
        }
    }

    public class EnemySpawner : SandboxContext
    {
        public int StartSpawnEntryIndex;
        public int StartSpawnCount = 1;
        public int Health = 100;
        public int Dexterity;
        public int Vitality;
        public int Perception;
        public int Leadership;
        public int HealthRegeneration;
        public int Armor;
        public int PrimaryDamage;

        [TableList(IsReadOnly = true, ShowIndexLabels = true)]
        public List<EnemySpawnerEntry> Entries = new List<EnemySpawnerEntry>();

        protected override void Start()
        {
            base.Start();

            RegisterEvents();
        }

#if UNITY_EDITOR
        [Button]
        [PropertyOrder(-100)]
        [GUIColorDefaultButton]
        private void ReloadEnemySheets()
        {
            List<EnemySheet> enemySheets = AssetDatabaseEx.LoadAssets<EnemySheet>();

            Entries = new List<EnemySpawnerEntry>();

            foreach (EnemySheet enemySheet in enemySheets)
            {
                if (enemySheet.Id == 0)
                {
                    continue;
                }

                var entry = new EnemySpawnerEntry(enemySheet);
                Entries.Add(entry);
            }
        }
#endif

        private void RegisterEvents()
        {
            foreach (EnemySpawnerEntry entry in Entries)
            {
                entry.SpawnButtonClicked = OnSpawnButtonClicked;
            }
        }

        private void OnSpawnButtonClicked(EnemySpawnerEntry entry)
        {
            // Should not be registered in Editor anyway.
            if (!Application.isPlaying)
            {
                return;
            }

            SpawnEnemy(entry).Forget();
        }

        public async UniTask<bool> SpawnEnemy(EnemySpawnerEntry entry)
        {
            var enemyObject = new EnemyPayload
            {
                SheetId = entry.EnemySheet.Id,
                Rank = entry.Rank,
                Dexterity = Dexterity,
                Vitality = Vitality,
                Perception = Perception,
                Leadership = Leadership,
                Health = Health,
                HealthRegeneration = HealthRegeneration,
                Armor = Armor,
                PrimaryDamage = PrimaryDamage
            };

            EnemyActor enemyActor = await World.CreateEnemyActor(enemyObject, World.HeroSpawnLocation);
            enemyActor.Commence();

            return true;
        }

        public async UniTask<bool> SpawnDefaults()
        {
            int index = StartSpawnEntryIndex;

            // No checking here
            await SpawnEnemy(Entries[index]);

            return true;
        }
    }
}
