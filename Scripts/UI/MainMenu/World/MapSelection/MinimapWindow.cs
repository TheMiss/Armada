using Armageddon.AssetManagement;
using Armageddon.UI.Base;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.World.MapSelection
{
    public class MinimapWindow : Widget
    {
        [SerializeField]
        private RawImage m_minimapRenderTexture;

        [ReadOnly]
        [ShowInInspector]
        public GameObject MinimapObject { private set; get; }

        [ReadOnly]
        [ShowInInspector]
        public int MapId { private set; get; }

        public RawImage MinimapRenderTexture => m_minimapRenderTexture;

        public async UniTaskVoid LoadAsync(int mapId, Transform parent)
        {
            MapId = mapId;

            MinimapObject = await Assets.InstantiateMinimapAsync(mapId, parent);
            MinimapObject.transform.localPosition = new Vector3(-4.75f, 2.5f, 50);
            MinimapObject.name = $"Minimap{mapId}";

            // MapId = mapId;
            // string path = $"{PrefabPaths.Maps}/Map{mapId}/Map{mapId}-Minimap.prefab";
            // MinimapObject = await Addressables.InstantiateAsync(path, parent);
            // MinimapObject.transform.localPosition = new Vector3(-4.75f, 2.5f, 50);
            // MinimapObject.name = $"Minimap{mapId}";
        }
    }
}
