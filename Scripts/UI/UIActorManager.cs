using System.Collections.Generic;
using System.Linq;
using Armageddon.Games;
using Armageddon.Mechanics.Characters;
using Armageddon.Worlds.Actors.Heroes;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.UI
{
    public class UIActorManager : GameContext
    {
        [SerializeField]
        private Camera m_cameraPrefab;

        private readonly Dictionary<Hero, HeroActor> m_heroActors = new();

        [HideInEditorMode]
        [ShowInInspector]
        public List<Camera> RenderTextureCameras { get; set; } = new();

        [HideInEditorMode]
        [ShowInInspector]
        public HeroActor CurrentHeroActor { get; set; }

        protected override void Start()
        {
            base.Start();

            Initialize();
        }

        public async UniTask<HeroActor> GetHeroAsync(Hero hero, bool setActive = true)
        {
            if (m_heroActors.TryGetValue(hero, out HeroActor heroActor))
            {
                if (setActive)
                {
                    heroActor.gameObject.SetActive(true);
                }

                return heroActor;
            }

            heroActor = await World.CreateHeroActorAsync(hero);

            m_heroActors.Add(hero, heroActor);

            return heroActor;
        }

        public void Initialize()
        {
            GameObject otherCamerasObject = GameObject.Find("OtherCameras");

            if (otherCamerasObject != null)
            {
                RenderTextureCameras = otherCamerasObject.GetComponentsInChildren<Camera>().ToList();
            }

            foreach (Camera renderTextureCamera in RenderTextureCameras)
            {
                renderTextureCamera.gameObject.SetActive(false);
            }
        }
    }
}
