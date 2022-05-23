using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Armageddon.Worlds.Actors
{
    public enum ScreenCollisionType
    {
        Bottom,
        Top,
        Left,
        Right
    }

    public class ActorHitScreenArgs : EventArgs
    {
        public ActorHitScreenArgs(ScreenCollisionType screenCollisionType)
        {
            ScreenCollisionType = screenCollisionType;
        }

        public ScreenCollisionType ScreenCollisionType { get; }
    }

    [DisallowMultipleComponent]
    public abstract class Actor : WorldContext
    {
        public static readonly IReadOnlyList<Actor> EmptyList = new List<Actor>();

        [SerializeField]
        private Collider2D m_collider2D;

        protected ContactFilter2D m_contactFilter2D;

        //private Transform m_transform;

        public Vector2 MinBounds { private set; get; }
        public Vector2 MaxBounds { private set; get; }
        public Vector2 Velocity { set; get; }
        public Collider2D Collider2D => m_collider2D;

        /// <summary>
        ///     See Unity's document. Collider2D.bounds is empty when Collider2D is disabled. Still empty even we try to enabled it
        ///     in the same frame. So store value here.
        /// </summary>
        public Bounds Bounds { private set; get; }

        public Vector2 Position
        {
            set => Transform.position = value;
            get => Transform.position;
        }

        public bool AllowPassLeftWall { set; get; } = true;
        public bool AllowPassRightWall { set; get; } = true;
        public bool AllowPassCeiling { set; get; } = true;
        public bool AllowPassGround { set; get; } = true;

        public bool AllowPassAll
        {
            get => AllowPassLeftWall && AllowPassRightWall && AllowPassCeiling && AllowPassGround;

            set
            {
                AllowPassLeftWall = value;
                AllowPassRightWall = value;
                AllowPassCeiling = value;
                AllowPassGround = value;
            }
        }

        // It's useful when some states want to control the actor's movement. If that's the case, set this to false
        public bool AutoHandleMovement { set; get; } = false;

        public bool ApplyGravity { set; get; } = true;

        public float DamageTextDirectionX { get; set; } = 1;

        protected override void Awake()
        {
            base.Awake();

            m_contactFilter2D = m_contactFilter2D.NoFilter();

            CalculateBounds();

            // CanTick = true;

            AllowPassAll = true;
        }

        protected override void Start()
        {
        }

        protected override void OnEnable()
        {
            // Don't enable m_collider2D right here away.
            // Do it at Initialize()

            base.OnEnable();

            m_collider2D.enabled = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // UPDATE: Not sure if below is still true, I haven't tested it.
            // Why do this and here?
            // Well, let met explain, sometimes.... later...
            // UPDATE: if not disabled then the hit collision still occurs.
            m_collider2D.enabled = false;
        }

        /// <summary>
        ///     This must be called after rescale the actor
        /// </summary>
        private void CalculateBounds()
        {
            Camera camera = Camera.main;
            float halfHeight = camera.orthographicSize;
            float halfWidth = camera.aspect * halfHeight;

            float minScreenX = -halfWidth;
            float maxScreenX = halfWidth;
            float minScreenY = -halfHeight;
            float maxScreenY = halfHeight;

            Vector3 extents = m_collider2D.bounds.extents;
            float halfColliderWidth = extents.x;
            float halfColliderHeight = extents.y;

            var minBounds = new Vector2();
            var maxBounds = new Vector2();
            minBounds.x = minScreenX + halfColliderWidth;
            maxBounds.x = maxScreenX - halfColliderWidth;
            minBounds.y = minScreenY + halfColliderHeight;
            maxBounds.y = maxScreenY - halfColliderHeight;

            MinBounds = minBounds;
            MaxBounds = maxBounds;

            Bounds = Collider2D.bounds;
        }

        public override string ToString()
        {
            return name;
        }

        protected void SetScale(AnimationCurve scaleAnimationCurve)
        {
            float scale = scaleAnimationCurve.Evaluate(Random.value);

            Transform.localScale = new Vector3(scale, scale, scale);

            CalculateBounds();
        }

        public override void Tick()
        {
            if (AutoHandleMovement)
            {
                HandleMovement();
            }

            HandleCollision();
            HandleWhenOffScreen();
        }

        protected virtual void HandleMovement()
        {
            Vector2 position = Transform.position;
            position += Velocity * Time.deltaTime;

            Transform.position = position;
        }

        protected virtual void HandleCollision()
        {
            Vector2 position = Transform.position;

            if (position.y < MinBounds.y)
            {
                if (!AllowPassGround)
                {
                    position.y = MinBounds.y;

                    var e = new ActorHitScreenArgs(ScreenCollisionType.Bottom);
                    OnScreenCollisionHit(e);
                }
            }
            else if (position.y > MaxBounds.y)
            {
                if (!AllowPassCeiling)
                {
                    position.y = MaxBounds.y;

                    var e = new ActorHitScreenArgs(ScreenCollisionType.Top);
                    OnScreenCollisionHit(e);
                }
            }

            if (position.x < MinBounds.x)
            {
                if (!AllowPassLeftWall)
                {
                    position.x = MinBounds.x;

                    var e = new ActorHitScreenArgs(ScreenCollisionType.Left);
                    OnScreenCollisionHit(e);
                }
            }
            else if (position.x > MaxBounds.x)
            {
                if (!AllowPassRightWall)
                {
                    position.x = MaxBounds.x;

                    var e = new ActorHitScreenArgs(ScreenCollisionType.Right);
                    OnScreenCollisionHit(e);
                }
            }

            Transform.position = position;
        }

        protected virtual void OnScreenCollisionHit(ActorHitScreenArgs e)
        {
        }

        protected virtual void HandleWhenOffScreen()
        {
            Vector3 position = Transform.position;

            //Bounds bounds = Collider2D.bounds;
            float sizeX = Bounds.size.x;
            float sizeY = Bounds.size.y;

            if (position.x < MinBounds.x - sizeX || position.x > MaxBounds.x + sizeX ||
                position.y < MinBounds.y - sizeY || position.y > MaxBounds.y + sizeY)
            {
                OnOffScreen();
            }
        }

        public virtual void OnOffScreen()
        {
        }

        public void InverseVelocity(bool x, bool y)
        {
            if (x)
            {
                Velocity = new Vector2(-Velocity.x, Velocity.y);
            }

            if (y)
            {
                Velocity = new Vector2(Velocity.x, -Velocity.y);
            }
        }

        public virtual bool IsFullyVisibleInScreen()
        {
            float sizeX = Collider2D.bounds.size.x;
            float sizeY = Collider2D.bounds.size.y;

            if (Transform.position.x > MaxBounds.x)
            {
                return false;
            }

            if (Transform.position.x < MinBounds.x)
            {
                return false;
            }

            if (Transform.position.y < MinBounds.y)
            {
                return false;
            }

            if (transform.position.y > MaxBounds.y)
            {
                return false;
            }

            return true;
        }

        public virtual bool IsPositionValidToGoTo(Vector2 position)
        {
            bool isValid = true;

            if (position.y < MinBounds.y)
            {
                if (!AllowPassGround)
                {
                    isValid = false;
                }
            }
            else if (position.y > MaxBounds.y)
            {
                if (!AllowPassCeiling)
                {
                    isValid = false;
                }
            }

            if (position.x < MinBounds.x)
            {
                if (!AllowPassLeftWall)
                {
                    isValid = false;
                }
            }
            else if (position.x > MaxBounds.x)
            {
                if (!AllowPassRightWall)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        public bool IsPointInsideBounds(Vector2 point)
        {
            bool isInside = true;

            if (point.y < MinBounds.y)
            {
                isInside = false;
            }

            if (point.y > MaxBounds.y)
            {
                isInside = false;
            }

            if (point.x < MinBounds.x)
            {
                isInside = false;
            }

            if (point.x > MaxBounds.x)
            {
                isInside = false;
            }

            return isInside;
        }
    }
}
