using System.Collections.Generic;
using System.Linq;
using Armageddon.Externals.OdinInspector;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Worlds.Environment
{
    public class Zone : WorldContext
    {
        [SerializeField]
        private Transform m_segmentsTransform;

        [SerializeField]
        private Transform m_activeSegmentsTransform;

        [ShowInPlayMode]
        private readonly float m_moveSpeed = 2;

        [ShowInPlayMode]
        private readonly List<ZoneSegment> m_activeSegments = new();

        [ShowInPlayMode]
        private List<ZoneSegment> m_segments;

        private float m_offsetY;

        public void Initialize()
        {
            m_segments = GetComponentsInChildren<ZoneSegment>().ToList();

            foreach (ZoneSegment segment in m_segments)
            {
                segment.gameObject.SetActive(false);
            }

            // m_activeSegmentsTransform = new GameObject("ActiveSegments").transform;
            // m_activeSegmentsTransform.SetParent(Transform);

            m_offsetY = CalculateOffsetY();
            float y = -m_offsetY;

            for (int i = 0; i < 3; i++)
            {
                ZoneSegment segment = GetRandomSegment();
                segment.Transform.SetParent(m_activeSegmentsTransform);
                segment.Transform.localPosition = new Vector3(0, y, 0);

                m_activeSegments.Add(segment);

                y += m_offsetY;
            }

            HideDebug();
            CanTick = true;
        }

        private float CalculateOffsetY()
        {
            // Assume that m_segments.Count will always be >= 2
            return Mathf.Abs(m_segments[1].Transform.localPosition.y - m_segments[0].Transform.localPosition.y);
        }

        private ZoneSegment GetRandomSegment()
        {
            int randomIndex = Random.Range(0, m_segments.Count);
            ZoneSegment segment = m_segments[randomIndex];
            segment.gameObject.SetActive(true);

            m_segments.Remove(segment);

            return segment;
        }

        private void ReturnToPool(ZoneSegment segment)
        {
            segment.Transform.SetParent(m_segmentsTransform);
            segment.gameObject.SetActive(false);

            m_segments.Add(segment);
            m_activeSegments.Remove(segment);
        }

        public override void Tick()
        {
            Vector3 translation = new Vector3(0, -m_moveSpeed) * Time.deltaTime;
            ZoneSegment disappearingSegment = null;

            foreach (ZoneSegment segment in m_activeSegments)
            {
                segment.Transform.Translate(translation);

                Vector3 position = segment.Transform.localPosition;
                if (position.y < -m_offsetY * 2)
                {
                    disappearingSegment = segment;
                }
            }

            if (disappearingSegment != null)
            {
                ZoneSegment segment = GetRandomSegment();
                segment.Transform.SetParent(m_activeSegmentsTransform);
                segment.Transform.localPosition = new Vector3(0, m_offsetY, 0);

                m_activeSegments.Add(segment);

                ReturnToPool(disappearingSegment);
            }
        }

        [Button]
        [GUIColorDefaultButton]
        private void ShowDebug()
        {
            SetShowDebug(true);
        }

        [Button]
        [GUIColorDefaultButton]
        private void HideDebug()
        {
            SetShowDebug(false);
        }

        private void SetShowDebug(bool value)
        {
            foreach (ZoneSegment segment in m_segments)
            {
                segment.SetShowFrameDisplay(value);
            }

            foreach (ZoneSegment segment in m_activeSegments)
            {
                segment.SetShowFrameDisplay(value);
            }
        }
    }
}
