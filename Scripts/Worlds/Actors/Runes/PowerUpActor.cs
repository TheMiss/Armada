using UnityEngine;

namespace Armageddon.Worlds.Actors.Runes
{
    public enum PowerUpType
    {
        Hero,
        Companion
    }

    public class PowerUpActor : RuneActor
    {
        [SerializeField]
        private PowerUpType m_type;

        public PowerUpType Type => m_type;
    }
}
