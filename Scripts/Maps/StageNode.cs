using System.Collections.Generic;
using System.Linq;
using Armageddon.Backend.Payloads;
using Armageddon.Externals.OdinInspector;
using Armageddon.UI.Base;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Armageddon.Maps
{
    public class StageNode : Widget
    {
        [ReadOnly]
        [Optional]
        [SerializeField]
        private MapNode m_mapNode;

        [ReadOnly]
        [SerializeField]
        private StageType m_stageType;

        [ReadOnly]
        [SerializeField]
        private int m_stageId;

        [ReadOnly]
        [SerializeField]
        private List<StageNodeConnector> m_connectors;

        [SerializeField]
        private TextMeshProUGUI m_stageText;

        [SerializeField]
        private Button m_stageButton;

        public MapNode MapNode => m_mapNode;

        public StageType StageType => m_stageType;

        public int MapId => m_mapNode.MapId;

        public int StageId => m_stageId;

        public IReadOnlyList<StageNodeConnector> Connectors => m_connectors;

        public Button StageButton => m_stageButton;

        public void SetText(string stageText)
        {
            m_stageText.text = stageText;
        }

#if UNITY_EDITOR

        [Button]
        [GUIColorDefaultButton]
        private void RefreshLinkedNodes()
        {
            RefreshLinkedNodes(true);
        }

        /// <summary>
        ///     Also snap line to a cell of Tilemap
        /// </summary>
        public void RefreshLinkedNodes(bool rebuildStageNodeByCell)
        {
            Tilemap tilemap = MapNode.Tilemap;

            StageNodeConnector[] connectors = GetComponentsInChildren<StageNodeConnector>();
            Dictionary<Vector3Int, StageNode> nodes = MapNode.GetStageNodeDictionary(rebuildStageNodeByCell);

            // Do one-way link connection only...
            foreach (StageNodeConnector connector in connectors)
            {
                // Transform linePlotterTransform = connector.Transform.Find("LinePlotterEditor");
                // var filledLine = connector.Transform.Find("FilledLine").GetComponent<LineRenderer>();
                // connector._SetField("m_filledLine", filledLine);

                var unfilledLine = connector.Transform.Find("UnfilledLine").GetComponent<LineRenderer>();
                connector._SetUnfilledLine(unfilledLine);
                int pointCount = connector.UnfilledLine.positionCount;

                for (int i = 0; i < pointCount; i++)
                {
                    Vector3 pointLocalPosition = unfilledLine.GetPosition(i);
                    Vector3 pointPosition = connector.Transform.TransformPoint(pointLocalPosition);

                    // Snap the line to grid cell
                    Vector3Int cell = tilemap.WorldToCell(pointPosition);
                    Vector3 cellToWorldPosition = tilemap.CellToWorld(cell);

                    pointLocalPosition = connector.Transform.InverseTransformPoint(cellToWorldPosition);
                    connector.UnfilledLine.SetPosition(i, pointLocalPosition);

                    if (i == pointCount - 1)
                    {
                        if (!nodes.TryGetValue(cell, out StageNode stageNode))
                        {
                            Debug.LogWarning(
                                $"The last point of {connector.name} ({connector.Transform.parent.name}) " +
                                "doesn't link to any stage node.");
                        }

                        if (stageNode != null)
                        {
                            connector._SetNextNode(stageNode);
                        }
                    }
                }
            }

            m_connectors = connectors.ToList();
        }

        public void _SetMapNode(MapNode mapNode)
        {
            m_mapNode = mapNode;
            EditorUtility.SetDirty(this);
        }

        public void _SetStageType(StageType stageType)
        {
            m_stageType = stageType;
            EditorUtility.SetDirty(this);
        }

        public void _SetStageId(int stageId)
        {
            m_stageId = stageId;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
