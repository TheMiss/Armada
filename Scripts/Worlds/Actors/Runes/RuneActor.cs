using UnityEngine;

namespace Armageddon.Worlds.Actors.Runes
{
    public abstract class RuneActor : Actor
    {
        [SerializeField]
        private Vector2 m_startingVelocity;

        public Vector2 StartingVelocity => m_startingVelocity;

        protected override void Awake()
        {
            base.Awake();

            AllowPassLeftWall = false;
            AllowPassRightWall = false;
            AllowPassGround = false;
            AllowPassCeiling = false;
        }

        protected override void HandleMovement()
        {
            Velocity += Physics2D.gravity * Time.deltaTime;

            Transform.Translate(Velocity * Time.deltaTime);
        }

        /// <summary>
        ///     Use this instead of Start().
        /// </summary>
        public virtual void Spawn()
        {
            Collider2D.enabled = true;
        }
    }
}
