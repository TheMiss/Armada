using Armageddon.Games;
using UnityEngine;

namespace Armageddon.Maps.Old
{
    public class ParallaxOld : GameContext
    {
        [SerializeField]
        private GameObject m_camera;

        [SerializeField]
        private float m_parallaxEffect;

        private float m_length;
        private float m_startPositionY;

        protected override void Start()
        {
            m_startPositionY = Transform.position.y;
            m_length = GetComponent<SpriteRenderer>().bounds.size.y;

            CanTick = true;
        }

        public override void Tick()
        {
            Vector3 cameraPosition = m_camera.transform.position;
            float cameraPositionY = cameraPosition.y * (1 - m_parallaxEffect);
            float offsetY = cameraPosition.y * m_parallaxEffect;

            Vector3 position = Transform.position;
            position = new Vector3(position.x, m_startPositionY + offsetY, position.z);
            Transform.position = position;

            if (cameraPositionY > m_startPositionY + m_length)
            {
                m_startPositionY += m_length;
            }
            else if (cameraPositionY < m_startPositionY - m_length)
            {
                m_startPositionY -= m_length;
            }
        }
    }
}
