using System;
using Armageddon.Games;
using Armageddon.Worlds.Actors.Characters;
using DG.Tweening;
using Purity.Common;
using UnityEngine;

namespace Armageddon
{
    public enum PlayerControllerMode
    {
        FollowPointer,
        MoveByPointerDelta
    }

    [DisallowMultipleComponent]
    public class PlayerController : GameContext
    {
        [SerializeField]
        private PlayerControllerMode m_mode;

        [SerializeField]
        private PlayerCharacterBase m_characterBase;

        [SerializeField]
        private float m_moveToTargetDurationFollowPointer = 0.5f;

        [SerializeField]
        private float m_moveToTargetDurationMoveByPointerDelta = 0.25f;

        [HideInInspector]
        public Vector3 TargetPosition;

        [ShowInPlayMode]
        public Camera MainCamera;

        private bool m_isGrabbing;
        private int m_grabFingerId;
        private Vector3 m_grabPointerPosition;
        private Vector3 m_grabCharacterPosition;

        public PlayerCharacterBase CharacterBase => m_characterBase;

        public PlayerControllerMode Mode
        {
            get => m_mode;
            set => m_mode = value;
        }

        protected override void Awake()
        {
            base.Awake();

            RegisterService(this);
        }

        protected override void Start()
        {
        }

        public void SetFiring(bool value)
        {
            CharacterBase.MainCharacter.SetFiring(value);
        }

        public void SetMainCharacterActor(CharacterActor characterActor)
        {
            CharacterBase.EquipMainCharacter(characterActor);
        }

        public override void Tick()
        {
            if (!CharacterBase.IsReadyToPlay())
            {
                return;
            }

            // Handle native touch events
            foreach (Touch touch in Input.touches)
            {
                HandleTouch(touch.fingerId, touch.position, touch.phase);
            }

            // Simulate touch events from mouse events
            if (Input.touchCount == 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    HandleTouch(10, Input.mousePosition, TouchPhase.Began);
                }

                if (Input.GetMouseButton(0))
                {
                    HandleTouch(10, Input.mousePosition, TouchPhase.Moved);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    HandleTouch(10, Input.mousePosition, TouchPhase.Ended);
                }
            }
        }

        private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
        {
            switch (Mode)
            {
                case PlayerControllerMode.FollowPointer:
                    HandleTouchFollowPointerMode(touchFingerId, touchPosition, touchPhase);
                    break;
                case PlayerControllerMode.MoveByPointerDelta:
                    HandleTouchMoveByOffsetMode(touchFingerId, touchPosition, touchPhase);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleTouchFollowPointerMode(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
        {
            switch (touchPhase)
            {
                case TouchPhase.Began:
                case TouchPhase.Moved:
                    TargetPosition = MainCamera.ScreenToWorldPoint(touchPosition);
                    TargetPosition.z = 0.0f;
                    CharacterBase.Transform.DOMove(TargetPosition, m_moveToTargetDurationFollowPointer);
                    break;
                case TouchPhase.Ended:
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Canceled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(touchPhase), touchPhase, null);
            }
        }

        private void HandleTouchMoveByOffsetMode(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase)
        {
            switch (touchPhase)
            {
                case TouchPhase.Began:
                    if (!m_isGrabbing)
                    {
                        m_isGrabbing = true;
                        m_grabFingerId = touchFingerId;
                        m_grabPointerPosition = MainCamera.ScreenToWorldPoint(touchPosition);
                        m_grabCharacterPosition = CharacterBase.Transform.position;
                    }

                    break;
                case TouchPhase.Moved:
                    if (m_isGrabbing && m_grabFingerId == touchFingerId)
                    {
                        Vector3 targetPosition = MainCamera.ScreenToWorldPoint(touchPosition);
                        targetPosition.z = 0f;
                        Vector3 offset = targetPosition - m_grabPointerPosition;

                        TargetPosition = m_grabCharacterPosition + offset;

                        if (m_moveToTargetDurationMoveByPointerDelta == 0)
                        {
                            Vector3 position = CheckPosition(TargetPosition, CharacterBase.MainCharacter);
                            CharacterBase.Transform.position = position;
                        }
                        else
                        {
                            Vector3 position = CheckPosition(TargetPosition, CharacterBase.MainCharacter);
                            CharacterBase.Transform.DOMove(position, m_moveToTargetDurationMoveByPointerDelta);
                        }
                    }

                    break;
                case TouchPhase.Ended:
                    if (m_grabFingerId == touchFingerId)
                    {
                        m_isGrabbing = false;
                    }

                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Canceled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(touchPhase), touchPhase, null);
            }
        }

        private Vector3 CheckPosition(Vector3 position, CharacterActor characterActor)
        {
            Vector2 minBounds = characterActor.MinBounds;
            Vector2 maxBounds = characterActor.MaxBounds;

            if (position.x < minBounds.x)
            {
                position.x = minBounds.x;
            }

            if (position.x > maxBounds.x)
            {
                position.x = maxBounds.x;
            }

            if (position.y < minBounds.y)
            {
                position.y = minBounds.y;
            }

            if (position.y > maxBounds.y)
            {
                position.y = maxBounds.y;
            }

            return position;
        }

        public void Commence()
        {
            CanTick = true;
            SetTicks(true);
        }

        public void Stop()
        {
            CanTick = false;
            SetTicks(false);
        }

        private void SetTicks(bool canTick)
        {
            CharacterBase.MainCharacter.CanTick = canTick;
        }
    }
}
