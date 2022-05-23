using System;
using System.Collections.Generic;
using Armageddon.Extensions;
using Armageddon.Mechanics.Combats;
using Armageddon.Mechanics.Inventories;
using Armageddon.Mechanics.Items;
using Armageddon.Sheets.Items;
using Armageddon.Worlds.Actors.Companions;
using Armageddon.Worlds.Actors.Unused;
using Armageddon.Worlds.Actors.Weapons;
using Cysharp.Threading.Tasks;
using Purity.Bullet2D.Shots;
using Purity.Bullet2D.Shots.Projectiles;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Armageddon.Worlds.Actors.Characters
{
    public class CharacterDiedArgs : EventArgs
    {
        public CharacterDiedArgs(CharacterActor characterActor)
        {
            CharacterActor = characterActor;
        }

        public CharacterActor CharacterActor { get; }
    }

    public abstract class CharacterActor : Actor
    {
        public const float FlickerInterval = 0.05f;

        [Header(nameof(CharacterActor) + " Fields", order = 1)]
        [SerializeField]
        private Animator m_animator;

        [SerializeField]
        private SpriteLibrary m_spriteLibrary;

        // [SerializeField]
        // private Damageable m_damageable;
        //
        // [SerializeField]
        // private Damager m_damager;

        [SerializeField]
        private Flicker m_flicker;

        [SerializeField]
        private Transform m_weaponParentTransform;

        [ReadOnly]
        [SerializeField]
        private List<PartResolver> m_partResolvers;

        private bool m_autoCastSkills;

        public float AutoCastCheckInterval { set; get; } = 1.0f;

        public long CurrentHealth => CombatEntity.CurrentHealth;

        public long Health => CombatEntity.Health.ValueLong;

        /// <summary>
        ///     This is server instance id. It must not confused with GetInstanceID().
        /// </summary>
        [ShowInPlayMode]
        public int ObjectId { get; private set; }

        [ShowInPlayMode]
        public CharacterHud Hud { get; private set; }

        public Animator Animator => m_animator;

        public SpriteLibrary SpriteLibrary => m_spriteLibrary;

        [ShowInPlayMode]
        public CombatEntity CombatEntity { get; private set; }

        // public CharacterActorInventory Inventory { get; private set; } = new CharacterActorInventory();

        protected AudioClip FireAudioClip { set; get; }

        public CharacterDescriptor Descriptor { get; private set; }

        [ReadOnly]
        [HideInEditorMode]
        [ShowInInspector]
        public List<WeaponActor> Weapons { protected set; get; }

        public List<PartResolver> PartResolvers => m_partResolvers;

        public bool AutoCastSkills
        {
            get => m_autoCastSkills;
            set => SetAutoCastSkills(value);
        }

        protected Transform WeaponParentTransform => m_weaponParentTransform;

        protected override void OnDestroy()
        {
            if (CombatEntity != null)
            {
                CombatEntity.StatusEffectsChanged -= OnStatusEffectsChanged;
            }

            base.OnDestroy();
        }

        private void OnValidate()
        {
            PartResolver[] partResolvers = GetComponentsInChildren<PartResolver>();

            if (PartResolvers.Count != partResolvers.Length)
            {
                PartResolvers.Clear();
                PartResolvers.AddRange(partResolvers);
            }
        }

        public event EventHandler<CharacterDiedArgs> Died;

        public virtual void Initialize(CharacterDescriptor descriptor)
        {
            Descriptor = descriptor;
            ObjectId = descriptor.Id;
            Hud = descriptor.CharacterHud;

            if (descriptor.CombatEntity == null)
            {
                Debug.LogWarning("descriptor.CombatEntity is null.");
                return;
            }

            CombatEntity = descriptor.CombatEntity;
            CombatEntity.StatusEffectsChanged += OnStatusEffectsChanged;
            // CombatEntity.StatusEff

            if (descriptor.AddCollisionHandler)
            {
            }

            gameObject.SetLayer(Descriptor.ActorLayer);

            // SetAppearance(args.Skin, args.Parts);

            // TODO: Check if need to remove
            // SpriteLibrary.spriteLibraryAsset = Skin.SpriteLibraryAsset;

            Weapons = new List<WeaponActor>();

            for (int i = 0; i < Descriptor.WeaponSlotCount; i++)
            {
                Weapons.Add(null);
            }

            CombatEntity.CompileStats();

            CanTick = true;
        }

        public virtual async UniTask EquipItemAsync(EquipmentSlotType slotType, Item item)
        {
            CombatEntity.EquipItem(slotType, item);

            if (item.Type == ItemType.PrimaryWeapon ||
                item.Type == ItemType.SecondaryWeapon)
            {
                if (item is Weapon weapon)
                {
                    WeaponSheet weaponSheet = weapon.Sheet;

                    WeaponActor weaponActor = await weaponSheet.CreateWeaponAsync();
                    weaponActor.Weapon = weapon;
                    weaponActor.gameObject.SetActive(true);

                    AttachWeapon(weaponActor, (int)item.Type);
                }
            }
            else if (item.Type == ItemType.Companion)
            {
                if (item is Companion companion)
                {
                    var companionBase = GetComponentInChildren<CompanionBase>();

                    if (companionBase == null)
                    {
                        Debug.LogWarning("companionBase is not found!");
                        return;
                    }

                    CompanionActor companionActor = await World.CreateCompanionActorAsync(companion);

                    Transform followingTarget = slotType switch
                    {
                        EquipmentSlotType.CompanionLeft => companionBase.LeftTransform,
                        EquipmentSlotType.CompanionRight => companionBase.RightTransform,
                        _ => null
                    };

                    companionActor.FollowingTarget = followingTarget;
                }
            }
        }

        public void UnequipItem(EquipmentSlotType slotType)
        {
            CombatEntity.UnequipItem(slotType);

            if (slotType == EquipmentSlotType.PrimaryWeapon ||
                slotType == EquipmentSlotType.SecondaryWeapon)
            {
                DetachWeapon((int)slotType);
            }
        }

        public void SetCameraForWeapons(Camera cam)
        {
            foreach (WeaponActor weapon in Weapons)
            {
                if (weapon == null)
                {
                    continue;
                }

                foreach (ShotShooter shotShooter in weapon.ShotShooters)
                {
                    shotShooter.Camera = cam;
                }
            }
        }

        public void CompileStats()
        {
            CombatEntity.CompileStats();
        }

        public virtual void SetAppearance(List<int> variantIndexes)
        {
            throw new NotImplementedException();
            //    SetAppearance(Skin, variantIndexes);
        }

        public virtual void SetAppearance(CharacterSkin characterSkin)
        {
            foreach (PartResolver partResolver in PartResolvers)
            {
                partResolver.SetDye(characterSkin);
            }
        }

        public void SetWeapons(params WeaponActor[] weapons)
        {
            int index = 0;
            foreach (WeaponActor weapon in weapons)
            {
                AttachWeapon(weapon, index++);
            }
        }

        public void DetachAllWeapons()
        {
            for (int i = 0; i < Weapons.Count; i++)
            {
                DetachWeapon(i);
            }
        }

        public void DetachWeapon(int index)
        {
            if (index >= Weapons.Count)
            {
                Debug.LogError("DetachWeapon: index >= Weapons.Count");
                return;
            }

            DestroyGameObject(Weapons[index]);

            Weapons[index] = null;
        }

        public void AttachWeapon(WeaponActor weapon, int index, bool detachPreviousWeapon = true, bool autoFire = true)
        {
            if (index >= Weapons.Count)
            {
                Debug.LogError("AttachWeapon: index >= Weapons.Count");
                return;
            }

            if (Weapons[index] != null && !detachPreviousWeapon)
            {
                Debug.LogWarning($"Weapon {index} already has persistent weapon attached!");
                return;
            }

            DetachWeapon(index);

            weapon.Transform.SetParent(WeaponParentTransform, false);
            weapon.IsFiring = autoFire;

            foreach (ShotShooter shooter in weapon.ShotShooters)
            {
                // shooter.ShotFired.RemoveListener(shot => OnShotFired(weapon, shot));
                shooter.ShotFired.AddListener(shot => OnShotFired(weapon, shot));
            }

            Weapons[index] = weapon;
        }

        protected virtual void OnShotFired(WeaponActor weaponActor, Shot shot)
        {
            shot.gameObject.SetLayer(Descriptor.BulletLayer);

            if (shot is Projectile projectile)
            {
                projectile.CollisionMask = Descriptor.BulletCollisionMask;

                projectile.CustomData ??= new Attack();

                var attack = (Attack)projectile.CustomData;
                attack.SetValues(weaponActor.Weapon, CombatEntity);

                // projectile.CustomData = Combat.SetAttack(weaponActor.Weapon, attack, CombatEntity);
                // projectile.DamagePerHit = CombatEntity.PrimaryDamage.Value;
            }
        }

        private void OnDamagerHit(object sender, DamagerHitArgs e)
        {
            Debug.Log($"{name} took {e.Hitter.Damage} damage from {e.Hitter.name}");
            //Animator.Play("Invincible");
            // m_damageable.SetInvulnerable(2);
            m_flicker.Begin(FlickerInterval);
        }

        private void OnBecomeVulnerable(object sender, EventArgs e)
        {
            //Animator.Play("Idle");
            m_flicker.End();
        }

        private void OnDied(object sender, DamageableDiedArgs e)
        {
            Debug.Log($"Get killed by {e.Killer.name}");

            // TODO: Dispatch event instead
            // Game.ShowGameOverMenu();
        }

        protected override void HandleMovement()
        {
        }

        private void SetAutoCastSkills(bool value)
        {
            m_autoCastSkills = value;

            if (m_autoCastSkills)
            {
                AutoCastSkillsCheck().Forget();
            }
        }

        protected async UniTaskVoid AutoCastSkillsCheck()
        {
            while (AutoCastSkills)
            {
                await UniTask.Delay((int)(AutoCastCheckInterval * 1000), DelayType.DeltaTime);
            }

            Debug.Log($"{name} stopped AutoCastSkillsCheck().");
        }

        protected virtual IReadOnlyList<Actor> GetTargets()
        {
            return EmptyList;
        }

        public void MoveTo(Vector2 targetPosition)
        {
            Vector3 position = transform.position;
            if (position.x < targetPosition.x)
            {
                // position.x += Core.MoveSpeed * Time.deltaTime;

                if (position.x > targetPosition.x)
                {
                    position.x = targetPosition.x;
                }
            }
            else if (position.x > targetPosition.x)
            {
                // position.x -= Core.MoveSpeed * Time.deltaTime;

                if (position.x < targetPosition.x)
                {
                    position.x = targetPosition.x;
                }
            }

            if (position.x < MinBounds.x)
            {
                position.x = MinBounds.x;
            }
            else if (position.x > MaxBounds.x)
            {
                position.x = MaxBounds.x;
            }

            transform.position = position;
        }

        public void SetFiring(bool value)
        {
            // foreach (Weapon weapon in Weapons)
            // {
            //     weapon.IsFiring = value;
            // }
        }

        public void TakeDamage(int damage)
        {
            CombatEntity.CurrentHealth -= damage;
        }

        public override void Tick()
        {
            base.Tick();

            CombatEntity?.Tick();

            if (Hud != null && Hud.gameObject.activeInHierarchy)
            {
                Hud.SetHealth(CurrentHealth, Health);
            }
        }

        protected virtual void OnStatusEffectsChanged(object sender, CombatEntityStatusEffectsChangedArgs e)
        {
        }

        public void UpdateHealth()
        {
            Hud.SetHealth((int)CurrentHealth, (int)Health);

            if (CurrentHealth <= 0)
            {
                Hud.gameObject.SetActive(false);
                Died?.Invoke(this, new CharacterDiedArgs(this));
            }
        }
    }
}
