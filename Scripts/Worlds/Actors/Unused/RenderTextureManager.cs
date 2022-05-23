using System.Collections.Generic;
using System.Linq;
using Armageddon.Games;
using Armageddon.Mechanics.Characters;
using Armageddon.Worlds.Actors.Characters;
using UnityEngine;

namespace Armageddon.Worlds.Actors.Unused
{
    public class RenderTextureManager : GameContext
    {
        public enum CreateType
        {
            ForCharacterHolder,
            ForCharacterStyleHolder,
            ForCharacterShopItemHolder
        }

        [SerializeField]
        private Location m_characterHolderRootLocation;

        [SerializeField]
        private Location m_characterStyleHolderRootLocation;

        private readonly Dictionary<Character, RenderTextureObject> m_renderTextureObjects = new();

        protected override void Awake()
        {
            base.Awake();

            RegisterService(this);
        }

        public void Remove(Character character)
        {
            if (!m_renderTextureObjects.TryGetValue(character, out RenderTextureObject renderTextureObject))
            {
                Debug.LogWarning($"Destroy: Can't find {character}!");
                return;
            }

            Destroy(renderTextureObject.GameObject);
            Destroy(renderTextureObject.Camera.gameObject);
            renderTextureObject.RenderTexture.Release();

            m_renderTextureObjects.Remove(character);
        }

        /// <summary>
        ///     Mus call Destroy with characterData as a key
        /// </summary>
        public RenderTexture Get(Character character, CreateType createType = CreateType.ForCharacterHolder)
        {
            if (m_renderTextureObjects.TryGetValue(character, out RenderTextureObject renderTextureObject))
            {
                return renderTextureObject.RenderTexture;
            }

            CharacterActor characterActor = null;

            switch (character)
            {
                case Hero heroData:
                    // characterActor = Game.HeroActorFactory.Create(heroData);
                    break;
                // case Companion companionData:
                //     // characterActor = Game.CompanionFactory.Create(companionData);
                //     break;
                default:
                    Debug.LogError($"{character} is not supported!");
                    return null;
            }


            var renderTexture = new RenderTexture((int)(512 * 1.0), (int)(512 * 1.0f), 24);

            int index = 0;

            while (true)
            {
                bool foundUsedIndex = m_renderTextureObjects.Any(keyValuePair => keyValuePair.Value.Index == index);

                if (!foundUsedIndex)
                {
                    break;
                }

                index++;
            }

            float offsetX = index * 10;
            Location rootLocation = m_characterHolderRootLocation;

            if (createType == CreateType.ForCharacterHolder)
            {
                offsetX = index * 10;
                rootLocation = m_characterHolderRootLocation;
            }
            else if (createType == CreateType.ForCharacterStyleHolder)
            {
                // offsetX = character.PresetIndex * 10;
                rootLocation = m_characterStyleHolderRootLocation;
            }

            var cameraObject = new GameObject($"Camera of {characterActor.name}");
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 2f;
            Vector3 cameraPosition = rootLocation;
            cameraPosition.x += offsetX;
            cameraPosition.z = -100;
            camera.transform.SetParent(rootLocation.Transform);
            camera.transform.position = cameraPosition;
            camera.targetTexture = renderTexture;

            Vector2 characterPosition = rootLocation;
            characterPosition.x += offsetX;
            characterActor.Transform.SetParent(cameraObject.transform);
            characterActor.Position = characterPosition;
            characterActor.CanTick = false; // To prevent any update(), except for animation maybe?


            renderTextureObject = new RenderTextureObject(characterActor.gameObject, index, camera, renderTexture);
            m_renderTextureObjects.Add(character, renderTextureObject);

            return renderTexture;
        }

        public void UpdateCharacter(Character character)
        {
            if (m_renderTextureObjects.TryGetValue(character, out RenderTextureObject renderTextureObject))
            {
                var characterActor = renderTextureObject.GameObject.GetComponent<CharacterActor>();

                if (characterActor == null)
                {
                    Debug.LogError("Can't find Character");
                }

                CharacterSkin characterSkin = character.Dyes[character.DyeId];
                characterActor.SetAppearance(characterSkin);
                // characterActor.SetAppearance(character.GetSelectedSkinPreset());
            }
        }

        private class RenderTextureObject
        {
            public RenderTextureObject(GameObject go, int index, Camera camera, RenderTexture renderTexture)
            {
                GameObject = go;
                Index = index;
                Camera = camera;
                RenderTexture = renderTexture;
            }

            public int Index { get; }
            public GameObject GameObject { get; }
            public Camera Camera { get; }
            public RenderTexture RenderTexture { get; }
        }
    }
}
