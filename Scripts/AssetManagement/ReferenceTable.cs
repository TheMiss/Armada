using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Purity.Common;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Armageddon.AssetManagement
{
    [Serializable]
    public class ReferenceEntry
    {
        [ReadOnly]
        [TableColumnWidth(60, Resizable = false)]
        [HideLabel]
        [SerializeField]
        private int m_id;

        // [ReadOnly] // AssetReference causes some stupid behavior...
        [HideLabel] // Doesn't work for now, neither the LabelText()
        [SerializeField]
        private AssetReference m_assetReference;

        public ReferenceEntry(int id, AssetReference assetReference)
        {
            m_id = id;
            m_assetReference = assetReference;
        }

        public int Id => m_id;

        public AssetReference AssetReference => m_assetReference;
    }

    /// <summary>
    ///     Should no longer be used.
    /// </summary>
    public abstract class ReferenceTable<T> : ScriptableObject where T : IIdentifiable
    {
        [TableList(IsReadOnly = true)]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
        [SerializeField]
        private List<ReferenceEntry> m_entries;

        public List<ReferenceEntry> Entries => m_entries;

        public virtual string[] SearchInFolders { get; } = { "Assets/Armageddon" };

        public AssetReference GetAssetReference(int id)
        {
            ReferenceEntry entry = m_entries.Find(x => x.Id == id);

            if (entry != null)
            {
                return entry.AssetReference;
            }

            Debug.LogError($"{name}: cannot find {id}!");
            return null;
        }

        public async UniTask<T> LoadAssetAsync(int id)
        {
            AssetReference assetReference = GetAssetReference(id);

            if (assetReference == null)
            {
                return default;
            }

            if (assetReference.Asset != null)
            {
                if (assetReference.Asset is T identifiable)
                {
                    Debug.Log($"Getting {typeof(T).Name}({id}).");
                    return identifiable;
                }

                Debug.LogError($"assetReference.Asset is not {nameof(IIdentifiable)}");
                return default;
            }

            Debug.Log($"Loading {typeof(T).Name}({id}).");

            var asset = await assetReference.LoadAssetAsync<T>();

            return asset;
        }

        public async UniTask<List<T>> LoadAssetsAsync(Action<T> callback = null)
        {
            var assets = new List<T>();

            foreach (ReferenceEntry entry in Entries)
            {
                T asset = await LoadAssetAsync(entry.Id);

                if (asset == null)
                {
                    continue;
                }

                callback?.Invoke(asset);
                assets.Add(asset);
            }

            return assets;
        }

#if UNITY_EDITOR

        private void OnEnable()
        {
            RefreshAssetReferences();
        }

        [Button]
        public void RefreshAssetReferences()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", SearchInFolders);

            m_entries.Clear();

            foreach (string guid in guids)
            {
                var assetReference = new AssetReference(guid);

                if (assetReference.editorAsset is IIdentifiable identifiable)
                {
                    var referenceEntry = new ReferenceEntry(identifiable.Id, assetReference);

                    m_entries.Add(referenceEntry);
                }
            }

            m_entries.Sort((x, y) => x.Id > y.Id ? 1 : -1);
        }
#endif
    }
}
