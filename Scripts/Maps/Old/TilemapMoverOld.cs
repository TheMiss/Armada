using Armageddon.Games;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace Armageddon.Maps.Old
{
    public class TilemapMoverOld : GameContext
    {
        [SerializeField]
        private Camera m_mainCamera;

        [SerializeField]
        private Tilemap m_tilemap;

        public float Height;
        public Bounds LocalBounds;
        public Vector3Int dx;
        public BoundsInt cellBounds;

        private bool m_grabbed;

        private Vector3 m_grabPosition;
        // private Vector3 m_initialPosition;

        protected override void Awake()
        {
            base.Awake();

            CanTick = true;

            LocalBounds = m_tilemap.localBounds;
            dx = m_tilemap.size;
            cellBounds = m_tilemap.cellBounds;
        }

        // void OnGUI()
        // {
        //     Vector3 point = new Vector3();
        //     Event   currentEvent = Event.current;
        //     Vector2 mousePos = new Vector2();
        //
        //     // Get the mouse position from Event.
        //     // Note that the y position from Event is inverted.
        //     mousePos.x = currentEvent.mousePosition.x;
        //     mousePos.y = m_mainCamera.pixelHeight - currentEvent.mousePosition.y;
        //
        //     point = m_mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, m_mainCamera.nearClipPlane));
        //
        //     GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        //     GUILayout.Label("Screen pixels: " + m_mainCamera.pixelWidth + ":" + m_mainCamera.pixelHeight);
        //     GUILayout.Label("Mouse position: " + mousePos);
        //     GUILayout.Label("World position: " + point.ToString("F3"));
        //     GUILayout.EndArea();
        // }

        // public override void Tick()

        public void LateUpdate()
        {
            if (!m_grabbed)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        m_grabbed = true;
                        m_grabPosition = hit.point;
                    }
                }
            }

            if (m_grabbed)
            {
                Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 point = hit.point;

                    // mousePosition.z = 0;
                    // m_grabbedPosition.z = 0;
                    Vector3 delta = m_grabPosition - point;

                    Vector3 newPosition = m_grabPosition + delta;
                    newPosition.z = -10;
                    newPosition.x = 0;

                    // Vector3 screenToWorldPoint = m_mainCamera.ScreenToWorldPoint(newPosition);
                    // Debug.Log($"screenToWorldPoint = {screenToWorldPoint} :: Input.mousePosition = {point}");

                    // newPosition = hit.point;
                    // newPosition.z = -10;
                    // newPosition.x = 0;
                    Debug.Log($"delta = {delta}");
                    m_mainCamera.transform.position = newPosition;
                }


                // Vector3 point = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
                // var grabWorldPosition = m_mainCamera.ScreenToWorldPoint(m_grabbedPosition);
                // // mousePosition.z = 0;
                // // m_grabbedPosition.z = 0;
                // var delta = grabWorldPosition - point;
                // Vector3 newPosition = grabWorldPosition + delta;
                // newPosition.z = -10;
                // newPosition.x = 0;
                //
                // // Vector3 screenToWorldPoint = m_mainCamera.ScreenToWorldPoint(newPosition);
                // // Debug.Log($"screenToWorldPoint = {screenToWorldPoint} :: Input.mousePosition = {point}");
                //
                // Debug.Log($"delta = {delta}");
                // m_mainCamera.transform.position = newPosition;
            }

            if (m_grabbed)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    m_grabbed = false;
                }
            }
        }

        // public void LateUpdate()
        // {
        //     if (!m_grabbed)
        //     {
        //         if (Input.GetMouseButtonDown(0))
        //         {
        //             m_grabbed = true;
        //             // m_grabbedPosition = Input.mousePosition;
        //             m_grabbedPosition = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //             m_initialPosition = m_mainCamera.transform.position;
        //             // m_grabbedPosition = m_initialPosition;
        //         }
        //     }
        //     
        //     if (m_grabbed)
        //     {
        //         Vector3 point = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //         // mousePosition.z = 0;
        //         // m_grabbedPosition.z = 0;
        //         var delta = m_grabbedPosition - point;
        //
        //         Vector3 newPosition = m_initialPosition + delta;
        //         newPosition.z = -10;
        //         newPosition.x = 0;
        //
        //         // Vector3 screenToWorldPoint = m_mainCamera.ScreenToWorldPoint(newPosition);
        //         // Debug.Log($"screenToWorldPoint = {screenToWorldPoint} :: Input.mousePosition = {point}");
        //
        //         Debug.Log($"delta = {delta}");
        //         m_mainCamera.transform.position = newPosition;
        //         
        //
        //         // Vector3 point = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //         // var grabWorldPosition = m_mainCamera.ScreenToWorldPoint(m_grabbedPosition);
        //         // // mousePosition.z = 0;
        //         // // m_grabbedPosition.z = 0;
        //         // var delta = grabWorldPosition - point;
        //         // Vector3 newPosition = grabWorldPosition + delta;
        //         // newPosition.z = -10;
        //         // newPosition.x = 0;
        //         //
        //         // // Vector3 screenToWorldPoint = m_mainCamera.ScreenToWorldPoint(newPosition);
        //         // // Debug.Log($"screenToWorldPoint = {screenToWorldPoint} :: Input.mousePosition = {point}");
        //         //
        //         // Debug.Log($"delta = {delta}");
        //         // m_mainCamera.transform.position = newPosition;
        //     }
        //     
        //     if (m_grabbed)
        //     {
        //         if (Input.GetMouseButtonUp(0))
        //         {
        //             m_grabbed = false;
        //         }
        //     }
        // }

        // private void LateUpdate()
        // {
        //     
        //     if (m_grabbed)
        //     {
        //         Vector3 screenToWorldPoint = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //         Vector3 delta = m_grabbedPosition - screenToWorldPoint;
        //         Debug.Log($"screenToWorldPoint = {screenToWorldPoint} :: Input.mousePosition = {Input.mousePosition}");
        //         Vector3 newPosition = m_grabbedPosition + delta;
        //         newPosition.z = -10;
        //         newPosition.x = 0;
        //         m_mainCamera.transform.position = newPosition;
        //     }
        //
        //     if (m_grabbed)
        //     {
        //         if (Input.GetMouseButtonUp(0))
        //         {
        //             m_grabbed = false;
        //         }
        //     }
        // }

        // private void OnMouseDown()
        // {
        //     if (!m_grabbed)
        //     {
        //         m_grabbed = true;
        //         m_grabbedPosition = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //     }
        // }
        //
        // private void OnMouseDrag()
        // {
        //     if (m_grabbed)
        //     {
        //         Vector3 screenToWorldPoint = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //         Vector3 delta = m_grabbedPosition - screenToWorldPoint;
        //         Debug.Log($"screenToWorldPoint = {screenToWorldPoint} :: Input.mousePosition = {Input.mousePosition}");
        //         Vector3 newPosition = m_grabbedPosition + delta;
        //         newPosition.z = -10;
        //         newPosition.x = 0;
        //         m_mainCamera.transform.position = newPosition;
        //     }
        // }
        //
        // private void OnMouseUp()
        // {
        //     if (m_grabbed)
        //     {
        //         m_grabbed = false;
        //     }
        // }

        public void OnPointerDown(PointerEventData eventData)
        {
            Vector3 worldPosition = eventData.pointerCurrentRaycast.worldPosition;
            if (!m_grabbed)
            {
                m_grabbed = true;
                m_grabPosition = worldPosition;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_grabbed)
            {
                Vector3 worldPosition = eventData.pointerCurrentRaycast.worldPosition;
                //Vector3 delta = eventData.;
                // Debug.Log($"screenToWorldPoint = {screenToWorldPoint} :: Input.mousePosition = {Input.mousePosition}");
                Vector3 newPosition = worldPosition;
                Debug.Log($"screenToWorldPoint = {worldPosition} :: Input.mousePosition = {Input.mousePosition}");
                newPosition.z = -10;
                newPosition.x = 0;
                m_mainCamera.transform.position = newPosition;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_grabbed)
            {
                m_grabbed = false;
            }
        }
    }
}
