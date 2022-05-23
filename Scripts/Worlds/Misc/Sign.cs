using Armageddon.Worlds.Actors;
using UnityEngine;

namespace Armageddon.Worlds.Misc
{
    public class Sign : Actor
    {
        public static float FlickerInterval;

        [SerializeField]
        private Flicker m_flicker;

        public void Begin()
        {
            m_flicker.Begin(FlickerInterval);
        }

        public void End()
        {
            m_flicker.End();
        }
    }
}
