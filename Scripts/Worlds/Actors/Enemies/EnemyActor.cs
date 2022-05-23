using System;
using System.Collections.Generic;
using Armageddon.Mechanics.Characters;
using Armageddon.Mechanics.Combats;
using Armageddon.Sheets.Actors;
using Armageddon.Worlds.Actors.Characters;
using NodeCanvas.StateMachines;
using Purity.Common;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Enemies
{
    public class EnemyDieArgs : EventArgs
    {
        public EnemyDieArgs(EnemyActor enemyActor)
        {
            EnemyActor = enemyActor;
        }

        public EnemyActor EnemyActor { get; }
    }

    public class EnemyReturnToPoolArgs : EventArgs
    {
        public EnemyReturnToPoolArgs(EnemyActor enemyActor)
        {
            EnemyActor = enemyActor;
        }

        public EnemyActor EnemyActor { get; }
    }

    public class EnemySpawnArgs
    {
        public EnemySpawnArgs(EnemyActor schema, EnemyDescriptor descriptor)
        {
            Prefab = schema;
            Descriptor = descriptor;
        }

        public EnemyActor Prefab { get; }
        public EnemyDescriptor Descriptor { get; }
    }

    /// <summary>
    ///     Base class for Enemies, but we call them Fiends!
    /// </summary>
    public abstract class EnemyActor : CharacterActor
    {
        [ShowInPlayMode]
        public FSMOwner FsmOwner;

        private CharacterCollisionHandler m_collisionHandler;

        public new EnemyDescriptor Descriptor => base.Descriptor as EnemyDescriptor;

        [ShowInPlayMode]
        public EnemySheet Sheet { get; set; }

        [ShowInPlayMode]
        public EnemyRank Rank { get; set; }

        [ShowInPlayMode]
        public CharacterSize Size { get; set; }

        [ShowInPlayMode]
        public CharacterElement Element { get; set; }

        public CharacterCollisionHandler CollisionHandler
        {
            get => m_collisionHandler;
            set => m_collisionHandler = value;
        }

        /// <summary>
        ///     Used for checking skill type in Editors
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            AllowPassLeftWall = false;
            AllowPassRightWall = false;
            AllowPassGround = false;
            AllowPassCeiling = false;

            // Damager.SetContactFilter(Layers.Hero);
        }

        protected override void HandleMovement()
        {
            if (ApplyGravity)
            {
                Velocity += Physics2D.gravity * Time.deltaTime;
            }

            Transform.Translate(Velocity * Time.deltaTime);
        }

        public override void OnOffScreen()
        {
            //ReturnToPool();
        }

        public override void Initialize(CharacterDescriptor characterDescriptor)
        {
            base.Initialize(characterDescriptor);

            if (!(characterDescriptor is EnemyDescriptor description))
            {
                Debug.LogWarning($"{characterDescriptor}.GetType().Name = {characterDescriptor.GetType().Name}");
                return;
            }

            Sheet = description.Sheet;
            Rank = description.Rank;
            // Size = description.Size;
            // Element = description.Element;

            // Transform.localScale = description.Size.ToVector3();

            FsmOwner = GetComponent<FSMOwner>();
            FsmOwner.enabled = false;

            gameObject.layer = Layers.Enemy;

            UpdateHudSize();
            UpdateHudPosition();
        }

        public void UpdateHudPosition()
        {
            Vector3 position = Transform.position;
            position.y += Bounds.size.y * 0.75f * Size.ToFloat();

            Hud.Transform.position = position;
        }

        private void UpdateHudSize()
        {
            Vector2 size = Hud.RectTransform.sizeDelta;
            Hud.RectTransform.sizeDelta = size * Size.ToFloat();
        }

        public override void Tick()
        {
            base.Tick();

            UpdateHudPosition();
        }

        protected IReadOnlyList<EnemyActor> GetNearbyEnemies()
        {
            // IReadOnlyList<EnemyActor> enemies = Stage.Enemies;
            IReadOnlyList<EnemyActor> enemies = new List<EnemyActor>();
            var nearEnemies = new List<EnemyActor>();

            foreach (EnemyActor enemy in enemies)
            {
                if (enemy == this)
                {
                    continue;
                }

                nearEnemies.Add(enemy);
            }

            nearEnemies.Sort(delegate(EnemyActor x, EnemyActor y)
            {
                float distanceX = Vector2.Distance(x.Transform.position, Transform.position);
                float distanceY = Vector2.Distance(y.Transform.position, Transform.position);

                return distanceX.CompareTo(distanceY);
            });

            return nearEnemies;
        }

        public virtual void Commence()
        {
            var fsmOwner = GetComponent<FSMOwner>();
            fsmOwner.enabled = true;
        }

        protected override void OnStatusEffectsChanged(object sender, CombatEntityStatusEffectsChangedArgs e)
        {
            base.OnStatusEffectsChanged(sender, e);

            if (e.ChangeType == CombatEntityStatusEffectsChangeType.Add)
            {
                Hud.AddStatusEffect(e.AddedStatusEffect);
            }
            else if (e.ChangeType == CombatEntityStatusEffectsChangeType.Remove)
            {
                Hud.RemoveStatusEffect(e.RemovedStatusEffect);
            }
        }

        protected override void OnGamePaused(object sender, EventArgs e)
        {
            base.OnGamePaused(sender, e);

            FsmOwner.enabled = false;
        }

        protected override void OnGameResumed(object sender, EventArgs e)
        {
            base.OnGameResumed(sender, e);

            FsmOwner.enabled = true;
        }
    }
}
