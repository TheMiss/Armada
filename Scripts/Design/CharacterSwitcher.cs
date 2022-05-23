using System.Collections.Generic;
using Armageddon.Worlds.Actors.Characters;
using UnityEngine;

namespace Armageddon.Design
{
    public class CharacterSwitcher : MonoBehaviour
    {
        public List<CharacterActor> CharacterActors;

        private void Start()
        {
            foreach (Transform childTransform in transform)
            {
                childTransform.gameObject.SetActive(false);
            }
        }
    }
}
