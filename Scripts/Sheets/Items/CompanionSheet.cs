using Armageddon.Extensions;
using Armageddon.Worlds.Actors.Companions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Armageddon.Sheets.Items
{
    [CreateAssetMenu(fileName = "CompanionSchema", menuName = "Companions/CompanionSchema", order = 0)]
    public class CompanionSheet : ItemSheet
    {
        // TODO: Add Companion Actor validation
        // [ValidateInput(nameof(ValidateHeroActorPrefab))]
        [Required]
        [SerializeField]
        private AssetReference m_actorPrefab;

        // [ValidateInput(nameof(ValidateWeaponPrefab))]
        [Required]
        [SerializeField]
        private WeaponSheet m_weaponSheet;

        public AssetReference ActorPrefab => m_actorPrefab;

        public WeaponSheet WeaponSheet => m_weaponSheet;

        // private bool ValidateWeaponPrefab(AssetReference assetReference, ref string message)
        // {
        //     return assetReference.HasComponent<WeaponActor>(ref message);
        // }

        public async UniTask<CompanionActor> CreateActorAsync(Transform parent)
        {
            return await ActorPrefab.InstantiateAsync<CompanionActor>(parent);
        }
    }
}
