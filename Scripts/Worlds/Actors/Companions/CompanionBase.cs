using UnityEngine;

namespace Armageddon.Worlds.Actors.Companions
{
    public class CompanionBase : WorldContext
    {
        [SerializeField]
        private Transform m_leftTransform;

        [SerializeField]
        private Transform m_rightTransform;

        public Transform LeftTransform => m_leftTransform;

        public Transform RightTransform => m_rightTransform;
    }
}
