using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Armageddon.AssetManagement;
using Armageddon.Backend.Functions;
using Armageddon.Backend.Payloads;
using Armageddon.Games;
using Armageddon.Mechanics;
using Armageddon.Mechanics.Characters;
using Armageddon.Mechanics.Combats;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.Sheets.Actors;
using Armageddon.Sheets.Items;
using Armageddon.UI.Base;
using Armageddon.Worlds.Actors;
using Armageddon.Worlds.Actors.Characters;
using Armageddon.Worlds.Actors.Companions;
using Armageddon.Worlds.Actors.Enemies;
using Armageddon.Worlds.Actors.Heroes;
using Armageddon.Worlds.Actors.Weapons;
using Armageddon.Worlds.Environment;
using Armageddon.Worlds.Misc;
using Cysharp.Threading.Tasks;
using Purity.Bullet2D.Shots;
using Purity.Bullet2D.Utilities;
using Purity.Common;
using Purity.Common.Extensions;
using ResolutionMagic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Armageddon.Worlds
{
    public class Spawn
    {
        /// <summary>
        ///     Can be used to postpone creation and use pool in the future
        /// </summary>
        public List<EnemyPayload> EnemyObjects = new();

        public List<EnemyActor> EnemyActors;
    }

    public class Wave
    {
        public List<Spawn> Spawns = new();
    }

    public enum BattleStageState
    {
        Prepare,
        Playing,
        Ending
    }

    public class WorldResult
    {
        public int StageId { get; set; }
        public List<int> KilledEnemies { get; set; } = new();
        public List<int> ReceivedDrops { get; set; } = new();
    }

    public class World : Context
    {
        private const string Prefabs = "Prefabs";
        public static float ScaleAnimationDuration = 0.5f;
        public static float ScaleAnimationStartPercent = 0.3f;
        public static float MinRandomDropSpeed = 3f;
        public static float MaxRandomDropSpeed = 8f;

        [BoxGroup(Prefabs)]
        [SerializeField]
        private CharacterHud m_characterHudPrefab;

        [SerializeField]
        private EnemyPool m_enemyPool;

        [SerializeField]
        private WorldObjectPool m_objectPool;

        [SerializeField]
        private HudPool m_hudPool;

        [SerializeField]
        private DropHudPool m_dropHudPool;

        [SerializeField]
        private Location m_startEnemySpawnLocation;

        [SerializeField]
        private Location m_heroSpawnLocation;

        [SerializeField]
        private BattleLog m_battleLog;

        private PlayerController m_playerController;

        private Queue<Wave> m_waveQueue = new();

        private List<Wave> m_waves = new();

        private List<EnemyActor> m_activeEnemies = new();

        private int m_actorId;

        // private List<int> m_killedEnemies;

        private WorldResult m_result;

        private Action<WorldResult> m_endGameCallback;

        public PlayerController PlayerController
        {
            get
            {
                if (m_playerController == null)
                {
                    m_playerController = GetService<PlayerController>();
                }

                return m_playerController;
            }
        }

        public BattleStageState State { get; private set; }

        public EnemyPool EnemyPool => m_enemyPool;

        [ShowInPlayMode]
        public HeroActor HeroActor { get; private set; }

        public HudPool HudPool => m_hudPool;

        public DropHudPool DropHudPool => m_dropHudPool;

        [ShowInPlayMode]
        private Transform EnemyParent { get; set; }

        public Location HeroSpawnLocation => m_heroSpawnLocation;

        public Location StartEnemySpawnLocation => m_startEnemySpawnLocation;

        public WorldObjectPool ObjectPool => m_objectPool;

        public BattleLog BattleLog => m_battleLog;

        protected override void Awake()
        {
            base.Awake();

            RegisterService(this);

            CanTick = true;

            var enemyParentObject = new GameObject("Enemies");
            EnemyParent = enemyParentObject.transform;
            EnemyParent.SetParent(Transform);
        }

        public async UniTask<bool> LoadObjects(StartGameReply reply, Player player)
        {
            // TODO: Change this hardcode.
            await LoadZoneAsync(1, 1);

            // We get only hero inventory from reply so just check with the current one.
            if (!player.HeroInventory.CheckIntegrity(reply.HeroInventory))
            {
                // TODO: Change State To MainMenu like other kind of cheating.
                return false;
            }

            m_actorId = 0;

            Hero hero = player.GetHero(reply.Hero.InstanceId);
            HeroActor = await CreateHeroActorAsync(hero, player.HeroInventory);

            for (int i = 0; i < player.HeroInventory.Slots.Count; i++)
            {
                var slotType = (EquipmentSlotType)i;
                Item item = player.HeroInventory.GetItemAtSlot(slotType);

                if (item != null)
                {
                    await HeroActor.EquipItemAsync(slotType, item);
                }
            }

            HeroActor.CompileStats();

            int totalEnemyCount = reply.BattleStage.GetTotalEnemyCount();
            CreateCharacterHudsInPool(totalEnemyCount);
            DropHudPool.Create(totalEnemyCount);

            await CreateEnemyActors(reply.BattleStage);

            m_result = new WorldResult
            {
                StageId = reply.BattleStage.StageId
            };

            Bullet2DManager.Instance.GetComponent<OnScreenDisplay>().enabled = false;

            return true;
        }

        public async UniTask<Zone> LoadZoneAsync(int mapId, int zoneId)
        {
            Zone zone = await Assets.InstantiateZone(mapId, zoneId, Transform);
            zone.Transform.SetAsFirstSibling();
            zone.Initialize();

            return zone;
        }

        public void Prepare()
        {
            if (HeroActor == null)
            {
                Debug.LogError($"{nameof(HeroActor)} is null");
                return;
            }

            PlayerController.MainCamera = Camera.main;
            PlayerController.CharacterBase.UnequipAll();

            PlayerController.CanTick = true;
            PlayerController.SetMainCharacterActor(HeroActor);
            PlayerController.SetFiring(true);

            PlayerController.CharacterBase.Transform.position = HeroSpawnLocation;
            PlayerController.TargetPosition = HeroSpawnLocation;
            PlayerController.Commence();

            RepositionAllWaves();

            ResolutionManager.Instance.RefreshResolution();

            State = BattleStageState.Prepare;

            // TODO: Play Audio
            // Game.Audio.PlayMusic(Game.Audio.TestMusicAudioClip);

            BattleLog.CreateLog(null);
        }

        private void CreateCharacterHudsInPool(int startupSize)
        {
            var batch = new ObjectPool<Widget>.Entry
            {
                SourceObject = m_characterHudPrefab,
                StartupSize = startupSize,
                WillExpand = true,
                ExpandingSize = startupSize / 2
            };

            HudPool.AddEntry(batch);
        }


        public async UniTask<HeroActor> CreateHeroActorAsync(Hero hero, HeroInventory heroInventory = null)
        {
            return await CreateHeroActorAsync(hero.SheetId, heroInventory);
        }

        public async UniTask<HeroActor> AddMainHeroActorAsync(int heroSheetId, HeroInventory heroInventory = null)
        {
            HeroActor = await CreateHeroActorAsync(heroSheetId, heroInventory);

            return HeroActor;
        }

        public async UniTask<HeroActor> CreateHeroActorAsync(int heroSheetId, HeroInventory heroInventory = null)
        {
            HeroSheet heroSheet = await Assets.LoadHeroSheetAsync(heroSheetId);

            var description = new CharacterDescriptor
            {
                ActorLayer = Layers.Player,
                BulletLayer = Layers.PlayerBullet,
                BulletCollisionMask = LayerMaskEx.GetMask(Layers.Enemy),
                CombatEntity = new CombatEntity(CombatEntityType.Hero, $"{heroSheet}", true)
            };

            HeroActor heroActor = await heroSheet.CreateActorAsync(Transform);
            heroActor.Initialize(description);
            heroActor.RemoveCloneFromName($"-{m_actorId++}");

            if (heroInventory == null)
            {
                return heroActor;
            }

            heroActor.DetachAllWeapons();

            // {
            //     Item item = heroInventory.GetItemAtSlot(EquipmentSlotType.PrimaryWeapon);
            //
            //     if (item != null)
            //     {
            //         heroActor.EquipItemAsync(EquipmentSlotType.PrimaryWeapon, item).Forget();
            //
            //         // var sheet = (WeaponSheet)item.Sheet;
            //         //
            //         // Weapon weapon = Instantiate(sheet.WeaponPrefab);
            //         // heroActor.AttachWeapon(weapon, 0);
            //     }
            // }
            // {
            //     Item item = heroInventory.GetItemAtSlot(EquipmentSlotType.SecondaryWeapon);
            //
            //     if (item != null)
            //     {
            //         heroActor.EquipItemAsync(EquipmentSlotType.SecondaryWeapon, item).Forget();
            //
            //         // var sheet = (WeaponSheet)item.Sheet;
            //         //
            //         // Weapon weapon = Instantiate(sheet.WeaponPrefab);
            //         // heroActor.AttachWeapon(weapon, 1);
            //     }
            // }

            heroActor.SetFiring(true);

            return heroActor;
        }

        public async UniTask<CompanionActor> CreateCompanionActorAsync(Companion companion)
        {
            CompanionSheet companionSheet = companion.Sheet;

            var description = new CharacterDescriptor
            {
                ActorLayer = Layers.Player,
                BulletLayer = Layers.PlayerBullet,
                BulletCollisionMask = LayerMaskEx.GetMask(Layers.Enemy),
                CombatEntity = new CombatEntity(CombatEntityType.Companion, $"{companionSheet}", true)
            };

            CompanionActor companionActor = await companionSheet.CreateActorAsync(Transform);
            companionActor.Initialize(description);
            companionActor.RemoveCloneFromName($"-{m_actorId++}");

            companionActor.DetachAllWeapons();

            {
                WeaponSheet weaponSheet = companionSheet.WeaponSheet;
                WeaponActor weaponActor = await weaponSheet.LoadWeaponAsync();

                var weapon = new Weapon
                {
                    Sheet = weaponSheet,
                    DamagePerSecond = new ItemStat(companion.DamagePerSecond),
                    FireRate = new ItemStat(companion.FireRate),
                    ShotsPerFire = new ItemStat(weaponActor.ShotsPerFire)
                };

                weapon.DamagePerShot = new ItemStat(weapon.DamagePerSecond / weapon.FireRate / weapon.ShotsPerFire);

                companionActor.EquipItemAsync(EquipmentSlotType.PrimaryWeapon, weapon).Forget();
            }

            companionActor.SetFiring(true);

            return companionActor;
        }

        private async UniTask CreateEnemyActors(BattleStagePayload battleStage)
        {
            foreach (EnemyActor activeEnemy in m_activeEnemies)
            {
                activeEnemy.Died -= OnEnemyDied;
            }

            m_activeEnemies = new List<EnemyActor>();
            m_waves = new List<Wave>();

            foreach (WavePayload waveObject in battleStage.Waves)
            {
                var spawns = new List<Spawn>();

                foreach (SpawnPayload spawnObject in waveObject.Spawns)
                {
                    var enemyActors = new List<EnemyActor>();

                    foreach (EnemyPayload enemyObject in spawnObject.Enemies)
                    {
                        EnemyActor enemyActor = await CreateEnemyActor(enemyObject, Vector2.zero);

                        enemyActors.Add(enemyActor);
                    }

                    var spawn = new Spawn
                    {
                        EnemyActors = enemyActors
                    };

                    spawns.Add(spawn);
                }

                var wave = new Wave
                {
                    Spawns = spawns
                };

                m_waves.Add(wave);
            }
        }

        private void RepositionAllWaves()
        {
            Vector2 startPosition = m_startEnemySpawnLocation;

            for (int waveIndex = 0; waveIndex < m_waves.Count; waveIndex++)
            {
                Wave wave = m_waves[waveIndex];
                for (int spawnIndex = 0; spawnIndex < wave.Spawns.Count; spawnIndex++)
                {
                    Spawn spawn = wave.Spawns[spawnIndex];
                    startPosition.x = 0.0f;

                    float lastOffset = 0;

                    for (int enemyIndex = 0; enemyIndex < spawn.EnemyActors.Count; enemyIndex++)
                    {
                        EnemyActor enemyActor = spawn.EnemyActors[enemyIndex];
                        startPosition.x += lastOffset;

                        enemyActor.Transform.localPosition = startPosition;
                        enemyActor.UpdateHudPosition();

                        if (enemyIndex < spawn.EnemyActors.Count - 1)
                        {
                            const float offsetFactor = 1.5f;

                            lastOffset = enemyActor.Bounds.size.x * offsetFactor * enemyActor.Size.ToFloat() * 0.5f;

                            EnemyActor nextEnemyActor = spawn.EnemyActors[enemyIndex + 1];
                            lastOffset += nextEnemyActor.Bounds.size.x * offsetFactor *
                                nextEnemyActor.Size.ToFloat() * 0.5f;
                        }
                    }

                    if (spawnIndex < wave.Spawns.Count - 1)
                    {
                        startPosition.y += GetBiggestOffsetY(spawn.EnemyActors) * 0.5f;

                        Spawn nextSpawn = wave.Spawns[spawnIndex + 1];
                        startPosition.y += GetBiggestOffsetY(nextSpawn.EnemyActors) * 0.5f;
                    }
                    else if (waveIndex < m_waves.Count - 1)
                    {
                        startPosition.y += GetBiggestOffsetY(spawn.EnemyActors) * 0.5f;

                        Spawn nextSpawn = m_waves[waveIndex + 1].Spawns[0];
                        startPosition.y += GetBiggestOffsetY(nextSpawn.EnemyActors) * 0.5f;
                    }
                }

                startPosition.y += 2.0f;
            }
        }

        private float GetBiggestOffsetY(List<EnemyActor> enemyActors)
        {
            float biggestBoundY = -1;
            float biggestScale = -1;

            foreach (EnemyActor enemyActor in enemyActors)
            {
                if (biggestBoundY < enemyActor.Bounds.size.y)
                {
                    biggestBoundY = enemyActor.Bounds.size.y;
                }

                if (biggestScale < enemyActor.Size.ToFloat())
                {
                    biggestScale = enemyActor.Size.ToFloat();
                }
            }

            return biggestBoundY * 1.5f * biggestScale;
        }

        public async UniTask<EnemyActor> CreateEnemyActor(EnemyPayload enemyPayload, Vector2 position)
        {
            EnemySheet sheet = await Assets.LoadEnemySheetAsync(enemyPayload.SheetId ?? 0);
            EnemyActor enemyActor = Instantiate(sheet.Prefab, EnemyParent);
            enemyActor.RemoveCloneFromName($"-{m_actorId++}");
            enemyActor.Transform.SetPositionAndRotation(position, Quaternion.identity);
            enemyActor.AllowPassAll = true;
            // We no longer use size from server, but we can still resize with a little value to make enemy look different.
            // enemyActor.Transform.localScale = enemyObject.size.ToVector2();

            var hud = HudPool.Get<CharacterHud>(m_characterHudPrefab);
            string enemyName = $"{enemyActor}";

            var combatEntity = new CombatEntity(CombatEntityType.Enemy, enemyName, false)
            {
                HasCustomAttributes = true,
                StartingDexterity = enemyPayload.Dexterity ?? 0,
                StartingVitality = enemyPayload.Vitality ?? 0,
                StartingPerception = enemyPayload.Perception ?? 0,
                StartingLeadership = enemyPayload.Leadership ?? 0,
                StartingHealth = enemyPayload.Health ?? 0,
                StartingHealthRegeneration = enemyPayload.HealthRegeneration ?? 0,
                StartingArmor = enemyPayload.Armor ?? 0,
                StartingAttackPower = enemyPayload.PrimaryDamage ?? 0,
                StartingCriticalResistance = 0.0f,
                Tier = (int)sheet.Tier,
                Size = sheet.Size,
                Race = sheet.Race,
                Element = sheet.Element
            };

            var drops = new List<StageDrop>();

            foreach (StageDropPayload dropObject in enemyPayload.Drops)
            {
                switch (dropObject.Type)
                {
                    case DropType.Currency:
                    {
                        var currency = new StageDropCurrency();
                        var drop = new StageDrop(dropObject.Id, currency);

                        drops.Add(drop);
                        break;
                    }
                    case DropType.Item:
                    {
                        ItemSheet itemSheet = await Assets.LoadItemSheetAsync(dropObject.Item.SheetId);
                        var item = new StageDropItem
                        {
                            Sheet = itemSheet,
                            Quality = dropObject.Item.Quality ?? ItemQuality.Common,
                            Quantity = dropObject.Item.Quantity ?? 1
                        };

                        var drop = new StageDrop(dropObject.Id, item);
                        drops.Add(drop);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var description = new EnemyDescriptor
            {
                CharacterHud = hud,
                Id = enemyPayload.Id,
                Level = enemyPayload.Level,
                Sheet = sheet,
                Rank = enemyPayload.Rank ?? EnemyRank.Regular,
                CombatEntity = combatEntity,
                Drops = drops
            };

            enemyActor.Initialize(description);

            return enemyActor;
        }

        public void Commence(Action<WorldResult> resultCallback)
        {
            var game = GetService<Game>();
            game.Resume();

            // m_killedEnemies = new List<int>();
            m_endGameCallback = resultCallback;
            m_waveQueue = new Queue<Wave>();
            m_waves.Reverse();

            foreach (Wave wave in m_waves)
            {
                m_waveQueue.Enqueue(wave);
            }

            m_waves.Clear();

            StopAllUniTasks();
            ReleaseWavesAsync().Forget();

            // BattleLog.CreateLog(null);
        }

        // public async UniTask CommenceAsync()
        // {
        //     // m_killedEnemies = new List<int>();
        //     m_waves.Reverse();
        //     m_waveQueue = new Queue<Wave>();
        //
        //     foreach (Wave wave in m_waves)
        //     {
        //         m_waveQueue.Enqueue(wave);
        //     }
        //
        //     m_waves.Clear();
        //
        //     StopAllUniTasks();
        //     await ReleaseWavesAsync();
        // }

        private async UniTask ReleaseWavesAsync()
        {
            State = BattleStageState.Playing;
            float spawnTimer = 0;
            Wave currentWave = m_waveQueue.Dequeue();
            Spawn nextSpawn = currentWave.Spawns.FirstOrDefault();
            CancellationToken token = GetCancellationToken(nameof(ReleaseWavesAsync));

            while (State == BattleStageState.Playing)
            {
                spawnTimer -= Time.deltaTime;

                if (spawnTimer <= 0)
                {
                    if (nextSpawn != null)
                    {
                        foreach (EnemyActor enemyActor in nextSpawn.EnemyActors)
                        {
                            enemyActor.Commence();
                            enemyActor.Died += OnEnemyDied;

                            m_activeEnemies.Add(enemyActor);
                        }
                    }

                    if (currentWave != null && currentWave.Spawns.Count > 0)
                    {
                        currentWave.Spawns.Remove(nextSpawn);
                        nextSpawn = currentWave.Spawns.FirstOrDefault();
                        spawnTimer = 1.0f;
                    }
                    else
                    {
                        // Wait until the player clears out the current wave.
                        if (m_activeEnemies.Count == 0)
                        {
                            if (m_waveQueue.Count > 0)
                            {
                                currentWave = m_waveQueue.Dequeue();
                                nextSpawn = currentWave.Spawns.FirstOrDefault();
                            }
                            else
                            {
                                currentWave = null;
                                Debug.Log("No more wave queue.");
                            }

                            // currentWave = m_waveQueue.Count > 0 ? m_waveQueue.Dequeue() : null;
                            // if (currentWave == null)
                            // {
                            //     Debug.Log("No more wave queue.");
                            //     // break;
                            // }
                            // else
                            // {
                            //     nextSpawn = currentWave.Spawns.FirstOrDefault();
                            // }
                        }
                    }
                }

                if (m_activeEnemies.Count <= 0 && m_waveQueue.Count == 0 && nextSpawn == null)
                {
                    Debug.Log("No more active enemies");
                    await UniTask.Delay(1000, cancellationToken: token);
                    EndGame();
                    break;
                }

                await UniTask.Yield(token);
            }
        }

        public void EndGame()
        {
            // var ui = GetService<UISystem>();
            // ui.InGameMenuScreen.

            State = BattleStageState.Ending;
            m_endGameCallback?.Invoke(m_result);
        }

        // private async UniTask ReleaseWavesAsync()
        // {
        //     State = BattleStageState.Playing;
        //     float spawnTimer = 0;
        //     Wave currentWave = m_waveQueue.Dequeue();
        //     Spawn nextSpawn = currentWave.Spawns.FirstOrDefault();
        //     CancellationToken token = GetCancellationToken(nameof(ReleaseWavesAsync));
        //
        //     while (State == BattleStageState.Playing)
        //     {
        //         foreach (Spawn spawn in currentWave.Spawns)
        //         {
        //             foreach (EnemyActor enemyActor in spawn.EnemyActors)
        //             {
        //                 enemyActor.Commence();
        //                 enemyActor.Died += OnEnemyDied;
        //                 
        //                 m_activeEnemies.Add(enemyActor);
        //             }
        //             
        //             await UniTask.Delay(1000, cancellationToken: token);
        //         }
        //
        //         currentWave.Spawns.Clear();
        //
        //
        //         if (currentWave.Spawns.Count == 0)
        //         {
        //             currentWave = m_waveQueue.Count > 0 ? m_waveQueue.Dequeue() : null;
        //         }
        //
        //         if (currentWave == null)
        //         {
        //             Debug.Log("No more wave queue.");
        //             break;
        //         }
        //
        //         await UniTask.Delay(3000, cancellationToken: token);
        //         // await UniTask.Yield();
        //     }
        //
        //     while (m_activeEnemies.Count > 0)
        //     {
        //         await UniTask.Yield(token);
        //         // await UniTask.Delay(1000);
        //     }
        //     
        //     Debug.Log("No more active enemies");
        //
        //     await UniTask.Delay(1000, cancellationToken: token);
        //     m_resultCallback?.Invoke(m_result);
        // }

        public void ClearResources()
        {
            HudPool.ClearEntries();

            DestroyGameObject(HeroActor);
            HeroActor = null;

            EnemyParent.DestroyChildren();
        }

        private void OnEnemyDied(object sender, CharacterDiedArgs e)
        {
            if (e.CharacterActor is EnemyActor enemyActor)
            {
                CreateDrops(enemyActor);

                m_result.KilledEnemies.Add(enemyActor.ObjectId);
                m_activeEnemies.Remove(enemyActor);
                enemyActor.Died -= OnEnemyDied;

                DestroyGameObject(enemyActor);
            }
        }

        private void CreateDrops(EnemyActor enemyActor)
        {
            foreach (StageDrop drop in enemyActor.Descriptor.Drops)
            {
                Debug.Log($"{enemyActor.name} has dropped {drop}");
                m_result.ReceivedDrops.Add(drop.Id);

                float angleDegree = Random.Range(0, 360);
                float length = Random.Range(1, 4f);
                Vector2 offset = Vector2.right.Rotate(angleDegree) * length;

                Vector2 startPosition = enemyActor.Position;
                Vector2 endPosition = startPosition + offset;
                CreateDrop(drop, startPosition, endPosition);
            }
        }

        private void CreateDrop(StageDrop drop, Vector2 startPosition, Vector2 endPosition)
        {
            DropActor dropActor = ObjectPool.GetDropActor();

            switch (drop.Type)
            {
                case DropType.Item:
                    dropActor.Initialize(drop, startPosition, endPosition, HeroActor);
                    dropActor.Execute(OnDropHitHeroCharacter);
                    break;
                case DropType.Currency:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDropHitHeroCharacter(DropActor dropActor)
        {
            // ObjectPool.AddObject(dropActor);
        }

        public override void Tick()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                // Commence();
            }
        }

        public void CreateTextDamage(float damage, Transform target)
        {
            DamageText damageText = ObjectPool.GetDamageText(DamageTextType.Normal);
            damageText.Play(target);
        }

        public void CreateFloatyText(AttackHit hit, Actor actor)
        {
            CreateTextDamage(hit.DamageDealt, actor, hit.Type.ToDamageTextType());
        }

        public void CreateTextDamage(float damage, Actor actor, DamageTextType type)
        {
            DamageText damageText = ObjectPool.GetDamageText(type);
            damageText.Play(actor, damage);

            // Really doesn't work. Looks chaotic
            // if (actor.DamageText == null)
            // {
            //     DamageText damageText = ObjectPool.GetDamageText(DamageTextType.Normal);
            //     damageText.Play(actor, damage);
            // }
            // else
            // {
            //     actor.DamageText.Play(actor, damage);
            // }
        }

        // protected override void OnGamePaused(object sender, EventArgs e)
        // {
        //     base.OnGamePaused(sender, e);
        //
        //     foreach (EnemyActor enemy in m_activeEnemies)
        //     {
        //         var fsmOwner = enemy.GetComponent<FSMOwner>();
        //         fsmOwner.enabled = false;
        //     }
        // }

        // protected override void OnGameResumed(object sender, EventArgs e)
        // {
        //     base.OnGameResumed(sender, e);
        //     
        //     foreach (EnemyActor enemy in m_activeEnemies)
        //     {
        //         var fsmOwner = enemy.GetComponent<FSMOwner>();
        //         fsmOwner.enabled = true;
        //     }
        // }
    }
}
