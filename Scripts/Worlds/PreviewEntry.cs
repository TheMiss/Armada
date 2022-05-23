using System.Collections.Generic;
using Armageddon.Mechanics.Characters;
using Armageddon.Worlds.Actors.Heroes;
using Purity.Common;
using UnityEngine;

namespace Armageddon.Worlds
{
    public class PreviewEntry : FastMonoBehaviour
    {
        public Camera Camera { get; set; }

        [ShowInPlayMode]
        public HeroActor HeroActor { get; set; }

        public Dictionary<Hero, HeroActor> HeroActors { get; set; } = new();

        public void SetActive(bool value)
        {
            // PreviewEntry.SetActive(false) is usually called during OnDisable, but Camera was destroyed so giving us this in Editor
            // MissingReferenceException: The object of type 'Camera' has been destroyed but you are still trying to access it.
            if (Camera != null)
            {
                Camera.gameObject.SetActive(value);
            }

            if (HeroActor == null)
            {
                return;
            }

            foreach (HeroActor heroActor in HeroActors.Values)
            {
                if (heroActor.gameObject != null)
                {
                    heroActor.gameObject.SetActive(value);
                }
            }
        }
    }
}
