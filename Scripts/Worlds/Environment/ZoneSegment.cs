using Purity.Common;
using UnityEngine;

namespace Armageddon.Worlds.Environment
{
    public class ZoneSegment : WorldContext
    {
        // For debug purpose
        [ShowInPlayMode]
        private GameObject m_frameObject;

        protected override void Awake()
        {
            base.Awake();

            m_frameObject = Transform.GetChild(0).gameObject;
        }

        public void SetShowFrameDisplay(bool show)
        {
            if (m_frameObject == null)
            {
                return;
            }

            m_frameObject.SetActive(show);
        }
    }
}
