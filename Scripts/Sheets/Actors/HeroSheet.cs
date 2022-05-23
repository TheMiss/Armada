using Armageddon.Extensions;
using Armageddon.Worlds.Actors.Heroes;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Armageddon.Sheets.Actors
{
    [CreateAssetMenu(fileName = "HeroSheet", menuName = "Heroes/HeroSheet", order = 0)]
    public class HeroSheet : CharacterSheet
    {
        [ValidateInput(nameof(ValidateHeroActorPrefab))]
        [Required]
        [SerializeField]
        private AssetReference m_actorPrefab;

        public AssetReference ActorPrefab => m_actorPrefab;

        private bool ValidateHeroActorPrefab(AssetReference assetReference, ref string message)
        {
            return assetReference.HasComponent<HeroActor>(ref message);
        }

        public async UniTask<HeroActor> CreateActorAsync(Transform parent)
        {
            return await ActorPrefab.InstantiateAsync<HeroActor>(parent);
        }
    }
}
