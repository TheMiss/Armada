using Armageddon.Games;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace Armageddon.Maps
{
    public class TilemapMover : GameContext
    {
        [SerializeField]
        private Transform m_mapRootTransform;

        private Vector3 m_acceleration;

        [HideInEditorMode]
        [ShowInInspector]
        private float m_decelerationRate = 0.035f;

        private float m_direction;

        private EventSystem m_eventSystem;

        private bool m_grabbed;
        private Vector3 m_grabMouseWorldPosition;
        private Vector3 m_grabTilemapPosition;
        private Camera m_mainCamera;

        private Tilemap m_tilemap;

        private Vector3 m_velocity;

        public bool CanDrag { set; get; } = true;

        protected override void Awake()
        {
            base.Awake();

            m_mainCamera = Camera.main;
            m_tilemap = m_mapRootTransform.GetComponentInChildren<Tilemap>();

            m_eventSystem = FindObjectOfType<EventSystem>();

            CanTick = true;
        }

        public override void Tick()
        {
            if (!CanDrag)
            {
                return;
            }

            if (!m_grabbed)
            {
                if (m_eventSystem.currentSelectedGameObject == null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        m_grabbed = true;
                        m_grabMouseWorldPosition =
                            m_mainCamera.ScreenToWorldPoint(
                                new Vector3(Input.mousePosition.x, -Input.mousePosition.y, -10f));
                        m_grabTilemapPosition = m_mapRootTransform.transform.position;

                        m_velocity = Vector3.zero;
                        m_acceleration = Vector3.zero;
                    }
                }
            }

            Vector3 delta = Vector3.zero;

            if (m_grabbed)
            {
                Vector3 point =
                    m_mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, -Input.mousePosition.y, -10f));
                delta = m_grabMouseWorldPosition - point;

                Vector3 position = m_grabTilemapPosition + delta;
                position.z = 0;
                position.x = 0;

                // Debug.Log($"delta = {delta}");
                m_mapRootTransform.transform.position = position;
            }

            if (m_grabbed)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    m_grabbed = false;

                    if (delta.magnitude > 0.1f)
                    {
                        m_velocity = delta;
                        m_velocity.x = 0.0f;

                        if (delta.y > 0)
                        {
                            m_direction = -1;
                        }
                        else if (delta.y < 0)
                        {
                            m_direction = 1;
                        }

                        m_acceleration = new Vector3(0, m_decelerationRate * m_direction, 0);
                    }
                }
            }

            //if (m_velocity.magnitude > 0.1f)
            if (!m_grabbed)
            {
                if (m_direction > 0)
                {
                    if (m_velocity.y > 0)
                    {
                        m_velocity.y = 0;
                        m_acceleration = Vector3.zero;
                    }
                }
                else if (m_direction < 0)
                {
                    if (m_velocity.y < 0)
                    {
                        m_velocity.y = 0;
                        m_acceleration = Vector3.zero;
                    }
                }

                m_velocity += m_acceleration;
            }

            // m_velocity *= m_decelerationRate;
            Transform tilemapTransform = m_mapRootTransform.transform;
            Vector3 tilemapPosition = tilemapTransform.position + m_velocity * Time.deltaTime;
            tilemapPosition.x = 0;
            tilemapPosition.z = 0;

            Bounds bounds = m_tilemap.localBounds;
            float boundY = bounds.center.y * 2f;
            const float offsetY = 0.6f;
            if (tilemapPosition.y < -boundY + offsetY)
            {
                tilemapPosition.y = -boundY + offsetY;
            }
            else if (tilemapPosition.y > 0 - offsetY)
            {
                tilemapPosition.y = 0 - offsetY;
            }

            tilemapTransform.position = tilemapPosition;
        }
    }
}
