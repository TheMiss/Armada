using Armageddon.Games;
using UnityEngine;

namespace Armageddon.Maps
{
    public class Parallax : GameContext
    {
        [SerializeField]
        private GameObject m_referenceObject;

        [SerializeField]
        private BoxCollider2D m_referenceCollider;

        [SerializeField]
        private bool m_referenceObjectIsCamera;

        [SerializeField]
        private float m_parallaxEffect;

        // [ShowInInspector]
        private Vector2 m_boundSize;

        // [ShowInInspector]
        private float m_startPositionY;

        protected override void Start()
        {
            m_startPositionY = Transform.position.y;

            m_boundSize = m_referenceCollider.bounds.size;
            CanTick = true;
        }

        public override void Tick()
        {
            Vector3 refPosition = m_referenceObject.transform.position;
            float parallaxBase = 0;

            if (m_referenceObjectIsCamera)
            {
                parallaxBase = 1;
            }

            float refPositionY = refPosition.y * (parallaxBase - m_parallaxEffect);
            float offsetY = refPosition.y * m_parallaxEffect;

            Vector3 position = Transform.position;
            position = new Vector3(position.x, m_startPositionY + offsetY, position.z);
            Transform.position = position;

            if (refPositionY > m_startPositionY + m_boundSize.y)
            {
                m_startPositionY += m_boundSize.y;
            }
            else if (refPositionY < m_startPositionY - m_boundSize.y)
            {
                m_startPositionY -= m_boundSize.y;
            }
        }
    }
}
