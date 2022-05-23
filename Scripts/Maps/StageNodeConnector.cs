using Purity.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Maps
{
    public class StageNodeConnector : FastMonoBehaviour
    {
        [ReadOnly]
        [SerializeField]
        private StageNode m_nextNode;

        [ReadOnly]
        [SerializeField]
        private LineRenderer m_unfilledLine;

        [ReadOnly]
        [SerializeField]
        private LineRenderer m_filledLine;

        public StageNode NextNode => m_nextNode;

        public LineRenderer UnfilledLine => m_unfilledLine;

        public LineRenderer FilledLine => m_filledLine;

#if UNITY_EDITOR
        public void _SetUnfilledLine(LineRenderer unfilledLine)
        {
            m_unfilledLine = unfilledLine;
        }

        public void _SetNextNode(StageNode nextNode)
        {
            m_nextNode = nextNode;
        }
#endif
    }
}
