using Armageddon.Games;
using Armageddon.Worlds.Actors;
using Armageddon.Worlds.Actors.Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Armageddon
{
    [DisallowMultipleComponent]
    public class PlayerCharacterBase : GameContext
    {
        [SerializeField]
        private Location m_characterLocation;

        [HideInEditorMode]
        [ShowInInspector]
        private CharacterActor m_mainCharacter;

        public Location CharacterLocation => m_characterLocation;

        public CharacterActor MainCharacter
        {
            get => m_mainCharacter;
            set => m_mainCharacter = value;
        }

        protected override void Start()
        {
            CanTick = true;
        }

        /// <summary>
        ///     Main controlled character
        /// </summary>
        /// <param name="character"></param>
        public void EquipMainCharacter(CharacterActor character)
        {
            MainCharacter = character;
            // MainCharacter.Transform.SetParent(CharacterLocation.transform);
            // MainCharacter.Transform.localPosition = Vector2.zero;
        }

        public bool IsReadyToPlay()
        {
            if (MainCharacter == null)
            {
                Debug.LogWarning("MainCharacter is null!");
                return false;
            }

            return true;
        }

        public override void Tick()
        {
            if (MainCharacter == null)
            {
                return;
            }

            MainCharacter.Transform.position = CharacterLocation;
        }

        public void UnequipAll()
        {
            // Should destroy?
            // Destroy(MainCharacter.gameObject);
            MainCharacter = null;
        }

        // public void MoveTo(Vector2 targetPosition)
        // {
        //     Vector3 position = transform.position;
        //     if (position.x < targetPosition.x)
        //     {
        //         position.x += Core.MoveSpeed * Time.deltaTime;
        //
        //         if (position.x > targetPosition.x)
        //         {
        //             position.x = targetPosition.x;
        //         }
        //     }
        //     else if (position.x > targetPosition.x)
        //     {
        //         position.x -= Core.MoveSpeed * Time.deltaTime;
        //
        //         if (position.x < targetPosition.x)
        //         {
        //             position.x = targetPosition.x;
        //         }
        //     }
        //
        //     if (position.x < MinBounds.x)
        //     {
        //         position.x = MinBounds.x;
        //     }
        //     else if (position.x > MaxBounds.x)
        //     {
        //         position.x = MaxBounds.x;
        //     }
        //
        //     transform.position = position;
        // }
    }
}
