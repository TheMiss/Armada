using System;
using System.Collections;
using Armageddon.Backend.Payloads;
using Armageddon.Extensions;
using Armageddon.Worlds.Actors;
using Armageddon.Worlds.Actors.Characters;
using Armageddon.Worlds.Actors.Enemies;
using Purity.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Armageddon.Worlds.Misc
{
    public class DropActor : Actor
    {
        [ShowInPlayMode]
        public static float MoveToTargetSpeed = 100;

        [ShowInPlayMode]
        public static float DropDecelerationRate = 0.965f;

        [SerializeField]
        private SpriteRenderer m_innerFrame;

        [SerializeField]
        private SpriteRenderer m_outerFrame;

        [SerializeField]
        private SpriteRenderer m_icon;

        private Action<DropActor> m_targetHitCallback;

        private DropHud m_hud;
        private Vector2 m_startPosition;
        private Vector2 m_endPosition;
        private Vector2 m_acceleration;
        private float m_targetSpeed;

        public Transform Target { get; private set; }

        public void Initialize(StageDrop stageDrop, Vector2 startPosition, Vector2 endPosition,
            CharacterActor targetActor)
        {
            Target = targetActor.Transform;
            Transform.position = startPosition;

            DropHud dropHud = World.DropHudPool.Get();
            dropHud.Owner = this;
            dropHud.Transform.position = startPosition;
            dropHud.SetText(stageDrop, startPosition);
            m_hud = dropHud;
            m_startPosition = startPosition;
            m_endPosition = endPosition;

            if (stageDrop.Type == DropType.Item)
            {
                StageDropItem item = stageDrop.Item;
                m_icon.sprite = item.Sheet.Icon;
            }

            Vector2 toTarget = m_endPosition - m_startPosition;
            toTarget.Normalize();
            float speed = Random.Range(World.MinRandomDropSpeed, World.MaxRandomDropSpeed);

            Velocity = toTarget * speed;
            m_targetSpeed = (Velocity * 0.2f).sqrMagnitude;
            CanTick = true;
            AutoHandleMovement = true;
            AllowPassAll = false;

            StopAllCoroutines();
        }

        protected override void OnScreenCollisionHit(ActorHitScreenArgs e)
        {
            if (e.ScreenCollisionType == ScreenCollisionType.Top ||
                e.ScreenCollisionType == ScreenCollisionType.Bottom)
            {
                Vector2 velocity = Velocity;
                velocity.y *= -1;
                Velocity = velocity;
            }
            else if (e.ScreenCollisionType == ScreenCollisionType.Left ||
                     e.ScreenCollisionType == ScreenCollisionType.Right)
            {
                Vector2 velocity = Velocity;
                velocity.x *= -1;
                Velocity = velocity;
            }
        }

        protected override void HandleMovement()
        {
            if (Velocity.sqrMagnitude > m_targetSpeed)
            {
                Velocity *= DropDecelerationRate;
            }
            else
            {
                Velocity.Scale(new Vector2(m_targetSpeed, m_targetSpeed));
            }

            base.HandleMovement();
        }

        // [Button]
        public void Execute(Action<DropActor> targetHitCallback)
        {
            m_targetHitCallback = targetHitCallback;

            m_hud.Transform.PlayScaleAnimation();
            // Transform.DOMove(m_endPosition, MonoBehaviourExtensions.ScaleAnimationDuration + 1);
            Transform.PlayScaleAnimation(completeCallback: () => { StartCoroutine(MoveToTarget()); });

            // const float originalScale = 0.01f;
            // const float startScale = originalScale * 0.3f;
            // const float endValueStep1 = originalScale + 0.0015f;
            // float halfDuration = UISettings.AddBalanceAnimationDuration * 0.5f;
            // Transform.localScale = new Vector2(startScale, startScale);
            // Transform.DOScale(endValueStep1, halfDuration).OnComplete(
            //     () => Transform.DOScale(originalScale, halfDuration).OnComplete(
            //         () => 
            //         {
            //             StartCoroutine(MoveToTarget());
            //             // Debug.Log("I'm done");
            //         }));

            // CanTick = true;
        }

        private IEnumerator MoveToTarget()
        {
            yield return new WaitForSeconds(1);

            m_hud.gameObject.SetActive(false);

            bool isDone = false;

            while (!isDone)
            {
                Vector2 thisPosition = Transform.position;
                Vector2 targetPosition = Target.position;
                Vector2 toTarget = targetPosition - thisPosition;
                toTarget.Normalize();

                Vector2 velocity = toTarget * MoveToTargetSpeed;
                thisPosition += velocity * Time.deltaTime;
                Transform.position = thisPosition;

                float distance = Vector2.Distance(thisPosition, targetPosition);
                if (distance < 1)
                {
                    m_targetHitCallback?.Invoke(this);
                    isDone = true;
                }

                yield return null;
            }

            World.ObjectPool.AddObject(this);
            World.DropHudPool.AddObject(m_hud);
        }
    }
}
