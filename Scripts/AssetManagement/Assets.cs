using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Armageddon.Maps;
using Armageddon.Sheets;
using Armageddon.Sheets.Abilities;
using Armageddon.Sheets.Actors;
using Armageddon.Sheets.Items;
using Armageddon.Worlds.Environment;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Armageddon.AssetManagement
{
    public static class Assets
    {
        public const string BasePath = "Assets/Armageddon";
        public const string ScriptableObjectsPath = BasePath + "/ScriptableObjects";
        public const string PrefabsPath = BasePath + "/Prefabs";
        public const string HeroesPath = ScriptableObjectsPath + "/Heroes";
        public const string EnemiesPath = ScriptableObjectsPath + "/Enemies";
        public const string ItemsPath = ScriptableObjectsPath + "/Items";
        public const string AbilitiesPath = ScriptableObjectsPath + "/Abilities";
        public const string StatusEffectsPath = ScriptableObjectsPath + "/StatusEffects";
        public const string EnchantmentsPath = ScriptableObjectsPath + "/Enchantments";
        public const string MapsPath = ScriptableObjectsPath + "/Maps";
        public const string MapPrefabsPath = PrefabsPath + "/Maps";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Debug.Log("Assets.Initialize");
            InitializeAsync().Forget();
        }

        public static async UniTask InitializeAsync()
        {
            // string path = $"{Items}/{nameof(ItemSheetReferenceTable)}.asset";
            // ItemSheetReferenceTable = await Addressables.LoadAssetAsync<ItemSheetReferenceTable>(path);
            //
            // path = $"{Items}/{nameof(CardSheetReferenceTable)}.asset";
            // CardSheetReferenceTable = await Addressables.LoadAssetAsync<CardSheetReferenceTable>(path);

            await UniTask.CompletedTask;
        }

        private static string GetPath(Type type)
        {
            string path = string.Empty;

            if (type == typeof(ItemSheet))
            {
                path = ItemsPath;
            }
            else if (type == typeof(AbilitySheet))
            {
                path = AbilitiesPath;
            }
            else if (type == typeof(EnemySheet))
            {
                path = EnemiesPath;
            }
            else
            {
                Debug.LogError($"GetPath: {type.Name} is not implemented!");
            }

            return path;
        }

        public static async UniTask<HeroSheet> LoadHeroSheetAsync(int sheetId)
        {
            string path = $"{HeroesPath}/HeroSheet{sheetId}.asset";
            var heroSheet = await Addressables.LoadAssetAsync<HeroSheet>(path);

            return heroSheet;
        }

        public static async UniTask<EnemySheet> LoadEnemySheetAsync(int sheetId)
        {
            string path = $"{EnemiesPath}/EnemySheet{sheetId}.asset";
            var enemySheet = await Addressables.LoadAssetAsync<EnemySheet>(path);

            return enemySheet;
        }

        public static async UniTask<ItemSheet> LoadItemSheetAsync(int sheetId)
        {
            string itemSheetName = $"ItemSheet{sheetId}";
            string path = $"{ItemsPath}/{itemSheetName}.asset";

            try
            {
                var itemSheet = await Addressables.LoadAssetAsync<ItemSheet>(path);

                return itemSheet;
            }
            catch (Exception e)
            {
                // We will get InvalidKeyException, but we can't catch it!!!
                Debug.LogError($"Cannot find {itemSheetName} ({e.Message})");

                // TODO: return default ItemSheet
                return null;
            }
        }

        public static ItemSheet LoadItemSheet(int sheetId)
        {
            return LoadSheet<ItemSheet>(sheetId);
        }

        public static T LoadSheet<T>(int sheetId) where T : Sheet
        {
            string sheetName = $"{typeof(T).Name}{sheetId}";
            string path = $"{GetPath(typeof(T))}/{sheetName}.asset";

            return LoadAsset<T>(path);
        }

        public static ExpTable LoadExpTable()
        {
            return LoadAsset<ExpTable>($"{ScriptableObjectsPath}/ExpTable.asset");
        }

        public static T LoadAsset<T>(object key)
        {
            try
            {
                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
                T asset = handle.WaitForCompletion();

                Addressables.Release(handle);

                return asset;
            }
            catch (Exception e)
            {
                // We will get InvalidKeyException, but we can't catch it!!!
                Debug.LogError($"Cannot find {key} ({e.Message})");

                // TODO: return default ItemSheet
                return default;
            }
        }

        public static async UniTask<T> LoadSheetAsync<T>(int sheetId)
        {
            string sheetName = $"{typeof(T).Name}{sheetId}";
            string path = $"{GetPath(typeof(T))}/{sheetName}.asset";

            return await LoadAssetAsync<T>(path);
        }

        public static async UniTask<T> LoadAssetAsync<T>(object key)
        {
            try
            {
                var asset = await Addressables.LoadAssetAsync<T>(key);

                return asset;
            }
            catch (Exception e)
            {
                // We will get InvalidKeyException, but we can't catch it!!!
                Debug.LogError($"Cannot find {key} ({e.Message})");

                return default;
            }
        }

        // public static async UniTask<AbilitySheet> LoadAbilitySheetAsync(int sheetId)
        // {
        //     string abilitySheetName = $"AbilitySheet{sheetId}";
        //     string path = $"{Abilities}/{abilitySheetName}.asset";
        //
        //     try
        //     {
        //         var abilitySheet = await Addressables.LoadAssetAsync<AbilitySheet>(path);
        //
        //         return abilitySheet;
        //     }
        //     catch (Exception e)
        //     {
        //         // We will get InvalidKeyException, but we can't catch it!!!
        //         Debug.LogError($"Cannot find {abilitySheetName} ({e.Message})");
        //
        //         // TODO: return default ItemSheet
        //         return null;
        //     }
        // }

        public static async UniTask<List<ConsumableSheet>> LoadConsumableSheetsAsync()
        {
            var consumableSheets =
                (List<ConsumableSheet>)await Addressables.LoadAssetsAsync<ConsumableSheet>(Labels.Consumables, null);
            consumableSheets.Sort((x, y) => x.Id < y.Id ? -1 : 1);

            return consumableSheets;
        }

        public static async UniTask<List<CardSheet>> LoadCardSheetsAsync()
        {
            var cardSheets = (List<CardSheet>)await Addressables.LoadAssetsAsync<CardSheet>(Labels.Cards, null);
            cardSheets.Sort((x, y) => x.Id < y.Id ? -1 : 1);

            return cardSheets;
        }

        public static async Task<List<ChestSheet>> LoadLootBoxSheetsAsync()
        {
            var lootBoxSheets =
                (List<ChestSheet>)await Addressables.LoadAssetsAsync<ChestSheet>(Labels.LootBoxes, null);
            lootBoxSheets.Sort((x, y) => x.Id < y.Id ? -1 : 1);

            return lootBoxSheets;
        }

        // ============================= Map =============================

        public static async UniTask<MapNode> InstantiateMapNodeAsync(int mapId)
        {
            string path = $"{MapPrefabsPath}/Map{mapId}/Map{mapId}.prefab";
            GameObject mapObject = await Addressables.InstantiateAsync(path);
            var mapNode = mapObject.GetComponent<MapNode>();

            return mapNode;
        }

        public static async UniTask<GameObject> InstantiateMinimapAsync(int mapId, Transform parent)
        {
            string path = $"{MapPrefabsPath}/Map{mapId}/Map{mapId}-Minimap.prefab";
            GameObject minimapObject = await Addressables.InstantiateAsync(path, parent);

            return minimapObject;
        }

        public static async UniTask<Zone> InstantiateZone(int mapId, int zoneId, Transform parent)
        {
            string path = $"{MapPrefabsPath}/Map{mapId}/Environment/Map{mapId}-Zone{zoneId}.prefab";
            GameObject zoneObject = await Addressables.InstantiateAsync(path, parent);

            var zone = zoneObject.GetComponent<Zone>();
            return zone;
        }

        // Using AssetReference is so slow, perhaps I'm doing it wrong.
        // public static ItemSheetReferenceTable ItemSheetReferenceTable { get; private set; }
        // public static CardSheetReferenceTable CardSheetReferenceTable { get; private set; }

        // Very slow
        // public static async UniTask<ItemSheet> LoadItemSheetAsync(int sheetId)
        // {
        //     return await ItemSheetReferenceTable.LoadAssetAsync(sheetId);
        // }

        // public static async UniTask LoadAlwaysInMemoryResources(Action<(float Progress, string AssetName)> callback = null)
        // {
        //     var stopwatch = new Stopwatch(nameof(LoadAlwaysInMemoryResources));
        //
        //     var referenceEntries = new List<ReferenceEntry>();
        //     
        //     referenceEntries.AddRange(ItemSheetReferenceTable.Entries);
        //     referenceEntries.AddRange(CardSheetReferenceTable.Entries);
        //
        //     int loadedCount = 0;
        //     foreach (ReferenceEntry referenceEntry in referenceEntries)
        //     {
        //         AssetReference assetReference = referenceEntry.AssetReference;
        //
        //         if (assetReference.Asset == null)
        //         {
        //             var obj = await assetReference.LoadAssetAsync<Object>();
        //             loadedCount++;
        //
        //             float progress = (float)loadedCount / referenceEntries.Count;
        //             (float Progress, string AssetName) result = (Progress:progress, AssetName:obj.name);
        //             callback?.Invoke(result);
        //         }
        //     }
        //
        //     stopwatch.Stop();
        // }
    }
}
