using Armageddon.Worlds.Actors.Characters;
using DG.Tweening;
using Purity.Common.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Companions
{
    public enum GuardianType
    {
        Fyrest
    }

    public class CompanionActor : CharacterActor
    {
        [ShowInInspector]
        public static readonly float MoveToTargetDuration = 0.35f;

        [ShowInInspector]
        private float m_offsetRotation;

        [ShowInInspector]
        [HideInEditorMode]
        public Transform FollowingTarget { get; set; }

        public override void Initialize(CharacterDescriptor descriptor)
        {
            base.Initialize(descriptor);

            AutoCastSkills = false;
            // Damager.enabled = false;

            CanTick = true;
        }

        public override void Tick()
        {
            base.Tick();

            // UpdateLocalMovement();
            // MoveTo(ProtectedHero.ProtectorPosition);

            Vector3 targetPosition = FollowingTarget.position;

            targetPosition.z = 0.0f;
            Transform.DOMove(targetPosition, MoveToTargetDuration);
        }

        public void UpdateLocalMovement()
        {
            // m_offsetRotation += RotateSpeed * Time.deltaTime;
            Vector2 offset = Vector2.right.Rotate(m_offsetRotation) * 0.6f;

            Transform.localPosition = offset;
        }

        // public void MoveTo(Vector2 targetPosition)
        // {
        //     m_offsetRotation += 90 * Time.deltaTime;
        //
        //     Vector2 offset = Vector2.right.Rotate(m_offsetRotation) * 0.6f;
        //
        //     targetPosition.x += offset.x;
        //     targetPosition.y += offset.y / 3.0f;
        //
        //     Vector3 position = transform.position;
        //     if (position.x < targetPosition.x)
        //     {
        //         position.x += MoveSpeed * Time.deltaTime;
        //
        //         if (position.x > targetPosition.x)
        //         {
        //             position.x = targetPosition.x;
        //         }
        //     }
        //     else if (position.x > targetPosition.x)
        //     {
        //         position.x -= MoveSpeed * Time.deltaTime;
        //
        //         if (position.x < targetPosition.x)
        //         {
        //             position.x = targetPosition.x;
        //         }
        //     }
        //
        //     if (position.y < targetPosition.y)
        //     {
        //         position.y += MoveSpeed * Time.deltaTime;
        //
        //         if (position.y > targetPosition.y)
        //         {
        //             position.y = targetPosition.y;
        //         }
        //     }
        //     else if (position.y > targetPosition.y)
        //     {
        //         position.y -= MoveSpeed * Time.deltaTime;
        //
        //         if (position.y < targetPosition.y)
        //         {
        //             position.y = targetPosition.y;
        //         }
        //     }
        //
        //     if (position.x < MinBounds.x)
        //     {
        //         position.x = MinBounds.x;
        //     }
        //     else if (position.x > MaxBounds.x)
        //     {
        //         position.x = MaxBounds.x;
        //     }
        //
        //     transform.position = position;
        //     //transform.position = position + (Vector3)offset;
        // }
    }
}
