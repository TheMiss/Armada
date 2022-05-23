using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Worlds.Environment
{
    public class ZoneUnit : WorldContext
    {
        [SerializeField]
        private bool m_rotate;

        [ShowIf(nameof(m_rotate))]
        [SerializeField]
        private float m_rotateSpeed;

        public override void Tick()
        {
            if (m_rotate)
            {
                Transform.Rotate(0, 0, m_rotateSpeed * Time.deltaTime);
            }
        }
    }
}
