using System.Collections.Generic;
using System.Linq;
using Armageddon.Backend.Payloads;
using Armageddon.Games;
using Armageddon.Localization;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace Armageddon.Maps
{
    public class MapNode : GameContext
    {
        [Required]
        [SerializeField]
        private Tilemap m_tilemap;

        [Required]
        [SerializeField]
        private TilemapMover m_tilemapMover;

        [Required]
        [SerializeField]
        private int m_mapId;

        [Required]
        [SerializeField]
        private GameObject m_mainStagesObject;

        [Required]
        [SerializeField]
        private GameObject m_specialStagesObject;

        [Required]
        [SerializeField]
        private List<StageNode> m_stageNodes;

        [Required]
        [SerializeField]
        private List<StageNode> m_mainStageNodes;

        [Required]
        [SerializeField]
        private List<StageNode> m_specialStageNodes;

        private Dictionary<Vector3Int, StageNode> m_stageNodeDictionary;

        public Tilemap Tilemap => m_tilemap;

        public TilemapMover TilemapMover => m_tilemapMover;

        public IReadOnlyList<StageNode> StageNodes => m_stageNodes;

        public IReadOnlyList<StageNode> MainStageNodes => m_mainStageNodes;

        public List<StageNode> SpecialStageNodes => m_specialStageNodes;

        public int MapId => m_mapId;

        public UnityEvent<StageNode> StageButtonClicked { set; get; } = new();

        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (StageNode stageNode in StageNodes)
            {
                stageNode.StageButton.onClick.AddListener(() => StageButtonClicked?.Invoke(stageNode));
            }
        }

        protected override void OnDisable()
        {
            foreach (StageNode stageNode in StageNodes)
            {
                stageNode.StageButton.onClick.RemoveAllListeners();
            }

            base.OnDisable();
        }

#if UNITY_EDITOR
        public Dictionary<Vector3Int, StageNode> GetStageNodeDictionary(bool rebuildDictionary)
        {
            if (m_stageNodeDictionary != null && !rebuildDictionary)
            {
                return m_stageNodeDictionary;
            }

            List<StageNode> mainStageNodes = GetComponentsInChildren<StageNode>().ToList();
            m_stageNodeDictionary = new Dictionary<Vector3Int, StageNode>();

            foreach (StageNode mainStageNode in mainStageNodes)
            {
                Vector3 pointPosition = mainStageNode.Transform.position;
                Vector3Int cell = m_tilemap.WorldToCell(pointPosition);

                m_stageNodeDictionary.Add(cell, mainStageNode);
            }

            return m_stageNodeDictionary;
        }

        [Button]
        public void ScanStageNodes()
        {
            m_mainStageNodes = m_mainStagesObject.GetComponentsInChildren<StageNode>().ToList();
            m_specialStageNodes = m_specialStagesObject.GetComponentsInChildren<StageNode>().ToList();

            m_stageNodes = new List<StageNode>();
            m_stageNodes.AddRange(m_mainStageNodes);
            m_stageNodes.AddRange(m_specialStageNodes);

            foreach (StageNode stageNode in StageNodes)
            {
                stageNode._SetMapNode(this);
            }
        }

        [Button]
        public void SnapStageNodes()
        {
            foreach (StageNode stageNode in StageNodes)
            {
                Vector3Int cell = Tilemap.WorldToCell(stageNode.Transform.position);
                Vector3 worldPosition = Tilemap.CellToWorld(cell);

                stageNode.Transform.position = worldPosition;
            }

            // foreach (StageNode mainStageNode in MainStageNodes)
            // {
            //     Vector3Int cell = Tilemap.WorldToCell(mainStageNode.Transform.position);
            //     Vector3 worldPosition = Tilemap.CellToWorld(cell);
            //
            //     mainStageNode.Transform.position = worldPosition;
            // }
            //
            // foreach (StageNode specialStageNode in SpecialStageNodes)
            // {
            //     Vector3Int cell = Tilemap.WorldToCell(specialStageNode.Transform.position);
            //     Vector3 worldPosition = Tilemap.CellToWorld(cell);
            //
            //     specialStageNode.Transform.position = worldPosition;
            // }
        }

        [Button]
        public void FillLineRandom()
        {
            // int stageIndex = Random.Range(0, m_mainStageNodes.Count);
            // StageNode mainStageNode = m_mainStageNodes[stageIndex];
            // int lineIndex = mainStageNode.LineIndex;
            //
            // Debug.Log(
            //     $"stageIndex = {stageIndex}, mainStageNode = {mainStageNode.StageNumber}, lineIndex = {lineIndex}");
            //
            // int maxLine = lineIndex + 1;
            // m_filledLine.positionCount = maxLine;
            //
            // for (int i = 0; i < maxLine; i++)
            // {
            //     Vector3 point = m_unfilledLine.GetPosition(i);
            //
            //     m_filledLine.SetPosition(i, point);
            // }
        }

        public void RenameStageNodes()
        {
            int stageId = 1;

            foreach (StageNode mainStageNode in MainStageNodes)
            {
                string text = Lexicon.StageNumber(stageId);
                mainStageNode.SetText(text);
                mainStageNode._SetStageType(StageType.Main);
                mainStageNode._SetStageId(stageId);
                mainStageNode.name = $"MainStage{stageId++}";
            }

            int eventId = 1;

            foreach (StageNode specialStageNode in SpecialStageNodes)
            {
                string text = $"SpecialStage{eventId}";
                specialStageNode.SetText(text);
                specialStageNode._SetStageType(StageType.Special);
                specialStageNode._SetStageId(eventId);
                specialStageNode.name = $"SpecialStage{eventId++}";
            }
        }

        public void RefreshStageNodeLinks()
        {
            // Build only once, boys, this is in editor. No need to optimize, but still do.
            GetStageNodeDictionary(true);

            foreach (StageNode stageNode in StageNodes)
            {
                stageNode.RefreshLinkedNodes(false);
            }
        }

        // [Button]
        // [GUIColorDefault]
        public void ProcessStageNodes()
        {
            ScanStageNodes();
            SnapStageNodes();
            RenameStageNodes();
            RefreshStageNodeLinks();
        }
#endif
    }
}
