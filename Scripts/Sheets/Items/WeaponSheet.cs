using System;
using Armageddon.Extensions;
using Armageddon.Mechanics.Items;
using Armageddon.Worlds.Actors.Weapons;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Armageddon.Sheets.Items
{
    [CreateAssetMenu(menuName = "Weapons/WeaponSheet", order = 0)]
    [Serializable]
    public class WeaponSheet : ItemSheet
    {
        [ValidateInput(nameof(ValidateWeaponPrefab))]
        [SerializeField]
        private AssetReference m_weaponPrefab;

        [SerializeField]
        private WeaponType m_weaponType;

        public AssetReference WeaponPrefab => m_weaponPrefab;

        public WeaponType WeaponType => m_weaponType;

        private bool ValidateWeaponPrefab(AssetReference assetReference, ref string message)
        {
            return assetReference.HasComponent<WeaponActor>(ref message);
        }

        public async UniTask<WeaponActor> CreateWeaponAsync()
        {
            return await WeaponPrefab.InstantiateAsync<WeaponActor>();
        }

        public async UniTask<WeaponActor> LoadWeaponAsync()
        {
            return await WeaponPrefab.LoadAsync<WeaponActor>();
        }
    }
}
