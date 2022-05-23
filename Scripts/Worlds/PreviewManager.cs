using System;
using System.Collections.Generic;
using Armageddon.Mechanics.Characters;
using Armageddon.Worlds.Actors.Heroes;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon.Worlds
{
    public class PreviewManager : WorldContext
    {
        [SerializeField]
        private Camera m_renderTextureCameraPrefab;

        [SerializeField]
        private float m_offsetX = 20;

        [HideInEditorMode]
        [ShowInInspector]
        private Dictionary<object, PreviewEntry> m_entries = new();

        private int m_index;

        protected override void Awake()
        {
            base.Awake();

            RegisterService(this);
        }

        protected override void Start()
        {
            base.Start();

            m_renderTextureCameraPrefab.gameObject.SetActive(false);
        }

        public async UniTask<PreviewEntry> ShowHeroAsync(object owner, Hero hero, Action<PreviewEntry> callback = null)
        {
            if (!m_entries.TryGetValue(owner, out PreviewEntry entry))
            {
                var position = new Vector3(m_index * m_offsetX, 0, -100);
                m_index++;

                Camera cam = Instantiate(m_renderTextureCameraPrefab, Transform);
                cam.gameObject.SetActive(true);
                // cam.gameObject.RemoveCloneFromName();
                cam.targetTexture = new RenderTexture(1024, 1024, 24);
                cam.transform.position = position;

                entry = cam.gameObject.AddComponent<PreviewEntry>();
                entry.gameObject.name = $"Entry{m_index}({((Behaviour)owner).name})";
                entry.Camera = cam;

                m_entries.Add(owner, entry);
            }

            // We cache here.
            if (!entry.HeroActors.TryGetValue(hero, out HeroActor heroActor))
            {
                heroActor = await World.CreateHeroActorAsync(hero);
                entry.HeroActors.Add(hero, heroActor);
            }

            if (entry.HeroActor != null)
            {
                entry.HeroActor.gameObject.SetActive(false);
            }

            entry.HeroActor = heroActor;
            entry.HeroActor.gameObject.SetActive(true);
            Vector3 heroPosition = entry.Camera.transform.position;
            heroPosition.z = 0;
            entry.HeroActor.Transform.position = heroPosition;

            callback?.Invoke(entry);

            return entry;
        }

        public void HideHeroes(object owner)
        {
            if (m_entries.TryGetValue(owner, out PreviewEntry entry))
            {
                entry.SetActive(false);
            }
        }
    }
}
