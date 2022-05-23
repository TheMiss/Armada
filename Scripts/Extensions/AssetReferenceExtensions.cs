using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Armageddon.Extensions
{
    /// <summary>
    ///     (Can be moved to Purity.Common later.)
    ///     Extensions that can be loaded directly with component.
    /// </summary>
    public static class AssetReferenceExtensions
    {
        public static async UniTask<T> LoadAsync<T>(this AssetReference assetReference) where T : Component
        {
            if (assetReference.Asset != null)
            {
                var component = ((GameObject)assetReference.Asset).GetComponent<T>();
                return component;
            }
            else
            {
                var assetObject = await assetReference.LoadAssetAsync<GameObject>();
                var component = assetObject.GetComponent<T>();

                return component;
            }

// #if UNITY_EDITOR
//             if (assetReference.editorAsset != null)
//             {
//                 var component = ((GameObject)assetReference.editorAsset).GetComponent<T>();
//                 return component;
//             }
//             else
//             {
//                 var assetObject = await assetReference.LoadAssetAsync<GameObject>();
//                 var component = assetObject.GetComponent<T>();
//
//                 return component;
//             }
// #else
//             var assetObject = await assetReference.LoadAssetAsync<GameObject>();
//             var component = assetObject.GetComponent<T>();
//
//             return component;
// #endif
        }

        public static async UniTask<T> InstantiateAsync<T>(this AssetReference assetReference,
            Transform parent = null, bool instantiateInWorldSpace = false) where T : Component
        {
            GameObject assetObject = await assetReference.InstantiateAsync(parent, instantiateInWorldSpace);
            var component = assetObject.GetComponent<T>();

            return component;
        }

        public static async UniTask<T> InstantiateAsync<T>(this AssetReference assetReference, Vector3 position,
            Quaternion rotation, Transform parent = null) where T : Component
        {
            GameObject assetObject = await assetReference.InstantiateAsync(position, rotation, parent);
            var component = assetObject.GetComponent<T>();

            return component;
        }

        public static bool HasComponent<T>(this AssetReference assetReference, ref string reportMessage)
            where T : Object
        {
#if UNITY_EDITOR
            switch (assetReference.editorAsset)
            {
                case null:
                {
                    reportMessage = "Null Reference";
                    return false;
                }
                case GameObject gameObject:
                {
                    var component = gameObject.GetComponent<T>();

                    if (component == null)
                    {
                        reportMessage = $"{assetReference.SubObjectName} is not {typeof(T).Name}.";
                        return false;
                    }

                    break;
                }
                case ScriptableObject _:
                    reportMessage = $"{assetReference.SubObjectName} is {nameof(ScriptableObject)}.";
                    return false;
            }
#endif
            return true;
        }
    }
}
